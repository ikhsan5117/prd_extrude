using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

var connectionString = "Server=10.14.149.34;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;";

try {
    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();
        Console.WriteLine("✔ Connected to SQL Server successfully!");

        // Hitung total baris
        int totalRows = 0;
        using (var cmd = new SqlCommand("SELECT COUNT(*) FROM SpsMasters", connection))
        {
            totalRows = (int)cmd.ExecuteScalar();
            Console.WriteLine($"Total SpsMasters rows: {totalRows}");
        }

        // Hitung baris jika dikelompokkan berdasarkan DocumentNumber saja
        int docNumGroups = 0;
        using (var cmd = new SqlCommand("SELECT COUNT(DISTINCT DocumentNumber) FROM SpsMasters", connection))
        {
            docNumGroups = (int)cmd.ExecuteScalar();
            Console.WriteLine($"Distinct DocumentNumber: {docNumGroups}");
        }

        // Mari kita buat program C# ini untuk membaca semua data, melakukan grouping di memori C#, 
        // lalu kita bisa lihat bagaimana cara terbaik untuk memetakan mereka!
        var allRows = new List<Dictionary<string, object>>();
        using (var cmd = new SqlCommand("SELECT * FROM SpsMasters", connection))
        using (var reader = cmd.ExecuteReader())
        {
            var columns = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(reader.GetName(i));
            }

            while (reader.Read())
            {
                var row = new Dictionary<string, object>();
                foreach (var col in columns)
                {
                    row[col] = reader[col];
                }
                allRows.Add(row);
            }
        }

        Console.WriteLine($"Read {allRows.Count} rows from SpsMasters.");

        // Lakukan grouping di C# untuk melihat kelompok dokumen berdasarkan seluruh kolom parameter teknis
        // Kita bandingkan semua kolom kecuali "Id" dan "ItemList"
        var groups = new List<List<Dictionary<string, object>>>();
        foreach (var row in allRows)
        {
            bool matched = false;
            foreach (var group in groups)
            {
                var rep = group[0];
                bool isSame = true;
                foreach (var kv in row)
                {
                    if (kv.Key == "Id" || kv.Key == "ItemList") continue;
                    
                    var val1 = kv.Value;
                    var val2 = rep[kv.Key];

                    if (val1 == DBNull.Value && val2 == DBNull.Value) continue;
                    if (val1 == DBNull.Value || val2 == DBNull.Value) { isSame = false; break; }
                    if (!val1.Equals(val2)) { isSame = false; break; }
                }

                if (isSame)
                {
                    group.Add(row);
                    matched = true;
                    break;
                }
            }

            if (!matched)
            {
                groups.Add(new List<Dictionary<string, object>> { row });
            }
        }

        Console.WriteLine($"Number of unique configurations (grouping by all technical parameters): {groups.Count}");
        
        // Cek apakah ada konflik jika hanya di-group berdasarkan DocumentNumber + Machine + MachineCode + RevisionNumber
        var simpleGroups = new List<List<Dictionary<string, object>>>();
        foreach (var row in allRows)
        {
            bool matched = false;
            foreach (var group in simpleGroups)
            {
                var rep = group[0];
                bool isSame = true;
                string[] keyCols = { "DocumentNumber", "Machine", "MachineCode", "RevisionNumber" };
                foreach (var col in keyCols)
                {
                    var val1 = row[col];
                    var val2 = rep[col];
                    if (val1 == DBNull.Value && val2 == DBNull.Value) continue;
                    if (val1 == DBNull.Value || val2 == DBNull.Value) { isSame = false; break; }
                    if (!val1.Equals(val2)) { isSame = false; break; }
                }

                if (isSame)
                {
                    group.Add(row);
                    matched = true;
                    break;
                }
            }

            if (!matched)
            {
                simpleGroups.Add(new List<Dictionary<string, object>> { row });
            }
        }
        Console.WriteLine($"Number of groups by (DocumentNumber + Machine + MachineCode + RevisionNumber): {simpleGroups.Count}");
    }
} catch (Exception ex) {
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}

