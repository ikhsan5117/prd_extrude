using System;
using System.IO;
using OfficeOpenXml;

namespace SearchExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            string[] targets = { "TA297", "1500", "TA298" };
            string[] files = {
                "Masterlist SPS CHS 2 Layer DIG.xlsx",
                "Masterlist SPS CHS 3 Layer DIG.xlsx",
                "Masterlist SPS Double Layer.xlsx",
                "Masterlist SPS Double Layer_Digitalisasi.xlsx"
            };

            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "../../");
            if (!File.Exists(Path.Combine(basePath, files[0])))
            {
                basePath = Directory.GetCurrentDirectory(); // Fallback to current dir
            }

            Console.WriteLine("Searching for items: " + string.Join(", ", targets));
            Console.WriteLine("Base Path: " + Path.GetFullPath(basePath));
            Console.WriteLine();

            bool foundAny = false;

            foreach (var filename in files)
            {
                string filepath = Path.Combine(basePath, filename);
                if (!File.Exists(filepath))
                {
                    continue;
                }

                try
                {
                    using var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var package = new ExcelPackage(stream);

                    foreach (var ws in package.Workbook.Worksheets)
                    {
                        int rowCount = ws.Dimension?.Rows ?? 0;
                        int colCount = ws.Dimension?.Columns ?? 0;

                        for (int r = 1; r <= rowCount; r++)
                        {
                            for (int c = 1; c <= colCount; c++)
                            {
                                var text = ws.Cells[r, c].Text;
                                if (string.IsNullOrWhiteSpace(text)) continue;

                                foreach (var target in targets)
                                {
                                    if (text.Contains(target, StringComparison.OrdinalIgnoreCase))
                                    {
                                        Console.WriteLine($"MATCH FOUND!");
                                        Console.WriteLine($"  File  : {filename}");
                                        Console.WriteLine($"  Sheet : {ws.Name}");
                                        Console.WriteLine($"  Cell  : Row {r}, Col {c} ({ws.Cells[r, c].Address})");
                                        Console.WriteLine($"  Value : '{text}'");
                                        // Print doc number at col 4 if possible
                                        var docNum = ws.Cells[r, 4].Text;
                                        Console.WriteLine($"  Doc # : '{docNum}'");
                                        Console.WriteLine();
                                        foundAny = true;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading {filename}: {ex.Message}");
                }
            }

            if (!foundAny)
            {
                Console.WriteLine("No matches found in any of the Excel files.");
            }
        }
    }
}
