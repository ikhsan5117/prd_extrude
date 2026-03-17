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
            string filePath = @"f:\VelastoProductionSystem\Masterlist SPS Double Layer_Digitalisasi.xlsx";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    foreach (DataTable table in result.Tables)
                    {
                        Console.WriteLine($"Sheet: {table.TableName}");
                        for (int r = 0; r < Math.Min(10, table.Rows.Count); r++)
                        {
                            Console.Write($"Row {r}: ");
                            for (int c = 0; c < table.Columns.Count; c++)
                            {
                                Console.Write($"[{table.Rows[r][c]}] ");
                            }
                            Console.WriteLine();
                        }
                        Console.WriteLine("-----------------------------------");
                    }
                }
            }
        }
    }
}
