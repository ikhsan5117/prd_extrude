#r "nuget: EPPlus, 7.5.3"
using OfficeOpenXml;
ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
var pkg = new ExcelPackage(new System.IO.FileInfo(@"D:\extrude\Masterlist SPS Double Layer_Digitalisasi.xlsx"));
var ws = pkg.Workbook.Worksheets[0];
for (int r = 4; r <= ws.Dimension.Rows; r++) {
    var txt = ws.Cells[r, 4].Text?.Trim() ?? "";
    if (txt.Contains("EPR")) {
        System.Console.WriteLine($"Row {r} Col 4: {txt}");
    }
}
