# 🔧 Solusi Upload SPS Masterlist - Perbaikan & Peningkatan

## 🎯 Masalah yang Ingin Diselesaikan

1. **Penempatan kolom tidak konsisten** antar 3 file SPS
2. **Ada kolom yang kurang** di beberapa file
3. **Deteksi format kadang gagal**
4. **Error message kurang informatif**

---

## ✅ Perbaikan yang Akan Dilakukan

### 1. **Enhanced Format Detection**
Tambahkan logging dan validasi yang lebih baik untuk mendeteksi format

### 2. **Better Error Handling**
Error message yang lebih spesifik tentang:
- Sheet mana yang dibaca
- Format apa yang terdeteksi
- Kolom mana yang bermasalah
- Berapa banyak data yang berhasil/gagal

### 3. **Column Validation**
Validasi bahwa kolom-kolom kunci ada di posisi yang benar

### 4. **Flexible Sheet Detection**
Tidak hanya mencari "Parameter Setting" tapi juga variasi seperti:
- "Parameter Setting"
- "Master 1"
- "SPS"
- "Data"
- Sheet yang memiliki data paling banyak

### 5. **Preview Mode**
Mode untuk melihat struktur Excel sebelum import

---

## 🔨 Implementasi Perbaikan

### Feature 1: Enhanced Logging & Detection

```csharp
// Tambahkan method helper untuk logging
private string DetectFormatWithLogging(DataTable table, out bool isDig3L, out bool isDig2L, out bool isLegacy)
{
    isDig3L = isDig2L = isLegacy = false;
    var log = new StringBuilder();
    
    log.AppendLine($"Sheet: {table.TableName}");
    log.AppendLine($"Total Rows: {table.Rows.Count}");
    log.AppendLine($"Total Columns: {table.Columns.Count}");
    
    if (table.Rows.Count > 0) {
        string title = GetV(table.Rows[0], 1).ToUpper();
        log.AppendLine($"Title (Row 0, Col 1): {title}");
        
        if (title.Contains("CHS 3 LAYER")) {
            isDig3L = true;
            log.AppendLine("✓ Detected: CHS 3 LAYER");
        }
        else if (title.Contains("CHS 2 LAYER")) {
            isDig2L = true;
            log.AppendLine("✓ Detected: CHS 2 LAYER");
        }
        else if (title.Contains("NON-CHS")) {
            isLegacy = true;
            log.AppendLine("✓ Detected: LEGACY/NON-CHS");
        }
    }
    
    // Fallback detection
    if (!isDig3L && !isDig2L && !isLegacy && table.Rows.Count > 5) {
        log.AppendLine("Primary detection failed, trying fallback...");
        
        string col107 = GetV(table.Rows[5], 107);
        string col89 = GetV(table.Rows[5], 89);
        string col50 = GetV(table.Rows[5], 50);
        
        log.AppendLine($"Row 5, Col 107: {col107}");
        log.AppendLine($"Row 5, Col 89: {col89}");
        log.AppendLine($"Row 5, Col 50: {col50}");
        
        if (col107.Equals("Item", StringComparison.OrdinalIgnoreCase)) {
            isDig3L = true;
            log.AppendLine("✓ Fallback Detected: CHS 3 LAYER (Col 107)");
        }
        else if (col89.Equals("Item", StringComparison.OrdinalIgnoreCase)) {
            isDig2L = true;
            log.AppendLine("✓ Fallback Detected: CHS 2 LAYER (Col 89)");
        }
        else if (col50.Equals("ITEM", StringComparison.OrdinalIgnoreCase)) {
            isLegacy = true;
            log.AppendLine("✓ Fallback Detected: LEGACY (Col 50)");
        }
    }
    
    if (!isDig3L && !isDig2L && !isLegacy) {
        log.AppendLine("❌ Format tidak terdeteksi!");
    }
    
    return log.ToString();
}
```

### Feature 2: Smart Sheet Selection

```csharp
private DataTable SelectBestSheet(DataSet result)
{
    var log = new StringBuilder();
    log.AppendLine("=== Mencari Sheet yang Tepat ===");
    
    // Priority 1: Sheet dengan nama yang mengandung keyword
    string[] keywords = { "Parameter Setting", "Master 1", "SPS", "Parameter", "Master" };
    
    foreach (DataTable table in result.Tables) {
        log.AppendLine($"Sheet: '{table.TableName}' ({table.Rows.Count} rows, {table.Columns.Count} cols)");
        
        foreach (var keyword in keywords) {
            if (table.TableName.Contains(keyword, StringComparison.OrdinalIgnoreCase)) {
                log.AppendLine($"✓ Selected: {table.TableName} (matched keyword: {keyword})");
                TempData["SheetLog"] = log.ToString();
                return table;
            }
        }
    }
    
    // Priority 2: Sheet dengan row terbanyak (kemungkinan data sheet)
    var largestSheet = result.Tables.Cast<DataTable>()
        .OrderByDescending(t => t.Rows.Count)
        .FirstOrDefault();
    
    if (largestSheet != null) {
        log.AppendLine($"✓ Selected: {largestSheet.TableName} (largest sheet with {largestSheet.Rows.Count} rows)");
        TempData["SheetLog"] = log.ToString();
        return largestSheet;
    }
    
    // Fallback: Sheet pertama
    log.AppendLine($"⚠ Using first sheet: {result.Tables[0].TableName}");
    TempData["SheetLog"] = log.ToString();
    return result.Tables[0];
}
```

