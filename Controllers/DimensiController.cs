using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Controllers
{
    public class DimensiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DimensiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Dimensi/Index
        public async Task<IActionResult> Index()
        {
            // If we want "Dimensi" to show the live measurement immediately,
            // we redirect to the current active dimension report.
            var activeDimensionReport = await _context.DimensionReports
                .Where(d => d.Status == "ACTIVE")
                .OrderByDescending(d => d.CreatedDate)
                .FirstOrDefaultAsync();

            if (activeDimensionReport != null)
            {
                return RedirectToAction(nameof(App), new { id = activeDimensionReport.Id });
            }

            // If no active report, check if there's any active production (NowProducing) 
            // that doesn't have a DimensionReport yet.
            var activeProduction = await _context.NowProducings
                .Where(n => n.ProductionEndTime == null)
                .OrderByDescending(n => n.CreatedDate)
                .FirstOrDefaultAsync();

            if (activeProduction != null)
            {
                // Auto-create a new Dimension Report for this active production
                var newReport = new DimensionReport
                {
                    DocumentNumber = "DIM-" + DateTime.Now.ToString("yyyyMMdd-HHmm"),
                    ProductionDate = activeProduction.ProductionDate,
                    HoseType = activeProduction.HoseType,
                    DimensionDisplay = activeProduction.Dimension,
                    Yarn = activeProduction.Yarn,
                    StandardLength = "100.0", // Defaul or should it be in NowProducing?
                    CustomerName = "-", 
                    Shift = "Shift 1",
                    Status = "ACTIVE",
                    CreatedBy = "QC Operator",
                    CreatedDate = DateTime.Now
                };

                _context.DimensionReports.Add(newReport);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(App), new { id = newReport.Id });
            }

            // If absolutely nothing is happening, show the history list
            var reports = await _context.DimensionReports
                .OrderByDescending(d => d.CreatedDate)
                .Take(20)
                .ToListAsync();

            return View(reports);
        }

        // GET: /Dimensi/App/5
        public async Task<IActionResult> App(int? id)
        {
            if (id == null) return RedirectToAction(nameof(Index));

            var report = await _context.DimensionReports
                .Include(r => r.Measurements)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null) return NotFound();

            // Fetch Masterlist data for standards fallback
            ViewBag.Masterlist = await _context.MasterlistSpsDoubleLayers
                .FirstOrDefaultAsync(m => m.HoseType == report.HoseType);

            return View(report);
        }

        // GET: /Dimensi/History
        public async Task<IActionResult> History()
        {
            var reports = await _context.DimensionReports
                .OrderByDescending(d => d.CreatedDate)
                .ToListAsync();
            return View(reports);
        }

        [HttpPost]
        public async Task<IActionResult> SaveData([FromBody] DimensionReportAppData data)
        {
            try
            {
                var report = await _context.DimensionReports
                    .Include(r => r.Measurements)
                    .FirstOrDefaultAsync(r => r.Id == data.ReportId);

                if (report == null) return Json(new { success = false, message = "Report not found" });

                // Update Report fields
                report.VinCode = data.VinCode ?? "";
                report.ActualLength = data.ActualLength ?? "";
                report.QtyOk = data.QtyOk;
                report.NgDimension = data.NgDimension;
                report.NgVisual = data.NgVisual;
                report.Remark = data.Remark ?? "";

                // Clear and Re-add measurements for simplicity (or update existing)
                _context.DimensionMeasurements.RemoveRange(report.Measurements);
                
                if (data.DimensionReadings != null)
                {
                    foreach (var mData in data.DimensionReadings)
                    {
                        var reading = new DimensionMeasurement
                        {
                            DimensionReportId = report.Id,
                            PointName = mData.PointName,
                            TimeSection = mData.TimeSection,
                            Initial = mData.Initial,
                            R1 = decimal.TryParse(mData.Reading1, out var r1) ? r1 : null,
                            R2 = decimal.TryParse(mData.Reading2, out var r2) ? r2 : null,
                            R3 = decimal.TryParse(mData.Reading3, out var r3) ? r3 : null,
                            R4 = decimal.TryParse(mData.Reading4, out var r4) ? r4 : null,
                            R5 = decimal.TryParse(mData.Reading5, out var r5) ? r5 : null,
                            RecordedTime = DateTime.Now
                        };
                        _context.DimensionMeasurements.Add(reading);
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReport([FromBody] DimensionReportAppData data)
        {
            // First save the latest data
            var saveResult = await SaveData(data);
            if (!(saveResult is JsonResult jr && (bool)((dynamic)jr.Value).success)) 
                return saveResult;

            // Then finalize status
            var report = await _context.DimensionReports.FindAsync(data.ReportId);
            if (report != null)
            {
                report.Status = "COMPLETED";
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true, redirectUrl = "/Dimensi/Index" });
        }
    }
}
