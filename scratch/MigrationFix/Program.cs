using Microsoft.Data.SqlClient;

string connectionString = "Server=10.14.149.34;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;MultipleActiveResultSets=True;";

Console.WriteLine("=== Fix Migration History & Add SpsId Column ===\n");

try
{
    using var connection = new SqlConnection(connectionString);
    connection.Open();
    Console.WriteLine("✓ Connected to database\n");
    
    // Step 1: Mark old migrations as applied
    string[] migrations = new[] {
        "20260426011455_InitialLocal",
        "20260428022301_AddMiddleMaterialFields",
        "20260430075941_AddItemCodeToProductionReport"
    };
    
    foreach (var migration in migrations)
    {
        string checkSql = $"SELECT COUNT(*) FROM __EFMigrationsHistory WHERE MigrationId = '{migration}'";
        using (var cmd = new SqlCommand(checkSql, connection))
        {
            int count = (int)cmd.ExecuteScalar();
            if (count == 0)
            {
                string insertSql = $"INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('{migration}', '8.0.0')";
                using (var insertCmd = new SqlCommand(insertSql, connection))
                {
                    insertCmd.ExecuteNonQuery();
                    Console.WriteLine($"✓ Marked {migration} as applied");
                }
            }
            else
            {
                Console.WriteLine($"✓ {migration} already applied");
            }
        }
    }
    
    Console.WriteLine("\n=== Checking ProductionReports table ===");
    
    // Step 2: Check if StandardParameterSettingId exists
    string checkColumnSql = @"
        SELECT COUNT(*) 
        FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'ProductionReports' 
        AND COLUMN_NAME = 'StandardParameterSettingId'";
    
    using (var cmd = new SqlCommand(checkColumnSql, connection))
    {
        int hasOldColumn = (int)cmd.ExecuteScalar();
        
        if (hasOldColumn > 0)
        {
            Console.WriteLine("✓ StandardParameterSettingId found - renaming to SpsId...");
            
            // Drop FK
            ExecuteSql(connection, @"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ProductionReports_StandardParameterSettings_StandardParameterSettingId')
                ALTER TABLE ProductionReports DROP CONSTRAINT FK_ProductionReports_StandardParameterSettings_StandardParameterSettingId");
            
            // Rename column
            ExecuteSql(connection, "EXEC sp_rename 'ProductionReports.StandardParameterSettingId', 'SpsId', 'COLUMN'");
            Console.WriteLine("  - Column renamed");
            
            // Drop old index
            ExecuteSql(connection, @"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProductionReports_StandardParameterSettingId')
                DROP INDEX IX_ProductionReports_StandardParameterSettingId ON ProductionReports");
            
            // Create new index
            ExecuteSql(connection, "CREATE INDEX IX_ProductionReports_SpsId ON ProductionReports(SpsId)");
            Console.WriteLine("  - Index recreated");
            
            // Add FK
            ExecuteSql(connection, @"
                ALTER TABLE ProductionReports
                ADD CONSTRAINT FK_ProductionReports_StandardParameterSettings_SpsId
                FOREIGN KEY (SpsId) REFERENCES StandardParameterSettings(Id)
                ON DELETE SET NULL");
            Console.WriteLine("  - Foreign key recreated");
        }
        else
        {
            // Check if SpsId already exists
            string checkSpsIdSql = @"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'ProductionReports' 
                AND COLUMN_NAME = 'SpsId'";
            
            using (var spsCmd = new SqlCommand(checkSpsIdSql, connection))
            {
                int hasSpsId = (int)spsCmd.ExecuteScalar();
                
                if (hasSpsId > 0)
                {
                    Console.WriteLine("✓ SpsId column already exists");
                }
                else
                {
                    Console.WriteLine("! Creating SpsId column from scratch...");
                    
                    ExecuteSql(connection, "ALTER TABLE ProductionReports ADD SpsId INT NULL");
                    ExecuteSql(connection, "CREATE INDEX IX_ProductionReports_SpsId ON ProductionReports(SpsId)");
                    ExecuteSql(connection, @"
                        ALTER TABLE ProductionReports
                        ADD CONSTRAINT FK_ProductionReports_StandardParameterSettings_SpsId
                        FOREIGN KEY (SpsId) REFERENCES StandardParameterSettings(Id)
                        ON DELETE SET NULL");
                    
                    Console.WriteLine("✓ SpsId column created");
                }
            }
        }
    }
    
    // Step 3: Ensure ItemCode exists
    string checkItemCodeSql = @"
        SELECT COUNT(*) 
        FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'ProductionReports' 
        AND COLUMN_NAME = 'ItemCode'";
    
    using (var cmd = new SqlCommand(checkItemCodeSql, connection))
    {
        int hasItemCode = (int)cmd.ExecuteScalar();
        if (hasItemCode == 0)
        {
            ExecuteSql(connection, "ALTER TABLE ProductionReports ADD ItemCode NVARCHAR(MAX) NULL");
            Console.WriteLine("✓ ItemCode column added");
        }
        else
        {
            Console.WriteLine("✓ ItemCode column already exists");
        }
    }
    
    // Step 4: Mark RenameSpsIdColumn migration as applied
    string checkRenameMigration = "SELECT COUNT(*) FROM __EFMigrationsHistory WHERE MigrationId = '20260504033412_RenameSpsIdColumn'";
    using (var cmd = new SqlCommand(checkRenameMigration, connection))
    {
        int count = (int)cmd.ExecuteScalar();
        if (count == 0)
        {
            string insertSql = "INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20260504033412_RenameSpsIdColumn', '8.0.0')";
            using (var insertCmd = new SqlCommand(insertSql, connection))
            {
                insertCmd.ExecuteNonQuery();
                Console.WriteLine("✓ RenameSpsIdColumn migration marked as applied");
            }
        }
    }
    
    Console.WriteLine("\n====================================");
    Console.WriteLine("✓✓✓ MIGRATION FIX COMPLETED! ✓✓✓");
    Console.WriteLine("====================================\n");
    
    // Verify
    Console.WriteLine("=== Final Schema ===");
    string verifySql = @"
        SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
        FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'ProductionReports' 
        AND COLUMN_NAME IN ('SpsId', 'ItemCode')
        ORDER BY COLUMN_NAME";
    
    using (var cmd = new SqlCommand(verifySql, connection))
    using (var reader = cmd.ExecuteReader())
    {
        while (reader.Read())
        {
            Console.WriteLine($"  {reader["COLUMN_NAME"]} ({reader["DATA_TYPE"]}, Nullable: {reader["IS_NULLABLE"]})");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ ERROR: {ex.Message}");
    Console.WriteLine($"\nPlease run the SQL script manually:");
    Console.WriteLine("f:\\VelastoProductionSystem\\Migrations\\FixMigrationHistory.sql");
    Environment.Exit(1);
}

static void ExecuteSql(SqlConnection connection, string sql)
{
    using var cmd = new SqlCommand(sql, connection);
    cmd.ExecuteNonQuery();
}
