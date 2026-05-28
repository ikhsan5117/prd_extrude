# 🧪 TEST PLAN: Fitur Edit SPS Master (Setelah Fix)

## ✅ YANG SUDAH DIPERBAIKI

### Fix #1: ViewBag.SyncItemCount Sekarang Akurat
**Sebelum:**
```csharp
ViewBag.SyncItemCount = 0;  // ❌ Selalu 0
```

**Sesudah:**
```csharp
// Hitung jumlah items lain yang akan terpengaruh oleh sync
int syncCount = 0;
if (!string.IsNullOrEmpty(noDoc.DocumentNumber) && noDoc.ItemLists != null && noDoc.ItemLists.Any())
{
    syncCount = noDoc.ItemLists.Count - 1; // Exclude current item
    if (syncCount < 0) syncCount = 0;
}
ViewBag.SyncItemCount = syncCount;
```

### Fix #2: Parameter syncToAll Sekarang Berfungsi
**Sebelum:**
```csharp
// ❌ Sync selalu terjadi, parameter tidak digunakan
SpsMapper.CopyProperties(item, noDoc);
_context.Update(noDoc);
```

**Sesudah:**
```csharp
if (syncToAll)
{
    // SYNC MODE: Update parent SpsNoDocs (affects all items)
    // ... update existing parent
}
else
{
    // SINGLE EDIT MODE: Create new SpsNoDocs for this item only
    var newNoDoc = SpsMapper.ToSpsNoDoc(item);
    // ... create new parent, move item to new parent
}
```

---

## 📋 TEST CASES

### ✅ Test Case 1: ViewBag.SyncItemCount - Menampilkan Jumlah Item yang Akan Ter-Sync

**Skenario:**
1. Cari item dengan NO.DOC yang memiliki multiple items (misal: "PM1270")
2. Klik tombol **Edit** pada salah satu item

**Expected Result:**
- ✅ Alert muncul: "Menyimpan akan mengubah: **X item** sekaligus"
- ✅ Badge menampilkan: "**X item** sekaligus" (bukan "tidak ada item lain")
- ✅ Detail menampilkan: "ditemukan **X item lain** dengan No. DOC yang sama"

**SQL untuk Verifikasi:**
```sql
-- Cari NO.DOC dengan multiple items
SELECT 
    sn.DocumentNumber,
    sn.Machine,
    COUNT(sil.Id) AS JumlahItems,
    STRING_AGG(sil.ItemList, ', ') AS Items
FROM SpsNoDocs sn
LEFT JOIN SpsItemLists sil ON sil.SpsNoDocId = sn.Id
WHERE sn.DocumentNumber IS NOT NULL AND sn.DocumentNumber <> ''
GROUP BY sn.Id, sn.DocumentNumber, sn.Machine
HAVING COUNT(sil.Id) > 1
ORDER BY COUNT(sil.Id) DESC;
```

---

### ✅ Test Case 2: Sync Mode (Checkbox ✓ Checked) - Update Semua Item dengan NO.DOC Sama

**Skenario:**
1. Edit item dengan NO.DOC "PM1270" yang memiliki 3 items (22-0, 22-b, 22-1)
2. Pastikan checkbox **"Terapkan ke semua item..."** ✅ **CHECKED**
3. Ubah nilai (misal: HeadTemp1_Asli dari 150 → 155)
4. Klik **UPDATE & SYNC MASSAL**

**Expected Result:**
- ✅ Success message: "✅ Berhasil! 3 item dengan NO.DOC 'PM1270' telah disinkronkan."
- ✅ Semua 3 items (22-0, 22-b, 22-1) sekarang punya HeadTemp1_Asli = 155
- ✅ Ketiga items tetap share SpsNoDocs yang sama (Id tidak berubah)

