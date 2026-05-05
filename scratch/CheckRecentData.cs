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
            SpsId,
            CreatedAt
        FROM ProductionReports 
        ORDER BY Id DESC";
    
    using var cmd = new SqlCommand(checkSql, connection);
    using var reader = cmd.ExecuteReader();
    
    Console.WriteLine("=== 10 Recent Production Reports ===");
    Console.WriteLine($"{"Id",-5} {"DocumentNumber",-20} {"ProductionDate",-12} {"ItemCode",-15} {"SpsId",-10} {"CreatedAt",-20}");
    Console.WriteLine(new string('-', 100));
    
    while (reader.Read())
    {
        Console.WriteLine($"{reader["Id"],-5} {reader["DocumentNumber"],-20} {reader["ProductionDate"],-12:yyyy-MM-dd} {reader["ItemCode"] ?? "NULL",-15} {reader["SpsId"] ?? "NULL",-10} {reader["CreatedAt"],-20:yyyy-MM-dd HH:mm}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ ERROR: {ex.Message}");
}
