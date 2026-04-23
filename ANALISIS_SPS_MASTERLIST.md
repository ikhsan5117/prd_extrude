# Analisis SPS Masterlist - Perbedaan Format dan Kolom

## 📋 Overview
Project ini mendukung 3 format SPS yang berbeda dengan penempatan kolom yang berbeda-beda:

### Format yang Didukung:
1. **CHS 3 Layer (isDig3L)**
2. **CHS 2 Layer (isDig2L)**
3. **Legacy / Non-CHS (isLegacy)**

---

## 🔍 Deteksi Format Otomatis

Sistem mendeteksi format berdasarkan:

### Metode 1: Title di Row 1 (Index 0)
- **CHS 3 LAYER** → isDig3L = true
- **CHS 2 LAYER** → isDig2L = true
- **NON-CHS** → isLegacy = true

### Metode 2: Fallback Detection di Row 5
- Kolom 107 = "Item" → CHS 3 Layer
- Kolom 89 = "Item" → CHS 2 Layer
- Kolom 50 = "ITEM" → Legacy

---

## 📊 Pemetaan Kolom (Column Mapping)

### Kolom Identifikasi Utama

| Field | CHS 3 Layer | CHS 2 Layer | Legacy |
|-------|-------------|-------------|--------|
| **ExcelId** | Kolom 0 | Kolom 0 | Kolom 0 |
| **No** | Kolom 1 | Kolom 1 | Kolom 1 |
| **Machine** | Kolom 2 | Kolom 2 | Kolom 2 |
| **DocumentNumber** | Kolom 3 | Kolom 3 | Kolom 3 |
| **RevisionNumber** | Kolom 5 | Kolom 5 | Kolom 3 |
| **Customer** | Kolom 6 | Kolom 6 | Kolom 4 |
| **RevisionDate** | Kolom 7 | Kolom 7 | Kolom 5 |
| **Formulasi** | Kolom 8 | Kolom 8 | Kolom 6 |
| **HoseType** | Kolom 10 | Kolom 8 | Kolom 7 |
| **Dimensi** | Kolom 11 | Kolom 9 | Kolom 8 |
| **Material** | Kolom 12 | Kolom 10 | Kolom 9 |
| **InnerTube** | Kolom 13 | Kolom 11 | Kolom 10 |
| **OuterCover** | Kolom 15 | Kolom 12 | Kolom 11 |
| **MiddleTube** | Kolom 14 | ❌ Kosong | ❌ Kosong |
| **ItemList** | **Kolom 106** | **Kolom 88** | **Kolom 49** |
| **MachineCode** | ❌ Tidak ada | ❌ Tidak ada | Kolom 51 |

### Use Limits Material

| Field | CHS 3 Layer | CHS 2 Layer | Legacy |
|-------|-------------|-------------|--------|
| **UseLimitsInner** | Kolom 16 | Kolom 13 | Kolom 12 |
| **UseLimitsMiddle** | Kolom 17 | ❌ Tidak ada | ❌ Tidak ada |
| **UseLimitsOuter** | Kolom 18 | Kolom 14 | Kolom 13 |

### Dies & Components

| Field | CHS 3 Layer | CHS 2 Layer | Legacy |
|-------|-------------|-------------|--------|
| **Yarn** | Kolom 20 | Kolom 15 | ❌ Tidak ada |
| **PitchYarn** | ❌ Tidak ada | Kolom 18 | ❌ Tidak ada |
| **TensionYarnInner** | Kolom 21 | Kolom 16 | ❌ Tidak ada |
| **TensionYarnOuter** | Kolom 22 | Kolom 17 | ❌ Tidak ada |
| **Nipple** | Kolom 23 | Kolom 19 | Kolom 14 |
| **TubeDie** | Kolom 24 | Kolom 20 | Kolom 15 |
| **MiddleDie** | Kolom 25 | Kolom 21 | ❌ Tidak ada |
| **CoverDie** | Kolom 26 | Kolom 22 | Kolom 16 |
| **SpacerDie** | Kolom 27 | ❌ Tidak ada | ❌ Tidak ada |
| **ADistance** | Kolom 28 | Kolom 26 | Kolom 27 |

### Mesh Screen

| Field | CHS 3 Layer | CHS 2 Layer | Legacy |
|-------|-------------|-------------|--------|
| **MeshScreen1** | Kolom 29 | Kolom 25 | Kolom 17 |
| **MeshScreen2** | Kolom 31 | Kolom 27 | Kolom 18 |
| **MeshScreen3** | Kolom 33 | ❌ Tidak ada | ❌ Tidak ada |

### Temperature Parameters - Material 1

| Field | CHS 3 Layer | CHS 2 Layer | Legacy |
|-------|-------------|-------------|--------|
| **HeadTemp1** | Kolom 34 | Kolom 31 | Kolom 20 |
| **Cylinder1_1** | Kolom 35 | Kolom 32 | Kolom 22 |
| **Cylinder2_1** | Kolom 36 | Kolom 33 | Kolom 24 |
| **Cylinder3_1** | Kolom 37 | Kolom 34 | ❌ Tidak ada |
| **ScrewTemp1** | Kolom 38 | Kolom 35 | Kolom 28 |
| **Feed1** | ❌ Tidak ada | ❌ Tidak ada | Kolom 26 |

