using System;
using System.IO;
using System.Data;
using ExcelDataReader;

namespace DbCheckApp
{
    class Program {
        static void Main(string[] args) {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            
            string filePath = "Masterlist SPS CHS 2 Layer DIG.xlsx";
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var result = reader.AsDataSet();
            
            DataTable table = null;
            foreach(DataTable t in result.Tables) {
                if (t.TableName.Contains("Parameter Setting") || t.TableName.Contains("Master 1")) {
                    table = t; break;
                }
            }

            if (table == null) return;

            Console.WriteLine($"=== ANALISIS DATA ROW 20 (TA1440) ===\n");
            
            var row = table.Rows[20];
            for (int i = 30; i <= 46; i++) {
                Console.WriteLine($"[{i}] {row[i]}");
            }
        }
    }
}
