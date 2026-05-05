using System;
using Microsoft.Data.SqlClient;

namespace DbCheckApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=10.14.149.34;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;MultipleActiveResultSets=True;";
            if (args.Length == 0) return;
            string sql = args[0];
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            bool hasData = false;
                            while (reader.Read())
                            {
                                hasData = true;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    Console.Write(reader.GetValue(i) + "\t|\t");
                                }
                                Console.WriteLine();
                            }
                            if(!hasData) Console.WriteLine("NO DATA FOUND");
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("ERROR: " + ex.Message); }
        }
    }
}
