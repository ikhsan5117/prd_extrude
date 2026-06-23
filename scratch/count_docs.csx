#r "nuget: EPPlus, 7.5.3"
using OfficeOpenXml;
using System.Linq;
using System.Collections.Generic;
ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
var pkg = new ExcelPackage(new System.IO.FileInfo(@"D:\extrude\Masterlist SPS Double Layer.xlsx"));
var ws = pkg.Workbook.Worksheets[0];
var docs = new HashSet<string>();
for (int r = 4; r <= ws.Dimension.Rows; r++) {
    var txt = ws.Cells[r, 4].Text?.Trim() ?? "";
    if (txt.StartsWith("SOP")) docs.Add(txt);
}
System.Console.WriteLine($"Total Unique SPS Docs in file: {docs.Count}");
