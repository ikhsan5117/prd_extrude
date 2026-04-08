using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Controllers
{
    public class ProductionReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductionReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductionReport (Monitoring List)
        public async Task<IActionResult> Index()
        {
            var reports = await _context.ProductionReports
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
            return View(reports);
        }

        // GET: ProductionReport/ParameterHistory
        public async Task<IActionResult> ParameterHistory()
        {
            var reports = await _context.ProductionReports
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
            return View(reports);
        }

        // GET: ProductionReport/DimensionHistory
        public async Task<IActionResult> DimensionHistory()
        {
            var reports = await _context.ProductionReports
                .Where(p => !string.IsNullOrEmpty(p.ActualLength) || !string.IsNullOrEmpty(p.VinCode) || p.QtyOk > 0 || p.NgDimension > 0 || p.NgVisual > 0)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
            return View(reports);
        }

        // GET: ProductionReport/App (New Tablet App Interface)
        public async Task<IActionResult> App(int? id)
        {
            if (id == null)
            {
                // Empty report for Scanner Mode
                var emptyReport = new ProductionReport 
                { 
                    Id = 0,
                    DocumentNumber = "---",
                    RevisionNumber = 0,
                    ProductionDate = DateTime.Today,
                    CustomerName = "---",
                    HoseType = "---",
                    Dimension = "---",
                    Yarn = "---",
                    Shift = "---"
                };
                return View(emptyReport);
            }

            var report = await _context.ProductionReports
                .Include(p => p.StandardParameterSetting)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (report == null) return NotFound();

            // Fetch Masterlist data as fallback standards
            ViewBag.Masterlist = await _context.MasterlistSpsDoubleLayers
                .FirstOrDefaultAsync(m => m.HoseType == report.HoseType);

            return View(report);
        }

        // GET: ProductionReport/AppByItem (For Barcode Scanner)
        public async Task<IActionResult> AppByItem(string itemCode)
        {
            var master = await _context.MasterlistSpsDoubleLayers
                .FirstOrDefaultAsync(m => m.ItemList == itemCode);
            
            if (master == null) 
            {
                TempData["ErrorMessage"] = $"Item Code '{itemCode}' tidak ditemukan di Database SPS Masterlist!";
                return RedirectToAction(nameof(App));
            }
            
            var report = new ProductionReport {
                Id = 0,
                DocumentNumber = master.DocumentNumber ?? "-",
                RevisionNumber = int.TryParse(master.RevisionNumber, out int rev) ? rev : 0,
                ProductionDate = DateTime.Today,
                CustomerName = master.Customer ?? "-",
                HoseType = master.HoseType ?? "-",
                Dimension = master.Dimensi ?? "-",
                Yarn = master.InnerTube ?? "-",
                Shift = "Shift 1"
            };
            
            ViewBag.Masterlist = master;
            TempData["SuccessMessage"] = $"Berhasil memuat spesifikasi untuk '{itemCode}'!";
            
            return View("App", report);
        }

        // AJAX API: Get SPS by ID
        [HttpGet]
        public async Task<IActionResult> GetSpsById(int id)
        {
            var sps = await _context.StandardParameterSettings.FindAsync(id);
            if (sps == null) return NotFound();
            return Json(sps);
        }

        // AJAX API: Get SPS by Item Code (Scanner)
        [HttpGet]
        public async Task<IActionResult> GetSpsByItem(string itemCode)
        {
            if (string.IsNullOrWhiteSpace(itemCode)) return BadRequest();
            
            var sanitizedCode = itemCode.Trim().ToUpper();
            
            // Try StandardParameterSettings first (Detailed)
            var sps = await _context.StandardParameterSettings
                .FirstOrDefaultAsync(s => 
                    (s.ItemList != null && s.ItemList.ToUpper().Contains(sanitizedCode)) || 
                    (s.ProductCode != null && s.ProductCode.ToUpper().Contains(sanitizedCode)));
            
            if (sps != null) return Json(sps);

            // Fallback to MasterlistSpsDoubleLayers (Brief)
            var master = await _context.MasterlistSpsDoubleLayers
                .FirstOrDefaultAsync(m => m.ItemList != null && m.ItemList.ToUpper().Contains(sanitizedCode));

            if (master != null)
            {
                // Map Master to a temporary SPS-like object for the form
                return Json(new {
                    ItemList = master.ItemList,
                    HoseType = master.HoseType,
                    CustomerName = master.Customer,
                    DocumentNumber = master.DocumentNumber,
                    RevisionNumber = master.RevisionNumber,
                    InnerMaterial = master.InnerTube,
                    OuterMaterial = master.OuterCover,
                    YarnType = master.Material,
                    
                    // Map Masterlist values (These are strings in the DB)
                    HeadTempInner = master.HeadTemp1,
                    Cylinder1TempInner = master.Cylinder1_1,
                    Cylinder2TempInner = master.Cylinder2_1,
                    ScrewTempInner = master.ScrewTemp1,
                    ScrewSpeedInner = master.ScrewSpeed1,
                    PressureInner = master.Pressure1,
                    FeedRollRatioInner = master.Feed1,
                    
                    HeadTempOuter = master.HeadTemp2,
                    Cylinder1TempOuter = master.Cylinder1_2,
                    Cylinder2TempOuter = master.Cylinder2_2,
                    ScrewTempOuter = master.ScrewTemp2,
                    ScrewSpeedOuter = master.ScrewSpeed2,
                    PressureOuter = master.Pressure2,
                    FeedRollRatioOuter = master.Feed2,
                    
                    InnerDie = master.Nipple,
                    TubeDie = master.TubeDie,
                    CoverDie = master.CoverDie,
                    ToleranceDie = master.ToleranceInner,
                    
                    ChillerWaterTemp = master.ChillerWaterTemp,
                    TakeupConveyorSpeed = master.TakeUpConveyorSpeed
                });
            }

            return NotFound();
        }

        // AJAX API: Get Planning Items by Date & Shift
        [HttpGet]
        public async Task<IActionResult> GetPlanningItems(DateTime date, string shift)
        {
            // Gunakan culture Indonesia
            var culture = new System.Globalization.CultureInfo("id-ID");
            
            // Ambil bagian tanggalnya saja "8 APRIL 2026" (tanpa nama hari agar lebih fleksibel)
            var datePart = date.ToString("d MMMM yyyy", culture).ToUpper();
            
            // Konversi Shift agar mendukung format SHIFT I/II/III dan SHIFT 1/2/3
            var shiftRaw = (shift ?? "").Trim().ToUpper();
            if (shiftRaw.StartsWith("SHIFT "))
            {
                shiftRaw = shiftRaw.Substring(6).Trim();
            }

            string shiftVariant1 = shiftRaw;
            string shiftVariant2 = shiftRaw;
            if (shiftRaw == "I") shiftVariant1 = "1";
            else if (shiftRaw == "II") shiftVariant1 = "2";
            else if (shiftRaw == "III") shiftVariant1 = "3";
            else if (shiftRaw == "1") shiftVariant1 = "I";
            else if (shiftRaw == "2") shiftVariant1 = "II";
            else if (shiftRaw == "3") shiftVariant1 = "III";

            var shiftSearch1 = "SHIFT " + shiftRaw;
            var shiftSearch2 = "SHIFT " + shiftVariant1;

            // Ambil data yang mengandung tanggal tersebut dan shift tersebut
            var itemRows = await _context.PlanningMasters
                .Where(p => p.DateShiftString != null &&
                            p.DateShiftString.ToUpper().Contains(datePart) &&
                            (p.DateShiftString.ToUpper().Contains(shiftSearch1) || p.DateShiftString.ToUpper().Contains(shiftSearch2)))
                .Select(p => new { p.PartName1, p.PartName2, p.DateShiftString })
                .ToListAsync();

            if (!itemRows.Any())
            {
                // Jika tidak ada data untuk shift yang dipilih, fallback ke semua shift pada tanggal yang sama
                itemRows = await _context.PlanningMasters
                    .Where(p => p.DateShiftString != null && p.DateShiftString.ToUpper().Contains(datePart))
                    .Select(p => new { p.PartName1, p.PartName2, p.DateShiftString })
                    .ToListAsync();
            }

            var items = itemRows
                .Select(p => {
                    var parsed = ParseDateShiftString(p.DateShiftString);
                    return new {
                        itemCode = p.PartName1,
                        itemName = p.PartName2,
                        dateShift = p.DateShiftString,
                        date = parsed.date,
                        shift = parsed.shift
                    };
                })
                .Distinct()
                .ToList();

            return Json(items);
        }

        private static (string date, string shift) ParseDateShiftString(string? dateShiftString)
        {
            if (string.IsNullOrWhiteSpace(dateShiftString)) return (string.Empty, string.Empty);

            var normalized = dateShiftString.ToUpper().Trim();
            var parts = normalized.Split(new[] { " SHIFT " }, StringSplitOptions.None);
            if (parts.Length != 2) return (string.Empty, string.Empty);

            var dayString = parts[0];
            var shiftString = parts[1].Trim();

            // Hapus hari nama di depan, contoh: "RABU, 8 APRIL 2026" -> "8 APRIL 2026"
            var commaIndex = dayString.IndexOf(',');
            if (commaIndex >= 0 && commaIndex + 1 < dayString.Length)
            {
                dayString = dayString.Substring(commaIndex + 1).Trim();
            }

            var dateParts = dayString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (dateParts.Length < 3) return (string.Empty, string.Empty);

            var day = dateParts[0].PadLeft(2, '0');
            var month = dateParts[1];
            var year = dateParts[2];

            var monthMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "JANUARI", "01" }, { "FEBRUARI", "02" }, { "MARET", "03" }, { "APRIL", "04" },
                { "MEI", "05" }, { "JUNI", "06" }, { "JULI", "07" }, { "AGUSTUS", "08" },
                { "SEPTEMBER", "09" }, { "OKTOBER", "10" }, { "NOVEMBER", "11" }, { "DESEMBER", "12" }
            };

            if (!monthMap.TryGetValue(month, out var monthNumber))
                monthNumber = "01";

            var formattedDate = $"{year}-{monthNumber}-{day}";

            string normalizedShift = shiftString;
            if (normalizedShift == "1") normalizedShift = "I";
            else if (normalizedShift == "2") normalizedShift = "II";
            else if (normalizedShift == "3") normalizedShift = "III";

            return (formattedDate, normalizedShift);
        }

        // GET: ProductionReport/Details/5 (The Monitoring Dashboard - Gambar 2 & 5)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var report = await _context.ProductionReports
                .Include(p => p.StandardParameterSetting)
                .Include(p => p.ProductionReadings)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (report == null) return NotFound();

            // Fetch Masterlist data as fallback standards
            ViewBag.Masterlist = await _context.MasterlistSpsDoubleLayers
                .FirstOrDefaultAsync(m => m.HoseType == report.HoseType);

            return View(report);
        }

        // GET: ProductionReport/Create (The "NOW I'M PRODUCE" Form - Gambar 1)
        public async Task<IActionResult> Create()
        {
            var hoseTypes = await _context.MasterlistSpsDoubleLayers
                .Where(m => !string.IsNullOrEmpty(m.HoseType))
                .Select(m => m.HoseType)
                .Distinct()
                .OrderBy(h => h)
                .ToListAsync();

            ViewBag.HoseTypes = new SelectList(hoseTypes);
            ViewBag.StandardParameterSettings = new SelectList(
                await _context.StandardParameterSettings.Where(s => s.IsActive).Select(s => new { Id = s.Id, Display = "[" + s.ItemList + "] - " + s.HoseType }).ToListAsync(), 
                "Id", 
                "Display"
            );

            return View(new ProductionReport 
            { 
                ProductionDate = DateTime.Today,
                Status = "NOW PRODUCING",
                Yarn = "---"
            });
        }

        // POST: ProductionReport/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductionReport report)
        {
            // Remove validation for fields that are not in Gambar 1
            ModelState.Remove("CreatedBy");
            
            if (ModelState.IsValid)
            {
                try
                {
                    if (report.ProductionDate == default)
                        report.ProductionDate = DateTime.Today;

                    report.CreatedBy = report.CreatedBy ?? "Operator 1";
                    
                    if (string.IsNullOrEmpty(report.Shift))
                    {
                        var shifts = await _context.ShiftMasters.OrderBy(s => s.StartTime).ToListAsync();
                        if (shifts.Any())
                        {
                            var now = DateTime.Now.TimeOfDay;
                            var currentShift = shifts.LastOrDefault(s => TimeSpan.Parse(s.StartTime) <= now) ?? shifts.Last();
                            report.Shift = currentShift.ShiftName;
                        }
                        else
                        {
                            report.Shift = "SHIFT 1";
                        }
                    }
                    report.CustomerName = report.CustomerName ?? "-";
                    report.CreatedDate = DateTime.Now;
                    report.Status = "NOW PRODUCING";

                    // Set default values for NOT NULL columns not present in the form
                    report.InnerMaterialActual = report.InnerMaterialActual ?? report.InnerMaterial ?? "-";
                    report.OuterMaterialActual = report.OuterMaterialActual ?? report.OuterMaterial ?? "-";
                    report.YarnActual = report.YarnActual ?? report.Yarn ?? "-";

                    _context.Add(report);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Produksi Dimulai! Silakan monitor di Dashboard Parameter History.";
                    return RedirectToAction(nameof(ParameterHistory));
                }
                catch (Exception ex)
                {
                    var innerMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    if (ex.InnerException?.InnerException != null) 
                        innerMsg = ex.InnerException.InnerException.Message;

                    Console.WriteLine($"Error saving production report: {innerMsg}");
                    ModelState.AddModelError("", "Gagal Simpan ke Database: " + innerMsg);
                }
            }
            return View(report);
        }

        // GET: ProductionReport/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var report = await _context.ProductionReports.FindAsync(id);
            if (report == null) return NotFound();

            ViewBag.StandardParameterSettings = new SelectList(_context.StandardParameterSettings.OrderByDescending(s => s.CreatedDate), "Id", "DocumentNumber", report.StandardParameterSettingId);

            return View(report);
        }

        // POST: ProductionReport/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductionReport report)
        {
            if (id != report.Id) return NotFound();

            ModelState.Remove("CreatedBy");
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(report);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Data berhasil diupdate!";
                    return RedirectToAction(nameof(ParameterHistory));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductionReportExists(report.Id)) return NotFound();
                    else throw;
                }
            }

            ViewBag.StandardParameterSettings = new SelectList(_context.StandardParameterSettings.OrderByDescending(s => s.CreatedDate), "Id", "DocumentNumber", report.StandardParameterSettingId);
            return View(report);
        }

        // GET: ProductionReport/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var report = await _context.ProductionReports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (report == null) return NotFound();

            return View(report);
        }

        // POST: ProductionReport/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.ProductionReports.FindAsync(id);
            if (report != null)
            {
                _context.ProductionReports.Remove(report);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Laporan berhasil dihapus!";
            }
            
            return RedirectToAction(nameof(ParameterHistory));
        }

        private bool ProductionReportExists(int id)
        {
            return _context.ProductionReports.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> FinishProduction(int id, int qtyOk, int ngDim, int ngVis, 
            decimal? WasteInnerAwal, decimal? WasteInnerAkhir, decimal? WasteCoverAwal, decimal? WasteCoverAkhir)
        {
            var report = await _context.ProductionReports.FindAsync(id);
            if (report != null)
            {
                report.QtyOk = qtyOk;
                report.NgDimension = ngDim;
                report.NgVisual = ngVis;
                report.WasteInnerAwal = WasteInnerAwal;
                report.WasteInnerAkhir = WasteInnerAkhir;
                report.WasteCoverAwal = WasteCoverAwal;
                report.WasteCoverAkhir = WasteCoverAkhir;

                report.Status = "COMPLETED";
                report.ProductionEndTime = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Produksi Selesai!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetMasterByHoseType(string hoseType)
        {
            var master = await _context.MasterlistSpsDoubleLayers
                .FirstOrDefaultAsync(m => m.HoseType == hoseType);
            return Json(master);
        }

        [HttpGet]
        public async Task<IActionResult> GetMasterByItem(string itemCode)
        {
            var master = await _context.MasterlistSpsDoubleLayers
                .FirstOrDefaultAsync(m => m.ItemList == itemCode);
            return Json(master);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStandards(int reportId, string itemCode)
        {
            var report = await _context.ProductionReports.FindAsync(reportId);
            var master = await _context.MasterlistSpsDoubleLayers
                .FirstOrDefaultAsync(m => m.ItemList == itemCode);

            if (report != null && master != null)
            {
                report.HoseType = master.HoseType ?? "";
                report.Dimension = master.Dimensi ?? "";
                report.InnerMaterial = master.InnerTube ?? master.Material ?? "";
                report.OuterMaterial = master.OuterCover ?? "";
                
                // Logic to update or create StandardParameterSetting can be added here
                
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> VerifyApproval(int id, string role, string pin)
        {
            var report = await _context.ProductionReports.FindAsync(id);
            if (report == null) return Json(new { success = false, message = "Report not found" });

            // SIMPLE PIN LOGIC: 4321 for Leader (Diperiksa), 1234 for Foreman (Disetujui)
            // In real app, check against StaffMaster/Users table
            bool isValid = false;
            string staffName = "";

            if (role == "LEADER" && pin == "4321") { isValid = true; staffName = "LEADER PRODUCTION"; }
            else if (role == "FOREMAN" && pin == "1234") { isValid = true; staffName = "FOREMAN EXTRUDER"; }

            if (isValid)
            {
                if (role == "LEADER")
                {
                    report.CheckedBy = staffName;
                    report.CheckedDate = DateTime.Now;
                    report.CheckedBySignature = staffName.ToUpper();
                }
                else
                {
                    report.ApprovedBy = staffName;
                    report.ApprovedDate = DateTime.Now;
                    report.ApprovedBySignature = staffName.ToUpper();
                    report.Status = "COMPLETED";
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, name = staffName, date = DateTime.Now.ToString("dd/MM/yyyy HH:mm") });
            }

            return Json(new { success = false, message = "PIN SALAH!" });
        }

        [HttpPost]
        public async Task<IActionResult> AddReading(ProductionReading reading)
        {
            if (reading.ProductionReportId != 0)
            {
                reading.ReadingTime = DateTime.Now;
                reading.RecordedBy = reading.RecordedBy ?? "Operator 1";
                _context.ProductionReadings.Add(reading);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = reading.ProductionReportId, portal = "parameter" });
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> SaveAppData([FromBody] DimensionReportAppData data)
        {
            try
            {
                ProductionReport? report;
                if (data.ReportId == 0)
                {
                    // Create new report from Scanned App
                    report = new ProductionReport
                    {
                        DocumentNumber = data.DocumentNumber ?? "-",
                        HoseType = data.HoseType ?? "-",
                        Dimension = data.DimensionDisplay ?? "-",
                        CustomerName = data.CustomerName ?? "-",
                        Yarn = data.Yarn ?? "-",
                        ProductionDate = DateTime.Today,
                        CreatedDate = DateTime.Now,
                        Status = "NOW PRODUCING",
                        InnerMaterialActual = "-",
                        OuterMaterialActual = "-",
                        YarnActual = "-",
                        CreatedBy = "Operator 1",
                        Shift = "Shift 1"
                    };
                    _context.ProductionReports.Add(report);
                }
                else
                {
                    report = await _context.ProductionReports.FindAsync(data.ReportId);
                    if (report == null) return Json(new { success = false, message = "Report not found" });
                }

                // Update common dimension readings & production info
                report.VinCode = data.VinCode;
                report.ActualLength = data.ActualLength;
                report.QtyOk = data.QtyOk ?? 0;
                report.NgDimension = data.NgDimension ?? 0;
                report.NgVisual = data.NgVisual ?? 0;
                report.Remark = data.Remark ?? "";

                await _context.SaveChangesAsync();
                return Json(new { success = true, reportId = report.Id, message = "Data saved successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAppReport([FromBody] DimensionReportAppData data)
        {
            try
            {
                var report = await _context.ProductionReports.FindAsync(data.ReportId);
                if (report == null) return Json(new { success = false, message = "Report not found" });

                // Update all data
                report.ActualLength = data.ActualLength;
                report.QtyOk = data.QtyOk ?? 0;
                report.NgDimension = data.NgDimension ?? 0;
                report.NgVisual = data.NgVisual ?? 0;
                report.Remark = data.Remark ?? "";
                
                // Update status
                report.Status = "SUBMITTED";
                report.CheckedBy = "QC Inspector";
                report.ApprovedBy = "Production Manager";

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Report submitted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportAppData(int id)
        {
            var report = await _context.ProductionReports
                .Include(p => p.StandardParameterSetting)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (report == null) return NotFound();

            // Create CSV export
            var csv = $"Document Number,{report.DocumentNumber}\n" +
                     $"Date,{report.ProductionDate:yyyy-MM-dd}\n" +
                     $"Shift,{report.Shift}\n" +
                     $"Customer,{report.CustomerName}\n" +
                     $"Hose Type,{report.HoseType}\n" +
                     $"Dimension,{report.Dimension}\n" +
                     $"Actual Length,{report.ActualLength}\n" +
                     $"Quantity OK,{report.QtyOk}\n" +
                     $"NG Dimension,{report.NgDimension}\n" +
                     $"NG Visual,{report.NgVisual}\n" +
                     $"Remark,{report.Remark}\n";

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", $"ProductionReport_{report.DocumentNumber}_{DateTime.Now:yyyyMMdd}.csv");
        }


        [HttpPost]
        public async Task<IActionResult> UpdateRecap(int reportId, string vinCode, string actualLength, int qtyOk, int ngDim, int ngVis, string remark)
        {
            var report = await _context.ProductionReports.FindAsync(reportId);
            if (report != null)
            {
                if (!string.IsNullOrEmpty(vinCode)) report.VinCode = vinCode;
                if (!string.IsNullOrEmpty(actualLength)) report.ActualLength = actualLength;
                
                report.QtyOk = qtyOk;
                report.NgDimension = ngDim;
                report.NgVisual = ngVis;
                report.Remark = remark;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
