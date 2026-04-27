using System;
using Microsoft.Data.SqlClient;

namespace DbCheckApp
{
    class Program {
        static void Main(string[] args) {
            string connStr = "Server=10.14.149.34;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;";
            using var conn = new SqlConnection(connStr);
            conn.Open();
            
            var query = @"
                SELECT CAST(CreatedDate AS DATE) as InputDate, CreatedBy, COUNT(*) as TotalInput
                FROM ProductionReports 
                GROUP BY CAST(CreatedDate AS DATE), CreatedBy
                ORDER BY InputDate DESC";
                
            using var cmd = new SqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            
            Console.WriteLine("Ringkasan Tanggal Input dan Nama Operator:");
            Console.WriteLine("---------------------------------------------------------------");
            while(reader.Read()) {
                var inputDate = reader.GetDateTime(0);
                var createdBy = reader.IsDBNull(1) ? "-" : reader.GetString(1);
                var totalInput = reader.GetInt32(2);
                
                Console.WriteLine($"Tanggal: {inputDate:yyyy-MM-dd} | Nama: {createdBy,-20} | Jumlah Input: {totalInput}");
            }
            Console.WriteLine("---------------------------------------------------------------");
        }
    }
}
