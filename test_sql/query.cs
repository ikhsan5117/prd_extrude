using System;
using System.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connectionString = "Server=10.14.149.34,1433;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;MultipleActiveResultSets=True;Encrypt=False;";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT TOP 5 DocumentNumber, HeadTemp1, HeadTemp1_Asli, HeadTemp1_Min, HeadTemp1_Max FROM SpsNoDocs", conn);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"Doc: {reader["DocumentNumber"]}, HeadTemp1: {reader["HeadTemp1"]}, Asli: {reader["HeadTemp1_Asli"]}, Min: {reader["HeadTemp1_Min"]}, Max: {reader["HeadTemp1_Max"]}");
                }
            }
        }
    }
}
