#r "nuget: EPPlus, 7.5.3"
using OfficeOpenXml;
ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
var pkg = new ExcelPackage(new System.IO.FileInfo(@"D:\extrude\Masterlist SPS Double Layer.xlsx"));
var ws = pkg.Workbook.Worksheets[0];
for (int c = 1; c <= 80; c++) {
    var txt = ws.Cells[25, c].Text?.Trim() ?? "";
    if (!string.IsNullOrEmpty(txt)) {
        System.Console.WriteLine($"Col {c}: {txt}");
    }
}