**SQL untuk Verifikasi:**
```sql
-- Cek apakah semua items dengan NO.DOC "PM1270" punya nilai yang sama
SELECT 
    sn.Id AS SpsNoDocId,
    sn.DocumentNumber,
    sn.HeadTemp1_Asli,
    sil.ItemList
FROM SpsNoDocs sn
LEFT JOIN SpsItemLists sil ON sil.SpsNoDocId = sn.Id
WHERE sn.DocumentNumber = 'PM1270'
ORDER BY sil.ItemList;

-- Expected: Semua rows punya HeadTemp1_Asli = 155 dan SpsNoDocId yang SAMA
```

---

### ✅ Test Case 3: Single Edit Mode (Checkbox ✗ Unchecked) - Update Hanya 1 Item

**Skenario:**
1. Edit item dengan NO.DOC "PM1270" yang memiliki 3 items (22-0, 22-b, 22-1)
2. **UNCHECK** checkbox "Terapkan ke semua item..." ❌
3. Ubah nilai (misal: HeadTemp1_Asli dari 155 → 160)
4. Klik **UPDATE HANYA ITEM INI** (tombol berubah otomatis)

**Expected Result:**
- ✅ Success message: "✅ Item ini berhasil diupdate (tidak mempengaruhi item lain dengan NO.DOC sama)."
- ✅ Hanya item yang di-edit (misal: 22-0) punya HeadTemp1_Asli = 160
- ✅ Item lain (22-b, 22-1) tetap HeadTemp1_Asli = 155
- ✅ Item yang di-edit sekarang punya SpsNoDocs baru (Id berbeda)

**SQL untuk Verifikasi:**
```sql
-- Cek apakah item yang di-edit sekarang punya SpsNoDocId berbeda
SELECT 
    sn.Id AS SpsNoDocId,
    sn.DocumentNumber,
    sn.HeadTemp1_Asli,
    sil.ItemList
FROM SpsNoDocs sn
LEFT JOIN SpsItemLists sil ON sil.SpsNoDocId = sn.Id
WHERE sn.DocumentNumber = 'PM1270'
ORDER BY sn.Id, sil.ItemList;

-- Expected:
-- - Item 22-0: SpsNoDocId = NEW_ID, HeadTemp1_Asli = 160
-- - Item 22-b, 22-1: SpsNoDocId = OLD_ID, HeadTemp1_Asli = 155
```

---

### ✅ Test Case 4: Edit Item dengan NO.DOC Kosong

**Skenario:**
1. Edit item dengan DocumentNumber = NULL atau kosong
2. Checkbox sync tidak muncul (karena tidak ada item lain)
3. Ubah nilai
4. Save

**Expected Result:**
- ✅ Alert menampilkan: "tidak ada item lain dengan No. DOC yang sama"
- ✅ Tidak ada checkbox sync
- ✅ Update berhasil tanpa masalah

---

### ✅ Test Case 5: Verifikasi UI - Tombol Berubah Dinamis

**Skenario:**
1. Edit item dengan NO.DOC yang memiliki multiple items
2. Checkbox **CHECKED** (default)

**Expected Result:**
- ✅ Tombol: **"UPDATE & SYNC MASSAL"** (orange/warning)
- ✅ Status box: orange background

**Skenario:**
1. **UNCHECK** checkbox

**Expected Result:**
- ✅ Tombol berubah: **"UPDATE HANYA ITEM INI"** (green/success)
- ✅ Status box: green background
- ✅ Text berubah sesuai

---

## 🔧 CARA TESTING MANUAL

### Step 1: Persiapan Data
```sql
-- Cari NO.DOC dengan multiple items untuk testing
SELECT TOP 5
    sn.DocumentNumber,
    sn.Machine,
    COUNT(sil.Id) AS JumlahItems,
    STRING_AGG(sil.ItemList, ', ') AS Items
FROM SpsNoDocs sn
LEFT JOIN SpsItemLists sil ON sil.SpsNoDocId = sn.Id
WHERE sn.DocumentNumber IS NOT NULL AND sn.DocumentNumber <> ''
GROUP BY sn.Id, sn.DocumentNumber, sn.Machine
HAVING COUNT(sil.Id) > 1
ORDER BY COUNT(sil.Id) DESC;
```

