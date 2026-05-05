-- =============================================
-- MANUAL MIGRATION: Add ItemCode Column
-- Date: 2026-04-29
-- Description: Menambahkan kolom ItemCode ke tabel DimensionReports
-- =============================================

USE [prd_extrude_hose];
GO

-- Cek apakah kolom sudah ada
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'DimensionReports' 
    AND COLUMN_NAME = 'ItemCode'
)
BEGIN
    -- Tambahkan kolom ItemCode
    ALTER TABLE [dbo].[DimensionReports]
    ADD [ItemCode] NVARCHAR(100) NULL;
    
    PRINT 'SUCCESS: Kolom ItemCode berhasil ditambahkan ke tabel DimensionReports';
END
ELSE
BEGIN
    PRINT 'INFO: Kolom ItemCode sudah ada di tabel DimensionReports';
END
GO

-- Verifikasi
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'DimensionReports' AND COLUMN_NAME = 'ItemCode';
GO
