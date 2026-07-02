// wwwroot/js/offline-sync.js
// Menggunakan Dexie.js untuk mengatur antrean IndexedDB
// PENTING: Tidak ada data yang pernah dihapus dari syncQueue — hanya data master/cache yang di-upsert.

const db = new Dexie('VelastoOfflineDB');
window.db = db;

// Definisikan struktur tabel
db.version(2).stores({
    syncQueue: '++id, url, timestamp, retryCount', // payload is not indexed but stored
    apiCache: 'key, timestamp'                     // Key-Value store untuk API responses
}).upgrade(tx => {
    // If upgrading from v1, no explicit migration needed as new table is just added
});

/**
 * Memasukkan request ke dalam antrean offline
 * @param {string} url - Tujuan URL API
 * @param {object} payload - Data JSON yang dikirim
 */
window.enqueueOfflineRequest = async function(url, payload) {
    try {
        await db.syncQueue.add({
            url: url,
            payload: payload,
            timestamp: new Date().getTime(),
            retryCount: 0
        });
        updateOfflineIndicator();
        console.log('[Offline Sync] Request ditambahkan ke antrean untuk: ' + url);
    } catch (err) {
        console.error('[Offline Sync] Gagal menyimpan ke IndexedDB', err);
    }
};

/**
 * Mencoba memproses ulang semua antrean di IndexedDB
 */
window.syncOfflineRequests = async function() {
    // Jika tidak ada internet, batalkan
    if (!navigator.onLine) return;

    try {
        const queueCount = await db.syncQueue.count();
        if (queueCount === 0) {
            updateOfflineIndicator();
            return; // Tidak ada antrean
        }

        console.log(`[Offline Sync] Memulai proses sinkronisasi ${queueCount} data...`);
        showSyncingIndicator(true);

        const items = await db.syncQueue.orderBy('timestamp').toArray();

        for (const item of items) {
            try {
                const response = await fetch(item.url, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(item.payload)
                });

                if (response.ok) {
                    // Berhasil dikirim, hapus dari antrean
                    await db.syncQueue.delete(item.id);
                    console.log(`[Offline Sync] Berhasil mensinkronkan antrean ID: ${item.id}`);
                } else {
                    // Jika server error (500) dsb, naikkan retryCount tapi tetap di antrean
                    console.warn(`[Offline Sync] Server error saat mensinkronkan ID: ${item.id}`);
                    await db.syncQueue.update(item.id, { retryCount: item.retryCount + 1 });
                }
            } catch (err) {
                // Fetch gagal (mungkin koneksi putus lagi di tengah jalan)
                console.error(`[Offline Sync] Koneksi terputus saat mencoba sinkronisasi ID: ${item.id}`);
                break; // Berhenti memproses sisanya, tunggu koneksi stabil lagi
            }
        }
    } catch (err) {
        console.error('[Offline Sync] Terjadi kesalahan saat membaca queue', err);
    } finally {
        showSyncingIndicator(false);
        updateOfflineIndicator();
    }
};

/**
 * Auto-Sync Master Data secara SENYAP (tanpa popup, tanpa tombol)
 * Menggunakan bulkPut (UPSERT) — tidak ada data yang dihapus.
 * Dipanggil otomatis setiap 30 menit saat online, dan juga saat halaman pertama kali dibuka.
 */
window.autoSyncMasterData = async function() {
    if (!navigator.onLine) return;

    try {
        console.log('[Auto-Sync] Memulai sinkronisasi master data secara otomatis...');

        const targetDate = new Date().toISOString().split('T')[0];
        const res = await fetch(`/OfflineSync/GetFullSnapshot?targetDate=${targetDate}`);
        if (!res.ok) {
            console.warn('[Auto-Sync] Server tidak merespon, akan dicoba lagi nanti.');
            return;
        }
        const result = await res.json();
        if (!result.success) {
            console.warn('[Auto-Sync] Server mengembalikan error:', result.message);
            return;
        }

        const cacheItems = [];
        const timestamp = new Date().getTime();

        // 1. Users & Machines (untuk Login Offline)
        if (result.users && result.machines) {
            cacheItems.push({
                key: 'LOGIN_DATA',
                data: { users: result.users, machines: result.machines },
                timestamp
            });
        }

        // 2. Planning (per tanggal + shift)
        if (result.plannings && result.plannings.length > 0) {
            const byDateShift = {};
            for (const p of result.plannings) {
                const key = `PLANNING_${p.date}_${p.shift}`;
                if (!byDateShift[key]) byDateShift[key] = [];
                byDateShift[key].push(p);
            }
            for (const [key, data] of Object.entries(byDateShift)) {
                cacheItems.push({ key, data, timestamp });
            }
        }

        // 3. SPS Parameter (SPS_PARAM_{itemCode})
        if (result.spsParamList) {
            for (const spsItem of result.spsParamList) {
                if (spsItem.itemCodes && spsItem.itemCodes.length > 0) {
                    for (const code of spsItem.itemCodes) {
                        if (code) {
                            cacheItems.push({
                                key: `SPS_PARAM_${code.toUpperCase().trim()}`,
                                data: spsItem.paramData,
                                timestamp
                            });
                        }
                    }
                }
                if (spsItem.documentNumber) {
                    cacheItems.push({
                        key: `SPS_PARAM_${spsItem.documentNumber.toUpperCase().trim()}`,
                        data: spsItem.paramData,
                        timestamp
                    });
                }
            }
        }

        // 4. SPS Dimensi (SPS_DIM_{documentNumber} dan SPS_DIM_{hoseType})
        if (result.spsDimList) {
            for (const sps of result.spsDimList) {
                if (sps.documentNumber) {
                    cacheItems.push({
                        key: `SPS_DIM_${sps.documentNumber.toUpperCase().trim()}`,
                        data: sps,
                        timestamp
                    });
                }
                if (sps.hoseType) {
                    cacheItems.push({
                        key: `SPS_DIM_${sps.hoseType.toUpperCase().trim()}`,
                        data: sps,
                        timestamp
                    });
                }
            }
        }

        // 5. Shift Master
        if (result.shiftMasters) {
            cacheItems.push({ key: 'SHIFT_MASTERS', data: result.shiftMasters, timestamp });
        }

        // 6. Summary hari ini
        if (result.summary) {
            cacheItems.push({ key: `SUMMARY_${targetDate}`, data: result.summary, timestamp });
        }

        // 7. Simpan waktu sinkronisasi terakhir
        cacheItems.push({ key: 'LAST_SYNC', data: { syncedAt: result.syncedAt }, timestamp });

        // UPSERT (bukan delete+insert) — tidak ada data lama yang hilang
        await db.apiCache.bulkPut(cacheItems);

        console.log(`[Auto-Sync] Selesai. ${cacheItems.length} item tersimpan ke IndexedDB. (${result.syncedAt})`);

        // Update indikator di navbar jika ada
        const syncStatus = document.getElementById('sync-status-text');
        if (syncStatus) {
            const now = new Date();
            syncStatus.textContent = `Tersinkron ${now.getHours().toString().padStart(2,'0')}:${now.getMinutes().toString().padStart(2,'0')}`;
        }

    } catch (e) {
        // Gagal sinkronisasi — tidak apa-apa, akan dicoba lagi di interval berikutnya
        console.warn('[Auto-Sync] Gagal sinkronisasi:', e.message);
    }
};