### Step 2: Test Sync Mode
1. Buka aplikasi → Menu SPS Master
2. Filter NO.DOC dengan hasil query di atas
3. Klik **Edit** pada item pertama
4. Verify: Alert menampilkan jumlah item yang benar
5. Checkbox **CHECKED**, ubah nilai (HeadTemp1_Asli)
6. Klik **UPDATE & SYNC MASSAL**
7. Verify: Semua items dengan NO.DOC sama ter-update

### Step 3: Test Single Edit Mode
1. Klik **Edit** lagi pada item yang sama
2. **UNCHECK** checkbox
3. Verify: Tombol berubah jadi "UPDATE HANYA ITEM INI"
4. Ubah nilai (HeadTemp1_Asli ke nilai berbeda)
5. Klik **UPDATE HANYA ITEM INI**
6. Verify: Hanya item ini yang berubah, item lain tidak terpengaruh

### Step 4: Verify Database
```sql
-- Cek hasil akhir
SELECT 
    sn.Id AS SpsNoDocId,
    sn.DocumentNumber,
    sn.Machine,
    sn.HeadTemp1_Asli,
    sil.ItemList
FROM SpsNoDocs sn
LEFT JOIN SpsItemLists sil ON sil.SpsNoDocId = sn.Id
WHERE sn.DocumentNumber = 'PM1270'  -- Ganti dengan NO.DOC yang Anda test
ORDER BY sn.Id, sil.ItemList;
```

---

## 📊 EXPECTED BEHAVIOR SUMMARY

| Mode | Checkbox | Hasil |
|------|----------|-------|
| **Sync Mode** | ✅ Checked | Update parent SpsNoDocs → Semua items ter-sync |
| **Single Edit** | ❌ Unchecked | Create new SpsNoDocs → Hanya 1 item berubah |

### Database Structure After Testing:
```
SEBELUM EDIT:
SpsNoDocs (Id: 100)
├─ DocumentNumber: "PM1270"
├─ HeadTemp1_Asli: 150
└─ ItemLists:
    ├─ 22-0
    ├─ 22-b
    └─ 22-1

SETELAH SYNC MODE (checkbox ✓):
SpsNoDocs (Id: 100)  <-- SAMA
├─ DocumentNumber: "PM1270"
├─ HeadTemp1_Asli: 155  <-- BERUBAH
└─ ItemLists:
    ├─ 22-0  <-- SEMUA TER-UPDATE
    ├─ 22-b
    └─ 22-1

SETELAH SINGLE EDIT MODE (checkbox ✗):
SpsNoDocs (Id: 100)
├─ DocumentNumber: "PM1270"
├─ HeadTemp1_Asli: 155
└─ ItemLists:
    ├─ 22-b  <-- TIDAK BERUBAH
    └─ 22-1

SpsNoDocs (Id: 200)  <-- BARU!
├─ DocumentNumber: "PM1270"
├─ HeadTemp1_Asli: 160  <-- NILAI BARU
└─ ItemLists:
    └─ 22-0  <-- PINDAH KE PARENT BARU
```

---

## ✅ CHECKLIST

- [ ] Test Case 1: ViewBag.SyncItemCount menampilkan jumlah yang benar
- [ ] Test Case 2: Sync Mode (checkbox checked) - semua items ter-update
- [ ] Test Case 3: Single Edit Mode (checkbox unchecked) - hanya 1 item ter-update
- [ ] Test Case 4: Edit item dengan NO.DOC kosong
- [ ] Test Case 5: UI tombol berubah dinamis sesuai checkbox
- [ ] Verify: Success message menampilkan jumlah items yang ter-sync
- [ ] Verify: Database structure correct setelah testing

---

## 🎯 SIGN OFF

**Tested By:** _________________  
**Date:** _________________  
**Result:** ☐ Pass  ☐ Fail  
**Notes:** _________________
