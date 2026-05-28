-- ================================================================
-- TEST SCRIPT: Verifikasi Fitur Edit Sync di SPS Master
-- ================================================================

-- 1. Cek struktur database SpsNoDoc dan SpsItemList
PRINT '=== 1. STRUKTUR DATABASE ==='
SELECT 'SpsNoDocs' AS TableName, COUNT(*) AS TotalRecords FROM SpsNoDocs;
SELECT 'SpsItemLists' AS TableName, COUNT(*) AS TotalRecords FROM SpsItemLists;

PRINT '';
PRINT '=== 2. SAMPLE DATA - Items dengan NO.DOC yang sama ==='
-- Cek apakah ada items dengan DocumentNumber yang sama
SELECT TOP 20
    sn.Id AS SpsNoDocId,
    sn.ExcelId,
    sn.DocumentNumber,
    sn.Machine,
    sn.Formulasi,
    sil.ItemList,
    COUNT(*) OVER (PARTITION BY sn.DocumentNumber, sn.Machine, sn.Formulasi) AS ItemCount
FROM SpsNoDocs sn
LEFT JOIN SpsItemLists sil ON sil.SpsNoDocId = sn.Id
WHERE sn.DocumentNumber IS NOT NULL AND sn.DocumentNumber <> ''
ORDER BY sn.DocumentNumber, sn.Machine, sn.Formulasi;

PRINT '';
PRINT '=== 3. GROUPING TEST - Berapa SpsNoDoc per DocumentNumber? ==='
-- Ini menunjukkan apakah multiple ExcelId share satu SpsNoDoc atau tidak
SELECT 
    DocumentNumber,
    Machine,
    Formulasi,
    COUNT(DISTINCT Id) AS JumlahSpsNoDoc,
    COUNT(*) AS TotalRows,
    STRING_AGG(ExcelId, ', ') AS ExcelIds
FROM SpsNoDocs
WHERE DocumentNumber IS NOT NULL AND DocumentNumber <> ''
GROUP BY DocumentNumber, Machine, Formulasi
HAVING COUNT(DISTINCT Id) >= 1
ORDER BY JumlahSpsNoDoc DESC, DocumentNumber;

PRINT '';
PRINT '=== 4. RELASI TEST - SpsNoDoc dengan Multiple ItemLists ==='
-- Cek apakah satu SpsNoDoc punya multiple ItemLists
SELECT TOP 20
    sn.Id,
    sn.DocumentNumber,
    sn.Machine,
    COUNT(sil.Id) AS JumlahItemList,
    STRING_AGG(sil.ItemList, ', ') AS ItemLists
FROM SpsNoDocs sn
LEFT JOIN SpsItemLists sil ON sil.SpsNoDocId = sn.Id
GROUP BY sn.Id, sn.DocumentNumber, sn.Machine
ORDER BY JumlahItemList DESC;

PRINT '';
PRINT '=== 5. NO.DOC KOSONG - Items dengan DocumentNumber NULL/Empty ==='
SELECT 
    Id,
    ExcelId,
    Machine,
    DocumentNumber,
    Formulasi,
    Customer
FROM SpsNoDocs
WHERE DocumentNumber IS NULL OR DocumentNumber = '' OR DocumentNumber = '-'
ORDER BY Machine, ExcelId;

PRINT '';
PRINT '=== KESIMPULAN ==='
PRINT 'Jika satu SpsNoDoc memiliki multiple ItemLists, maka edit satu item akan otomatis sync ke item lain.';
PRINT 'Jika setiap ExcelId memiliki SpsNoDoc terpisah, maka edit TIDAK akan sync otomatis.';
