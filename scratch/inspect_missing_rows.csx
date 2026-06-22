#r "nuget: EPPlus, 7.5.3"
using System; using System.IO; using OfficeOpenXml;
ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

void ShowRow(string file, string sheet, int row)
{
    using var pkg = new ExcelPackage(new FileInfo(file));
    var ws = pkg.Workbook.Worksheets[sheet];
    Console.WriteLine($"\n=== {Path.GetFileName(file)} [{sheet}] baris {row} ===");
    Console.WriteLine($"  Col2 No      : {ws.Cells[row,2].Text}");
    Console.WriteLine($"  Col3 Machine : {ws.Cells[row,3].Text}");
    Console.WriteLine($"  Col4 Doc     : '{ws.Cells[row,4].Text}'");
    Console.WriteLine($"  Col5         : {ws.Cells[row,5].Text}");
    Console.WriteLine($"  Col6         : {ws.Cells[row,6].Text}");
    Console.WriteLine($"  Col8 Formulasi: {(ws.Cells[row,8].Text?.Length>70 ? ws.Cells[row,8].Text.Substring(0,70)+"..." : ws.Cells[row,8].Text)}");
    Console.WriteLine($"  Col Item     : {ws.Cells[row, ws.Dimension.Columns >= 108 ? 108 : 51].Text ?? ws.Cells[row,90].Text}");
    // baris tetangga
    if (row > 7) {
        Console.WriteLine($"  [baris {row-1}] Doc: '{ws.Cells[row-1,4].Text}' | Item: {ws.Cells[row-1, ws.Dimension.Columns >= 108 ? 108 : 51].Text}");
    }
    if (row < ws.Dimension.Rows) {
        Console.WriteLine($"  [baris {row+1}] Doc: '{ws.Cells[row+1,4].Text}' | Item: {ws.Cells[row+1, ws.Dimension.Columns >= 108 ? 108 : 51].Text}");
    }
}

ShowRow(@"D:\extrude\Masterlist SPS CHS 2 Layer DIG.xlsx", "Parameter Setting", 82);
ShowRow(@"D:\extrude\Masterlist SPS CHS 3 Layer DIG.xlsx", "Master 1", 23);
ShowRow(@"D:\extrude\Masterlist SPS Double Layer.xlsx", "Parameter Setting", 55);
ShowRow(@"D:\extrude\Masterlist SPS Double Layer.xlsx", "Parameter Setting", 69);
ShowRow(@"D:\extrude\Masterlist SPS Double Layer.xlsx", "Parameter Setting", 70);
ShowRow(@"D:\extrude\Masterlist SPS Double Layer.xlsx", "Parameter Setting", 71);
ShowRow(@"D:\extrude\Masterlist SPS Double Layer.xlsx", "Parameter Setting", 72);