### Feature 3: Column Validation

```csharp
private bool ValidateColumns(DataTable table, bool isDig3L, bool isDig2L, bool isLegacy, out string errorMsg)
{
    errorMsg = "";
    int expectedItemCol = isDig3L ? 106 : (isDig2L ? 88 : 49);
    
    // Check if table has enough columns
    if (table.Columns.Count < expectedItemCol) {
        errorMsg = $"Excel tidak memiliki cukup kolom. Expected: {expectedItemCol + 1}, Found: {table.Columns.Count}";
        return false;
    }
    
    // Check if Item column exists in row 5 header
    if (table.Rows.Count > 5) {
        string itemHeader = GetV(table.Rows[5], expectedItemCol);
        if (!itemHeader.Contains("Item", StringComparison.OrdinalIgnoreCase) && 
            !itemHeader.Equals(expectedItemCol.ToString())) {
            errorMsg = $"Kolom Item tidak ditemukan di posisi {expectedItemCol}. Found: '{itemHeader}'";
            return false;
        }
    }
    
    return true;
}
```

### Feature 4: Preview Mode API

```csharp
[HttpPost]
public async Task<IActionResult> PreviewExcel(IFormFile file)
{
    if (file == null || file.Length == 0) 
        return Json(new { success = false, message = "File tidak ditemukan" });

    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

    try
    {
        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet();
                
                // List all sheets
                var sheets = result.Tables.Cast<DataTable>()
                    .Select(t => new {
                        name = t.TableName,
                        rows = t.Rows.Count,
                        columns = t.Columns.Count
                    }).ToList();
                
                // Select best sheet
                var table = SelectBestSheet(result);
                
                // Detect format
                bool isDig3L, isDig2L, isLegacy;
                string detectionLog = DetectFormatWithLogging(table, out isDig3L, out isDig2L, out isLegacy);
                
                string format = isDig3L ? "CHS 3 LAYER" : 
                               (isDig2L ? "CHS 2 LAYER" : 
                               (isLegacy ? "LEGACY" : "UNKNOWN"));
                
                // Validate columns
                string errorMsg;
                bool isValid = ValidateColumns(table, isDig3L, isDig2L, isLegacy, out errorMsg);
                
                // Get sample data (first 3 rows after header)
                var sampleData = new List<object>();
                int startRow = 6;
                for (int i = startRow; i < Math.Min(startRow + 3, table.Rows.Count); i++) {
                    var row = table.Rows[i];
                    sampleData.Add(new {
                        idExcel = GetV(row, 0),
                        no = GetV(row, 1),
                        machine = GetV(row, 2),
                        itemList = GetV(row, isDig3L ? 106 : (isDig2L ? 88 : 49))
                    });
                }
                
                return Json(new {
                    success = true,
                    sheets = sheets,
                    selectedSheet = table.TableName,
                    detectedFormat = format,
                    isValid = isValid,
                    validationError = errorMsg,
                    detectionLog = detectionLog,
                    sampleData = sampleData,
                    expectedItemColumn = isDig3L ? 106 : (isDig2L ? 88 : 49)
                });
            }
        }
    }
    catch (Exception ex) {
        return Json(new { success = false, message = ex.Message });
    }
}
```

---

## 📋 Checklist Upload Excel

Gunakan checklist ini sebelum upload:

### ✅ Struktur File Excel
- [ ] File format: .xlsx atau .xls
- [ ] Ada sheet "Parameter Setting" atau "Master 1"
- [ ] Row 1 memiliki title format yang jelas
- [ ] Row 5 memiliki header kolom (angka atau label)
- [ ] Row 6 adalah baris pertama data
- [ ] Tidak ada merged cells di area data

### ✅ Kolom Kunci (sesuai format)
**CHS 3 Layer:**
- [ ] Kolom 106: Item List
- [ ] Kolom 0: ID Excel
- [ ] Kolom 2: Machine
- [ ] Kolom 6: Customer

