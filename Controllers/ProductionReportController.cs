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

        // GET: ProductionReport/App (New Tablet App Interface)
        public async Task<IActionResult> App(int? id)
        {
            if (id == null)
            {
                // If no ID is specified, redirect to the latest active report
                var latestReport = await _context.ProductionReports
                    .Where(pr => pr.Status == "NOW PRODUCING")
                    .OrderByDescending(pr => pr.CreatedDate)
                    .FirstOrDefaultAsync();

                if (latestReport != null)
                {
                    return RedirectToAction(nameof(App), new { id = latestReport.Id });
                }

                // If no active report, find any latest report
                latestReport = await _context.ProductionReports
                    .OrderByDescending(pr => pr.CreatedDate)
                    .FirstOrDefaultAsync();

                if (latestReport != null)
                {
                    return RedirectToAction(nameof(App), new { id = latestReport.Id });
                }

                return NotFound("Belum ada laporan produksi yang tersedia.");
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

            return View(new ProductionReport 
            { 
                ProductionDate = DateTime.Today,
                DocumentNumber = "VI-SOP-PROD-131",
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
                    report.Shift = report.Shift ?? "Shift 1";
                    report.CustomerName = report.CustomerName ?? "-";
                    report.CreatedDate = DateTime.Now;
                    report.Status = "NOW PRODUCING";

                    // Set default values for NOT NULL columns not present in the form
                    report.InnerMaterialActual = report.InnerMaterialActual ?? report.InnerMaterial ?? "-";
                    report.OuterMaterialActual = report.OuterMaterialActual ?? report.OuterMaterial ?? "-";
                    report.YarnActual = report.YarnActual ?? report.Yarn ?? "-";

                    _context.Add(report);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Produksi Dimulai! Silakan monitor di Dashboard.";
                    return RedirectToAction(nameof(Details), new { id = report.Id });
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
                    return RedirectToAction(nameof(Index));
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
                var report = await _context.ProductionReports.FindAsync(data.ReportId);
                if (report == null) return Json(new { success = false, message = "Report not found" });

                // Update dimension readings
                report.ActualLength = data.ActualLength;
                report.QtyOk = data.QtyOk;
                report.NgDimension = data.NgDimension;
                report.NgVisual = data.NgVisual;
                report.Remark = data.Remark ?? "";

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Data saved successfully" });
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
                report.QtyOk = data.QtyOk;
                report.NgDimension = data.NgDimension;
                report.NgVisual = data.NgVisual;
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
        public async Task<IActionResult> UpdateRecap(int reportId, string vinCode, int qtyOk, int ngDim, int ngVis, string remark)
        {
            var report = await _context.ProductionReports.FindAsync(reportId);
            if (report != null)
            {
                report.VinCode = vinCode;
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
