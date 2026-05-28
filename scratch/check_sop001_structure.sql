-- ================================================================
-- QUERY: Memeriksa Struktur Data NO.DOC "SOP/001"
-- ================================================================

PRINT '=== 1. Cek Semua SpsNoDocs dengan NO.DOC = SOP/001 ==='
SELECT 
    sn.Id AS SpsNoDocId,
    sn.ExcelId,
    sn.No,
    sn.DocumentNumber AS [NO.DOC],
    sn.Machine,
    sn.Formulasi,
    sn.Customer,
    sn.HoseType,
    sn.Dimensi
FROM SpsNoDocs sn
WHERE sn.DocumentNumber = 'SOP/001'
ORDER BY sn.Id;

PRINT '';
PRINT '=== 2. Cek ItemLists untuk Setiap SpsNoDocs ==='
SELECT 
    sn.Id AS SpsNoDocId,
    sn.ExcelId,
    sn.DocumentNumber AS [NO.DOC],
    sil.Id AS ItemListId,
    sil.ItemList
FROM SpsNoDocs sn
LEFT JOIN SpsItemLists sil ON sil.SpsNoDocId = sn.Id
WHERE sn.DocumentNumber = 'SOP/001'
ORDER BY sn.Id, sil.ItemList;

PRINT '';
PRINT '=== 3. Summary: Berapa SpsNoDocs dan Total Items? ==='
SELECT 
    COUNT(DISTINCT sn.Id) AS JumlahSpsNoDocs,
    COUNT(sil.Id) AS TotalItems,
    STRING_AGG(CAST(sn.Id AS VARCHAR), ', ') AS SpsNoDocIds,
    STRING_AGG(sil.ItemList, ', ') AS AllItemLists
FROM SpsNoDocs sn
LEFT JOIN SpsItemLists sil ON sil.SpsNoDocId = sn.Id
WHERE sn.DocumentNumber = 'SOP/001';

PRINT '';
PRINT '=== EXPECTED RESULT SETELAH FIX: ==='
PRINT 'Jika ada 2 items (ITEM001, ITEM002) dengan NO.DOC SOP/001:';
PRINT '';
PRINT 'SCENARIO A: 1 SpsNoDocs dengan 2 ItemLists';
PRINT '  - SpsNoDocId: 100';
PRINT '  - ItemLists: ITEM001, ITEM002';
PRINT '  - ViewBag.SyncItemCount = 1 (2 items - 1 current = 1 item lain)';
PRINT '';
PRINT 'SCENARIO B: 2 SpsNoDocs terpisah, masing-masing 1 ItemLists';
PRINT '  - SpsNoDocId: 100 → ItemList: ITEM001';
PRINT '  - SpsNoDocId: 101 → ItemList: ITEM002';
PRINT '  - ViewBag.SyncItemCount = 1 (2 items - 1 current = 1 item lain)';
PRINT '';
PRINT 'SEKARANG fix sudah handle kedua scenario!';
