#r "nuget: EPPlus, 7.5.3"
using System; using System.IO; using OfficeOpenXml;
ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
var files = new (string File, string Sheet, int Col)[]
{
    ("D:\\extrude\\Masterlist SPS CHS 3 Layer DIG.xlsx", "Master 1", 108),
    ("D:\\extrude\\Masterlist SPS Double Layer.xlsx", "Parameter Setting", 51),
    ("D:\\extrude\\Masterlist SPS Double Layer.xlsx", "Print", 51),
    ("D:\\extrude\\Masterlist SPS Double Layer_Digitalisasi.xlsx", "Parameter Setting", 51),
};
foreach (var (f, s, col) in files)
{
    using var pkg = new ExcelPackage(new FileInfo(f));
    var ws = pkg.Workbook.Worksheets[s];
    Console.WriteLine($"{Path.GetFileName(f)} [{s}] col {col} header='{ws.Cells[3, col].Text}'");
    for (int r = 7; r <= 12; r++)
    {
        var item = ws.Cells[r, col].Text ?? "";
        if (item.Length > 50) item = item.Substring(0, 50) + "...";
        Console.WriteLine($"  row {r}: doc='{ws.Cells[r, 4].Text}' item='{item}'");
    }
}
