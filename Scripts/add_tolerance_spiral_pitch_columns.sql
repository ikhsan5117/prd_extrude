-- Add missing ToleranceSpiralPitch columns to SpsNoDocs table
-- Run this script manually on the database: 10.14.149.34

USE prd_extrude_hose;
GO

-- Check if columns exist before adding
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('SpsNoDocs') 
               AND name = 'ToleranceSpiralPitch_Min')
BEGIN
    ALTER TABLE SpsNoDocs
    ADD ToleranceSpiralPitch_Min DECIMAL(18, 4) NULL;
    PRINT 'Column ToleranceSpiralPitch_Min added successfully';
END
ELSE
BEGIN
    PRINT 'Column ToleranceSpiralPitch_Min already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('SpsNoDocs') 
               AND name = 'ToleranceSpiralPitch_Asli')
BEGIN
    ALTER TABLE SpsNoDocs
    ADD ToleranceSpiralPitch_Asli DECIMAL(18, 4) NULL;
    PRINT 'Column ToleranceSpiralPitch_Asli added successfully';
END
ELSE
BEGIN
    PRINT 'Column ToleranceSpiralPitch_Asli already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('SpsNoDocs') 
               AND name = 'ToleranceSpiralPitch_Max')
BEGIN
    ALTER TABLE SpsNoDocs
    ADD ToleranceSpiralPitch_Max DECIMAL(18, 4) NULL;
    PRINT 'Column ToleranceSpiralPitch_Max added successfully';
END
ELSE
BEGIN
    PRINT 'Column ToleranceSpiralPitch_Max already exists';
END
GO

PRINT 'Script completed successfully';
GO
