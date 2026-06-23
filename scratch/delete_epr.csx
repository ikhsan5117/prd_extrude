#r "nuget: Microsoft.Data.SqlClient, 5.2.0"
using System;
using Microsoft.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connString = "Server=10.14.149.34,1433;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;Connect Timeout=30;";
        using (var conn = new SqlConnection(connString))
        {
            conn.Open();
            
            // Cek jumlah EPR doc
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM SpsNoDocs WHERE DocumentNumber LIKE '%EPR%'", conn))
            {
                int count = (int)cmd.ExecuteScalar();
                Console.WriteLine($"Found {count} EPR documents in DB.");
            }
            
            // Hapus EPR doc item list
            using (var cmd = new SqlCommand("DELETE FROM SpsItemLists WHERE DocumentNumber LIKE '%EPR%'", conn))
            {
                int deletedItems = cmd.ExecuteNonQuery();
                Console.WriteLine($"Deleted {deletedItems} EPR items.");
            }
            
            // Hapus EPR doc
            using (var cmd = new SqlCommand("DELETE FROM SpsNoDocs WHERE DocumentNumber LIKE '%EPR%'", conn))
            {
                int deletedDocs = cmd.ExecuteNonQuery();
                Console.WriteLine($"Deleted {deletedDocs} EPR documents.");
            }
        }
    }
}
