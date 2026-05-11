using System.Data;
using Microsoft.Data.SqlClient;

var connectionString = "Server=10.14.149.34;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;";

try {
    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();
        
        var query = "SELECT TOP 1 Id, ItemList, DocumentNumber FROM SpsMasters";
        using (var command = new SqlCommand(query, connection))
        using (var reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                Console.WriteLine($"? SpsMasters table accessible!");
                Console.WriteLine($"  Sample: ID={reader["Id"]}, ItemList={reader["ItemList"]}, DocNum={reader["DocumentNumber"]}");
            }
            else
            {
                Console.WriteLine("? No data found in SpsMasters");
            }
        }
    }
} catch (Exception ex) {
    Console.WriteLine($"Error: {ex.Message}");
}
