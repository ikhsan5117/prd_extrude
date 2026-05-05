# 🎯 Panduan Cepat: Upload SPS Masterlist

## ✅ Yang Sudah Diperbaiki

### 1. **Deteksi Format Otomatis yang Lebih Baik**
   - ✓ Deteksi CHS 3 Layer (kolom Item: 106)
   - ✓ Deteksi CHS 2 Layer (kolom Item: 88)
   - ✓ Deteksi Legacy/Non-CHS (kolom Item: 49)
   - ✓ Fallback detection jika title tidak ada
   - ✓ Logging detail untuk troubleshooting

### 2. **Sheet Selection yang Lebih Smart**
   - ✓ Prioritas 1: Sheet "Parameter Setting"
   - ✓ Prioritas 2: Sheet "Master 1"
   - ✓ Prioritas 3: Sheet dengan kata "SPS", "Parameter", "Master"
   - ✓ Fallback: Sheet dengan data terbanyak

### 3. **Validasi Excel**
   - ✓ Cek jumlah kolom cukup
   - ✓ Cek jumlah baris minimal
   - ✓ Cek kolom kunci tidak kosong
   - ✓ Error message yang informatif

### 4. **Preview Feature** (BARU!)
   - ✓ Preview file sebelum import
   - ✓ Lihat format yang terdeteksi
   - ✓ Lihat sample data
   - ✓ Validasi sebelum import
   - ✓ Detection log untuk debugging

### 5. **Error Handling yang Lebih Baik**
   - ✓ Success message dengan detail lengkap
   - ✓ Error message dengan stack trace
   - ✓ Sheet detection log
   - ✓ Logging ke ILogger untuk monitoring

---

## 📋 Cara Menggunakan

### Step 1: Buka SPS Masterlist Page
Navigate ke: **MASTER DATA → SPS Master**

### Step 2: Klik "Sinkronisasi Excel"
Button hijau di kanan atas: **"🔼 Sinkronisasi Excel"**

### Step 3: Pilih File Excel
- Klik area upload atau browse file
- Pilih file .xlsx atau .xls
- File yang didukung:
  - ✅ Masterlist SPS CHS 3 Layer DIG.xlsx
  - ✅ Masterlist SPS CHS 2 Layer DIG.xlsx
  - ✅ Masterlist SPS Double Layer_Digitalisasi.xlsx

### Step 4: PREVIEW File (Recommended!)
- Setelah pilih file, klik button **"👁 Preview"**
- Review informasi:
  - ✅ Sheet yang dipilih
  - ✅ Format yang terdeteksi
  - ✅ Kolom Item yang digunakan
  - ✅ Sample data (5 baris pertama)
  - ✅ Estimasi jumlah records
  - ✅ Validasi passed/failed

### Step 5: Import
- Jika preview OK, klik **"☁ IMPORT SEKARANG"**
- Wait sampai proses selesai
- Lihat success message dengan detail:
  - Format yang terdeteksi
  - Sheet yang digunakan
  - Kolom Item
  - Jumlah data baru
  - Jumlah data diperbarui

---

## 🔍 Apa yang Harus Dicek di Preview

### ✅ Checklist Preview:

1. **Format Terdeteksi Benar?**
   - CHS 3 Layer untuk file 3 layer
   - CHS 2 Layer untuk file 2 layer
   - Legacy untuk file Non-CHS

2. **Sheet yang Dipilih Benar?**
   - Harus "Parameter Setting" atau "Master 1"
   - Bukan sheet lain yang tidak relevan

3. **Kolom Item Benar?**
   - CHS 3 Layer: Kolom 106 (DG)
   - CHS 2 Layer: Kolom 88 (CJ)
   - Legacy: Kolom 49 (AW)

4. **Sample Data Terisi?**
   - ID Excel ada (contoh: 1-1, 2-1)
   - Machine terisi
   - Item List terisi (bisa comma-separated)
   - Customer terisi
   - Dimensi terisi

