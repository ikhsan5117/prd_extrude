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

                    string createSql = @"
IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'StandardParameterSettingId'
          AND Object_ID = Object_ID(N'dbo.ProductionReports'))
BEGIN
    ALTER TABLE [dbo].[ProductionReports] ADD [StandardParameterSettingId] int NULL;
    PRINT 'Added StandardParameterSettingId to ProductionReports.';
    -- We can also add the FK if needed, but for now just the column
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SensorIngestLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SensorIngestLogs] (
        [Id] int NOT NULL IDENTITY,
        [DeviceId] nvarchar(100) NOT NULL,
        [MachineCode] nvarchar(50) NOT NULL,
        [ProductionReportId] int NULL,
        [SensorTimestamp] datetime2 NOT NULL,
        [MetricType] nvarchar(100) NOT NULL,
        [MetricValue] decimal(18,4) NOT NULL,
        [Unit] nvarchar(20) NULL,
        [Quality] nvarchar(20) NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [ErrorMessage] nvarchar(500) NULL,
        [IdempotencyKey] nvarchar(200) NULL,
        [IngestedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_SensorIngestLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SensorIngestLogs_ProductionReports_ProductionReportId] FOREIGN KEY ([ProductionReportId]) REFERENCES [dbo].[ProductionReports] ([Id])
    );

    CREATE INDEX [IX_SensorIngestLogs_DeviceId] ON [dbo].[SensorIngestLogs] ([DeviceId]);
    CREATE UNIQUE INDEX [IX_SensorIngestLogs_IdempotencyKey] ON [dbo].[SensorIngestLogs] ([IdempotencyKey]) WHERE [IdempotencyKey] IS NOT NULL;
    CREATE INDEX [IX_SensorIngestLogs_MachineCode_SensorTimestamp] ON [dbo].[SensorIngestLogs] ([MachineCode], [SensorTimestamp]);
    CREATE INDEX [IX_SensorIngestLogs_ProductionReportId] ON [dbo].[SensorIngestLogs] ([ProductionReportId]);

    PRINT 'SensorIngestLogs table created successfully.';
END
ELSE
BEGIN
    PRINT 'SensorIngestLogs table already exists.';
END
";
                    using (var cmd = new SqlCommand(createSql, conn))
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
