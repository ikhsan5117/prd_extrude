#r "nuget: EPPlus, 7.5.3"

using System;
using System.IO;
using System.Linq;
using OfficeOpenXml;

ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

// Cek semua file, tampilkan SEMUA header baris 3
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
    Console.WriteLine("  Rows: " + rows + ", Cols: " + cols);
    
    // Tampilkan seluruh header baris 3
    Console.WriteLine("  Header baris 3:");
    for (int c = 1; c <= cols; c++)
    {
        var v = ws.Cells[3, c].Text;
        if (!string.IsNullOrWhiteSpace(v))
            Console.WriteLine("    Col " + c + ": " + v);
    }
    
    // Cari nilai yang mirip kode item (HN, TA, NA, dll) di 20 baris awal
    Console.WriteLine("  Cari kode item di baris 7-12 semua kolom:");
    for (int r = 7; r <= Math.Min(rows, 12); r++)
    {
        for (int c = 1; c <= Math.Min(cols, 15); c++)
        {
            var v = ws.Cells[r, c].Text?.Trim() ?? "";
            // Cari pola kode item: 2 huruf + angka (HN0170, TA1080, NA3311)
            if (System.Text.RegularExpressions.Regex.IsMatch(v, @"^[A-Z]{2}\d{3,}$"))
                Console.WriteLine("    Row " + r + " Col " + c + ": " + v);
        }
    }
    
    pkg.Dispose();
}