5. **Validasi Passed?**
   - ✅ Valid = hijau = siap import
   - ⚠ Ada Masalah = kuning/merah = perbaiki dulu

---

## ⚠️ Troubleshooting

### Problem 1: "Format tidak terdeteksi"

**Gejala:**
```
Format Terdeteksi: UNKNOWN
```

**Solusi:**
1. Buka Excel, cek Row 1, Kolom B (kolom ke-2)
2. Harus ada text:
   - "CHS 3 LAYER" untuk 3 layer
   - "CHS 2 LAYER" untuk 2 layer
   - "NON-CHS" untuk legacy
3. Atau cek Row 5/6, kolom Item (107/89/50) harus ada tulisan "Item"
4. Jika tidak ada, tambahkan manual di Row 1

**Contoh yang benar:**
```
Row 1: [Empty] | CHS 3 LAYER | [Other data...]
```

---

### Problem 2: "Sheet tidak ditemukan"

**Gejala:**
```
Sheet: Sheet1 (bukan Parameter Setting)
```

**Solusi:**
1. Buka Excel
2. Rename sheet aktif menjadi "Parameter Setting"
3. Atau rename menjadi "Master 1"
4. Save Excel
5. Upload ulang

---

### Problem 3: "Kolom tidak cukup"

**Gejala:**
```
Expected: 107 columns, Found: 80
```

**Solusi:**
1. File Excel Anda kehilangan kolom
2. Bandingkan dengan template asli
3. Pastikan tidak ada kolom yang dihapus
4. Restore dari backup atau re-download template

**Jumlah kolom minimum:**
- CHS 3 Layer: 107 kolom (A sampai DG)
- CHS 2 Layer: 89 kolom (A sampai CJ)
- Legacy: 50 kolom (A sampai AX)

---

### Problem 4: "Item List kosong"

**Gejala:**
```
Sample Data menunjukkan Item List: (kosong)
```

**Solusi:**
1. Cek kolom Item di Excel (kolom 106/88/49)
2. Pastikan ada data item, bisa:
   - Single item: `ITEM-001`
   - Multiple items: `ITEM-001, ITEM-002, ITEM-003`
3. Jika kosong, isi dengan item code yang sesuai
4. Jangan ada space berlebih

---

### Problem 5: "Data tidak masuk semua"

**Gejala:**
- Preview: Estimasi 100 records
- Hasil import: Hanya 50 records

**Kemungkinan:**
1. **ID Excel duplicate** → Data di-update, bukan ditambah
2. **Row kosong** di tengah data → Import berhenti
3. **ID Excel kosong** → Row diskip

**Solusi:**
1. Pastikan ID Excel unique dan tidak kosong
2. Hapus row kosong di tengah data
3. Data mulai dari Row 6 (baris ke-6)
4. Jangan ada merged cells

---

### Problem 6: "Error: Index out of range"

**Gejala:**
```
Error: Index was out of range. Must be non-negative and less than the size of the collection
```

**Penyebab:**
- Kolom yang dicari tidak ada (misal mencari kolom 106 tapi file hanya 80 kolom)

**Solusi:**
1. Gunakan **Preview** untuk cek berapa kolom yang ada
2. Pastikan format Excel sesuai dengan template
3. Jangan edit/hapus struktur kolom template
4. Re-download template jika perlu

---

## 📊 Perbedaan 3 Format SPS

### Format 1: CHS 3 Layer
```
✓ Ada 3 material (Inner, Middle, Outer)
✓ Ada MiddleTube
✓ Ada SpacerDie, ADistance
✓ Ada MeshScreen3
✓ Ada parameter Material 3 (HeadTemp3, Cylinder3, dll)
✓ Kolom Item: 106 (DG)
✓ Total kolom: 107+
```

