#r "nuget: EPPlus, 7.5.3"
using OfficeOpenXml;
ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

var files = new[] {
    @"D:\extrude\Masterlist SPS CHS 2 Layer DIG.xlsx",
    @"D:\extrude\Masterlist SPS Double Layer.xlsx",
    @"D:\extrude\Masterlist SPS CHS 3 Layer DIG.xlsx",
    @"D:\extrude\Masterlist SPS Double Layer_Digitalisasi.xlsx"
};

var searchItems = new[] { "NA2140", "NA2661", "PE1755" };

foreach (var file in files) {
    if (!System.IO.File.Exists(file)) continue;
    using var pkg = new ExcelPackage(new System.IO.FileInfo(file));
    var ws = pkg.Workbook.Worksheets[0];
    
    for (int r = 1; r <= ws.Dimension.Rows; r++) {
        for (int c = 1; c <= ws.Dimension.Columns; c++) {
            var txt = ws.Cells[r, c].Text?.Trim() ?? "";
            foreach(var item in searchItems) {
                if (txt.Contains(item)) {
                    var doc = ws.Cells[r, 4].Text?.Trim() ?? "UNKNOWN DOC";
                    System.Console.WriteLine($"Found {item} in {System.IO.Path.GetFileName(file)} Row {r} Col {c} (Doc: {doc})");
                }
            }
        }
    }
}
