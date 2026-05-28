-- Query untuk memverifikasi relasi One-to-Many: DocumentNumber → ItemList

-- 1. Hitung berapa banyak ItemList per DocumentNumber
SELECT 
    DocumentNumber,
    COUNT(*) as TotalItems,
    COUNT(DISTINCT ItemList) as UniqueItems,
    STRING_AGG(ItemList, ', ') as ItemListSample
FROM SpsMasters
WHERE DocumentNumber IS NOT NULL
GROUP BY DocumentNumber
HAVING COUNT(*) > 1  -- Hanya tampilkan dokumen yang punya lebih dari 1 item
ORDER BY TotalItems DESC;

-- 2. Contoh konkret: Tampilkan 20 baris pertama dengan grouping
SELECT TOP 20
    DocumentNumber,
    ItemList,
    Machine,
    HoseType,
    Dimensi,
    Customer
FROM SpsMasters
WHERE DocumentNumber IS NOT NULL
ORDER BY DocumentNumber, ItemList;

-- 3. Summary statistics
SELECT 
    'Total Documents' as Metric, 
    COUNT(DISTINCT DocumentNumber) as Value
FROM SpsMasters
WHERE DocumentNumber IS NOT NULL
UNION ALL
SELECT 
    'Total Items (Rows)', 
    COUNT(*) 
FROM SpsMasters
UNION ALL
SELECT 
    'Average Items per Document', 
    AVG(CAST(ItemCount AS FLOAT))
FROM (
    SELECT DocumentNumber, COUNT(*) as ItemCount
    FROM SpsMasters
    WHERE DocumentNumber IS NOT NULL
    GROUP BY DocumentNumber
) sub;
