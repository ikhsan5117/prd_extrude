#r "nuget: System.Data.SqlClient, 4.8.5"
using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text.Json;

string connStr = "Server=10.14.149.34,1433;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;";
SqlConnection conn = new SqlConnection(connStr);
conn.Open();

string query = "SELECT * FROM SpsNoDocs WHERE DocumentNumber = 'SOP/PROD/HOSE/SPS/24/10/13'";
SqlCommand cmd = new SqlCommand(query, conn);
SqlDataReader reader = cmd.ExecuteReader();

if (reader.Read())
{
    var dict = new Dictionary<string, object>();
    for (int i = 0; i < reader.FieldCount; i++)
    {
        var val = reader.GetValue(i);
        if (val != DBNull.Value && !string.IsNullOrWhiteSpace(val.ToString()))
        {
            dict[reader.GetName(i)] = val;
        }
    }
    Console.WriteLine(JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true }));
}
conn.Close();
