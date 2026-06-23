#r "nuget: Microsoft.Data.SqlClient, 5.2.0"
using System;
using Microsoft.Data.SqlClient;

string connString = "Server=10.14.149.34,1433;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;Connect Timeout=30;";
using (var conn = new SqlConnection(connString))
{
    conn.Open();
    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM SpsNoDocs WHERE DocumentNumber LIKE '%EPR%'", conn))
    {
        Console.WriteLine($"Before Delete EPR Docs: {cmd.ExecuteScalar()}");
    }
    
    using (var cmd = new SqlCommand("DELETE FROM SpsItemLists WHERE DocumentNumber LIKE '%EPR%'", conn))
    {
        Console.WriteLine($"Deleted Items: {cmd.ExecuteNonQuery()}");
    }
    using (var cmd = new SqlCommand("DELETE FROM SpsNoDocs WHERE DocumentNumber LIKE '%EPR%'", conn))
    {
        Console.WriteLine($"Deleted Docs: {cmd.ExecuteNonQuery()}");
    }
    
    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM SpsNoDocs WHERE DocumentNumber LIKE '%EPR%'", conn))
    {
        Console.WriteLine($"After Delete EPR Docs: {cmd.ExecuteScalar()}");
    }
}
