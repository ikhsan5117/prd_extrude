using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace SpsSyncApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            Console.WriteLine("======================================================================");
            Console.WriteLine("          SPS MASTERLIST EXCEL TO DATABASE SYNCHRONIZER");
            Console.WriteLine("======================================================================");
            Console.WriteLine("Starting at: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            // 1. Load connection string from appsettings.json
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "../../");
            string appsettingsPath = Path.Combine(basePath, "appsettings.json");
            if (!File.Exists(appsettingsPath))
            {
                basePath = Directory.GetCurrentDirectory(); // Fallback to current dir
                appsettingsPath = Path.Combine(basePath, "appsettings.json");
            }

            Console.WriteLine($"Reading configuration from: {Path.GetFullPath(appsettingsPath)}");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Path.GetFullPath(appsettingsPath))!)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.Error.WriteLine("Error: DefaultConnection connection string not found in appsettings.json!");
                return;
            }

            // 2. Set up DbContext
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using var context = new ApplicationDbContext(optionsBuilder.Options);
            Console.WriteLine("Connected to database successfully.");

            // 3. Define Excel files to sync
            var excelFiles = new[]
            {
                "Masterlist SPS CHS 2 Layer DIG.xlsx",
                "Masterlist SPS CHS 3 Layer DIG.xlsx",
                "Masterlist SPS Double Layer.xlsx",
                "Masterlist SPS Double Layer_Digitalisasi.xlsx"
            };

            int totalDocsProcessed = 0;
            int totalDocsCreated = 0;
            int totalDocsUpdated = 0;
            int totalItemsAdded = 0;
            int totalErrors = 0;

            foreach (var filename in excelFiles)
            {
                string filepath = Path.Combine(basePath, filename);
                Console.WriteLine();
                Console.WriteLine($"----------------------------------------------------------------------");
                Console.WriteLine($"[+] Processing file: {filename}");
                Console.WriteLine($"----------------------------------------------------------------------");

                if (!File.Exists(filepath))
                {
                    Console.Error.WriteLine($"Error: File '{filepath}' does not exist. Skipping.");
                    totalErrors++;
                    continue;
                }

                try
                {
                    using var stream = File.OpenRead(filepath);
                    using var package = new ExcelPackage(stream);

                    // Choose sheet using the same smart logic
                    ExcelWorksheet? worksheet = null;
                    string[] sheetKeywords = { "Digitalisasi", "Parameter Setting", "SPS", "Master" };
                    foreach (var ws in package.Workbook.Worksheets)
                    {
                        foreach (var keyword in sheetKeywords)
                        {
                            if (ws.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                            {
                                worksheet = ws;
                                Console.WriteLine($"Selected sheet: '{ws.Name}' (matched keyword: '{keyword}')");
                                break;
                            }
                        }
                        if (worksheet != null) break;
                    }

                    if (worksheet == null)
                    {
                        worksheet = package.Workbook.Worksheets
                            .OrderByDescending(w => w.Dimension?.Rows ?? 0)
                            .FirstOrDefault();
                        if (worksheet != null)
                        {
                            Console.WriteLine($"Selected largest sheet: '{worksheet.Name}' ({worksheet.Dimension?.Rows ?? 0} rows)");
                        }
                    }

                    if (worksheet == null)
                    {
                        Console.Error.WriteLine("Error: No worksheet found in workbook. Skipping.");
                        totalErrors++;
                        continue;
                    }

                    int rowCount = worksheet.Dimension?.Rows ?? 0;
                    int colCount = worksheet.Dimension?.Columns ?? 0;
                    Console.WriteLine($"Sheet dimensions: {rowCount} rows, {colCount} columns");

                    // Detect Header and Data Start Row
                    int headerRow = 1;
                    int headerRow2 = 0;
                    int dataStartRow = 2;

                    var row1Col1 = worksheet.Cells[1, 1].Text?.Trim().ToUpper() ?? "";
                    var row1Col2 = worksheet.Cells[1, 2].Text?.Trim().ToUpper() ?? "";
                    var row1Text = row1Col1 + " " + row1Col2;

                    if (row1Text.Contains("NON-CHS") || row1Text.Contains("SINGLE") || row1Text.Contains("DOUBLE LAYER PARAMETER"))
                    {
                        Console.WriteLine("Format: NON-CHS / Double Layer");
                        headerRow = 3;
                        headerRow2 = 5;
                        dataStartRow = 6;
                    }
                    else if (row1Text.Contains("CHS 2 LAYER") || row1Text.Contains("CHS 3 LAYER") || row1Text.Contains("DIGITALISASI"))
                    {
                        Console.WriteLine("Format: CHS Digitalisasi");
                        headerRow = 3;
                        headerRow2 = 6;
                        dataStartRow = 7;
                    }
                    else if (row1Text.Contains("IDENTIFIKASI") || row1Text.Contains("DOKUMEN"))
                    {
                        Console.WriteLine("Format: Template SPS Full");
                        headerRow = 2;
                        dataStartRow = 3;
                    }
                    else
                    {
                        Console.WriteLine("Format: Simple / Default");
                        headerRow = 1;
                        dataStartRow = 2;
                    }

                    bool isDig3L = row1Text.Contains("CHS 3 LAYER");
                    bool isDig2L = row1Text.Contains("CHS 2 LAYER");
                    bool isLegacy = row1Text.Contains("NON-CHS") || row1Text.Contains("SINGLE") || row1Text.Contains("DOUBLE LAYER PARAMETER");
                    bool isTemplateFull = row1Text.Contains("IDENTIFIKASI") || row1Text.Contains("DOKUMEN");

                    // Initialize Column Indices
                    int idxDimensi = 0, idxItem = 0, idxInner = 0, idxOuter = 0, idxMiddle = 0;
                    int idxRev = 0, idxRevDate = 0, idxCustomer = 0, idxFormulasi = 0;
                    int idxYarn = 0, idxPitchYarn = 0, idxTensionIn = 0, idxTensionOut = 0;
                    int idxNipple = 0, idxTubeDie = 0, idxMiddleDie = 0, idxCoverDie = 0, idxSpacerDie = 0, idxADistance = 0;
                    int idxMeshDim1 = 0, idxMeshScreen1 = 0, idxMeshDim2 = 0, idxMeshScreen2 = 0, idxMeshDim3 = 0, idxMeshScreen3 = 0;
                    int idxUseLimitsInner = 0, idxUseLimitsOuter = 0, idxUseLimitsMiddle = 0;
                    int idxHeadTemp1 = 0, idxCyl11 = 0, idxCyl21 = 0, idxCyl31 = 0, idxScrewTemp1 = 0;
                    int idxHeadTemp2 = 0, idxCyl12 = 0, idxCyl22 = 0, idxCyl32 = 0, idxScrewTemp2 = 0;
                    int idxHeadTemp3 = 0, idxCyl13 = 0, idxCyl23 = 0, idxCyl33 = 0, idxScrewTemp3 = 0;
                    int idxScrewSpeed1 = 0, idxScrewSpeed2 = 0, idxScrewSpeed3 = 0;
                    int idxFeedRoll1 = 0, idxFeedRoll2 = 0, idxFeedRoll3 = 0;
                    int idxPressure1 = 0, idxPressure2 = 0, idxPressure3 = 0;
                    int idxCurrentValue = 0, idxAmMeter = 0, idxAmMeter2 = 0, idxAmMeter3 = 0;
                    int idxPresetValue = 0, idxControlValue = 0;
                    int idxSpiralPitchSetting = 0, idxSpiralPitchDisplay = 0, idxSpiralSpeed = 0, idxHoseSpeed = 0;
                    int idxUnsmoothSurface = 0, idxMarkingSort = 0, idxTextMarkingMaterial = 0, idxMarkingColour = 0;
                    int idxChillerWaterTemp = 0, idxDancerPosition = 0, idxCaterpillarGap = 0;
                    int idxTakeUpConveyorSpeed = 0, idxCoolConveyorSpeed = 0, idxCoolConveyorSpeed2 = 0, idxConveyorRatio = 0;
                    int idxToleranceInner = 0, idxToleranceOuter = 0;
                    int idxTebalInner = 0, idxTebalInnerMiddle = 0, idxTebalTotal = 0, idxSelisihTebal = 0;
                    int idxThickTarget = 0, idxThickTol = 0, idxThickLCL = 0, idxThickMin = 0, idxThickUCL = 0, idxThickMax = 0;
                    int idxInnerTarget = 0, idxInnerTol = 0, idxInnerLCL = 0, idxInnerMin = 0, idxInnerUCL = 0, idxInnerMax = 0;
                    int idxInnerMidTarget = 0, idxInnerMidTol = 0, idxInnerMidLCL = 0, idxInnerMidMin = 0, idxInnerMidUCL = 0, idxInnerMidMax = 0;
                    int idxTotalTarget = 0, idxTotalTol = 0, idxTotalLCL = 0, idxTotalMin = 0, idxTotalUCL = 0, idxTotalMax = 0;
                    int idxFeed1 = 0, idxFeed2 = 0, idxFeed3 = 0, idxTebalOuter = 0, idxODSensor = 0, idxCuttingSpeed = 0;

                    if (isDig3L)
                    {
                        idxDimensi = 12;
                        idxItem = 108;
                        idxRev = 7; idxRevDate = 9; idxCustomer = 8; idxFormulasi = 10;
                        idxInner = 14; idxMiddle = 15; idxOuter = 16;
                        idxUseLimitsInner = 17; idxUseLimitsMiddle = 18; idxUseLimitsOuter = 19;
                        idxYarn = 20; idxTensionIn = 21; idxTensionOut = 22;
                        idxNipple = 23; idxTubeDie = 24; idxMiddleDie = 25; idxCoverDie = 26;
                        idxSpacerDie = 27; idxADistance = 28;
                        idxMeshDim1 = 29; idxMeshScreen1 = 30;
                        idxMeshDim2 = 31; idxMeshScreen2 = 32;
                        idxMeshDim3 = 33; idxMeshScreen3 = 34;
                        idxHeadTemp1 = 35; idxCyl11 = 36; idxCyl21 = 37; idxCyl31 = 38; idxScrewTemp1 = 39;
                        idxHeadTemp2 = 40; idxCyl12 = 41; idxCyl22 = 42; idxCyl32 = 43; idxScrewTemp2 = 44;
                        idxHeadTemp3 = 45; idxCyl13 = 46; idxCyl23 = 47; idxCyl33 = 48; idxScrewTemp3 = 49;
                        idxScrewSpeed1 = 50; idxScrewSpeed2 = 51; idxScrewSpeed3 = 52;
                        idxFeedRoll1 = 53; idxFeedRoll2 = 54; idxFeedRoll3 = 55;
                        idxPressure1 = 56; idxPressure2 = 57; idxPressure3 = 58;
                        idxAmMeter = 59; idxAmMeter2 = 60; idxAmMeter3 = 61;
                        idxPresetValue = 62; idxControlValue = 63;
                        idxSpiralPitchSetting = 64; idxSpiralPitchDisplay = 65;
                        idxSpiralSpeed = 66; idxHoseSpeed = 67; idxUnsmoothSurface = 68;
                        idxMarkingSort = 69; idxTextMarkingMaterial = 70; idxMarkingColour = 71;
                        idxChillerWaterTemp = 72; idxCaterpillarGap = 73;
                        idxTakeUpConveyorSpeed = 74; idxCoolConveyorSpeed = 75; idxCoolConveyorSpeed2 = 76;
                        idxConveyorRatio = 77;
                        idxToleranceInner = 78; idxToleranceOuter = 79;
                        idxTebalInner = 80; idxTebalInnerMiddle = 81; idxTebalTotal = 82; idxSelisihTebal = 83;
                        idxInnerTarget = 84; idxInnerTol = 85; idxInnerLCL = 86; idxInnerMin = 87; idxInnerUCL = 88; idxInnerMax = 89;
                        idxThickTarget = 90; idxThickTol = 91; idxThickLCL = 92; idxThickMin = 93; idxThickUCL = 94; idxThickMax = 95;
                        idxInnerMidTarget = 96; idxInnerMidTol = 97; idxInnerMidLCL = 98; idxInnerMidMin = 99; idxInnerMidUCL = 100; idxInnerMidMax = 101;
                        idxTotalTarget = 102; idxTotalTol = 103; idxTotalLCL = 104; idxTotalMin = 105; idxTotalUCL = 106; idxTotalMax = 107;
                    }
                    else if (isDig2L)
                    {
                        idxDimensi = 12;
                        idxItem = 90;
                        idxRev = 7; idxRevDate = 9; idxCustomer = 8; idxFormulasi = 10;
                        idxInner = 14; idxOuter = 15;
                        idxUseLimitsInner = 16; idxUseLimitsOuter = 17;
                        idxYarn = 18; idxPitchYarn = 19;
                        idxTensionIn = 20; idxTensionOut = 21;
                        idxNipple = 22; idxTubeDie = 23; idxMiddleDie = 24; idxCoverDie = 25;
                        idxSpacerDie = 26; idxADistance = 27;
                        idxMeshDim1 = 28; idxMeshScreen1 = 29;
                        idxMeshDim2 = 30; idxMeshScreen2 = 31;
                        idxHeadTemp1 = 32; idxCyl11 = 33; idxCyl21 = 34; idxCyl31 = 35; idxScrewTemp1 = 36;
                        idxHeadTemp2 = 37; idxCyl12 = 38; idxCyl22 = 39; idxCyl32 = 40; idxScrewTemp2 = 41;
                        idxScrewSpeed1 = 42; idxScrewSpeed2 = 43;
                        idxFeedRoll1 = 44; idxFeedRoll2 = 45;
                        idxPressure1 = 46; idxPressure2 = 47;
                        idxCurrentValue = 48; idxAmMeter = 49; idxAmMeter2 = 50;
                        idxPresetValue = 51; idxControlValue = 52;
                        idxSpiralPitchSetting = 53; idxSpiralPitchDisplay = 54;
                        idxSpiralSpeed = 55; idxHoseSpeed = 56; idxUnsmoothSurface = 57;
                        idxMarkingSort = 58; idxTextMarkingMaterial = 59; idxMarkingColour = 60;
                        idxChillerWaterTemp = 61; idxDancerPosition = 62; idxCaterpillarGap = 63;
                        idxTakeUpConveyorSpeed = 64; idxCoolConveyorSpeed = 65; idxConveyorRatio = 66;
                        idxToleranceInner = 67; idxToleranceOuter = 68;
                        idxTebalInner = 69; idxTebalTotal = 70; idxSelisihTebal = 71;
                        idxInnerTarget = 72; idxInnerTol = 73; idxInnerLCL = 74; idxInnerMin = 75; idxInnerUCL = 76; idxInnerMax = 77;
                        idxThickTarget = 78; idxThickTol = 79; idxThickLCL = 80; idxThickMin = 81; idxThickUCL = 82; idxThickMax = 83;
                        idxTotalTarget = 84; idxTotalTol = 85; idxTotalLCL = 86; idxTotalMin = 87; idxTotalUCL = 88; idxTotalMax = 89;
                    }
                    else if (isTemplateFull)
                    {
                        idxDimensi = 9;
                        idxItem = 302;
                        idxRev = 5; idxRevDate = 6; idxCustomer = 7; idxFormulasi = 10;
                        idxInner = 13; idxMiddle = 15; idxOuter = 14;
                        idxUseLimitsInner = 16; idxUseLimitsOuter = 17; idxUseLimitsMiddle = 18;
                        idxNipple = 19; idxTubeDie = 23; idxCoverDie = 27; idxMiddleDie = 31; idxSpacerDie = 35; idxADistance = 39;
                        idxYarn = 43; idxTensionIn = 44; idxTensionOut = 45;
                        idxMeshScreen1 = 46; idxMeshDim1 = 47;
                        idxMeshScreen2 = 51; idxMeshDim2 = 52;
                        idxMeshScreen3 = 56; idxMeshDim3 = 57;
                        idxPitchYarn = 61;
                        idxHeadTemp1 = 65; idxCyl11 = 69; idxCyl21 = 73; idxCyl31 = 77; idxScrewTemp1 = 81; idxScrewSpeed1 = 85; idxPressure1 = 89;
                        idxHeadTemp2 = 93; idxCyl12 = 97; idxCyl22 = 101; idxCyl32 = 105; idxScrewTemp2 = 109; idxScrewSpeed2 = 113; idxPressure2 = 117;
                        idxHeadTemp3 = 121; idxCyl13 = 125; idxCyl23 = 129; idxCyl33 = 133; idxScrewTemp3 = 137; idxScrewSpeed3 = 141; idxPressure3 = 145;
                        idxCurrentValue = 149;
                        idxFeed1 = 150; idxFeed2 = 154; idxFeed3 = 158;
                        idxFeedRoll1 = 162; idxFeedRoll2 = 166; idxFeedRoll3 = 170;
                        idxAmMeter = 174; idxAmMeter2 = 178; idxAmMeter3 = 182;
                        idxPresetValue = 186; idxControlValue = 190;
                        idxSpiralPitchSetting = 194; idxSpiralPitchDisplay = 198;
                        idxSpiralSpeed = 202; idxHoseSpeed = 206;
                        idxChillerWaterTemp = 210; idxDancerPosition = 214; idxCaterpillarGap = 218;
                        idxTakeUpConveyorSpeed = 226; idxCoolConveyorSpeed = 230; idxCoolConveyorSpeed2 = 234; idxConveyorRatio = 238;
                        idxODSensor = 242; idxCuttingSpeed = 222;
                        idxToleranceInner = 250; idxToleranceOuter = 254;
                        idxTebalInner = 258; idxTebalOuter = 262; idxTebalInnerMiddle = 266; idxTebalTotal = 270; idxSelisihTebal = 274;
                        idxInnerTarget = 278; idxInnerTol = 279; idxInnerLCL = 280; idxInnerMin = 281; idxInnerUCL = 282; idxInnerMax = 283;
                        idxInnerMidTarget = 284; idxInnerMidTol = 285; idxInnerMidLCL = 286; idxInnerMidMin = 287; idxInnerMidUCL = 288; idxInnerMidMax = 289;
                        idxThickTarget = 290; idxThickTol = 291; idxThickLCL = 292; idxThickMin = 293; idxThickUCL = 294; idxThickMax = 295;
                        idxTotalTarget = 296; idxTotalTol = 297; idxTotalLCL = 298; idxTotalMin = 299; idxTotalUCL = 300; idxTotalMax = 301;
                    }
                    else if (isLegacy || (headerRow == 3 && worksheet.Cells[3, 43].Text?.Trim().Contains("± Inner") == true))
                    {
                        idxDimensi = 10;
                        idxItem = 51;
                        idxRev = 5; idxRevDate = 7; idxCustomer = 6; idxFormulasi = 8;
                        idxInner = 12; idxOuter = 13;
                        idxUseLimitsInner = 14; idxUseLimitsOuter = 15;
                        idxNipple = 16; idxTubeDie = 17; idxCoverDie = 18;
                        idxMeshScreen1 = 19; idxMeshScreen2 = 20;
                        idxHeadTemp1 = 21; idxHeadTemp2 = 22;
                        idxCyl11 = 23; idxCyl21 = 25;
                        idxCyl12 = 24; idxCyl22 = 26;
                        idxScrewTemp1 = 29; idxScrewTemp2 = 30;
                        idxScrewSpeed1 = 31; idxScrewSpeed2 = 32;
                        idxFeedRoll1 = 27; idxFeedRoll2 = 28;
                        idxPressure1 = 33; idxPressure2 = 34;
                        idxAmMeter = 35;
                        idxMarkingSort = 37; idxTextMarkingMaterial = 38; idxMarkingColour = 39;
                        idxChillerWaterTemp = 40;
                        idxTakeUpConveyorSpeed = 42;
                        idxToleranceInner = 43;
                        idxToleranceOuter = 44;
                        idxTebalInner = 45;
                        idxTebalTotal = 47;
                        idxSelisihTebal = 48;
                    }
                    else
                    {
                        idxDimensi = 10;
                        idxItem = 51;
                        idxRev = 5; idxRevDate = 7; idxCustomer = 6; idxFormulasi = 8;
                        idxInner = 12; idxOuter = 13;
                        idxUseLimitsInner = 14; idxUseLimitsOuter = 15;
                        idxNipple = 16; idxTubeDie = 17; idxCoverDie = 18;
                        idxMeshScreen1 = 19; idxMeshScreen2 = 20;
                        idxHeadTemp1 = 21; idxHeadTemp2 = 22;
                        idxADistance = 28;
                    }

                    // Build Headers dictionary
                    var headers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    for (int col = 1; col <= colCount; col++)
                    {
                        var val = worksheet.Cells[headerRow, col].Text?.Trim() ?? "";
                        if (!string.IsNullOrWhiteSpace(val) && !val.All(char.IsDigit))
                        {
                            var normalizedKey = NormalizeKey(val);
                            headers[normalizedKey] = col;
                            headers[val.ToUpper()] = col;
                        }
                    }

                    if (headerRow2 > 0)
                    {
                        for (int col = 1; col <= colCount; col++)
                        {
                            var val = worksheet.Cells[headerRow2, col].Text?.Trim() ?? "";
                            if (!string.IsNullOrWhiteSpace(val) && !val.All(char.IsDigit))
                            {
                                var normalizedKey = NormalizeKey(val);
                                if (!headers.ContainsKey(normalizedKey))
                                {
                                    headers[normalizedKey] = col;
                                    headers[val.ToUpper()] = col;
                                }
                            }
                        }
                    }

                    // Special Check: last 20 columns for "Item"
                    for (int col = Math.Max(1, colCount - 19); col <= colCount; col++)
                    {
                        var val3 = worksheet.Cells[headerRow, col].Text?.Trim().ToUpper() ?? "";
                        var val6 = headerRow2 > 0 ? worksheet.Cells[headerRow2, col].Text?.Trim().ToUpper() ?? "" : "";
                        if (val3.Contains("ITEM") || val6.Contains("ITEM"))
                        {
                            headers["ITEM"] = col;
                            headers["ITEMLIST"] = col;
                        }
                    }

                    // Helper column lookup
                    int FindColumn(params string[] possibleNames)
                    {
                        var matchedColumns = new List<int>();
                        foreach (var name in possibleNames)
                        {
                            var normalized = NormalizeKey(name);
                            if (headers.ContainsKey(normalized))
                                matchedColumns.Add(headers[normalized]);
                            if (headers.ContainsKey(name.ToUpper()))
                                matchedColumns.Add(headers[name.ToUpper()]);
                        }
                        if (matchedColumns.Count == 0)
                        {
                            foreach (var name in possibleNames)
                            {
                                var normalized = NormalizeKey(name);
                                foreach (var kvp in headers)
                                {
                                    if ((kvp.Key.Length >= 3 && normalized.Length >= 3) &&
                                        (kvp.Key.Contains(normalized) || normalized.Contains(kvp.Key)))
                                    {
                                        matchedColumns.Add(kvp.Value);
                                    }
                                }
                            }
                        }
                        return matchedColumns.Count > 0 ? matchedColumns.Min() : 0;
                    }

                    string GetCellValue(int r, int c)
                    {
                        if (c <= 0) return "";
                        var cellValue = worksheet.Cells[r, c].Value;
                        if (cellValue == null) return "";
                        var val = cellValue.ToString()?.Trim() ?? "";
                        return (string.IsNullOrWhiteSpace(val) || val == "-") ? "" : val;
                    }

                    int idExcelCol = FindColumn("ID", "ID EXCEL", "EXCEL ID", "EXCELID", "NO");
                    int docNumberCol = FindColumn("NO. DOC", "NO.DOC", "NODOC", "DOCUMENT NUMBER", "NO. DOCUMENT", "DOCUMENTNUMBER", "NO DOCUMENT", "DOC NUMBER", "DOC NO", "DOCNO", "SOP NUMBER", "PROD NUMBER", "VI-SOP-PROD", "VISOPPROD", "DOC", "DOCUMENT");
                    int machineCol = FindColumn("MACHINE", "MESIN");
                    int machineCodeCol = FindColumn("MC", "MACHINE CODE", "MACHINECODE", "KODE MESIN", "M/C");
                    int itemListCol = FindColumn("ITEM LIST", "ITEMLIST", "ITEM");
                    if (itemListCol <= 0) itemListCol = idxItem;

                    int noCol = FindColumn("NO", "NUMBER", "NO.");
                    int hoseTypeCol = FindColumn("HOSE TYPE", "HOSETYPE", "TYPE");
                    int dimensiCol = FindColumn("DIMENSI", "DIMENSION", "DIM");
                    int materialCol = FindColumn("MATERIAL TSM", "MATERIAL", "MATERIALTSM", "TSM");

                    if (docNumberCol == 0 && idExcelCol > 0) docNumberCol = idExcelCol;

                    if (docNumberCol == 0)
                    {
                        Console.Error.WriteLine("Error: Could not identify Document Number column. Skipping file.");
                        totalErrors++;
                        continue;
                    }

                    // Group by doc number
                    var docGroups = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);
                    int consecutiveEmpty = 0;
                    for (int row = dataStartRow; row <= rowCount; row++)
                    {
                        var docNum = GetCellValue(row, docNumberCol);
                        if (string.IsNullOrWhiteSpace(docNum) ||
                            docNum.ToUpper().Contains("DOC") ||
                            docNum.ToUpper().Contains("DOCUMENT") ||
                            docNum.StartsWith("#REF") ||
                            docNum.StartsWith("#N/A") ||
                            docNum.StartsWith("#VALUE") ||
                            docNum.StartsWith("#ERROR") ||
                            docNum.StartsWith("#") ||
                            docNum.StartsWith("ROW-") ||
                            docNum == "-" ||
                            docNum.Length < 3)
                        {
                            consecutiveEmpty++;
                            if (consecutiveEmpty >= 100) break;
                            continue;
                        }
                        consecutiveEmpty = 0;

                        if (!docGroups.ContainsKey(docNum))
                            docGroups[docNum] = new List<int>();
                        docGroups[docNum].Add(row);
                    }

                    Console.WriteLine($"Found {docGroups.Count} unique document numbers to process.");

                    int fileDocsCreated = 0;
                    int fileDocsUpdated = 0;
                    int fileItemsAdded = 0;

                    // Helper to parse ± values
                    (decimal? min, decimal? asli, decimal? max) ParsePlusMinusValue(string input)
                    {
                        if (string.IsNullOrWhiteSpace(input)) return (null, null, null);
                        try
                        {
                            var cleaned = input.Trim().Replace(" ", "").Replace(",", ".");
                            if (cleaned.StartsWith("max", StringComparison.OrdinalIgnoreCase) ||
                                cleaned.StartsWith("maks", StringComparison.OrdinalIgnoreCase))
                            {
                                var numStart = 0;
                                for (int i = 0; i < cleaned.Length; i++)
                                {
                                    if (char.IsDigit(cleaned[i])) { numStart = i; break; }
                                }
                                if (numStart > 0)
                                {
                                    var numPart = cleaned.Substring(numStart);
                                    if (decimal.TryParse(numPart, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal maxValue))
                                        return (null, null, maxValue);
                                }
                            }
                            if (cleaned.StartsWith("min", StringComparison.OrdinalIgnoreCase))
                            {
                                var numStart = 0;
                                for (int i = 0; i < cleaned.Length; i++)
                                {
                                    if (char.IsDigit(cleaned[i])) { numStart = i; break; }
                                }
                                if (numStart > 0)
                                {
                                    var numPart = cleaned.Substring(numStart);
                                    if (decimal.TryParse(numPart, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal minValue))
                                        return (minValue, null, null);
                                }
                            }

                            var plusMinusSymbols = new[] { "\u00B1", "±", "Â±", "+/-", "+-", "~" };
                            foreach (var symbol in plusMinusSymbols)
                            {
                                if (cleaned.Contains(symbol))
                                {
                                    var parts = cleaned.Split(new[] { symbol }, StringSplitOptions.None);
                                    if (parts.Length == 2)
                                    {
                                        if (decimal.TryParse(parts[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal standard) &&
                                            decimal.TryParse(parts[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal tolerance))
                                        {
                                            return (standard - tolerance, standard, standard + tolerance);
                                        }
                                    }
                                }
                            }

                            if (decimal.TryParse(cleaned, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal singleValue))
                            {
                                return (null, singleValue, null);
                            }
                        }
                        catch { }
                        return (null, null, null);
                    }

                    void AssignFullOrManual(int firstRow, int rawCol, Action<decimal?> setMin, Action<decimal?> setAsli, Action<decimal?> setMax)
                    {
                        if (rawCol <= 0) { setMin(null); setAsli(null); setMax(null); return; }
                        var rawVal = GetCellValue(firstRow, rawCol);
                        var parsed = ParsePlusMinusValue(rawVal);
                        if (parsed.min != null || parsed.asli != null || parsed.max != null)
                        {
                            setMin(parsed.min); setAsli(parsed.asli); setMax(parsed.max);
                        }
                        else if (isTemplateFull)
                        {
                            var ci = System.Globalization.CultureInfo.InvariantCulture;
                            var ns = System.Globalization.NumberStyles.Any;
                            var minStr = GetCellValue(firstRow, rawCol + 1);
                            var stdStr = GetCellValue(firstRow, rawCol + 2);
                            var maxStr = GetCellValue(firstRow, rawCol + 3);
                            decimal? minV = decimal.TryParse(minStr, ns, ci, out var mv) ? mv : (decimal?)null;
                            decimal? stdV = decimal.TryParse(stdStr, ns, ci, out var sv) ? sv : (decimal?)null;
                            decimal? maxV = decimal.TryParse(maxStr, ns, ci, out var xv) ? xv : (decimal?)null;
                            setMin(minV); setAsli(stdV); setMax(maxV);
                        }
                        else
                        {
                            setMin(null); setAsli(null); setMax(null);
                        }
                    }

                    // Process each document group
                    foreach (var group in docGroups)
                    {
                        string docNumber = group.Key;
                        int firstRow = group.Value.First();

                        // Query database for existing document
                        var existingDoc = await context.SpsNoDocs
                            .Include(s => s.ItemLists)
                            .FirstOrDefaultAsync(s => s.DocumentNumber == docNumber);

                        SpsNoDoc spsNoDoc;
                        bool isNewDoc = false;

                        if (existingDoc != null)
                        {
                            spsNoDoc = existingDoc;
                            fileDocsUpdated++;
                        }
                        else
                        {
                            spsNoDoc = new SpsNoDoc();
                            isNewDoc = true;
                            fileDocsCreated++;
                        }

                        // Map fields
                        spsNoDoc.DocumentNumber = docNumber;
                        spsNoDoc.No = GetCellValue(firstRow, noCol);
                        spsNoDoc.Machine = GetCellValue(firstRow, machineCol);
                        var machineCodeVal = machineCodeCol > 0 ? GetCellValue(firstRow, machineCodeCol) : "";
                        spsNoDoc.MachineCode = !string.IsNullOrEmpty(machineCodeVal) ? machineCodeVal : GetCellValue(firstRow, machineCol);
                        spsNoDoc.RevisionNumber = GetCellValue(firstRow, idxRev);
                        spsNoDoc.RevisionDate = GetCellValue(firstRow, idxRevDate);
                        spsNoDoc.Customer = GetCellValue(firstRow, idxCustomer);
                        spsNoDoc.Formulasi = GetCellValue(firstRow, idxFormulasi);
                        spsNoDoc.HoseType = GetCellValue(firstRow, hoseTypeCol);
                        spsNoDoc.Dimensi = idxDimensi > 0 ? GetCellValue(firstRow, idxDimensi) : GetCellValue(firstRow, dimensiCol);
                        spsNoDoc.Material = GetCellValue(firstRow, materialCol);
                        spsNoDoc.InnerTube = GetCellValue(firstRow, idxInner);
                        spsNoDoc.OuterCover = GetCellValue(firstRow, idxOuter);
                        spsNoDoc.MiddleTube = idxMiddle > 0 ? GetCellValue(firstRow, idxMiddle) : "";
                        spsNoDoc.UseLimitsInner = GetCellValue(firstRow, idxUseLimitsInner);
                        spsNoDoc.UseLimitsOuter = GetCellValue(firstRow, idxUseLimitsOuter);
                        spsNoDoc.UseLimitsMiddle = idxUseLimitsMiddle > 0 ? GetCellValue(firstRow, idxUseLimitsMiddle) : "";
                        spsNoDoc.Yarn = idxYarn > 0 ? GetCellValue(firstRow, idxYarn) : "";
                        spsNoDoc.PitchYarn = idxPitchYarn > 0 ? GetCellValue(firstRow, idxPitchYarn) : "";
                        spsNoDoc.TensionYarnInner = idxTensionIn > 0 ? GetCellValue(firstRow, idxTensionIn) : "";
                        spsNoDoc.TensionYarnOuter = idxTensionOut > 0 ? GetCellValue(firstRow, idxTensionOut) : "";
                        spsNoDoc.Nipple = GetCellValue(firstRow, idxNipple);
                        spsNoDoc.TubeDie = GetCellValue(firstRow, idxTubeDie);
                        spsNoDoc.CoverDie = GetCellValue(firstRow, idxCoverDie);
                        spsNoDoc.MiddleDie = idxMiddleDie > 0 ? GetCellValue(firstRow, idxMiddleDie) : "";
                        spsNoDoc.SpacerDie = idxSpacerDie > 0 ? GetCellValue(firstRow, idxSpacerDie) : "";
                        spsNoDoc.ADistance = idxADistance > 0 ? GetCellValue(firstRow, idxADistance) : "";
                        spsNoDoc.MeshDim1 = idxMeshDim1 > 0 ? GetCellValue(firstRow, idxMeshDim1) : "";
                        spsNoDoc.MeshScreen1 = idxMeshScreen1 > 0 ? GetCellValue(firstRow, idxMeshScreen1) : "";
                        spsNoDoc.MeshDim2 = idxMeshDim2 > 0 ? GetCellValue(firstRow, idxMeshDim2) : "";
                        spsNoDoc.MeshScreen2 = idxMeshScreen2 > 0 ? GetCellValue(firstRow, idxMeshScreen2) : "";
                        spsNoDoc.MeshDim3 = idxMeshDim3 > 0 ? GetCellValue(firstRow, idxMeshDim3) : "";
                        spsNoDoc.MeshScreen3 = idxMeshScreen3 > 0 ? GetCellValue(firstRow, idxMeshScreen3) : "";
                        spsNoDoc.ToleranceInner = idxToleranceInner > 0 ? GetCellValue(firstRow, idxToleranceInner) : "";
                        spsNoDoc.ToleranceOuter = idxToleranceOuter > 0 ? GetCellValue(firstRow, idxToleranceOuter) : "";

                        // Parse ± values
                        AssignFullOrManual(firstRow, idxNipple, v => spsNoDoc.Nipple_Min = v, v => spsNoDoc.Nipple_Asli = v, v => spsNoDoc.Nipple_Max = v);
                        AssignFullOrManual(firstRow, idxTubeDie, v => spsNoDoc.TubeDie_Min = v, v => spsNoDoc.TubeDie_Asli = v, v => spsNoDoc.TubeDie_Max = v);
                        AssignFullOrManual(firstRow, idxCoverDie, v => spsNoDoc.CoverDie_Min = v, v => spsNoDoc.CoverDie_Asli = v, v => spsNoDoc.CoverDie_Max = v);
                        if (idxMiddleDie > 0) AssignFullOrManual(firstRow, idxMiddleDie, v => spsNoDoc.MiddleDie_Min = v, v => spsNoDoc.MiddleDie_Asli = v, v => spsNoDoc.MiddleDie_Max = v);
                        if (idxSpacerDie > 0) AssignFullOrManual(firstRow, idxSpacerDie, v => spsNoDoc.SpacerDie_Min = v, v => spsNoDoc.SpacerDie_Asli = v, v => spsNoDoc.SpacerDie_Max = v);
                        if (idxADistance > 0) AssignFullOrManual(firstRow, idxADistance, v => spsNoDoc.ADistance_Min = v, v => spsNoDoc.ADistance_Asli = v, v => spsNoDoc.ADistance_Max = v);

                        if (idxMeshDim1 > 0) AssignFullOrManual(firstRow, idxMeshDim1, v => spsNoDoc.MeshDim1_Min = v, v => spsNoDoc.MeshDim1_Asli = v, v => spsNoDoc.MeshDim1_Max = v);
                        if (idxMeshDim2 > 0) AssignFullOrManual(firstRow, idxMeshDim2, v => spsNoDoc.MeshDim2_Min = v, v => spsNoDoc.MeshDim2_Asli = v, v => spsNoDoc.MeshDim2_Max = v);
                        if (idxMeshDim3 > 0) AssignFullOrManual(firstRow, idxMeshDim3, v => spsNoDoc.MeshDim3_Min = v, v => spsNoDoc.MeshDim3_Asli = v, v => spsNoDoc.MeshDim3_Max = v);

                        if (idxPitchYarn > 0) AssignFullOrManual(firstRow, idxPitchYarn, v => spsNoDoc.PitchYarn_Min = v, v => spsNoDoc.PitchYarn_Asli = v, v => spsNoDoc.PitchYarn_Max = v);

                        if (idxHeadTemp1 > 0) AssignFullOrManual(firstRow, idxHeadTemp1, v => spsNoDoc.HeadTemp1_Min = v, v => spsNoDoc.HeadTemp1_Asli = v, v => spsNoDoc.HeadTemp1_Max = v);
                        if (idxHeadTemp2 > 0) AssignFullOrManual(firstRow, idxHeadTemp2, v => spsNoDoc.HeadTemp2_Min = v, v => spsNoDoc.HeadTemp2_Asli = v, v => spsNoDoc.HeadTemp2_Max = v);
                        if (idxHeadTemp3 > 0) AssignFullOrManual(firstRow, idxHeadTemp3, v => spsNoDoc.HeadTemp3_Min = v, v => spsNoDoc.HeadTemp3_Asli = v, v => spsNoDoc.HeadTemp3_Max = v);

                        if (idxCyl11 > 0) AssignFullOrManual(firstRow, idxCyl11, v => spsNoDoc.Cylinder1_1_Min = v, v => spsNoDoc.Cylinder1_1_Asli = v, v => spsNoDoc.Cylinder1_1_Max = v);
                        if (idxCyl12 > 0) AssignFullOrManual(firstRow, idxCyl12, v => spsNoDoc.Cylinder1_2_Min = v, v => spsNoDoc.Cylinder1_2_Asli = v, v => spsNoDoc.Cylinder1_2_Max = v);
                        if (idxCyl13 > 0) AssignFullOrManual(firstRow, idxCyl13, v => spsNoDoc.Cylinder1_3_Min = v, v => spsNoDoc.Cylinder1_3_Asli = v, v => spsNoDoc.Cylinder1_3_Max = v);

                        if (idxCyl21 > 0) AssignFullOrManual(firstRow, idxCyl21, v => spsNoDoc.Cylinder2_1_Min = v, v => spsNoDoc.Cylinder2_1_Asli = v, v => spsNoDoc.Cylinder2_1_Max = v);
                        if (idxCyl22 > 0) AssignFullOrManual(firstRow, idxCyl22, v => spsNoDoc.Cylinder2_2_Min = v, v => spsNoDoc.Cylinder2_2_Asli = v, v => spsNoDoc.Cylinder2_2_Max = v);
                        if (idxCyl23 > 0) AssignFullOrManual(firstRow, idxCyl23, v => spsNoDoc.Cylinder2_3_Min = v, v => spsNoDoc.Cylinder2_3_Asli = v, v => spsNoDoc.Cylinder2_3_Max = v);

                        if (idxCyl31 > 0) AssignFullOrManual(firstRow, idxCyl31, v => spsNoDoc.Cylinder3_1_Min = v, v => spsNoDoc.Cylinder3_1_Asli = v, v => spsNoDoc.Cylinder3_1_Max = v);
                        if (idxCyl32 > 0) AssignFullOrManual(firstRow, idxCyl32, v => spsNoDoc.Cylinder3_2_Min = v, v => spsNoDoc.Cylinder3_2_Asli = v, v => spsNoDoc.Cylinder3_2_Max = v);
                        if (idxCyl33 > 0) AssignFullOrManual(firstRow, idxCyl33, v => spsNoDoc.Cylinder3_3_Min = v, v => spsNoDoc.Cylinder3_3_Asli = v, v => spsNoDoc.Cylinder3_3_Max = v);

                        if (idxScrewTemp1 > 0) AssignFullOrManual(firstRow, idxScrewTemp1, v => spsNoDoc.ScrewTemp1_Min = v, v => spsNoDoc.ScrewTemp1_Asli = v, v => spsNoDoc.ScrewTemp1_Max = v);
                        if (idxScrewTemp2 > 0) AssignFullOrManual(firstRow, idxScrewTemp2, v => spsNoDoc.ScrewTemp2_Min = v, v => spsNoDoc.ScrewTemp2_Asli = v, v => spsNoDoc.ScrewTemp2_Max = v);
                        if (idxScrewTemp3 > 0) AssignFullOrManual(firstRow, idxScrewTemp3, v => spsNoDoc.ScrewTemp3_Min = v, v => spsNoDoc.ScrewTemp3_Asli = v, v => spsNoDoc.ScrewTemp3_Max = v);

                        if (idxScrewSpeed1 > 0) AssignFullOrManual(firstRow, idxScrewSpeed1, v => spsNoDoc.ScrewSpeed1_Min = v, v => spsNoDoc.ScrewSpeed1_Asli = v, v => spsNoDoc.ScrewSpeed1_Max = v);
                        if (idxScrewSpeed2 > 0) AssignFullOrManual(firstRow, idxScrewSpeed2, v => spsNoDoc.ScrewSpeed2_Min = v, v => spsNoDoc.ScrewSpeed2_Asli = v, v => spsNoDoc.ScrewSpeed2_Max = v);
                        if (idxScrewSpeed3 > 0) AssignFullOrManual(firstRow, idxScrewSpeed3, v => spsNoDoc.ScrewSpeed3_Min = v, v => spsNoDoc.ScrewSpeed3_Asli = v, v => spsNoDoc.ScrewSpeed3_Max = v);

                        if (idxPressure1 > 0)
                        {
                            spsNoDoc.Pressure1 = GetCellValue(firstRow, idxPressure1);
                            AssignFullOrManual(firstRow, idxPressure1, v => spsNoDoc.Pressure1_Min = v, v => spsNoDoc.Pressure1_Asli = v, v => spsNoDoc.Pressure1_Max = v);
                        }
                        if (idxPressure2 > 0)
                        {
                            spsNoDoc.Pressure2 = GetCellValue(firstRow, idxPressure2);
                            AssignFullOrManual(firstRow, idxPressure2, v => spsNoDoc.Pressure2_Min = v, v => spsNoDoc.Pressure2_Asli = v, v => spsNoDoc.Pressure2_Max = v);
                        }
                        if (idxPressure3 > 0)
                        {
                            spsNoDoc.Pressure3 = GetCellValue(firstRow, idxPressure3);
                            AssignFullOrManual(firstRow, idxPressure3, v => spsNoDoc.Pressure3_Min = v, v => spsNoDoc.Pressure3_Asli = v, v => spsNoDoc.Pressure3_Max = v);
                        }

                        int feed1Col = idxFeed1 > 0 ? idxFeed1 : idxFeedRoll1;
                        if (feed1Col > 0)
                        {
                            spsNoDoc.Feed1 = GetCellValue(firstRow, feed1Col);
                            AssignFullOrManual(firstRow, feed1Col, v => spsNoDoc.Feed1_Min = v, v => spsNoDoc.Feed1_Asli = v, v => spsNoDoc.Feed1_Max = v);
                        }
                        int feed2Col = idxFeed2 > 0 ? idxFeed2 : idxFeedRoll2;
                        if (feed2Col > 0)
                        {
                            spsNoDoc.Feed2 = GetCellValue(firstRow, feed2Col);
                            AssignFullOrManual(firstRow, feed2Col, v => spsNoDoc.Feed2_Min = v, v => spsNoDoc.Feed2_Asli = v, v => spsNoDoc.Feed2_Max = v);
                        }
                        int feed3Col = idxFeed3 > 0 ? idxFeed3 : idxFeedRoll3;
                        if (feed3Col > 0)
                        {
                            spsNoDoc.Feed3 = GetCellValue(firstRow, feed3Col);
                            AssignFullOrManual(firstRow, feed3Col, v => spsNoDoc.Feed3_Min = v, v => spsNoDoc.Feed3_Asli = v, v => spsNoDoc.Feed3_Max = v);
                        }

                        if (idxAmMeter > 0) AssignFullOrManual(firstRow, idxAmMeter, v => spsNoDoc.AmMeter_Min = v, v => spsNoDoc.AmMeter_Asli = v, v => spsNoDoc.AmMeter_Max = v);
                        if (idxAmMeter2 > 0) AssignFullOrManual(firstRow, idxAmMeter2, v => spsNoDoc.AmMeter2_Min = v, v => spsNoDoc.AmMeter2_Asli = v, v => spsNoDoc.AmMeter2_Max = v);
                        if (idxAmMeter3 > 0) AssignFullOrManual(firstRow, idxAmMeter3, v => spsNoDoc.AmMeter3_Min = v, v => spsNoDoc.AmMeter3_Asli = v, v => spsNoDoc.AmMeter3_Max = v);

                        if (idxCurrentValue > 0) spsNoDoc.CurrentValue = GetCellValue(firstRow, idxCurrentValue);
                        if (idxPresetValue > 0) AssignFullOrManual(firstRow, idxPresetValue, v => spsNoDoc.PresetValue_Min = v, v => spsNoDoc.PresetValue_Asli = v, v => spsNoDoc.PresetValue_Max = v);
                        if (idxControlValue > 0) AssignFullOrManual(firstRow, idxControlValue, v => spsNoDoc.ControlValue_Min = v, v => spsNoDoc.ControlValue_Asli = v, v => spsNoDoc.ControlValue_Max = v);

                        if (idxSpiralPitchSetting > 0) AssignFullOrManual(firstRow, idxSpiralPitchSetting, v => spsNoDoc.SpiralPitchSetting_Min = v, v => spsNoDoc.SpiralPitchSetting_Asli = v, v => spsNoDoc.SpiralPitchSetting_Max = v);
                        if (idxSpiralPitchDisplay > 0) AssignFullOrManual(firstRow, idxSpiralPitchDisplay, v => spsNoDoc.SpiralPitchDisplay_Min = v, v => spsNoDoc.SpiralPitchDisplay_Asli = v, v => spsNoDoc.SpiralPitchDisplay_Max = v);
                        if (idxSpiralSpeed > 0) AssignFullOrManual(firstRow, idxSpiralSpeed, v => spsNoDoc.SpiralSpeed_Min = v, v => spsNoDoc.SpiralSpeed_Asli = v, v => spsNoDoc.SpiralSpeed_Max = v);
                        if (idxHoseSpeed > 0) AssignFullOrManual(firstRow, idxHoseSpeed, v => spsNoDoc.HoseSpeed_Min = v, v => spsNoDoc.HoseSpeed_Asli = v, v => spsNoDoc.HoseSpeed_Max = v);
                        if (idxUnsmoothSurface > 0) spsNoDoc.UnsmoothSurface = GetCellValue(firstRow, idxUnsmoothSurface);

                        if (idxMarkingSort > 0) spsNoDoc.MarkingSort = GetCellValue(firstRow, idxMarkingSort);
                        if (idxTextMarkingMaterial > 0) spsNoDoc.TextMarkingMaterial = GetCellValue(firstRow, idxTextMarkingMaterial);
                        if (idxMarkingColour > 0) spsNoDoc.MarkingColour = GetCellValue(firstRow, idxMarkingColour);

                        if (idxChillerWaterTemp > 0)
                        {
                            spsNoDoc.ChillerWaterTemp = GetCellValue(firstRow, idxChillerWaterTemp);
                            AssignFullOrManual(firstRow, idxChillerWaterTemp, v => spsNoDoc.ChillerWaterTemp_Min = v, v => spsNoDoc.ChillerWaterTemp_Asli = v, v => spsNoDoc.ChillerWaterTemp_Max = v);
                        }
                        if (idxDancerPosition > 0) AssignFullOrManual(firstRow, idxDancerPosition, v => spsNoDoc.DancerPosition_Min = v, v => spsNoDoc.DancerPosition_Asli = v, v => spsNoDoc.DancerPosition_Max = v);
                        if (idxCaterpillarGap > 0)
                        {
                            spsNoDoc.CaterpillarGap = GetCellValue(firstRow, idxCaterpillarGap);
                            AssignFullOrManual(firstRow, idxCaterpillarGap, v => spsNoDoc.CaterpillarGap_Min = v, v => spsNoDoc.CaterpillarGap_Asli = v, v => spsNoDoc.CaterpillarGap_Max = v);
                        }
                        if (idxTakeUpConveyorSpeed > 0)
                        {
                            spsNoDoc.TakeUpConveyorSpeed = GetCellValue(firstRow, idxTakeUpConveyorSpeed);
                            AssignFullOrManual(firstRow, idxTakeUpConveyorSpeed, v => spsNoDoc.TakeUpConveyorSpeed_Min = v, v => spsNoDoc.TakeUpConveyorSpeed_Asli = v, v => spsNoDoc.TakeUpConveyorSpeed_Max = v);
                        }
                        if (idxCoolConveyorSpeed > 0) AssignFullOrManual(firstRow, idxCoolConveyorSpeed, v => spsNoDoc.CoolConveyorSpeed_Min = v, v => spsNoDoc.CoolConveyorSpeed_Asli = v, v => spsNoDoc.CoolConveyorSpeed_Max = v);
                        if (idxCoolConveyorSpeed2 > 0) AssignFullOrManual(firstRow, idxCoolConveyorSpeed2, v => spsNoDoc.CoolConveyorSpeed2_Min = v, v => spsNoDoc.CoolConveyorSpeed2_Asli = v, v => spsNoDoc.CoolConveyorSpeed2_Max = v);
                        if (idxConveyorRatio > 0) AssignFullOrManual(firstRow, idxConveyorRatio, v => spsNoDoc.ConveyorRatio_Min = v, v => spsNoDoc.ConveyorRatio_Asli = v, v => spsNoDoc.ConveyorRatio_Max = v);

                        if (idxToleranceInner > 0) AssignFullOrManual(firstRow, idxToleranceInner, v => spsNoDoc.ToleranceInner_Min = v, v => spsNoDoc.ToleranceInner_Asli = v, v => spsNoDoc.ToleranceInner_Max = v);
                        if (idxToleranceOuter > 0) AssignFullOrManual(firstRow, idxToleranceOuter, v => spsNoDoc.ToleranceOuter_Min = v, v => spsNoDoc.ToleranceOuter_Asli = v, v => spsNoDoc.ToleranceOuter_Max = v);

                        if (idxTebalInner > 0) AssignFullOrManual(firstRow, idxTebalInner, v => spsNoDoc.TebalInner_Min = v, v => spsNoDoc.TebalInner_Asli = v, v => spsNoDoc.TebalInner_Max = v);
                        if (idxTebalOuter > 0) AssignFullOrManual(firstRow, idxTebalOuter, v => spsNoDoc.TebalOuter_Min = v, v => spsNoDoc.TebalOuter_Asli = v, v => spsNoDoc.TebalOuter_Max = v);
                        if (idxTebalInnerMiddle > 0) AssignFullOrManual(firstRow, idxTebalInnerMiddle, v => spsNoDoc.TebalInnerMiddle_Min = v, v => spsNoDoc.TebalInnerMiddle_Asli = v, v => spsNoDoc.TebalInnerMiddle_Max = v);
                        if (idxTebalTotal > 0) AssignFullOrManual(firstRow, idxTebalTotal, v => spsNoDoc.TebalTotal_Min = v, v => spsNoDoc.TebalTotal_Asli = v, v => spsNoDoc.TebalTotal_Max = v);
                        if (idxSelisihTebal > 0) AssignFullOrManual(firstRow, idxSelisihTebal, v => spsNoDoc.SelisihTebal_Min = v, v => spsNoDoc.SelisihTebal_Asli = v, v => spsNoDoc.SelisihTebal_Max = v);

                        if (idxODSensor > 0) AssignFullOrManual(firstRow, idxODSensor, v => spsNoDoc.OdSensor_Min = v, v => spsNoDoc.OdSensor_Asli = v, v => spsNoDoc.OdSensor_Max = v);
                        if (idxCuttingSpeed > 0) AssignFullOrManual(firstRow, idxCuttingSpeed, v => spsNoDoc.CuttingSpeed_Min = v, v => spsNoDoc.CuttingSpeed_Asli = v, v => spsNoDoc.CuttingSpeed_Max = v);

                        // Quality matrix targets
                        if (idxInnerTarget > 0) spsNoDoc.InnerTarget = GetCellValue(firstRow, idxInnerTarget);
                        if (idxInnerTol > 0) spsNoDoc.InnerTol = GetCellValue(firstRow, idxInnerTol);
                        if (idxInnerLCL > 0) spsNoDoc.InnerLCL = GetCellValue(firstRow, idxInnerLCL);
                        if (idxInnerMin > 0) spsNoDoc.InnerMin = GetCellValue(firstRow, idxInnerMin);
                        if (idxInnerUCL > 0) spsNoDoc.InnerUCL = GetCellValue(firstRow, idxInnerUCL);
                        if (idxInnerMax > 0) spsNoDoc.InnerMax = GetCellValue(firstRow, idxInnerMax);

                        if (idxThickTarget > 0) spsNoDoc.ThickTarget = GetCellValue(firstRow, idxThickTarget);
                        if (idxThickTol > 0) spsNoDoc.ThickTol = GetCellValue(firstRow, idxThickTol);
                        if (idxThickLCL > 0) spsNoDoc.ThickLCL = GetCellValue(firstRow, idxThickLCL);
                        if (idxThickMin > 0) spsNoDoc.ThickMin = GetCellValue(firstRow, idxThickMin);
                        if (idxThickUCL > 0) spsNoDoc.ThickUCL = GetCellValue(firstRow, idxThickUCL);
                        if (idxThickMax > 0) spsNoDoc.ThickMax = GetCellValue(firstRow, idxThickMax);

                        if (idxInnerMidTarget > 0) spsNoDoc.InnerMidTarget = GetCellValue(firstRow, idxInnerMidTarget);
                        if (idxInnerMidTol > 0) spsNoDoc.InnerMidTol = GetCellValue(firstRow, idxInnerMidTol);
                        if (idxInnerMidLCL > 0) spsNoDoc.InnerMidLCL = GetCellValue(firstRow, idxInnerMidLCL);
                        if (idxInnerMidMin > 0) spsNoDoc.InnerMidMin = GetCellValue(firstRow, idxInnerMidMin);
                        if (idxInnerMidUCL > 0) spsNoDoc.InnerMidUCL = GetCellValue(firstRow, idxInnerMidUCL);
                        if (idxInnerMidMax > 0) spsNoDoc.InnerMidMax = GetCellValue(firstRow, idxInnerMidMax);

                        if (idxTotalTarget > 0) spsNoDoc.TotalTarget = GetCellValue(firstRow, idxTotalTarget);
                        if (idxTotalTol > 0) spsNoDoc.TotalTol = GetCellValue(firstRow, idxTotalTol);
                        if (idxTotalLCL > 0) spsNoDoc.TotalLCL = GetCellValue(firstRow, idxTotalLCL);
                        if (idxTotalMin > 0) spsNoDoc.TotalMin = GetCellValue(firstRow, idxTotalMin);
                        if (idxTotalUCL > 0) spsNoDoc.TotalUCL = GetCellValue(firstRow, idxTotalUCL);
                        if (idxTotalMax > 0) spsNoDoc.TotalMax = GetCellValue(firstRow, idxTotalMax);

                        spsNoDoc.IsActive = true;

                        if (isNewDoc)
                        {
                            context.SpsNoDocs.Add(spsNoDoc);
                        }
                        else
                        {
                            context.SpsNoDocs.Update(spsNoDoc);
                        }

                        // Save document changes
                        await context.SaveChangesAsync();

                        // Process Item Lists mappings
                        foreach (var rNum in group.Value)
                        {
                            var itemListValue = GetCellValue(rNum, itemListCol);
                            if (string.IsNullOrWhiteSpace(itemListValue)) continue;

                            var items = itemListValue.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => s.Trim())
                                .Where(s => !string.IsNullOrEmpty(s))
                                .ToList();

                            foreach (var item in items)
                            {
                                bool itemExists = !isNewDoc && existingDoc?.ItemLists != null &&
                                                 existingDoc.ItemLists.Any(i => i.ItemList == item);

                                if (!itemExists)
                                {
                                    // Check database directly just to be safe
                                    bool existsInDb = await context.SpsItemLists
                                        .AnyAsync(i => i.DocumentNumber == docNumber && i.ItemList == item);

                                    if (!existsInDb)
                                    {
                                        context.SpsItemLists.Add(new SpsItemList
                                        {
                                            DocumentNumber = spsNoDoc.DocumentNumber,
                                            ItemList = item
                                        });
                                        await context.SaveChangesAsync();
                                        fileItemsAdded++;
                                    }
                                }
                            }
                        }
                    }

                    Console.WriteLine($"[+] File summary: Documents created: {fileDocsCreated}, updated: {fileDocsUpdated}. Item List mapped: {fileItemsAdded}");
                    totalDocsCreated += fileDocsCreated;
                    totalDocsUpdated += fileDocsUpdated;
                    totalItemsAdded += fileItemsAdded;
                    totalDocsProcessed += docGroups.Count;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[!] Error processing file {filename}: {ex.Message}");
                    Console.Error.WriteLine(ex.StackTrace);
                    totalErrors++;
                }
            }

            Console.WriteLine();
            Console.WriteLine("======================================================================");
            Console.WriteLine("                         FINAL SYNC REPORT");
            Console.WriteLine("======================================================================");
            Console.WriteLine($"Total Documents Processed : {totalDocsProcessed}");
            Console.WriteLine($"Total Documents Created   : {totalDocsCreated}");
            Console.WriteLine($"Total Documents Updated   : {totalDocsUpdated}");
            Console.WriteLine($"Total Item Codes Linked   : {totalItemsAdded}");
            Console.WriteLine($"Total Errors Encountered  : {totalErrors}");
            Console.WriteLine("Sync finished at: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine("======================================================================");
        }

        private static string NormalizeKey(string val)
        {
            if (string.IsNullOrWhiteSpace(val)) return "";
            return val.ToUpper()
                .Replace("|", "")
                .Replace("#", "")
                .Replace(".", "")
                .Replace(" ", "")
                .Replace("_", "")
                .Replace("-", "");
        }
    }
}