### Format 2: CHS 2 Layer
```
✓ Ada 2 material (Inner, Outer)
✗ Tidak ada MiddleTube
✗ Tidak ada SpacerDie, ADistance
✗ Tidak ada MeshScreen3
✗ Tidak ada parameter Material 3
✓ Kolom Item: 88 (CJ)
✓ Total kolom: 89+
```

### Format 3: Legacy / Non-CHS
```
✓ Ada 2 material (Inner, Outer)
✗ Tidak ada MiddleTube
✗ Tidak ada Yarn, TensionYarn
✗ Tidak ada MiddleDie
✓ Ada MachineCode (kolom 51)
✓ Ada Feed1, Feed2
✓ Kolom Item: 49 (AW)
✓ Total kolom: 50+
```

---

## 🎓 Tips & Best Practices

### ✅ DO's:
1. **Selalu gunakan Preview** sebelum import
2. **Backup database** sebelum import data besar
3. **Test dengan 1 file** dulu sebelum upload semua
4. **Gunakan template asli** jangan edit struktur
5. **Cek duplicate ID Excel** sebelum upload
6. **Hapus row kosong** di tengah data
7. **Save Excel** dalam format .xlsx

### ❌ DON'Ts:
1. ❌ Jangan hapus/tambah kolom di template
2. ❌ Jangan merge cells di area data
3. ❌ Jangan ubah nama sheet sembarangan
4. ❌ Jangan upload tanpa preview
5. ❌ Jangan edit saat upload sedang proses
6. ❌ Jangan skip validasi error
7. ❌ Jangan langsung clear data tanpa backup

---

## 🚀 Advanced: Preview API

Untuk testing via API:

### Endpoint:
```
POST /MasterlistSpsDoubleLayers/PreviewExcel
Content-Type: multipart/form-data
```

### Form Data:
```
file: [Excel file]
```

### Response JSON:
```json
{
  "success": true,
  "fileName": "Masterlist SPS CHS 3 Layer DIG.xlsx",
  "fileSize": 524288,
  "sheets": [
    { "name": "Parameter Setting", "rows": 150, "columns": 120 }
  ],
  "selectedSheet": "Parameter Setting",
  "detectedFormat": "CHS 3 LAYER",
  "isValid": true,
  "validationError": "",
  "detectionLog": "...",
  "expectedItemColumn": 106,
  "totalRows": 150,
  "totalColumns": 120,
  "sampleData": [...],
  "estimatedRecords": "~144 records"
}
```

---

## 📞 Need Help?

Jika masih ada masalah setelah mengikuti panduan ini:

1. **Gunakan Preview Feature** untuk lihat detection log
2. **Screenshot error message** yang muncul
3. **Cek apakah format Excel sesuai template**
4. **Review ANALISIS_SPS_MASTERLIST.md** untuk detail kolom
5. **Review SOLUSI_SPS_UPLOAD.md** untuk troubleshooting lengkap

**File yang sudah dibuat:**
- ✅ [ANALISIS_SPS_MASTERLIST.md](ANALISIS_SPS_MASTERLIST.md) - Analisis lengkap perbedaan format
- ✅ [SOLUSI_SPS_UPLOAD.md](SOLUSI_SPS_UPLOAD.md) - Solusi dan perbaikan detail
- ✅ [PANDUAN_CEPAT.md](PANDUAN_CEPAT.md) - Panduan ini

**Improvements yang sudah diimplementasikan:**
- ✅ Enhanced format detection dengan logging
- ✅ Smart sheet selection
- ✅ Excel structure validation
- ✅ Preview mode dengan sample data
- ✅ Better error handling & messages
- ✅ ILogger integration untuk monitoring
- ✅ UI improvements untuk tampilan message

---

## 🎉 Summary

Sistem SPS Upload sekarang sudah **jauh lebih robust** dengan:
1. ✅ Auto-detection 3 format berbeda
2. ✅ Preview sebelum import
3. ✅ Validasi dan error handling yang baik
4. ✅ Logging detail untuk troubleshooting
5. ✅ UI yang informatif

**Selamat menggunakan! 🚀**
