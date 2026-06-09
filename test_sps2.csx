#r "nuget: Microsoft.Data.SqlClient, 5.1.4"
using Microsoft.Data.SqlClient;
using System;
using var conn = new SqlConnection("Server=(localdb)\\mssqllocaldb;Database=VelastoProductionSystem;Trusted_Connection=True;");
conn.Open();
using var cmd = conn.CreateCommand();
cmd.CommandText = "SELECT Dimensi, InnerTarget, ThickTarget, ToleranceInner_Asli, TebalInner_Asli, InnerTol, ThickTol FROM SpsNoDocs WHERE DocumentNumber = 'VI-SOP-PROD-132'";
using var reader = cmd.ExecuteReader();
while (reader.Read()) {
    Console.WriteLine($"Dimensi: {reader["Dimensi"]} | InnerTarget: {reader["InnerTarget"]} | ThickTarget: {reader["ThickTarget"]} | ToleranceInner_Asli: {reader["ToleranceInner_Asli"]} | TebalInner_Asli: {reader["TebalInner_Asli"]} | InnerTol: {reader["InnerTol"]} | ThickTol: {reader["ThickTol"]}");
}
