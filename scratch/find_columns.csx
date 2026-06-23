#r "nuget: EPPlus, 7.5.3"

using System;
using System.IO;
using System.Linq;
using OfficeOpenXml;

ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

var files = new[] {
    @"D:\extrude\Masterlist SPS CHS 2 Layer DIG.xlsx",
    @"D:\extrude\Masterlist SPS Double Layer.xlsx",
    @"D:\extrude\Masterlist SPS CHS 3 Layer DIG.xlsx",
    @"D:\extrude\Masterlist SPS Double Layer_Digitalisasi.xlsx"
};

foreach (var file in files)
{
    if (!File.Exists(file)) continue;
    var pkg = new ExcelPackage(new FileInfo(file));
    var ws = pkg.Workbook.Worksheets[0];
    int cols = ws.Dimension != null ? ws.Dimension.Columns : 0;
    int rows = ws.Dimension != null ? ws.Dimension.Rows : 0;
    
    Console.WriteLine("\n=== " + Path.GetFileName(file) + " ===");
    
    // Find column that might contain item codes like HN, NA, TA, etc.
    // Read the first 50 rows
    for (int r = 1; r <= Math.Min(rows, 50); r++)
    {
        for (int c = 1; c <= cols; c++)
        {
            var v = ws.Cells[r, c].Text?.Trim() ?? "";
            if (v.StartsWith("HN") || v.StartsWith("TA") || v.StartsWith("NA") || v.StartsWith("PE"))
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(v, @"^[A-Z]{2}\d{2,}$"))
                {
                    Console.WriteLine("    Row " + r + " Col " + c + ": " + v);
                }
            }
            if (v.Contains("SOP/PROD")) 
            {
                Console.WriteLine("    Doc Row " + r + " Col " + c + ": " + v);
            }
        }
    }
    
    pkg.Dispose();
}
