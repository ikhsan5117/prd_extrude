#r "nuget: EPPlus, 7.5.3"
#r "nuget: Microsoft.Data.SqlClient, 5.2.1"

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using Microsoft.Data.SqlClient;

ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

var connString = "Server=10.14.149.34,1433;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;Encrypt=False;Connect Timeout=30;";
var dbItems = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

using (var conn = new SqlConnection(connString))
{
    conn.Open();
    var cmd = new SqlCommand("SELECT DISTINCT LTRIM(RTRIM(ItemList)) FROM SpsItemLists WHERE ItemList IS NOT NULL AND ItemList <> ''", conn);
    var r = cmd.ExecuteReader();
    while (r.Read()) dbItems.Add(r.GetString(0));
}

var fileConfigs = new[]
{
    (File: @"D:\extrude\Masterlist SPS CHS 2 Layer DIG.xlsx",   HeaderRow: 3, DataRow: 7, ItemCol: 90, DocCol: 4),
    (File: @"D:\extrude\Masterlist SPS CHS 3 Layer DIG.xlsx",   HeaderRow: 3, DataRow: 7, ItemCol: 0,  DocCol: 4),
    (File: @"D:\extrude\Masterlist SPS Double Layer.xlsx",       HeaderRow: 3, DataRow: 7, ItemCol: 0,  DocCol: 4),
};

var missing = new List<(string Item, string DocNum, string File)>();

foreach (var cfg in fileConfigs)
{
    if (!File.Exists(cfg.File)) continue;
    
    var fname = Path.GetFileName(cfg.File);
    using var pkg = new ExcelPackage(new FileInfo(cfg.File));
    var ws = pkg.Workbook.Worksheets[0];
    int rows = ws.Dimension?.Rows ?? 0;
    int cols = ws.Dimension?.Columns ?? 0;
    
    int itemCol = cfg.ItemCol;
    if (itemCol == 0)
    {
        for (int c = 1; c <= cols; c++)
        {
            var h = ws.Cells[cfg.HeaderRow, c].Text?.Trim().ToUpper();
            if (h == "ITEM" || h == "ITEM CODE" || h == "KODE ITEM") { itemCol = c; break; }
        }
    }
    
    if (itemCol == 0) continue;
    
    for (int row = cfg.DataRow; row <= rows; row++)
    {
        var codeRaw = ws.Cells[row, itemCol].Text?.Trim();
        if (string.IsNullOrWhiteSpace(codeRaw)) continue;
        
        var docNum = ws.Cells[row, cfg.DocCol].Text?.Trim() ?? "";
        
        // Pisahkan jika ada koma (misal: "NA1440,NA1430" atau "TA1773, TA1774")
        var codes = codeRaw.Split(new[] { ',', ';', '/', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var c in codes)
        {
            var code = c.Trim();
            if (!Regex.IsMatch(code, @"[A-Za-z]") || !Regex.IsMatch(code, @"\d")) continue;
            
            if (!dbItems.Contains(code))
            {
                // Cek agar tidak terduplikasi di list hasil
                if (!missing.Any(m => m.Item == code))
                    missing.Add((code, docNum, fname));
            }
        }
    }
}

if (missing.Count > 0)
{
    var sorted = missing.OrderBy(x => x.Item).ToList();
    Console.WriteLine($"{"No",-4} {"Item Code",-12} {"No. Document",-35} {"File"}");
    Console.WriteLine(new string('-', 90));
    for (int i = 0; i < Math.Min(sorted.Count, 30); i++)
        Console.WriteLine($"{i+1,-4} {sorted[i].Item,-12} {sorted[i].DocNum,-35} {sorted[i].File}");
    
    if (sorted.Count > 30) Console.WriteLine($"... dan {sorted.Count - 30} item lainnya.");
    Console.WriteLine($"\nTotal Item yang belum ada di DB: {sorted.Count}");
}
else
{
    Console.WriteLine("Semua item di Excel sudah ada di DB!");
}