### Temperature Parameters - Material 2

| Field | CHS 3 Layer | CHS 2 Layer | Legacy |
|-------|-------------|-------------|--------|
| **HeadTemp2** | Kolom 39 | Kolom 36 | Kolom 21 |
| **Cylinder1_2** | Kolom 40 | Kolom 37 | Kolom 23 |
| **Cylinder2_2** | Kolom 41 | Kolom 38 | Kolom 25 |
| **Cylinder3_2** | Kolom 42 | Kolom 39 | ❌ Tidak ada |
| **ScrewTemp2** | Kolom 43 | Kolom 40 | Kolom 29 |
| **Feed2** | ❌ Tidak ada | ❌ Tidak ada | Kolom 27 |

### Temperature Parameters - Material 3 (Only 3-Layer)

| Field | CHS 3 Layer | CHS 2 Layer | Legacy |
|-------|-------------|-------------|--------|
| **HeadTemp3** | Kolom 44 | ❌ Tidak ada | ❌ Tidak ada |
| **Cylinder1_3** | Kolom 45 | ❌ Tidak ada | ❌ Tidak ada |
| **Cylinder2_3** | Kolom 46 | ❌ Tidak ada | ❌ Tidak ada |
| **Cylinder3_3** | Kolom 47 | ❌ Tidak ada | ❌ Tidak ada |
| **ScrewTemp3** | Kolom 48 | ❌ Tidak ada | ❌ Tidak ada |

### Speed & Pressure Parameters

| Field | CHS 3 Layer | CHS 2 Layer | Legacy |
|-------|-------------|-------------|--------|
| **ScrewSpeed1** | Kolom 49 | Kolom 41 | Kolom 30 |
| **ScrewSpeed2** | Kolom 50 | Kolom 42 | Kolom 31 |
| **FeedRollRatio1** | ❌ Tidak ada | Kolom 43 | ❌ Tidak ada |
| **FeedRollRatio2** | ❌ Tidak ada | Kolom 44 | ❌ Tidak ada |
| **Pressure1** | Kolom 55 | Kolom 45 | Kolom 32 |
| **Pressure2** | Kolom 56 | Kolom 46 | Kolom 33 |
| **Pressure3** | Kolom 57 | ❌ Tidak ada | ❌ Tidak ada |
| **CurrentValue** | ❌ Tidak ada | Kolom 47 | ❌ Tidak ada |
| **AmMeter** | Kolom 58 | Kolom 48 | Kolom 34 |
| **AmMeter2** | Kolom 59 | Kolom 49 | ❌ Tidak ada |
| **OdSensor** | Kolom 61 | Kolom 49 | Kolom 35 |
| **PresetValue** | ❌ Tidak ada | Kolom 50 | ❌ Tidak ada |
| **ControlValue** | ❌ Tidak ada | Kolom 51 | ❌ Tidak ada |
| **SpiralPitchSetting** | ❌ Tidak ada | Kolom 52 | ❌ Tidak ada |
| **SpiralPitchDisplay** | ❌ Tidak ada | Kolom 53 | ❌ Tidak ada |
| **SpiralSpeed** | ❌ Tidak ada | Kolom 54 | ❌ Tidak ada |
| **HoseSpeed** | ❌ Tidak ada | Kolom 55 | ❌ Tidak ada |
| **UnsmoothSurface** | ❌ Tidak ada | Kolom 56 | ❌ Tidak ada |

### Marking & Process Parameters

| Field | CHS 3 Layer | CHS 2 Layer | Legacy |
|-------|-------------|-------------|--------|
| **MarkingSort** | Kolom 68 | Kolom 57 | Kolom 36 |
| **TextMarkingMaterial** | Kolom 69 | Kolom 58 | Kolom 37 |
| **MarkingColour** | Kolom 70 | Kolom 59 | Kolom 38 |
| **ChillerWaterTemp** | Kolom 71 | Kolom 60 | Kolom 39 |
| **DancerPosition** | ❌ Tidak ada | Kolom 61 | ❌ Tidak ada |
| **CaterpillarGap** | Kolom 72 | Kolom 62 | ❌ Tidak ada |
| **TakeUpConveyorSpeed** | Kolom 73 | Kolom 63 | Kolom 41 |
| **CoolConveyorSpeed** | Kolom 74 | Kolom 64 | ❌ Tidak ada |
| **ConveyorRatio** | ❌ Tidak ada | Kolom 65 | ❌ Tidak ada |

### Tolerance & Thickness Parameters

| Field | CHS 3 Layer | CHS 2 Layer | Legacy |
|-------|-------------|-------------|--------|
| **ToleranceInner** | Kolom 77 | Kolom 66 | Kolom 42 |
| **ToleranceOuter** | Kolom 78 | Kolom 67 | Kolom 43 |
| **TebalInner** | Kolom 79 | Kolom 68 | Kolom 44 |
| **TebalOuter** | Kolom 80 | Kolom 69 | Kolom 45 |
| **TebalTotal** | Kolom 81 | Kolom 70 | Kolom 46 |
| **SelisihTebal** | Kolom 82 | Kolom 70 | Kolom 47 |

