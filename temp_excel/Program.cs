using System;
using System.Data;
using System.IO;
using ExcelDataReader;

namespace ExcelInspector
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"F:\VelastoProductionSystem\Masterlist SPS Double Layer_Digitalisasi.xlsx";
            
            try {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet();
                        foreach (DataTable table in result.Tables)
                        {
                            Console.WriteLine($"[SHEET_NAME]: {table.TableName}");
                            for (int r = 0; r < Math.Min(5, table.Rows.Count); r++)
                            {
                                Console.Write($"[ROW_{r}]: ");
                                for (int c = 0; c < table.Columns.Count; c++)
                                {
                                    string val = table.Rows[r][c]?.ToString()?.Replace("\n", " ").Replace("\r", " ") ?? "";
                                    Console.Write($"|C{c}: {val}| ");
                                }
                                Console.WriteLine();
                            }
                            Console.WriteLine("-----------------------------------");
                        }
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }
    }
}
