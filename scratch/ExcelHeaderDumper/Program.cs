using System;
using System.IO;
using System.Data;
using System.Linq;
using ExcelDataReader;

namespace ExcelHeaderDumper
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            string[] files = {
                @"F:\VelastoProductionSystem\Masterlist SPS CHS 2 Layer DIG.xlsx",
                @"F:\VelastoProductionSystem\Masterlist SPS CHS 3 Layer DIG.xlsx",
                @"F:\VelastoProductionSystem\Masterlist SPS Double Layer_Digitalisasi.xlsx"
            };

            foreach (var file in files)
            {
                Console.WriteLine($"\nANALYZING FILE: {Path.GetFileName(file)}");
                if (!File.Exists(file)) { Console.WriteLine("ERROR: File not found."); continue; }

                try
                {
                    using (var stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var result = reader.AsDataSet();
                            foreach (DataTable table in result.Tables)
                            {
                                if (table.TableName != "Parameter Setting" && table.TableName != "Master 1") continue;
                                
                                Console.WriteLine($"\n--- SHEET: {table.TableName} ---");
                                for (int i = 0; i < Math.Min(15, table.Rows.Count); i++)
                                {
                                    Console.Write($"Row {i + 1}: ");
                                    for (int j = 0; j < table.Columns.Count; j++)
                                    {
                                        string val = table.Rows[i][j]?.ToString()?.Trim() ?? "";
                                        if (!string.IsNullOrEmpty(val)) Console.Write($"[C{j}:{val}] ");
                                    }
                                    Console.WriteLine();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine($"ERROR: {ex.Message}"); }
            }
        }
    }
}
