#r "nuget: EPPlus, 7.5.3"
using OfficeOpenXml;
ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
using (var pkg = new ExcelPackage(new System.IO.FileInfo(@"D:\extrude\Masterlist SPS Double Layer.xlsx")))
{
    var ws = pkg.Workbook.Worksheets[0];
    for(int c=45; c<=55; c++) System.Console.WriteLine($"Col {c}: " + ws.Cells[3,c].Text);
}