### Detailed Quality Matrix (CHS 2 Layer)

| Field | Index (0-based) | Description |
|-------|-----------------|-------------|
| **Inner Target/Tol/LCL/Min/UCL/Max** | 71-76 | Detailed tolerance for Inner dimension |
| **Thick Target/Tol/LCL/Min/UCL/Max** | 77-82 | Detailed tolerance for Wall Thickness |
| **Total Target/Tol/LCL/Min/UCL/Max** | 83-88 | Detailed tolerance for Total Thickness |

---

## ⚠️ Masalah yang Teridentifikasi

### 1. **Kolom Item List Berbeda Jauh**
   - CHS 3 Layer: Kolom **106**
   - CHS 2 Layer: Kolom **88**
   - Legacy: Kolom **49**
   
   **Dampak**: Jika deteksi format gagal, data Item List tidak akan terbaca dengan benar.

### 2. **Kolom yang Hanya Ada di Format Tertentu**
   - **MiddleTube**: Hanya di CHS 3 Layer
   - **UseLimitsMiddle**: Hanya di CHS 3 Layer
   - **SpacerDie**: Hanya di CHS 3 Layer
   - **ADistance**: Hanya di CHS 3 Layer
   - **MeshScreen3**: Hanya di CHS 3 Layer
   - **Material 3 Parameters**: Hanya di CHS 3 Layer
   - **MachineCode**: Hanya di Legacy
   - **Feed1/Feed2**: Hanya di Legacy

### 3. **Sheet Name Berbeda**
   Sistem mencari sheet dengan nama:
   - "Parameter Setting" (prioritas pertama)
   - "Master 1" (fallback)

---

## ✅ Solusi yang Sudah Diimplementasikan

### 1. **Auto-Detection Format**
   Sistem otomatis mendeteksi format berdasarkan title dan posisi kolom "Item"

### 2. **Dynamic Column Mapping**
   Setiap format memiliki pemetaan kolom sendiri (idxItem, idxRev, dll)

### 3. **Fallback Item Column Detection**
   Jika deteksi format gagal, sistem mencari kolom "Item" di row 5 header

### 4. **Conditional Field Assignment**
   Field hanya diisi jika format mendukungnya (contoh: MiddleTube hanya untuk 3-Layer)

---

## 🛠️ Rekomendasi Perbaikan

### 1. **Validasi Sheet Name Lebih Ketat**
   ```csharp
   // Tambahkan logging untuk sheet yang ditemukan
   foreach(DataTable t in result.Tables) {
       Console.WriteLine($"Found sheet: {t.TableName}");
       if (t.TableName.Contains("Parameter Setting") || 
           t.TableName.Contains("Master 1")) {
           table = t; break;
       }
   }
   ```

### 2. **Tambahkan Validasi Format**
   ```csharp
   // Setelah deteksi format, tambahkan validasi
   if (!isDig3L && !isDig2L && !isLegacy) {
       throw new Exception("Format SPS tidak dikenali. Pastikan row 1 memiliki title 'CHS 3 LAYER', 'CHS 2 LAYER', atau 'NON-CHS'");
   }
   ```

### 3. **Tambahkan Error Message Lebih Detail**
   ```csharp
   TempData["ErrorMessage"] = $"Format: {detectedFormat}, Sheet: {table.TableName}, Item Col: {idxItem}, Total Rows: {table.Rows.Count}";
   ```

### 4. **Dokumentasi Template Excel**
   Buat template Excel standar untuk setiap format dengan kolom yang sudah ditandai jelas

---

## 📝 Cara Menggunakan

### Upload Excel SPS:
1. Pastikan Excel memiliki sheet "Parameter Setting" atau "Master 1"
2. Row 1 harus memiliki title format (CHS 3 LAYER / CHS 2 LAYER / NON-CHS)
3. Row 6 adalah baris pertama data (startRow = 6)
4. Kolom Item List harus berada di posisi yang sesuai format:
   - CHS 3 Layer: Kolom 106
   - CHS 2 Layer: Kolom 88
   - Legacy: Kolom 49

### Jika Upload Gagal:
1. Cek sheet name - harus "Parameter Setting" atau "Master 1"
2. Cek row 1 - harus ada title format yang jelas
3. Cek row 5 - harus ada header dengan angka/label kolom
4. Cek row 6 - baris pertama data harus dimulai dari sini

---

## 📊 Kesimpulan

Sistem sudah **cukup robust** untuk handle 3 format SPS yang berbeda, namun:
- **Perlu template Excel yang konsisten** untuk setiap format
- **Perlu dokumentasi clear** tentang struktur Excel yang diharapkan
- **Perlu error handling** yang lebih informatif untuk troubleshooting
- **Perlu validasi** bahwa Excel yang diupload sesuai format yang diharapkan
