-- ============================================================
-- DROP UNUSED TABLES - prd_extrude_hose
-- Tables: DailyPlanActivities, DailyPlanExecutions, LotTags,
--         NowProducings, PackingStandards, StandardParameterSettings
-- 
-- PENTING: Jalankan di database prd_extrude_hose
-- Pastikan sudah backup sebelum menjalankan script ini!
-- ============================================================

USE prd_extrude_hose;
GO

PRINT '=== MULAI DROP TABEL ===';

-- -------------------------------------------------------
-- 1. DailyPlanActivities
--    (drop dulu sebelum DailyPlanExecutions jika ada FK)
-- -------------------------------------------------------
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DailyPlanActivities')
BEGIN
    -- Hapus FK yang merujuk ke tabel ini dulu jika ada
    DECLARE @sql NVARCHAR(MAX) = '';
    SELECT @sql += 'ALTER TABLE [' + OBJECT_NAME(parent_object_id) + '] DROP CONSTRAINT [' + name + '];' + CHAR(13)
    FROM sys.foreign_keys
    WHERE referenced_object_id = OBJECT_ID('DailyPlanActivities');
    IF LEN(@sql) > 0 EXEC sp_executesql @sql;

    DROP TABLE [dbo].[DailyPlanActivities];
    PRINT '✅ DailyPlanActivities - DROPPED';
END
ELSE
    PRINT '⚠️  DailyPlanActivities - tidak ada, dilewati';
GO

-- -------------------------------------------------------
-- 2. DailyPlanExecutions
-- -------------------------------------------------------
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DailyPlanExecutions')
BEGIN
    DECLARE @sql2 NVARCHAR(MAX) = '';
    SELECT @sql2 += 'ALTER TABLE [' + OBJECT_NAME(parent_object_id) + '] DROP CONSTRAINT [' + name + '];' + CHAR(13)
    FROM sys.foreign_keys
    WHERE referenced_object_id = OBJECT_ID('DailyPlanExecutions');
    IF LEN(@sql2) > 0 EXEC sp_executesql @sql2;

    DROP TABLE [dbo].[DailyPlanExecutions];
    PRINT '✅ DailyPlanExecutions - DROPPED';
END
ELSE
    PRINT '⚠️  DailyPlanExecutions - tidak ada, dilewati';
GO

-- -------------------------------------------------------
-- 3. LotTags
--    (ada FK ke ProductionReports, harus drop FK dulu)
-- -------------------------------------------------------
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LotTags')
BEGIN
    DECLARE @sql3 NVARCHAR(MAX) = '';
    SELECT @sql3 += 'ALTER TABLE [' + OBJECT_NAME(parent_object_id) + '] DROP CONSTRAINT [' + name + '];' + CHAR(13)
    FROM sys.foreign_keys
    WHERE referenced_object_id = OBJECT_ID('LotTags')
       OR parent_object_id = OBJECT_ID('LotTags');
    IF LEN(@sql3) > 0 EXEC sp_executesql @sql3;

    DROP TABLE [dbo].[LotTags];
    PRINT '✅ LotTags - DROPPED';
END
ELSE
    PRINT '⚠️  LotTags - tidak ada, dilewati';
GO

-- -------------------------------------------------------
-- 4. NowProducings
-- -------------------------------------------------------
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'NowProducings')
BEGIN
    DECLARE @sql4 NVARCHAR(MAX) = '';
    SELECT @sql4 += 'ALTER TABLE [' + OBJECT_NAME(parent_object_id) + '] DROP CONSTRAINT [' + name + '];' + CHAR(13)
    FROM sys.foreign_keys
    WHERE referenced_object_id = OBJECT_ID('NowProducings');
    IF LEN(@sql4) > 0 EXEC sp_executesql @sql4;

    DROP TABLE [dbo].[NowProducings];
    PRINT '✅ NowProducings - DROPPED';
END
ELSE
    PRINT '⚠️  NowProducings - tidak ada, dilewati';
GO

-- -------------------------------------------------------
-- 5. PackingStandards
-- -------------------------------------------------------
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PackingStandards')
BEGIN
    DECLARE @sql5 NVARCHAR(MAX) = '';
    SELECT @sql5 += 'ALTER TABLE [' + OBJECT_NAME(parent_object_id) + '] DROP CONSTRAINT [' + name + '];' + CHAR(13)
    FROM sys.foreign_keys
    WHERE referenced_object_id = OBJECT_ID('PackingStandards');
    IF LEN(@sql5) > 0 EXEC sp_executesql @sql5;

    DROP TABLE [dbo].[PackingStandards];
    PRINT '✅ PackingStandards - DROPPED';
END
ELSE
    PRINT '⚠️  PackingStandards - tidak ada, dilewati';
GO

-- -------------------------------------------------------
-- 6. StandardParameterSettings
--    (ada FK dari ProductionReports.SpsId dan DimensionReports.SpsId)
--    Drop FK dulu dari tabel yang merujuk ke sini
-- -------------------------------------------------------
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'StandardParameterSettings')
BEGIN
    -- Drop FK dari ProductionReports
    DECLARE @fk1 NVARCHAR(200);
    SELECT @fk1 = name FROM sys.foreign_keys
    WHERE parent_object_id = OBJECT_ID('ProductionReports')
      AND referenced_object_id = OBJECT_ID('StandardParameterSettings');
    IF @fk1 IS NOT NULL
    BEGIN
        EXEC('ALTER TABLE [dbo].[ProductionReports] DROP CONSTRAINT [' + @fk1 + ']');
        PRINT '  → FK dari ProductionReports dihapus';
    END

    -- Drop FK dari DimensionReports
    DECLARE @fk2 NVARCHAR(200);
    SELECT @fk2 = name FROM sys.foreign_keys
    WHERE parent_object_id = OBJECT_ID('DimensionReports')
      AND referenced_object_id = OBJECT_ID('StandardParameterSettings');
    IF @fk2 IS NOT NULL
    BEGIN
        EXEC('ALTER TABLE [dbo].[DimensionReports] DROP CONSTRAINT [' + @fk2 + ']');
        PRINT '  → FK dari DimensionReports dihapus';
    END

    DROP TABLE [dbo].[StandardParameterSettings];
    PRINT '✅ StandardParameterSettings - DROPPED';
END
ELSE
    PRINT '⚠️  StandardParameterSettings - tidak ada, dilewati';
GO

-- -------------------------------------------------------
-- Verifikasi: tampilkan semua tabel yang tersisa
-- -------------------------------------------------------
PRINT '';
PRINT '=== TABEL YANG TERSISA ===';
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
  AND TABLE_SCHEMA = 'dbo'
ORDER BY TABLE_NAME;
GO

PRINT '=== SELESAI ===';
