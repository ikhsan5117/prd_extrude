using System;
using Microsoft.Data.SqlClient;

namespace SpsChecker
{
    class Program
    {
        static string ElwpConn = "Server=10.14.149.34;Database=ELWP_PRD;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            using var conn = new SqlConnection(ElwpConn);
            conn.Open();
            // Search for NA2033 on 2026-04-22
            string query = @"
                SELECT p.KodeItem, m.NamaMesin 
                FROM produksi.tb_elwp_produksi_plannings p
                LEFT JOIN produksi.tb_elwp_produksi_mesins m ON p.MesinId = m.Id
                WHERE p.TanggalPlanning >= '2026-04-22' AND p.TanggalPlanning < '2026-04-23'
                AND p.KodeItem LIKE '%NA2033%'";
            
            using var cmd = new SqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"Item: {reader["KodeItem"]} | Machine: {reader["NamaMesin"]}");
            }
        }
    }
}
