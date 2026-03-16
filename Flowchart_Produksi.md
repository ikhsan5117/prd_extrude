# Alur Kerja Produksi Digital (Extrusion Workflow)
PT. Velasto Indonesia

Dokumen ini menjelaskan integrasi laporan kertas (Gambar 1-5) ke dalam sistem digital baru.

## 📊 Flowchart Alur Produksi

```mermaid
graph TD
    %% Titik Mulai
    START((<b>MULAI SHIFT</b>)) --> SCAN[<b>1. SCAN BARCODE</b><br/><i>Tarik Data SPS Masterlist</i>]
    
    %% Tahap 1
    subgraph TAHAP_1 [<b>TAHAP 1: IZIN JALAN (NOW I'M PRODUCE)</b>]
        SCAN --> VERIFY[Verifikasi Hose Type & Dimensi]
        VERIFY --> LOT_INPUT[Input No. Lot & SG Material<br/><i>(Inner, Middle, Outer)</i>]
        LOT_INPUT --> SPV_CHECK{<b>SPV CHECK</b><br/>Validasi Material?}
        SPV_CHECK -- REJECT --> LOT_INPUT
        SPV_CHECK -- APPROVE --> START_PROD[Klik: START PRODUCTION]
    end

    %% Tahap 2
    subgraph TAHAP_2 [<b>TAHAP 2: MONITORING (DASHBOARD)</b>]
        START_PROD --> DASHBOARD{<b>DASHBOARD MONITORING</b>}
        
        DASHBOARD --> TAB_PARAM[<b>TAB: PARAMETER SETTING</b><br/><i>Monitoring Mesin (Gambar 5)</i>]
        TAB_PARAM --> LOG_PARAM[Input Suhu, Speed, Press<br/><i>Setiap Jam</i>]
        
        DASHBOARD --> TAB_DIM[<b>TAB: DIMENSION REPORT</b><br/><i>Monitoring Ukuran (Gambar 2)</i>]
        TAB_DIM --> LOG_DIM[Ukur & Input Diameter/Tebal<br/><i>Setiap 30 Menit</i>]
    end

    %% Tahap 3
    subgraph TAHAP_3 [<b>TAHAP 3: PENYELESAIAN (FINISHING)</b>]
        LOG_PARAM --> CHECK_DONE{Selesai Produksi?}
        LOG_DIM --> CHECK_DONE
        
        CHECK_DONE -- YA --> INPUT_FINAL[Input Total Qty OK & NG]
        INPUT_FINAL --> FINISH[Klik: FINISH PRODUCTION]
        FINISH --> GENERATE_TAG[<b>AUTO-GENERATE LOT TAG</b><br/><i>Cetak Label (Gambar 4)</i>]
    end

    %% Final
    GENERATE_TAG --> END((<b>WAREHOUSE READY</b>))

    %% Styling
    style START fill:#f8fafc,stroke:#475569,stroke-width:2px
    style TAHAP_1 fill:#eff6ff,stroke:#1d4ed8,color:#1e3a8a
    style TAHAP_2 fill:#fff7ed,stroke:#c2410c,color:#7c2d12
    style TAHAP_3 fill:#f0fdf4,stroke:#15803d,color:#14532d
    style END fill:#f8fafc,stroke:#475569,stroke-width:2px
```

---

## 📝 Penjelasan Detail Per Tahap

### 1. Persiapan: SPS Masterlist (Gambar 3)
Data standar (suhu, dimensi, yarn) ditarik otomatis dari database Masterlist. 
*   **Keunggulan**: Operator tidak perlu menghafal atau melihat kertas folder standar lagi.

### 2. Mulai: Now I'm Produce (Gambar 1)
Tahap verifikasi material sebelum mesin jalan.
*   **Input**: No. Lot & Berat Jenis (SG) Material.
*   **Aturan**: Tidak boleh jalan jika belum ada verifikasi Supervisor.

### 3. Monitoring: Parameter Setting (Gambar 5)
Kontrol kondisi mesin selama produksi berlangsung.
*   **Lokasi**: Tab "Parameter Setting" di Dashboard.
*   **Data**: Suhu Head, Cylinder, Kecepatan Ulir (Screw), dan Tekanan (Pressure).

### 4. Kontrol Kualitas: Dimension Report (Gambar 2)
Pengecekan fisik produk demi menjaga kualitas.
*   **Lokasi**: Tab "Dimension Report" di Dashboard.
*   **Data**: Diameter Dalam (ID), Tebal Layer, Panjang Potong, dll.
*   **Audit**: Sistem mencatat jam pengecekan untuk memastikan kontrol dilakukan rutin.

### 5. Akhir: Lot Tag Hose (Gambar 4)
Penutupan laporan dan identifikasi barang.
*   **Output**: Label "Lot Tag" otomatis Tercetak.
*   **Traceability**: Di Lot Tag ada QR Code yang jika dicariditambahkandanscan, akan muncul riwayat siapa yang memproduksi (Gambar 1) dan berapa dimensinya (Gambar 2).

---
*Dibuat oleh AI Antigravity untuk PT. Velasto Indonesia*
