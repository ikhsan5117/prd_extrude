#r "nuget: EPPlus, 7.5.3"
using OfficeOpenXml;
ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

var files = new[] {
    @"D:\extrude\Masterlist SPS CHS 2 Layer DIG.xlsx",
    @"D:\extrude\Masterlist SPS Double Layer.xlsx",
    @"D:\extrude\Masterlist SPS CHS 3 Layer DIG.xlsx",
    @"D:\extrude\Masterlist SPS Double Layer_Digitalisasi.xlsx"
};

foreach (var file in files) {
    if (!System.IO.File.Exists(file)) continue;
    using var pkg = new ExcelPackage(new System.IO.FileInfo(file));
    var ws = pkg.Workbook.Worksheets[0];
    System.Console.WriteLine($"\nFile: {System.IO.Path.GetFileName(file)}");
    
    // Asumsi header ada di baris 3 atau 4
    for (int r = 1; r <= 5; r++) {
        for (int c = 1; c <= 60; c++) {
            var txt = ws.Cells[r, c].Text?.Trim() ?? "";
            if (txt.Contains("DOC") || txt.Contains("EPR") || txt.Contains("SPS")) {
                System.Console.WriteLine($"  Row {r} Col {c}: {txt}");
            }
        }
    }
}
