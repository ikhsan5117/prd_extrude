#r "nuget: EPPlus, 7.5.3"
using OfficeOpenXml;
ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

var files = new[] {
    @"D:\extrude\Masterlist SPS CHS 2 Layer DIG.xlsx",
    @"D:\extrude\Masterlist SPS Double Layer.xlsx",
    @"D:\extrude\Masterlist SPS CHS 3 Layer DIG.xlsx",
    @"D:\extrude\Masterlist SPS Double Layer_Digitalisasi.xlsx"
};

var targets = new[] {
    "SOP/PROD/HOSE/24/10/068",
    "SOP/PROD/HOSE/SPS/24/10/05",
    "SOP/PROD/HOSE/SPS/24/10/12",
    "SOP/PROD/HOSE/SPS/24/10/19",
    "SOP/PROD/HOSE/SPS/24/10/20",
    "SOP/PROD/HOSE/SPS/24/10/23",
    "SOP/PROD/HOSE/SPS/24/10/25"
};

foreach (var file in files)
{
    if (!System.IO.File.Exists(file)) continue;
    using var pkg = new ExcelPackage(new System.IO.FileInfo(file));
    var ws = pkg.Workbook.Worksheets[0];
    int cols = ws.Dimension?.Columns ?? 0;
    int rows = ws.Dimension?.Rows ?? 0;
    
    for (int r = 1; r <= rows; r++)
    {
        for (int c = 1; c <= cols; c++)
        {
            var txt = ws.Cells[r, c].Text?.Trim() ?? "";
            if (targets.Contains(txt))
            {
                System.Console.WriteLine($"Found {txt} in file: {System.IO.Path.GetFileName(file)} at Row: {r}");
            }
        }
    }
}
