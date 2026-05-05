-- Fix Migration History and Add SpsId Column
-- This script resolves migration conflicts and adds the SpsId column

USE VelastoProductionSystem;
GO

-- Step 1: Mark existing migrations as applied (fake apply)
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20260426011455_InitialLocal')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20260426011455_InitialLocal', '8.0.0');
    PRINT '✓ InitialLocal migration marked as applied';
END
ELSE
    PRINT '✓ InitialLocal already in history';
GO

IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20260428022301_AddMiddleMaterialFields')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20260428022301_AddMiddleMaterialFields', '8.0.0');
    PRINT '✓ AddMiddleMaterialFields migration marked as applied';
END
ELSE
    PRINT '✓ AddMiddleMaterialFields already in history';
GO

IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20260430075941_AddItemCodeToProductionReport')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20260430075941_AddItemCodeToProductionReport', '8.0.0');
    PRINT '✓ AddItemCodeToProductionReport migration marked as applied';
END
ELSE
    PRINT '✓ AddItemCodeToProductionReport already in history';
GO

-- Step 2: Check if StandardParameterSettingId exists (old column)
PRINT '';
PRINT '=== Checking Current Schema ===';
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'ProductionReports' 
    AND COLUMN_NAME = 'StandardParameterSettingId'
)
BEGIN
    PRINT '✓ StandardParameterSettingId column found - will rename to SpsId';
    
    -- Drop foreign key if exists
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ProductionReports_StandardParameterSettings_StandardParameterSettingId')
    BEGIN
        ALTER TABLE ProductionReports
        DROP CONSTRAINT FK_ProductionReports_StandardParameterSettings_StandardParameterSettingId;
        PRINT '  - Foreign key constraint dropped';
    END
    
    -- Rename column
    EXEC sp_rename 'ProductionReports.StandardParameterSettingId', 'SpsId', 'COLUMN';
    PRINT '  - Column renamed to SpsId';
    
    -- Drop old index if exists
    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProductionReports_StandardParameterSettingId' AND object_id = OBJECT_ID('ProductionReports'))
    BEGIN
        DROP INDEX IX_ProductionReports_StandardParameterSettingId ON ProductionReports;
        PRINT '  - Old index dropped';
    END
    
    -- Create new index
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProductionReports_SpsId' AND object_id = OBJECT_ID('ProductionReports'))
    BEGIN
        CREATE INDEX IX_ProductionReports_SpsId ON ProductionReports(SpsId);
        PRINT '  - New index created';
    END
    
    -- Add foreign key constraint
    ALTER TABLE ProductionReports
    ADD CONSTRAINT FK_ProductionReports_StandardParameterSettings_SpsId
    FOREIGN KEY (SpsId) REFERENCES StandardParameterSettings(Id)
    ON DELETE SET NULL;
    PRINT '  - Foreign key constraint recreated';
    PRINT '✓ Column rename completed!';
END
ELSE IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'ProductionReports' 
    AND COLUMN_NAME = 'SpsId'
)
BEGIN
    PRINT '✓ SpsId column already exists - no action needed';
END
ELSE
BEGIN
    -- Neither column exists, create SpsId from scratch
    PRINT '! Creating SpsId column from scratch...';
    
    ALTER TABLE ProductionReports 
    ADD SpsId INT NULL;
    
    CREATE INDEX IX_ProductionReports_SpsId ON ProductionReports(SpsId);
    
    ALTER TABLE ProductionReports
    ADD CONSTRAINT FK_ProductionReports_StandardParameterSettings_SpsId
    FOREIGN KEY (SpsId) REFERENCES StandardParameterSettings(Id)
    ON DELETE SET NULL;
    
    PRINT '✓ SpsId column created successfully!';
END
GO

-- Step 3: Ensure ItemCode column exists
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'ProductionReports' 
    AND COLUMN_NAME = 'ItemCode'
)
BEGIN
    ALTER TABLE ProductionReports 
    ADD ItemCode NVARCHAR(MAX) NULL;
    PRINT '✓ ItemCode column added';
END
ELSE
    PRINT '✓ ItemCode column already exists';
GO

-- Step 4: Mark RenameSpsIdColumn migration as applied
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20260504033412_RenameSpsIdColumn')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20260504033412_RenameSpsIdColumn', '8.0.0');
    PRINT '✓ RenameSpsIdColumn migration marked as applied';
END
ELSE
    PRINT '✓ RenameSpsIdColumn already in history';
GO

-- Step 5: Verify final schema
PRINT '';
PRINT '=== Final Schema Verification ===';
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'ProductionReports' 
AND COLUMN_NAME IN ('SpsId', 'ItemCode')
ORDER BY COLUMN_NAME;

-- Check migrations history
PRINT '';
PRINT '=== Migration History ===';
SELECT MigrationId, ProductVersion 
FROM __EFMigrationsHistory 
ORDER BY MigrationId;

PRINT '';
PRINT '====================================';
PRINT '✓✓✓ MIGRATION FIX COMPLETED! ✓✓✓';
PRINT '====================================';
GO
