#r "nuget: EPPlus, 7.5.3"
using OfficeOpenXml;
ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
var pkg = new ExcelPackage(new System.IO.FileInfo(@"D:\extrude\Masterlist SPS Double Layer.xlsx"));
var ws = pkg.Workbook.Worksheets[0];
int[] rows = { 11, 18, 25, 26, 29, 31 };
foreach(var r in rows) {
    System.Console.WriteLine($"Row {r} Col 34: '{ws.Cells[r, 34].Text}'");
}
