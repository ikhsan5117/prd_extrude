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

var dbDocs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
var dbItems = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

string connString = "Server=10.14.149.34,1433;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;Connect Timeout=30;";
using (var conn = new SqlConnection(connString))
{
    conn.Open();
    using (var cmd = new SqlCommand("SELECT DocumentNumber FROM SpsNoDocs WHERE DocumentNumber IS NOT NULL", conn))
    using (var reader = cmd.ExecuteReader())
    {
        while (reader.Read())
        {
            var doc = reader.GetString(0).Trim();
            if (!string.IsNullOrEmpty(doc)) dbDocs.Add(doc);
        }
    }
    
    using (var cmd = new SqlCommand("SELECT ItemList FROM SpsItemLists WHERE ItemList IS NOT NULL", conn))
    using (var reader = cmd.ExecuteReader())
    {
        while (reader.Read())
        {
            var item = reader.GetString(0).Trim();
            if (!string.IsNullOrEmpty(item)) dbItems.Add(item);
        }
    }
}

var excelDocs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
var excelItems = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

var files = new[] {
    @"D:\extrude\Masterlist SPS CHS 2 Layer DIG.xlsx",
    @"D:\extrude\Masterlist SPS Double Layer.xlsx",
    @"D:\extrude\Masterlist SPS CHS 3 Layer DIG.xlsx",
    @"D:\extrude\Masterlist SPS Double Layer_Digitalisasi.xlsx"
};

var itemRegex = new Regex(@"^[A-Z]{2}\d{2,}$", RegexOptions.Compiled);

foreach (var file in files)
{
    if (!File.Exists(file)) continue;
    using var pkg = new ExcelPackage(new FileInfo(file));
    var ws = pkg.Workbook.Worksheets[0];
    int cols = ws.Dimension?.Columns ?? 0;
    int rows = ws.Dimension?.Rows ?? 0;
    
    for (int r = 1; r <= rows; r++)
    {
        for (int c = 1; c <= cols; c++)
        {
            var v = ws.Cells[r, c].Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(v)) continue;
            
            // Collect doc numbers
            if (v.StartsWith("SOP/PROD"))
            {
                excelDocs.Add(v);
            }
            
            // Collect item codes (HN0170, TA1080, etc)
            if (v.StartsWith("HN") || v.StartsWith("TA") || v.StartsWith("NA") || v.StartsWith("PE") || v.StartsWith("HR"))
            {
                if (itemRegex.IsMatch(v))
                {
                    excelItems.Add(v);
                }
            }
        }
    }
}

var missingDocs = excelDocs.Except(dbDocs).OrderBy(x => x).ToList();
var missingItems = excelItems.Except(dbItems).OrderBy(x => x).ToList();

Console.WriteLine("=== HASIL ANALISA ===");
Console.WriteLine($"Total No. Dokumen di DB: {dbDocs.Count}");
Console.WriteLine($"Total No. Dokumen di Excel: {excelDocs.Count}");
Console.WriteLine($"Total No. Dokumen kurang di DB: {missingDocs.Count}");

if (missingDocs.Any())
{
    Console.WriteLine("\n[No. Dokumen yang BELUM ADA di Database]");
    foreach (var d in missingDocs) Console.WriteLine("- " + d);
}

Console.WriteLine($"\nTotal Item List di DB: {dbItems.Count}");
Console.WriteLine($"Total Item List di Excel: {excelItems.Count}");
Console.WriteLine($"Total Item List kurang di DB: {missingItems.Count}");

if (missingItems.Any())
{
    Console.WriteLine("\n[Item List yang BELUM ADA di Database]");
    foreach (var i in missingItems) Console.WriteLine("- " + i);
}
