using Microsoft.Data.SqlClient;

string connectionString = "Server=localhost;Database=VelastoProductionSystem;Trusted_Connection=True;TrustServerCertificate=True;";

try
{
    using var connection = new SqlConnection(connectionString);
    connection.Open();
    
    string checkSql = @"
        SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
        FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'ProductionReports' 
        AND COLUMN_NAME IN ('SpsId', 'StandardParameterSettingId', 'ItemCode')
        ORDER BY COLUMN_NAME";
    
    using var cmd = new SqlCommand(checkSql, connection);
    using var reader = cmd.ExecuteReader();
    
    Console.WriteLine("=== ProductionReports Columns ===");
    bool hasSpsId = false;
    bool hasOldColumn = false;
    
    while (reader.Read())
    {
        string colName = reader["COLUMN_NAME"].ToString()!;
        Console.WriteLine($"  {colName} ({reader["DATA_TYPE"]}, Nullable: {reader["IS_NULLABLE"]})");
        
        if (colName == "SpsId") hasSpsId = true;
        if (colName == "StandardParameterSettingId") hasOldColumn = true;
    }
    
    Console.WriteLine();
    if (hasSpsId)
    {
        Console.WriteLine("✓ SpsId column EXISTS - Database is ready!");
    }
    else if (hasOldColumn)
    {
        Console.WriteLine("⚠ StandardParameterSettingId exists but NOT renamed yet!");
        Console.WriteLine("  Run: dotnet run --project scratch/MigrationFix/MigrationFix.csproj");
    }
    else
    {
        Console.WriteLine("❌ Neither SpsId nor StandardParameterSettingId found!");
        Console.WriteLine("  Column needs to be created.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ ERROR: {ex.Message}");
}
