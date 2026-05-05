using Microsoft.Data.SqlClient;

string connectionString = "Server=10.14.149.34;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;MultipleActiveResultSets=True;";

try
{
    using var connection = new SqlConnection(connectionString);
    connection.Open();
    
    // Check 10 most recent records
    string checkSql = @"
        SELECT TOP 10 
            Id, 
            DocumentNumber,
            ProductionDate,
            ItemCode,
            SpsId
        FROM ProductionReports 
        ORDER BY Id DESC";
    
    using var cmd = new SqlCommand(checkSql, connection);
    using var reader = cmd.ExecuteReader();
    
    Console.WriteLine("=== 10 Recent Production Reports ===");
    Console.WriteLine($"{"Id",-5} {"DocumentNumber",-20} {"ProductionDate",-20} {"ItemCode",-15} {"SpsId",-10}");
    Console.WriteLine(new string('-', 80));
    
    int nullCount = 0;
    int notNullCount = 0;
    
    while (reader.Read())
    {
        var spsId = reader["SpsId"];
        var spsIdStr = spsId == DBNull.Value ? "NULL" : spsId.ToString();
        
        if (spsId == DBNull.Value)
            nullCount++;
        else
            notNullCount++;
            
        Console.WriteLine($"{reader["Id"],-5} {reader["DocumentNumber"],-20} {reader["ProductionDate"],-20:yyyy-MM-dd HH:mm} {reader["ItemCode"] ?? "NULL",-15} {spsIdStr,-10}");
    }
    
    Console.WriteLine(new string('-', 80));
    Console.WriteLine($"Summary: {notNullCount} records with SpsId, {nullCount} records without SpsId");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ ERROR: {ex.Message}");
}
