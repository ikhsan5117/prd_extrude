#r "nuget: Microsoft.Data.SqlClient, 5.2.0"
using System;
using System.Linq;
using Microsoft.Data.SqlClient;

string connString = "Server=10.14.149.34,1433;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;Connect Timeout=30;";
using (var conn = new SqlConnection(connString))
{
    conn.Open();
    var missingDocs = new[] {
        "SOP/PROD/HOSE/24/10/068",
        "SOP/PROD/HOSE/SPS/24/10/05",
        "SOP/PROD/HOSE/SPS/24/10/12",
        "SOP/PROD/HOSE/SPS/24/10/19",
        "SOP/PROD/HOSE/SPS/24/10/20",
        "SOP/PROD/HOSE/SPS/24/10/23",
        "SOP/PROD/HOSE/SPS/24/10/25"
    };
    
    Console.WriteLine("Checking Documents:");
    foreach(var doc in missingDocs) {
        using (var cmd = new SqlCommand("SELECT COUNT(*) FROM SpsNoDocs WHERE DocumentNumber = @doc", conn)) {
            cmd.Parameters.AddWithValue("@doc", doc);
            int count = (int)cmd.ExecuteScalar();
            Console.WriteLine($"- {doc}: {(count > 0 ? "FOUND" : "MISSING")}");
        }
    }
    
    var missingItems = new[] { "NA2140", "NA2661", "PE1755" };
    Console.WriteLine("\nChecking Items:");
    foreach(var item in missingItems) {
        using (var cmd = new SqlCommand("SELECT COUNT(*) FROM SpsItemLists WHERE ItemList = @item", conn)) {
            cmd.Parameters.AddWithValue("@item", item);
            int count = (int)cmd.ExecuteScalar();
            Console.WriteLine($"- {item}: {(count > 0 ? "FOUND" : "MISSING")}");
        }
    }
}
