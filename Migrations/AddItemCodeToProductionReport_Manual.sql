-- Migration: AddItemCodeToProductionReport
-- Date: 2026-04-30
-- Description: Add ItemCode column to ProductionReports table

-- Check if column exists before adding
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[ProductionReports]') 
    AND name = 'ItemCode'
)
BEGIN
    ALTER TABLE [dbo].[ProductionReports]
    ADD [ItemCode] NVARCHAR(100) NULL;
    
    PRINT 'Column ItemCode added successfully to ProductionReports table.';
END
ELSE
BEGIN
    PRINT 'Column ItemCode already exists in ProductionReports table.';
END
GO
