#r "nuget: EPPlus, 7.5.3"
using System; using System.IO; using System.Text.RegularExpressions;
using OfficeOpenXml;
ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

void ScanFile(string file, string sheet)
{
    using var pkg = new ExcelPackage(new FileInfo(file));
    var ws = pkg.Workbook.Worksheets[sheet];
    int rows = ws.Dimension?.Rows ?? 0;
    int cols = ws.Dimension?.Columns ?? 0;
    Console.WriteLine($"\n{Path.GetFileName(file)} [{sheet}] rows={rows} cols={cols}");
    for (int c = 1; c <= cols; c++)
    {
        int hits = 0;
        string sample = "";
        for (int r = 7; r <= Math.Min(rows, 200); r++)
        {
            var v = ws.Cells[r, c].Text?.Trim() ?? "";
            if (Regex.IsMatch(v, @"[A-Z]{1,3}[-]?\d{3,}") && v.Length < 200)
            {
                hits++;
                if (sample == "") sample = v.Length > 40 ? v.Substring(0, 40) + "..." : v;
            }
        }
        if (hits >= 3)
            Console.WriteLine($"  Col {c} ({ws.Cells[3,c].Text}): {hits} hits, sample='{sample}'");
    }
}

ScanFile(@"D:\extrude\Masterlist SPS Double Layer.xlsx", "Parameter Setting");
ScanFile(@"D:\extrude\Masterlist SPS Double Layer.xlsx", "Print");
ScanFile(@"D:\extrude\Masterlist SPS CHS 3 Layer DIG.xlsx", "Master 1");
