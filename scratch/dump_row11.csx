#r "nuget: EPPlus, 7.5.3"
using OfficeOpenXml;
ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
var pkg = new ExcelPackage(new System.IO.FileInfo(@"D:\extrude\Masterlist SPS Double Layer.xlsx"));
var ws = pkg.Workbook.Worksheets[0];
for (int c = 1; c <= 80; c++) {
    var txt = ws.Cells[11, c].Text?.Trim() ?? "";
    if (!string.IsNullOrEmpty(txt)) {
        var header = ws.Cells[4, c].Text?.Trim() ?? ws.Cells[5, c].Text?.Trim() ?? "";
        System.Console.WriteLine($"Col {c} [{header}]: {txt}");
    }
}
