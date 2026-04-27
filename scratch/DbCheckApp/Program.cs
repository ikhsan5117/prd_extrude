using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DbCheckApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=10.14.149.34;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;MultipleActiveResultSets=True;";
            
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    Console.WriteLine("Connected to Production DB.");

                    string createTableSql = @"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DimensionSummaries]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DimensionSummaries] (
        [Id] int NOT NULL IDENTITY,
        [DimensionReportId] int NOT NULL,
        [PartNumber] nvarchar(max) NULL,
        [VinCode] nvarchar(max) NULL,
        [StandardLength] nvarchar(max) NULL,
        [ActualLength] nvarchar(max) NULL,
        [QtyTarget] int NOT NULL,
        [QtyOk] int NOT NULL,
        [NgDimension] int NOT NULL,
        [NgVisual] int NOT NULL,
        CONSTRAINT [PK_DimensionSummaries] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DimensionSummaries_DimensionReports_DimensionReportId] FOREIGN KEY ([DimensionReportId]) REFERENCES [dbo].[DimensionReports] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_DimensionSummaries_DimensionReportId] ON [dbo].[DimensionSummaries] ([DimensionReportId]);
    
    PRINT 'DimensionSummaries table created successfully.';
END
ELSE
BEGIN
    PRINT 'DimensionSummaries table already exists.';
END
";

                    using (var cmd = new SqlCommand(createTableSql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Also we should mark the EF Core migrations history so it doesn't try to apply it again.
                    // Let's insert the migration AddDimensionSummary manually if it doesn't exist.
                    string insertMigrationSql = @"
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__EFMigrationsHistory]') AND type in (N'U'))
BEGIN
    IF NOT EXISTS (SELECT * FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = '20260426082529_AddDimensionSummary')
    BEGIN
        INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES ('20260426082529_AddDimensionSummary', '8.0.0');
        PRINT 'Migration AddDimensionSummary recorded.';
    END
END
";
                    using (var cmd = new SqlCommand(insertMigrationSql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    Console.WriteLine("Done.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