/**
 * Tetap dipertahankan untuk kompatibilitas dengan tombol SYNC MASTER DATA yang ada.
 * Sekarang hanya wrapper dari autoSyncMasterData dengan notifikasi Swal.
 */
window.downloadOfflineMasterData = async function(targetDate = null) {
    if (!navigator.onLine) {
        Swal.fire('Koneksi Terputus', 'Anda harus online untuk menyinkronkan data master.', 'error');
        return;
    }
    try {
        Swal.fire({
            title: 'MENGUNDUH MASTER DATA',
            html: 'Menyinkronkan semua data ke perangkat...<br>Mohon tunggu sebentar.',
            allowOutsideClick: false,
            didOpen: () => Swal.showLoading()
        });
        await window.autoSyncMasterData();
        Swal.fire({
            title: 'SINKRONISASI SUKSES',
            text: 'Semua master data berhasil disimpan. Aplikasi kini kebal Offline!',
            icon: 'success',
            timer: 3000,
            showConfirmButton: false,
            toast: true,
            position: 'top-end'
        });
    } catch (e) {
        console.error(e);
        Swal.fire('Error', 'Terjadi kesalahan sistem saat sinkronisasi: ' + e.message, 'error');
    }
};

/**
 * Memperbarui tampilan UI Indikator Offline
 */
window.updateOfflineIndicator = async function() {
    const indicator = document.getElementById('offline-indicator');
    const badge = document.getElementById('offline-badge');
    if (!indicator || !badge) return;

    try {
        const count = await db.syncQueue.count();
        
        if (!navigator.onLine) {
            // Sedang Offline
            indicator.style.display = 'flex';
            indicator.className = 'nav-item sub text-danger fw-bold';
            indicator.innerHTML = `<i class="bi bi-wifi-off me-2"></i><span>OFFLINE (${count} Antrean)</span>`;
        } else if (count > 0) {
            // Online tapi masih ada antrean
            indicator.style.display = 'flex';
            indicator.className = 'nav-item sub text-warning fw-bold';
            indicator.innerHTML = `<i class="bi bi-cloud-arrow-up me-2"></i><span>MENUNGGU SYNC (${count})</span>`;
        } else {
            // Online & Sinkron
            indicator.style.display = 'none';
        }
    } catch (e) {
        console.error(e);
    }
};

window.showSyncingIndicator = function(isSyncing) {
    const indicator = document.getElementById('offline-indicator');
    if (!indicator) return;
    if (isSyncing) {
        indicator.style.display = 'flex';
        indicator.className = 'nav-item sub text-info fw-bold';
        indicator.innerHTML = `<i class="bi bi-arrow-repeat spin-icon me-2"></i><span>SINKRONISASI...</span>`;
    }
};

// ─── Event Listeners ─────────────────────────────────────────────────────────

window.addEventListener('online', () => {
    console.log('[Network] Koneksi Internet Tersambung');
    setTimeout(window.syncOfflineRequests, 2000);   // Kirim laporan yang antri
    setTimeout(window.autoSyncMasterData, 5000);    // Refresh master data
    updateOfflineIndicator();
});

window.addEventListener('offline', () => {
    console.log('[Network] Koneksi Internet Terputus');
    updateOfflineIndicator();
});

// Pengecekan saat halaman dimuat
document.addEventListener('DOMContentLoaded', () => {
    updateOfflineIndicator();
    if (navigator.onLine) {
        setTimeout(window.syncOfflineRequests, 1000);
        setTimeout(window.autoSyncMasterData, 3000); // Auto-sync 3 detik setelah halaman terbuka
    }
});

// Auto-sync setiap 30 menit selama browser dibuka
setInterval(() => {
    if (navigator.onLine) {
        window.autoSyncMasterData();
    }
}, 30 * 60 * 1000);
