#r "nuget: EPPlus, 7.5.3"
using OfficeOpenXml;
ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
using (var pkg = new ExcelPackage(new System.IO.FileInfo(@"D:\extrude\Masterlist SPS Double Layer.xlsx")))
{
    var ws = pkg.Workbook.Worksheets[0];
    int rowCount = ws.Dimension.Rows;
    int bothCount = 0;
    int eprOnlyCount = 0;
    int spsOnlyCount = 0;

    for (int r = 4; r <= rowCount; r++)
    {
        string sps = ws.Cells[r, 4].Text;
        string epr = ws.Cells[r, 49].Text;
        
        bool hasSps = !string.IsNullOrWhiteSpace(sps) && sps.StartsWith("SOP");
        bool hasEpr = !string.IsNullOrWhiteSpace(epr) && epr.StartsWith("SOP");

        if (hasSps && hasEpr) bothCount++;
        else if (hasEpr) eprOnlyCount++;
        else if (hasSps) spsOnlyCount++;
    }
    System.Console.WriteLine($"Both SPS and EPR on same row: {bothCount}");
    System.Console.WriteLine($"EPR ONLY (no SPS on row): {eprOnlyCount}");
    System.Console.WriteLine($"SPS ONLY (no EPR on row): {spsOnlyCount}");
}
