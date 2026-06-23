#r "nuget: EPPlus, 7.5.3"
using OfficeOpenXml;
ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
var pkg = new ExcelPackage(new System.IO.FileInfo(@"D:\extrude\Masterlist SPS Double Layer.xlsx"));
var ws = pkg.Workbook.Worksheets[0];

int itemListCol = 0;
// Cari kolom item list di header
for (int c = 1; c <= 60; c++) {
    var txt = ws.Cells[3, c].Text?.Trim().ToUpper() ?? ws.Cells[4, c].Text?.Trim().ToUpper() ?? "";
    if (txt.Contains("ITEM") || txt.Contains("ITEM LIST") || txt.Contains("ITEMLIST")) {
        itemListCol = c;
        break;
    }
}
if (itemListCol == 0) itemListCol = 2; // Default legacy

int[] rows = { 11, 18, 25, 26, 29, 31 };
foreach(var r in rows) {
    var doc = ws.Cells[r, 4].Text?.Trim() ?? "";
    var items = ws.Cells[r, itemListCol].Text?.Trim() ?? "";
    System.Console.WriteLine($"Doc: {doc} | Items: {items}");
}
