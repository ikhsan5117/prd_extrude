using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExcelDataReader;
using System.Data;
using System.Text;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Controllers
{
    public class MasterlistSpsDoubleLayersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MasterlistSpsDoubleLayersController> _logger;

        public MasterlistSpsDoubleLayersController(ApplicationDbContext context, ILogger<MasterlistSpsDoubleLayersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.MasterlistSpsDoubleLayers.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.MasterlistSpsDoubleLayers
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> ClearData()
        {
            _context.MasterlistSpsDoubleLayers.RemoveRange(_context.MasterlistSpsDoubleLayers);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0) {
                TempData["ErrorMessage"] = "File tidak ditemukan atau kosong";
                return RedirectToAction(nameof(Index));
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet();
                        
                        // Gunakan helper method untuk pilih sheet terbaik
                        DataTable table = SelectBestSheet(result);

                        // Gunakan helper method untuk deteksi format dengan logging
                        bool isDig3L, isDig2L, isLegacy;
                        string detectionLog = DetectFormatWithLogging(table, out isDig3L, out isDig2L, out isLegacy);
                        
                        // Validasi struktur Excel
                        string validationError;
                        if (!ValidateExcelStructure(table, isDig3L, isDig2L, isLegacy, out validationError)) {
                            TempData["ErrorMessage"] = $"Validasi gagal: {validationError}\n\n{detectionLog}";
                            return RedirectToAction(nameof(Index));
                        }

                        // Cek apakah format terdeteksi
                        if (!isDig3L && !isDig2L && !isLegacy) {
                            TempData["ErrorMessage"] = $"Format SPS tidak terdeteksi!\n\n{detectionLog}";
                            return RedirectToAction(nameof(Index));
                        }

                        // 2. Set Indices
                        int idxItem, idxRev, idxCust, idxDate, idxForm, idxType, idxDim, idxMat, idxInner, idxOuter;
                        int idxDoc = 3;
                        int idxMC = isLegacy ? 51 : -1;

                        if (isDig3L) {
                            idxItem = 107;
                            idxRev = 6; idxCust = 7; idxDate = 8; idxForm = 9;
                            idxType = 10; idxDim = 11; idxMat = 12; idxInner = 13; idxOuter = 15;
                        } else if (isDig2L) {
                            idxItem = 89;
                            idxRev = 6; idxCust = 7; idxDate = 8; idxForm = 9;
                            idxType = 10; idxDim = 11; idxMat = 12; idxInner = 13; idxOuter = 14;
                        } else {
                            // Legacy / Non-CHS
                            idxItem = 50;
                            idxRev = 4; idxCust = 5; idxDate = 6; idxForm = 7;
                            idxType = 8; idxDim = 9; idxMat = 10; idxInner = 11; idxOuter = 12;
                        }

                        
                        // 2b. Dynamic Item Column Detection (Fallback)
                        // HANYA jalan jika format TIDAK terdeteksi (untuk safety)
                        // Jika format sudah terdeteksi, gunakan index yang sudah di-set
                        if ((!isDig3L && !isDig2L && !isLegacy) && table.Rows.Count > 5) {
                            _logger.LogWarning("Format tidak terdeteksi, mencoba dynamic item column detection...");
                            for (int c = 0; c < table.Rows[5].ItemArray.Length; c++) {
                                string val = GetV(table.Rows[5], c).ToLower();
                                if (val.Contains("item") || val == "107" || val == "89" || val == "50") {
                                    idxItem = c;
                                    _logger.LogInformation($"Dynamic detection: Item column set to {c}");
                                    break;
                                }
                            }
                        } else if (isDig3L || isDig2L || isLegacy) {
                            _logger.LogInformation($"Format terdeteksi, Item column: {idxItem}");
                        }

                        int startRow = 6;
                        var existingData = await _context.MasterlistSpsDoubleLayers.ToListAsync();
                        int newCount = 0, updateCount = 0;

                        for (int i = startRow; i < table.Rows.Count; i++)
                        {
                            var row = table.Rows[i];
                            string idExcel = GetV(row, 0);
                            if (string.IsNullOrEmpty(idExcel) || idExcel.Length > 20 || idExcel == "1" || idExcel == "2") continue;

                            string rawItems = GetV(row, idxItem);
                            if (string.IsNullOrEmpty(rawItems)) continue;

                            var itemsList = rawItems.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();

                            foreach (var itemCode in itemsList)
                            {
                                var target = existingData.FirstOrDefault(x => x.ExcelId == idExcel && x.ItemList == itemCode) ?? new MasterlistSpsDoubleLayer();
                                bool isNew = (target.Id == 0);

                                target.ExcelId = idExcel;
                                target.No = GetV(row, 1);
                                target.Machine = GetV(row, 2);
                                target.ItemList = itemCode;
                                target.DocumentNumber = GetV(row, idxDoc);
                                target.RevisionNumber = GetV(row, idxRev);
                                target.Customer = GetV(row, idxCust);
                                target.RevisionDate = GetV(row, idxDate);
                                target.Formulasi = GetV(row, idxForm);
                                target.HoseType = GetV(row, idxType);
                                target.Dimensi = GetV(row, idxDim);
                                target.Material = GetV(row, idxMat);
                                target.InnerTube = GetV(row, idxInner);
                                target.OuterCover = GetV(row, idxOuter);
                                target.MiddleTube = isDig3L ? GetV(row, 14) : "";

                                // Use Limits Mapping
                                if (isDig3L) {
                                    target.UseLimitsInner = GetV(row, 16);
                                    target.UseLimitsMiddle = GetV(row, 17);
                                    target.UseLimitsOuter = GetV(row, 18);
                                } else if (isDig2L) {
                                    target.UseLimitsInner = GetV(row, 15);
                                    target.UseLimitsOuter = GetV(row, 16);
                                    target.PitchYarn = GetV(row, 18);
                                } else {
                                    target.UseLimitsInner = GetV(row, 13);
                                    target.UseLimitsOuter = GetV(row, 14);
                                }

                                if (idxMC != -1) target.MachineCode = GetV(row, idxMC);

                                // Dies & Mesh
                                if (isDig3L) {
                                    target.Yarn = GetV(row, 19);
                                    target.TensionYarnInner = GetV(row, 20);
                                    target.TensionYarnOuter = GetV(row, 21);
                                    target.Nipple = GetV(row, 22);
                                    target.TubeDie = GetV(row, 23);
                                    target.MiddleDie = GetV(row, 24);
                                    target.CoverDie = GetV(row, 25);
                                    target.SpacerDie = GetV(row, 26);
                                    target.ADistance = GetV(row, 27);
                                    target.MeshScreen1 = GetV(row, 29);
                                    target.MeshScreen2 = GetV(row, 31);
                                    target.MeshScreen3 = GetV(row, 33);
                                } else if (isDig2L) {
                                    target.Yarn = GetV(row, 17);
                                    target.TensionYarnInner = GetV(row, 19);
                                    target.TensionYarnOuter = GetV(row, 20);
                                    target.Nipple = GetV(row, 21);
                                    target.TubeDie = GetV(row, 22);
                                    target.MiddleDie = GetV(row, 23);
                                    target.CoverDie = GetV(row, 24);
                                    target.SpacerDie = GetV(row, 25);
                                    target.ADistance = GetV(row, 26);
                                    target.MeshScreen1 = GetV(row, 28);
                                    target.MeshScreen2 = GetV(row, 30);
                                } else {
                                    target.Nipple = GetV(row, 15);
                                    target.TubeDie = GetV(row, 16);
                                    target.CoverDie = GetV(row, 17);
                                    target.MeshScreen1 = GetV(row, 18);
                                    target.MeshScreen2 = GetV(row, 19);
                                    target.ADistance = GetV(row, 27); // Added for Legacy if exists
                                }

                                // Parameters
                                if (isDig3L) {
                                    target.HeadTemp1 = GetV(row, 34); target.Cylinder1_1 = GetV(row, 35);
                                    target.Cylinder2_1 = GetV(row, 36); target.Cylinder3_1 = GetV(row, 37);
                                    target.ScrewTemp1 = GetV(row, 38);
                                    target.HeadTemp2 = GetV(row, 39); target.Cylinder1_2 = GetV(row, 40);
                                    target.Cylinder2_2 = GetV(row, 41); target.Cylinder3_2 = GetV(row, 42);
                                    target.ScrewTemp2 = GetV(row, 43);
                                    target.HeadTemp3 = GetV(row, 44); target.Cylinder1_3 = GetV(row, 45);
                                    target.Cylinder2_3 = GetV(row, 46); target.Cylinder3_3 = GetV(row, 47);
                                    target.ScrewTemp3 = GetV(row, 48);
                                    target.ScrewSpeed1 = GetV(row, 49); target.ScrewSpeed2 = GetV(row, 50);
                                    target.ScrewSpeed3 = GetV(row, 51);
                                    target.Pressure1 = GetV(row, 55); target.Pressure2 = GetV(row, 56);
                                    target.AmMeter = GetV(row, 58);
                                    target.AmMeter2 = GetV(row, 59);
                                    target.OdSensor = GetV(row, 61);

                                    target.MarkingSort = GetV(row, 68);
                                    target.TextMarkingMaterial = GetV(row, 69);
                                    target.MarkingColour = GetV(row, 70);
                                    target.ChillerWaterTemp = GetV(row, 71);
                                    target.CaterpillarGap = GetV(row, 72);
                                    target.TakeUpConveyorSpeed = GetV(row, 73);
                                    target.CoolConveyorSpeed = GetV(row, 74);
                                    
                                    target.ToleranceInner = GetV(row, 77);
                                    target.ToleranceOuter = GetV(row, 78);
                                    target.TebalInner = GetV(row, 79);
                                    target.TebalOuter = GetV(row, 80);
                                    target.TebalTotal = GetV(row, 81);
                                    target.SelisihTebal = GetV(row, 82);
                                } else if (isDig2L) {
                                    target.HeadTemp1 = GetV(row, 31); target.Cylinder1_1 = GetV(row, 32);
                                    target.Cylinder2_1 = GetV(row, 33); target.Cylinder3_1 = GetV(row, 34);
                                    target.ScrewTemp1 = GetV(row, 35);
                                    target.HeadTemp2 = GetV(row, 36); target.Cylinder1_2 = GetV(row, 37);
                                    target.Cylinder2_2 = GetV(row, 38); target.Cylinder3_2 = GetV(row, 39);
                                    target.ScrewTemp2 = GetV(row, 40);
                                    target.ScrewSpeed1 = GetV(row, 41); target.ScrewSpeed2 = GetV(row, 42);
                                    target.FeedRollRatio1 = GetV(row, 43);
                                    target.FeedRollRatio2 = GetV(row, 44);
                                    target.Pressure1 = GetV(row, 45); target.Pressure2 = GetV(row, 46);
                                    target.CurrentValue = GetV(row, 47);
                                    target.AmMeter = GetV(row, 48);
                                    target.AmMeter2 = GetV(row, 49);
                                    target.PresetValue = GetV(row, 50);
                                    target.ControlValue = GetV(row, 51);
                                    target.SpiralPitchSetting = GetV(row, 52);
                                    target.SpiralPitchDisplay = GetV(row, 53);
                                    target.SpiralSpeed = GetV(row, 54);
                                    target.HoseSpeed = GetV(row, 55);
                                    target.UnsmoothSurface = GetV(row, 56);

                                    target.MarkingSort = GetV(row, 57);
                                    target.TextMarkingMaterial = GetV(row, 58);
                                    target.MarkingColour = GetV(row, 59);
                                    target.ChillerWaterTemp = GetV(row, 60);
                                    target.DancerPosition = GetV(row, 61);
                                    target.CaterpillarGap = GetV(row, 62);
                                    target.TakeUpConveyorSpeed = GetV(row, 63);
                                    target.CoolConveyorSpeed = GetV(row, 64);
                                    target.ConveyorRatio = GetV(row, 65);

                                    target.ToleranceInner = GetV(row, 66);
                                    target.ToleranceOuter = GetV(row, 67);
                                    target.TebalInner = GetV(row, 68);
                                    target.TebalOuter = GetV(row, 69);
                                    target.TebalTotal = GetV(row, 70);
                                    target.SelisihTebal = GetV(row, 71);

                                    // Quality Matrix CHS 2L
                                    target.InnerTarget = GetV(row, 72);
                                    target.InnerTol = GetV(row, 73);
                                    target.InnerLCL = GetV(row, 74);
                                    target.InnerMin = GetV(row, 75);
                                    target.InnerUCL = GetV(row, 76);
                                    target.InnerMax = GetV(row, 77);
                                    
                                    target.ThickTarget = GetV(row, 78);
                                    target.ThickTol = GetV(row, 79);
                                    target.ThickLCL = GetV(row, 80);
                                    target.ThickMin = GetV(row, 81);
                                    target.ThickUCL = GetV(row, 82);
                                    target.ThickMax = GetV(row, 83);

                                    target.TotalTarget = GetV(row, 84);
                                    target.TotalTol = GetV(row, 85);
                                    target.TotalLCL = GetV(row, 86);
                                    target.TotalMin = GetV(row, 87);
                                    target.TotalUCL = GetV(row, 88);
                                    target.TotalMax = GetV(row, 89);
                                } else {
                                    target.HeadTemp1 = GetV(row, 20); target.HeadTemp2 = GetV(row, 21);
                                    target.Cylinder1_1 = GetV(row, 22); target.Cylinder1_2 = GetV(row, 23);
                                    target.Cylinder2_1 = GetV(row, 24); target.Cylinder2_2 = GetV(row, 25);
                                    target.Feed1 = GetV(row, 26); target.Feed2 = GetV(row, 27);
                                    target.ScrewTemp1 = GetV(row, 28); target.ScrewTemp2 = GetV(row, 29);
                                    target.ScrewSpeed1 = GetV(row, 30); target.ScrewSpeed2 = GetV(row, 31);
                                    target.Pressure1 = GetV(row, 32); target.Pressure2 = GetV(row, 33);
                                    target.AmMeter = GetV(row, 34);
                                    target.OdSensor = GetV(row, 35);

                                    target.MarkingSort = GetV(row, 36);
                                    target.TextMarkingMaterial = GetV(row, 37);
                                    target.MarkingColour = GetV(row, 38);
                                    target.ChillerWaterTemp = GetV(row, 39);
                                    target.CuttingSpeed = GetV(row, 40);
                                    target.TakeUpConveyorSpeed = GetV(row, 41);
                                    target.ToleranceInner = GetV(row, 42);
                                    target.ToleranceOuter = GetV(row, 43);
                                    target.TebalInner = GetV(row, 44);
                                    target.TebalOuter = GetV(row, 45);
                                    target.TebalTotal = GetV(row, 46);
                                    target.SelisihTebal = GetV(row, 47);
                                }

                                if (isNew) { _context.MasterlistSpsDoubleLayers.Add(target); newCount++; }
                                else { _context.MasterlistSpsDoubleLayers.Update(target); updateCount++; }
                            }
                        }
                        await _context.SaveChangesAsync();
                        
                        string detectedFormat = isDig3L ? "CHS 3 Layer" : (isDig2L ? "CHS 2 Layer" : "Legacy/Non-CHS");
                        string sheetLog = TempData["SheetLog"]?.ToString() ?? "";
                        
                        // Validasi: hanya tampilkan success jika ada data yang masuk
                        if (newCount == 0 && updateCount == 0) {
                            TempData["ErrorMessage"] = $"⚠ Tidak ada data yang diimport!\n" +
                                                      $"Format: {detectedFormat}\n" +
                                                      $"Sheet: {table.TableName}\n" +
                                                      $"Kolom Item: {idxItem}\n" +
                                                      $"Total rows di Excel: {table.Rows.Count}\n" +
                                                      $"Start row: {startRow}\n\n" +
                                                      $"Kemungkinan penyebab:\n" +
                                                      $"- ID Excel kosong atau tidak valid\n" +
                                                      $"- Item List kosong di kolom {idxItem}\n" +
                                                      $"- Data dimulai bukan dari row 6\n" +
                                                      $"- Kolom Item salah (pastikan kolom {idxItem} berisi item codes)";
                            _logger.LogWarning($"Import warning: Format={detectedFormat}, 0 records imported");
                        } else {
                            TempData["SuccessMessage"] = $"✓ Import Berhasil!\n" +
                                                        $"Format: {detectedFormat}\n" +
                                                        $"Sheet: {table.TableName}\n" +
                                                        $"Kolom Item: {idxItem}\n" +
                                                        $"Data baru: {newCount}\n" +
                                                        $"Data diperbarui: {updateCount}\n" +
                                                        $"Total: {newCount + updateCount} records";
                            
                            _logger.LogInformation($"Import sukses: {detectedFormat}, {newCount} new, {updateCount} updated");
                        }
                    }
                }
            }
            catch (Exception ex) { 
                _logger.LogError(ex, "Error during ImportExcel");
                TempData["ErrorMessage"] = $"Error: {ex.Message}\n\nDetail: {ex.InnerException?.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        private string GetV(DataRow row, int colIndex)
        {
            try {
                if (colIndex < 0 || colIndex >= row.Table.Columns.Count) return "";
                return row[colIndex]?.ToString()?.Trim() ?? "";
            } catch { return ""; }
        }

        /// <summary>
        /// Pilih sheet terbaik dari Excel berdasarkan nama atau ukuran
        /// </summary>
        private DataTable SelectBestSheet(DataSet result)
        {
            var log = new StringBuilder();
            log.AppendLine("=== SHEET DETECTION ===");
            
            // Priority 1: Sheet dengan nama yang mengandung keyword
            string[] keywords = { "Parameter Setting", "Master 1", "SPS", "Parameter", "Master" };
            
            foreach (DataTable table in result.Tables) {
                log.AppendLine($"Found: '{table.TableName}' ({table.Rows.Count} rows, {table.Columns.Count} cols)");
                
                foreach (var keyword in keywords) {
                    if (table.TableName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) {
                        log.AppendLine($"✓ Selected: {table.TableName} (matched: {keyword})");
                        _logger.LogInformation(log.ToString());
                        TempData["SheetLog"] = log.ToString();
                        return table;
                    }
                }
            }
            
            // Priority 2: Sheet dengan row terbanyak
            var largestSheet = result.Tables.Cast<DataTable>()
                .OrderByDescending(t => t.Rows.Count)
                .FirstOrDefault();
            
            if (largestSheet != null && largestSheet.Rows.Count > 5) {
                log.AppendLine($"✓ Selected largest: {largestSheet.TableName} ({largestSheet.Rows.Count} rows)");
                _logger.LogInformation(log.ToString());
                TempData["SheetLog"] = log.ToString();
                return largestSheet;
            }
            
            // Fallback: Sheet pertama
            var firstSheet = result.Tables[0];
            log.AppendLine($"⚠ Using first sheet: {firstSheet.TableName}");
            _logger.LogWarning(log.ToString());
            TempData["SheetLog"] = log.ToString();
            return firstSheet;
        }

        /// <summary>
        /// Deteksi format SPS dengan logging detail
        /// </summary>
        private string DetectFormatWithLogging(DataTable table, out bool isDig3L, out bool isDig2L, out bool isLegacy)
        {
            isDig3L = isDig2L = isLegacy = false;
            var log = new StringBuilder();
            
            log.AppendLine("=== FORMAT DETECTION ===");
            log.AppendLine($"Sheet: {table.TableName}");
            log.AppendLine($"Rows: {table.Rows.Count}, Columns: {table.Columns.Count}");
            
            // Method 1: Check title in Row 1
            if (table.Rows.Count > 0) {
                string title = GetV(table.Rows[0], 1).ToUpper();
                log.AppendLine($"Title (Row 1, Col B): '{title}'");
                
                if (title.Contains("CHS 3 LAYER") || title.Contains("3 LAYER")) {
                    isDig3L = true;
                    log.AppendLine("✓ PRIMARY: CHS 3 LAYER");
                }
                else if (title.Contains("CHS 2 LAYER") || title.Contains("2 LAYER")) {
                    isDig2L = true;
                    log.AppendLine("✓ PRIMARY: CHS 2 LAYER");
                }
                else if (title.Contains("NON-CHS") || title.Contains("NON CHS")) {
                    isLegacy = true;
                    log.AppendLine("✓ PRIMARY: LEGACY/NON-CHS");
                }
                else {
                    log.AppendLine("⚠ No format found in title");
                }
            }
            
            // Method 2: Fallback - check Item column position
            if (!isDig3L && !isDig2L && !isLegacy && table.Rows.Count > 5) {
                log.AppendLine("Trying fallback detection...");
                
                // Check multiple rows for "Item" indicator
                for (int checkRow = 2; checkRow <= Math.Min(6, table.Rows.Count - 1); checkRow++) {
                    string col107 = GetV(table.Rows[checkRow], 107);
                    string col89 = GetV(table.Rows[checkRow], 89);
                    string col50 = GetV(table.Rows[checkRow], 50);
                    
                    if (!string.IsNullOrEmpty(col107) && (col107.Contains("Item") || col107.Contains("107"))) {
                        isDig3L = true;
                        log.AppendLine($"✓ FALLBACK: CHS 3 LAYER (Row {checkRow}, Col 107: '{col107}')");
                        break;
                    }
                    else if (!string.IsNullOrEmpty(col89) && (col89.Contains("Item") || col89.Contains("89") || col89.Contains("90"))) {
                        isDig2L = true;
                        log.AppendLine($"✓ FALLBACK: CHS 2 LAYER (Row {checkRow}, Col 89: '{col89}')");
                        break;
                    }
                    else if (!string.IsNullOrEmpty(col50) && (col50.Contains("Item") || col50.Contains("ITEM") || col50.Contains("50") || col50.Contains("51"))) {
                        isLegacy = true;
                        log.AppendLine($"✓ FALLBACK: LEGACY (Row {checkRow}, Col 50: '{col50}')");
                        break;
                    }
                }
            }
            
            if (!isDig3L && !isDig2L && !isLegacy) {
                log.AppendLine("❌ FORMAT TIDAK TERDETEKSI!");
                log.AppendLine("Pastikan:");
                log.AppendLine("- Row 1 memiliki title 'CHS 3 LAYER' / 'CHS 2 LAYER' / 'NON-CHS'");
                log.AppendLine("- Atau kolom Item ada di posisi 107/89/50");
            }
            
            string logResult = log.ToString();
            _logger.LogInformation(logResult);
            return logResult;
        }

        /// <summary>
        /// Validasi struktur kolom Excel
        /// </summary>
        private bool ValidateExcelStructure(DataTable table, bool isDig3L, bool isDig2L, bool isLegacy, out string errorMsg)
        {
            errorMsg = "";
            int expectedItemCol = isDig3L ? 107 : (isDig2L ? 89 : 50);
            int minColumns = expectedItemCol + 1;
            
            // Check 1: Jumlah kolom cukup
            if (table.Columns.Count < minColumns) {
                errorMsg = $"❌ Kolom tidak cukup! Expected: {minColumns}, Found: {table.Columns.Count}";
                _logger.LogError(errorMsg);
                return false;
            }
            
            // Check 2: Ada cukup baris data
            if (table.Rows.Count < 7) {
                errorMsg = $"❌ Data terlalu sedikit! Expected: min 7 rows, Found: {table.Rows.Count}";
                _logger.LogError(errorMsg);
                return false;
            }
            
            // Check 3: Kolom kunci tidak kosong
            int testRow = 6; // First data row
            if (testRow < table.Rows.Count) {
                string idExcel = GetV(table.Rows[testRow], 0);
                string machine = GetV(table.Rows[testRow], 2);
                string itemList = GetV(table.Rows[testRow], expectedItemCol);
                
                if (string.IsNullOrEmpty(idExcel) && string.IsNullOrEmpty(itemList)) {
                    errorMsg = $"⚠ Warning: Row {testRow + 1} memiliki ID Excel dan Item List kosong";
                    _logger.LogWarning(errorMsg);
                    // Not a hard fail, just warning
                }
            }
            
            _logger.LogInformation($"✓ Validasi passed: {table.Columns.Count} columns, {table.Rows.Count} rows");
            return true;
        }

        /// <summary>
        /// Preview Excel untuk debugging - API endpoint
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PreviewExcel(IFormFile file)
        {
            if (file == null || file.Length == 0) 
                return Json(new { success = false, message = "File tidak ditemukan" });

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet();
                        
                        // List all sheets
                        var sheets = result.Tables.Cast<DataTable>()
                            .Select(t => new {
                                name = t.TableName,
                                rows = t.Rows.Count,
                                columns = t.Columns.Count
                            }).ToList();
                        
                        // Select best sheet
                        var table = SelectBestSheet(result);
                        
                        // Detect format
                        bool isDig3L, isDig2L, isLegacy;
                        string detectionLog = DetectFormatWithLogging(table, out isDig3L, out isDig2L, out isLegacy);
                        
                        string format = isDig3L ? "CHS 3 LAYER" : 
                                       (isDig2L ? "CHS 2 LAYER" : 
                                       (isLegacy ? "LEGACY" : "UNKNOWN"));
                        
                        // Validate structure
                        string errorMsg;
                        bool isValid = ValidateExcelStructure(table, isDig3L, isDig2L, isLegacy, out errorMsg);
                        
                        int expectedItemCol = isDig3L ? 107 : (isDig2L ? 89 : 50);
                        
                        // Get sample data
                        var sampleData = new List<object>();
                        int startRow = 6;
                        for (int i = startRow; i < Math.Min(startRow + 5, table.Rows.Count); i++) {
                            var row = table.Rows[i];
                            string idExcel = GetV(row, 0);
                            if (string.IsNullOrEmpty(idExcel)) continue;
                            
                            sampleData.Add(new {
                                rowIndex = i + 1,
                                idExcel = idExcel,
                                no = GetV(row, 1),
                                machine = GetV(row, 2),
                                customer = GetV(row, isDig3L || isDig2L ? 6 : 4),
                                itemList = GetV(row, expectedItemCol),
                                dimensi = GetV(row, isDig3L ? 11 : (isDig2L ? 9 : 8))
                            });
                        }
                        
                        return Json(new {
                            success = true,
                            fileName = file.FileName,
                            fileSize = file.Length,
                            sheets = sheets,
                            selectedSheet = table.TableName,
                            detectedFormat = format,
                            isValid = isValid,
                            validationError = errorMsg,
                            detectionLog = detectionLog,
                            expectedItemColumn = expectedItemCol,
                            totalRows = table.Rows.Count,
                            totalColumns = table.Columns.Count,
                            sampleData = sampleData,
                            estimatedRecords = sampleData.Count > 0 ? $"~{table.Rows.Count - 6} records" : "0 records"
                        });
                    }
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error in PreviewExcel");
                return Json(new { success = false, message = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}
