-- Migration: AddSpsIdToDimensionReport
-- Date: 2026-05-04
-- Description: Add SpsId foreign key column to DimensionReports table

USE [prd_extrude_hose];
GO

-- Add SpsId column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[DimensionReports]') AND name = 'SpsId')
BEGIN
    ALTER TABLE [dbo].[DimensionReports]
    ADD [SpsId] int NULL;
    
    PRINT 'Column SpsId added to DimensionReports table';
END
ELSE
BEGIN
    PRINT 'Column SpsId already exists in DimensionReports table';
END
GO

-- Create index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DimensionReports_SpsId' AND object_id = OBJECT_ID(N'[dbo].[DimensionReports]'))
BEGIN
    CREATE INDEX [IX_DimensionReports_SpsId] ON [dbo].[DimensionReports] ([SpsId]);
    
    PRINT 'Index IX_DimensionReports_SpsId created';
END
ELSE
BEGIN
    PRINT 'Index IX_DimensionReports_SpsId already exists';
END
GO

-- Add foreign key constraint
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_DimensionReports_StandardParameterSettings_SpsId' AND parent_object_id = OBJECT_ID(N'[dbo].[DimensionReports]'))
BEGIN
    ALTER TABLE [dbo].[DimensionReports]
    ADD CONSTRAINT [FK_DimensionReports_StandardParameterSettings_SpsId]
    FOREIGN KEY ([SpsId]) REFERENCES [dbo].[StandardParameterSettings] ([Id])
    ON DELETE SET NULL;
    
    PRINT 'Foreign key FK_DimensionReports_StandardParameterSettings_SpsId created';
END
ELSE
BEGIN
    PRINT 'Foreign key FK_DimensionReports_StandardParameterSettings_SpsId already exists';
END
GO

-- Update migration history
IF NOT EXISTS (SELECT * FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = '20260504120000_AddSpsIdToDimensionReport')
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20260504120000_AddSpsIdToDimensionReport', '8.0.0');
    
    PRINT 'Migration history updated';
END
ELSE
BEGIN
    PRINT 'Migration already recorded in history';
END
GO

PRINT 'Migration AddSpsIdToDimensionReport completed successfully!';
GO
