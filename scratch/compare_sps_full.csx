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

var connString = "Server=10.14.149.34,1433;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;Encrypt=False;Connect Timeout=30;";
var dbPairs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
var dbItems = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
var dbRows = new List<(string Item, string Doc)>();

using (var conn = new SqlConnection(connString))
{
    conn.Open();
    var cmd = new SqlCommand("SELECT LTRIM(RTRIM(ItemList)), LTRIM(RTRIM(DocumentNumber)) FROM SpsItemLists WHERE ItemList IS NOT NULL AND ItemList <> ''", conn);
    var r = cmd.ExecuteReader();
    while (r.Read())
    {
        var item = r.GetString(0);
        var doc = r.GetString(1);
        dbItems.Add(Norm(item));
        dbPairs.Add($"{Norm(item)}|{doc}");
        dbRows.Add((item, doc));
    }
}

string Norm(string s) => (s ?? "").Replace("-", "").Trim().ToUpperInvariant();

IEnumerable<string> SplitCodes(string raw)
{
    if (string.IsNullOrWhiteSpace(raw)) yield break;
    foreach (var c in raw.Split(new[] { ',', ';', '/', ' ' }, StringSplitOptions.RemoveEmptyEntries))
    {
        var code = c.Trim();
        if (Regex.IsMatch(code, @"[A-Za-z]") && Regex.IsMatch(code, @"\d"))
            yield return Norm(code);
    }
}

bool IsBadDoc(string doc) =>
    string.IsNullOrWhiteSpace(doc) ||
    doc is "-" or "#REF!" or "4" or "SOP/PROD" or "SPS" or "VI-SOP-PROD-###";

var fileConfigs = new[]
{
    (File: @"D:\extrude\Masterlist SPS CHS 2 Layer DIG.xlsx", Sheet: "Parameter Setting", HeaderRow: 3, DataRow: 7, ItemCol: 90, DocCol: 4),
    (File: @"D:\extrude\Masterlist SPS CHS 3 Layer DIG.xlsx", Sheet: "Master 1", HeaderRow: 3, DataRow: 7, ItemCol: 108, DocCol: 4),
    (File: @"D:\extrude\Masterlist SPS Double Layer.xlsx", Sheet: "Parameter Setting", HeaderRow: 3, DataRow: 7, ItemCol: 51, DocCol: 4),
    (File: @"D:\extrude\Masterlist SPS Double Layer_Digitalisasi.xlsx", Sheet: "Parameter Setting", HeaderRow: 3, DataRow: 7, ItemCol: 51, DocCol: 4),
};

var excelItems = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
var excelPairs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
var excelItemToDocs = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

foreach (var cfg in fileConfigs)
{
    if (!File.Exists(cfg.File)) { Console.WriteLine($"SKIP: {cfg.File}"); continue; }

    using var pkg = new ExcelPackage(new FileInfo(cfg.File));
    var ws = pkg.Workbook.Worksheets[cfg.Sheet];

    int rows = ws.Dimension?.Rows ?? 0;
    int itemCol = cfg.ItemCol;

    int count = 0;
    for (int row = cfg.DataRow; row <= rows; row++)
    {
        var codeRaw = ws.Cells[row, itemCol].Text?.Trim();
        var docNum = ws.Cells[row, cfg.DocCol].Text?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(codeRaw)) continue;
        if (codeRaw.Equals("Item", StringComparison.OrdinalIgnoreCase)) continue;

        foreach (var code in SplitCodes(codeRaw))
        {
            excelItems.Add(code);
            if (!IsBadDoc(docNum))
            {
                excelPairs.Add($"{code}|{docNum}");
                if (!excelItemToDocs.ContainsKey(code))
                    excelItemToDocs[code] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                excelItemToDocs[code].Add(docNum);
            }
            count++;
        }
    }

    Console.WriteLine($"{Path.GetFileName(cfg.File)} [{cfg.Sheet}]: {count} item codes (col {itemCol})");
}

var missingInDb = excelItems.Where(x => !dbItems.Contains(x)).OrderBy(x => x).ToList();
var extraInDb = dbItems.Where(x => !excelItems.Contains(x)).OrderBy(x => x).ToList();
var matchedItems = excelItems.Count(x => dbItems.Contains(x));
var matchedPairs = excelPairs.Count(x => dbPairs.Contains(x));
var missingPairs = excelPairs.Where(x => !dbPairs.Contains(x)).OrderBy(x => x).ToList();

Console.WriteLine();
Console.WriteLine("===== RINGKASAN PERBANDINGAN SpsItemList vs Excel SPS =====");
Console.WriteLine($"Database SpsItemLists : {dbRows.Count} baris | {dbItems.Count} item unik | {dbPairs.Count} pasangan item+doc");
Console.WriteLine($"Excel (4 masterlist)  : {excelItems.Count} item unik | {excelPairs.Count} pasangan item+doc");
Console.WriteLine($"Item cocok (ada di keduanya)           : {matchedItems}");
Console.WriteLine($"Pasangan (item+doc) cocok              : {matchedPairs}");
Console.WriteLine($"Item di Excel, BELUM ada di DB         : {missingInDb.Count}");
Console.WriteLine($"Item di DB, TIDAK ada di Excel         : {extraInDb.Count}");
Console.WriteLine($"Pasangan di Excel, BELUM ada di DB       : {missingPairs.Count}");

var outDir = @"D:\extrude\scratch";
Directory.CreateDirectory(outDir);

var missingItemsCsv = Path.Combine(outDir, "sps_missing_in_db.csv");
using (var sw = new StreamWriter(missingItemsCsv, false, Encoding.UTF8))
{
    sw.WriteLine("ItemCode,DocumentNumbers,Status");
    foreach (var x in missingInDb)
    {
        var docs = excelItemToDocs.TryGetValue(x, out var d) ? string.Join("; ", d) : "";
        sw.WriteLine($"\"{x}\",\"{docs}\",\"BELUM DI DB\"");
    }
}
Console.WriteLine($"\nDisimpan: {missingItemsCsv}");

var extraDbCsv = Path.Combine(outDir, "sps_extra_in_db.csv");
using (var sw = new StreamWriter(extraDbCsv, false, Encoding.UTF8))
{
    sw.WriteLine("ItemCode,DocumentNumber,DbId");
    foreach (var x in extraInDb)
    {
        foreach (var row in dbRows.Where(r => Norm(r.Item) == x))
            sw.WriteLine($"\"{x}\",\"{row.Doc}\",\"\"");
    }
}
Console.WriteLine($"Disimpan: {extraDbCsv}");

Console.WriteLine();
Console.WriteLine("--- Item BELUM ada di DB ---");
foreach (var x in missingInDb)
{
    var docs = excelItemToDocs.TryGetValue(x, out var d) ? string.Join(", ", d.Take(2)) : "(tanpa doc valid)";
    Console.WriteLine($"  {x,-12} doc: {docs}");
}

Console.WriteLine();
Console.WriteLine("--- Sample item di DB tapi tidak di Excel (15 pertama) ---");
foreach (var x in extraInDb.Take(15))
{
    var doc = dbRows.FirstOrDefault(r => Norm(r.Item) == x).Doc;
    Console.WriteLine($"  {x,-12} doc: {doc}");
}
if (extraInDb.Count > 15)
    Console.WriteLine($"  ... dan {extraInDb.Count - 15} item lainnya");
