-- DB Cleanup Safe Script
-- Target DB: prd_extrude_hose
-- Default behavior: DRY RUN (ROLLBACK)
-- To apply changes, set @Apply = 1

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Apply bit = 1;

PRINT '==========================================';
PRINT ' DB CLEANUP (SAFE MODE)';
PRINT '==========================================';
PRINT 'Apply mode: ' + CASE WHEN @Apply = 1 THEN 'COMMIT' ELSE 'DRY RUN (ROLLBACK)' END;
PRINT '';

BEGIN TRANSACTION;

BEGIN TRY
    DECLARE @rows int;

    -- 1) Normalize whitespace on key text columns
    UPDATE dbo.DimensionReports
    SET
        ItemCode = NULLIF(LTRIM(RTRIM(ItemCode)), ''),
        HoseType = NULLIF(LTRIM(RTRIM(HoseType)), ''),
        DocumentNumber = NULLIF(LTRIM(RTRIM(DocumentNumber)), ''),
        VinCode = NULLIF(LTRIM(RTRIM(VinCode)), '');
    SET @rows = @@ROWCOUNT;
    PRINT 'Step 1 - Normalize text in DimensionReports: ' + CAST(@rows AS varchar(20));

    UPDATE dbo.ProductionReports
    SET
        ItemCode = NULLIF(LTRIM(RTRIM(ItemCode)), ''),
        HoseType = NULLIF(LTRIM(RTRIM(HoseType)), ''),
        DocumentNumber = NULLIF(LTRIM(RTRIM(DocumentNumber)), ''),
        VinCode = NULLIF(LTRIM(RTRIM(VinCode)), '');
    SET @rows = @@ROWCOUNT;
    PRINT 'Step 1 - Normalize text in ProductionReports: ' + CAST(@rows AS varchar(20));

    -- 2) Backfill DimensionReports from ProductionReports by DocumentNumber
    ;WITH src AS (
        SELECT
            dr.Id,
            dr.ItemCode AS DrItemCode,
            dr.HoseType AS DrHoseType,
            dr.SpsId AS DrSpsId,
            pr.ItemCode AS PrItemCode,
            pr.HoseType AS PrHoseType,
            pr.SpsId AS PrSpsId
        FROM dbo.DimensionReports dr
        INNER JOIN dbo.ProductionReports pr
            ON pr.DocumentNumber = dr.DocumentNumber
    )
    UPDATE dr
    SET
        ItemCode = CASE
            WHEN dr.ItemCode IS NULL THEN src.PrItemCode
            WHEN TRY_CONVERT(bigint, dr.ItemCode) IS NOT NULL AND src.PrItemCode IS NOT NULL THEN src.PrItemCode
            ELSE dr.ItemCode
        END,
        HoseType = CASE
            WHEN dr.HoseType IS NULL THEN src.PrHoseType
            WHEN dr.ItemCode IS NOT NULL AND UPPER(dr.HoseType) = UPPER(dr.ItemCode) AND src.PrHoseType IS NOT NULL THEN src.PrHoseType
            ELSE dr.HoseType
        END,
        SpsId = COALESCE(dr.SpsId, src.PrSpsId)
    FROM dbo.DimensionReports dr
    INNER JOIN src ON src.Id = dr.Id;
    SET @rows = @@ROWCOUNT;
    PRINT 'Step 2 - Backfill DimensionReports from ProductionReports: ' + CAST(@rows AS varchar(20));

    -- 3) Backfill DimensionReports.SpsId by ItemCode -> StandardParameterSettings.ItemList
    UPDATE dr
    SET dr.SpsId = s.Id
    FROM dbo.DimensionReports dr
    INNER JOIN dbo.StandardParameterSettings s
        ON UPPER(LTRIM(RTRIM(s.ItemList))) = UPPER(LTRIM(RTRIM(dr.ItemCode)))
    WHERE dr.SpsId IS NULL
      AND dr.ItemCode IS NOT NULL
      AND s.ItemList IS NOT NULL;
    SET @rows = @@ROWCOUNT;
    PRINT 'Step 3 - Backfill DimensionReports.SpsId from ItemCode: ' + CAST(@rows AS varchar(20));

    -- 4) Cleanup orphan child rows
    DELETE dm
    FROM dbo.DimensionMeasurements dm
    LEFT JOIN dbo.DimensionReports dr ON dr.Id = dm.DimensionReportId
    WHERE dr.Id IS NULL;
    SET @rows = @@ROWCOUNT;
    PRINT 'Step 4 - Delete orphan DimensionMeasurements: ' + CAST(@rows AS varchar(20));

    DELETE ds
    FROM dbo.DimensionSummaries ds
    LEFT JOIN dbo.DimensionReports dr ON dr.Id = ds.DimensionReportId
    WHERE dr.Id IS NULL;
    SET @rows = @@ROWCOUNT;
    PRINT 'Step 4 - Delete orphan DimensionSummaries: ' + CAST(@rows AS varchar(20));

    DELETE prd
    FROM dbo.ProductionReadings prd
    LEFT JOIN dbo.ProductionReports pr ON pr.Id = prd.ProductionReportId
    WHERE pr.Id IS NULL;
    SET @rows = @@ROWCOUNT;
    PRINT 'Step 4 - Delete orphan ProductionReadings: ' + CAST(@rows AS varchar(20));

    DELETE pml
    FROM dbo.ProductionMaterialLots pml
    LEFT JOIN dbo.ProductionReports pr ON pr.Id = pml.ProductionReportId
    WHERE pr.Id IS NULL;
    SET @rows = @@ROWCOUNT;
    PRINT 'Step 4 - Delete orphan ProductionMaterialLots: ' + CAST(@rows AS varchar(20));

    DELETE sil
    FROM dbo.SensorIngestLogs sil
    LEFT JOIN dbo.ProductionReports pr ON pr.Id = sil.ProductionReportId
    WHERE sil.ProductionReportId IS NOT NULL
      AND pr.Id IS NULL;
    SET @rows = @@ROWCOUNT;
    PRINT 'Step 4 - Delete orphan SensorIngestLogs: ' + CAST(@rows AS varchar(20));

    -- 5) Post-cleanup diagnostics
    PRINT '';
    PRINT '=== Diagnostics After Cleanup ===';

    SELECT
        NumericItemCodeRows = COUNT(*)
    FROM dbo.DimensionReports
    WHERE ItemCode IS NOT NULL
      AND TRY_CONVERT(bigint, ItemCode) IS NOT NULL;

    SELECT
        MissingSpsIdRows = COUNT(*)
    FROM dbo.DimensionReports
    WHERE SpsId IS NULL;

    SELECT TOP (20)
        dr.Id,
        dr.DocumentNumber,
        dr.ItemCode,
        dr.HoseType,
        dr.SpsId,
        dr.CreatedDate
    FROM dbo.DimensionReports dr
    WHERE dr.ItemCode IS NOT NULL
      AND TRY_CONVERT(bigint, dr.ItemCode) IS NOT NULL
    ORDER BY dr.CreatedDate DESC;

    IF @Apply = 1
    BEGIN
        COMMIT TRANSACTION;
        PRINT '';
        PRINT 'COMMIT DONE - Cleanup applied successfully.';
    END
    ELSE
    BEGIN
        ROLLBACK TRANSACTION;
        PRINT '';
        PRINT 'DRY RUN DONE - No data was changed (rolled back).';
        PRINT 'Set @Apply = 1 to apply for real.';
    END
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT '';
    PRINT 'ERROR: ' + ERROR_MESSAGE();
    THROW;
END CATCH;

