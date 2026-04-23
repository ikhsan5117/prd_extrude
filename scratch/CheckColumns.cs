using Microsoft.Data.SqlClient;
using System;

class Program {
    static void Main() {
        string connString = "Server=10.14.149.34;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;";
        using (var conn = new SqlConnection(connString)) {
            conn.Open();
            var cmd = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MasterlistSpsDoubleLayers'", conn);
            using (var reader = cmd.ExecuteReader()) {
                while (reader.Read()) {
                    Console.WriteLine(reader["COLUMN_NAME"]);
                }
            }
        }
    }
}
