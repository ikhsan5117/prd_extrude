#r "nuget: EPPlus, 7.5.3"
#r "nuget: Microsoft.Data.SqlClient, 5.2.1"

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using Microsoft.Data.SqlClient;

ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

var targets = new[] { "PM1230", "PM1270", "PM1280", "PM1510", "FM1450", "FM1940", "MS0230", "NA2060", "NA2120", "NA2140", "NA2661" };
var targetSet = new HashSet<string>(targets, StringComparer.OrdinalIgnoreCase);

string Norm(string s) => (s ?? "").Replace("-", "").Replace(" ", "").Trim().ToUpperInvariant();

var files = new[]
{
    @"D:\extrude\Masterlist SPS CHS 2 Layer DIG.xlsx",
    @"D:\extrude\Masterlist SPS CHS 3 Layer DIG.xlsx",
    @"D:\extrude\Masterlist SPS Double Layer.xlsx",
    @"D:\extrude\Masterlist SPS Double Layer_Digitalisasi.xlsx",
};

var hits = new List<(string Item, string File, string Sheet, int Row, string DocCol4, string ItemCell, string NearbyInfo)>();

foreach (var file in files)
{
    if (!File.Exists(file)) continue;
    using var pkg = new ExcelPackage(new FileInfo(file));
    foreach (var ws in pkg.Workbook.Worksheets)
    {
        int rows = ws.Dimension?.Rows ?? 0;
        int cols = ws.Dimension?.Columns ?? 0;
        if (rows == 0) continue;

        for (int r = 1; r <= Math.Min(rows, 500); r++)
        {
            for (int c = 1; c <= cols; c++)
            {
                var text = ws.Cells[r, c].Text?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(text)) continue;

                foreach (var part in text.Split(new[] { ',', ';', '/', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var code = Norm(part);
                    if (!targetSet.Contains(code)) continue;

                    var doc = ws.Cells[r, 4].Text?.Trim() ?? "";
                    var no = ws.Cells[r, 2].Text?.Trim() ?? "";
                    var machine = ws.Cells[r, 3].Text?.Trim() ?? "";
                    var formulasi = ws.Cells[r, 8].Text?.Trim() ?? "";
                    if (formulasi.Length > 60) formulasi = formulasi.Substring(0, 60) + "...";

                    hits.Add((
                        code,
                        Path.GetFileName(file),
                        ws.Name,
                        r,
                        doc,
                        text.Length > 80 ? text.Substring(0, 80) + "..." : text,
                        $"No={no} | Machine={machine} | Formulasi={formulasi}"
                    ));
                }
            }
        }
    }
}

Console.WriteLine("===== KONTEKS 11 ITEM DI EXCEL (semua kemunculan) =====\n");

foreach (var item in targets)
{
    var itemHits = hits.Where(h => h.Item.Equals(item, StringComparison.OrdinalIgnoreCase)).ToList();
    Console.WriteLine($"--- {item} ({itemHits.Count} kemunculan) ---");
    if (itemHits.Count == 0)
    {
        Console.WriteLine("  Tidak ditemukan di Excel");
        continue;
    }

    foreach (var h in itemHits)
    {
        var docStatus = string.IsNullOrWhiteSpace(h.DocCol4) || h.DocCol4 is "-" or "#REF!"
            ? "[DOC KOSONG/ERROR]"
            : h.DocCol4;
        Console.WriteLine($"  File   : {h.File}");
        Console.WriteLine($"  Sheet  : {h.Sheet} | Baris: {h.Row}");
        Console.WriteLine($"  Doc(D4): {docStatus}");
        Console.WriteLine($"  Cell   : {h.ItemCell}");
        Console.WriteLine($"  Info   : {h.NearbyInfo}");
        Console.WriteLine();
    }
}

// Cek apakah item serupa (prefix) sudah ada di DB dengan doc
Console.WriteLine("\n===== REFERENSI: ITEM SEJENIS YANG SUDAH ADA DI DB =====\n");

var dbPassword = "H1s@na2025!!";
var connString = $"Server=10.14.149.34,1433;Database=prd_extrude_hose;User Id=usrvelasto;Password={dbPassword};TrustServerCertificate=True;Encrypt=False;Connect Timeout=30;";
var conn = new SqlConnection(connString);
conn.Open();

foreach (var item in targets)
{
    var prefix = new string(item.TakeWhile(char.IsLetter).ToArray());
    if (prefix.Length < 2) prefix = item.Substring(0, 2);

    var cmd = new SqlCommand(
        "SELECT TOP 5 ItemList, DocumentNumber FROM SpsItemLists WHERE ItemList LIKE @p + '%' ORDER BY ItemList",
        conn);
    cmd.Parameters.AddWithValue("@p", prefix);

    var similar = new List<string>();
    using (var r = cmd.ExecuteReader())
    {
        while (r.Read())
            similar.Add($"{r.GetString(0)} -> {r.GetString(1)}");
    }

    if (similar.Count > 0)
    {
        Console.WriteLine($"{item} — item {prefix}* yang sudah di DB (contoh):");
        foreach (var s in similar.Take(3))
            Console.WriteLine($"  {s}");
        Console.WriteLine();
    }
}