**CHS 2 Layer:**
- [ ] Kolom 88: Item List
- [ ] Kolom 0: ID Excel
- [ ] Kolom 2: Machine
- [ ] Kolom 6: Customer

**Legacy:**
- [ ] Kolom 49: Item List
- [ ] Kolom 0: ID Excel
- [ ] Kolom 2: Machine
- [ ] Kolom 4: Customer

### ✅ Data Quality
- [ ] ID Excel tidak kosong dan unique
- [ ] Item List terisi (bisa comma-separated)
- [ ] Machine name terisi
- [ ] Tidak ada row kosong di tengah data

---

## 🚀 Cara Menggunakan Perbaikan

### Step 1: Gunakan Preview Mode
Sebelum import, gunakan endpoint `/MasterlistSpsDoubleLayers/PreviewExcel` untuk:
- Melihat sheet apa saja yang ada
- Melihat format apa yang terdeteksi
- Melihat apakah validasi passed
- Melihat sample data yang akan diimport

### Step 2: Import dengan Logging
Setelah yakin, lakukan import. Sistem akan memberikan feedback:
- Format yang terdeteksi
- Jumlah data baru vs updated
- Kolom Item yang digunakan
- Error jika ada

### Step 3: Verify Results
Cek di halaman Index apakah data sudah masuk dengan benar:
- ID Excel sesuai
- Machine sesuai
- Item List terbaca
- Parameter-parameter terisi

---

## 🐛 Troubleshooting

### Problem: "Format tidak terdeteksi"
**Solusi:**
1. Cek Row 1, Kolom 1 - harus ada "CHS 3 LAYER", "CHS 2 LAYER", atau "NON-CHS"
2. Jika tidak ada, cek Row 5 - harus ada "Item" di kolom 107/89/50
3. Tambahkan title format secara manual di Row 1

### Problem: "Kolom tidak cukup"
**Solusi:**
1. CHS 3 Layer butuh minimal 107 kolom
2. CHS 2 Layer butuh minimal 89 kolom
3. Legacy butuh minimal 50 kolom
4. Jangan hapus kolom-kolom di Excel

### Problem: "Sheet tidak ditemukan"
**Solusi:**
1. Rename sheet menjadi "Parameter Setting"
2. Atau rename menjadi "Master 1"
3. Atau pastikan sheet berisi data paling banyak

### Problem: "Item List kosong"
**Solusi:**
1. Cek kolom Item List (106/88/49) memiliki data
2. Data bisa comma-separated: "ITEM1, ITEM2, ITEM3"
3. Pastikan tidak ada space berlebih
4. Pastikan bukan formula Excel yang error

### Problem: "Data tidak masuk semua"
**Solusi:**
1. Cek ID Excel - harus unique per item
2. Jika ada duplicate ID + Item, data akan di-update, bukan ditambah
3. Cek startRow = 6 - data harus mulai dari baris ke-6
4. Jangan ada row kosong di tengah data

---

## 📊 Testing Plan

### Test Case 1: Upload CHS 3 Layer
- File: Masterlist SPS CHS 3 Layer DIG.xlsx
- Expected: Deteksi format 3-Layer, kolom 106 untuk Item
- Verify: MiddleTube, SpacerDie, MeshScreen3 terisi

### Test Case 2: Upload CHS 2 Layer
- File: Masterlist SPS CHS 2 Layer DIG.xlsx
- Expected: Deteksi format 2-Layer, kolom 88 untuk Item
- Verify: Tidak ada MiddleTube, tidak ada Material 3 parameters

### Test Case 3: Upload Legacy
- File: Masterlist SPS Double Layer_Digitalisasi.xlsx
- Expected: Deteksi format Legacy, kolom 49 untuk Item
- Verify: MachineCode terisi, Feed1/Feed2 terisi

### Test Case 4: Mixed Upload
- Upload semua 3 file secara berurutan
- Expected: Semua data masuk tanpa conflict
- Verify: Total data = sum of all items from all files

---

## 💡 Tips & Best Practices

1. **Backup Database** sebelum import data besar
2. **Gunakan Clear Data** jika ingin reset dan import ulang
3. **Upload satu file** dulu untuk testing sebelum upload semua
4. **Cek log message** setelah upload untuk memastikan sukses
5. **Jangan edit Excel** saat sedang proses upload
6. **Gunakan template** yang sudah validated untuk data baru

---

## 📞 Support

Jika masih ada masalah setelah mengikuti panduan ini:

1. Capture screenshot error message
2. Export Excel yang bermasalah
3. Catat: format apa (3-Layer/2-Layer/Legacy)
4. Catat: berapa banyak data yang harusnya masuk
5. Catat: apa yang terjadi (error, data kosong, data salah)

Sistem sudah dilengkapi dengan logging yang detail untuk troubleshooting!
