-- Manual SQL Script to Update Database Schema
-- Run this script manually in SQL Server Management Studio or via sqlcmd

-- Step 1: Check if StandardParameterSettingId column exists
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'ProductionReports' 
    AND COLUMN_NAME = 'StandardParameterSettingId'
)
BEGIN
    PRINT 'StandardParameterSettingId column exists. Proceeding with rename...';
    
    -- Drop foreign key constraint first
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ProductionReports_StandardParameterSettings_StandardParameterSettingId')
    BEGIN
        ALTER TABLE ProductionReports
        DROP CONSTRAINT FK_ProductionReports_StandardParameterSettings_StandardParameterSettingId;
        PRINT 'Foreign key constraint dropped';
    END
    
    -- Rename column
    EXEC sp_rename 'ProductionReports.StandardParameterSettingId', 'SpsId', 'COLUMN';
    PRINT 'Column renamed to SpsId';
    
    -- Drop old index
    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProductionReports_StandardParameterSettingId')
    BEGIN
        DROP INDEX IX_ProductionReports_StandardParameterSettingId ON ProductionReports;
        PRINT 'Old index dropped';
    END
    
    -- Create new index
    CREATE INDEX IX_ProductionReports_SpsId ON ProductionReports(SpsId);
    PRINT 'New index created';
    
    -- Add foreign key constraint back
    ALTER TABLE ProductionReports
    ADD CONSTRAINT FK_ProductionReports_StandardParameterSettings_SpsId
    FOREIGN KEY (SpsId) REFERENCES StandardParameterSettings(Id)
    ON DELETE SET NULL;
    PRINT 'Foreign key constraint recreated';
    
    PRINT 'Column rename completed successfully!';
END
ELSE IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'ProductionReports' 
    AND COLUMN_NAME = 'SpsId'
)
BEGIN
    PRINT 'SpsId column already exists. No action needed.';
END
ELSE
BEGIN
    -- Neither column exists, create SpsId from scratch
    PRINT 'Creating SpsId column...';
    
    ALTER TABLE ProductionReports 
    ADD SpsId INT NULL;
    
    CREATE INDEX IX_ProductionReports_SpsId ON ProductionReports(SpsId);
    
    ALTER TABLE ProductionReports
    ADD CONSTRAINT FK_ProductionReports_StandardParameterSettings_SpsId
    FOREIGN KEY (SpsId) REFERENCES StandardParameterSettings(Id)
    ON DELETE SET NULL;
    
    PRINT 'SpsId column created successfully!';
END
GO

-- Verify the result
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'ProductionReports' 
AND COLUMN_NAME IN ('SpsId', 'StandardParameterSettingId', 'ItemCode')
ORDER BY COLUMN_NAME;
GO
