-- Add StandardParameterSettingId if not exists
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'ProductionReports' 
    AND COLUMN_NAME = 'SpsId'
)
BEGIN
    ALTER TABLE ProductionReports 
    ADD SpsId INT NULL;
    
    -- Add foreign key
    ALTER TABLE ProductionReports
    ADD CONSTRAINT FK_ProductionReports_StandardParameterSettings_SpsId
    FOREIGN KEY (SpsId) REFERENCES StandardParameterSettings(Id);
    
    -- Add index
    CREATE INDEX IX_ProductionReports_SpsId 
    ON ProductionReports(SpsId);
    
    PRINT 'SpsId column added successfully';
END
ELSE
BEGIN
    PRINT 'SpsId column already exists';
END
GO

-- Add ItemCode if not exists
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'ProductionReports' 
    AND COLUMN_NAME = 'ItemCode'
)
BEGIN
    ALTER TABLE ProductionReports 
    ADD ItemCode NVARCHAR(MAX) NULL;
    
    PRINT 'ItemCode column added successfully';
END
ELSE
BEGIN
    PRINT 'ItemCode column already exists';
END
GO

-- Verify columns exist
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'ProductionReports' 
AND COLUMN_NAME IN ('ItemCode', 'SpsId')
ORDER BY COLUMN_NAME;
