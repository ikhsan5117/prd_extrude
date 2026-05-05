-- Migration: AddItemCodeToDimensionReport
-- Date: 2026-04-29
-- Description: Add ItemCode column to DimensionReports table

USE [VelastoProduction_PRD];
GO

-- Check if column exists before adding
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[DimensionReports]') 
    AND name = 'ItemCode'
)
BEGIN
    ALTER TABLE [dbo].[DimensionReports]
    ADD [ItemCode] NVARCHAR(100) NULL;
    
    PRINT 'Column ItemCode added successfully to DimensionReports table.';
END
ELSE
BEGIN
    PRINT 'Column ItemCode already exists in DimensionReports table.';
END
GO
