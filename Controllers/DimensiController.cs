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
        public IActionResult Index()
        {
            // Just return a blank report view
            var report = new DimensionReport
            {
                DocumentNumber = "",
                ProductionDate = DateTime.Now,
                Shift = GetCurrentShift(),
                Status = "DRAFT"
            };
            return View("App", report);
        }

        private string GetCurrentShift()
        {
            var hour = DateTime.Now.Hour;
            if (hour >= 7 && hour < 15) return "Shift 1";
            if (hour >= 15 && hour < 23) return "Shift 2";
            return "Shift 3";
        }

        [HttpGet]
        public async Task<JsonResult> GetSpsStandard(string hoseType)
        {
            if (string.IsNullOrEmpty(hoseType)) return Json(new { success = false, message = "Input empty" });

            // Search across multiple identifier fields
            var standard = await _context.MasterlistSpsDoubleLayers
                .FirstOrDefaultAsync(m => 
                    m.HoseType == hoseType || 
                    m.ExcelId == hoseType || 
                    m.DocumentNumber == hoseType || 
                    m.ItemList == hoseType ||
                    (m.HoseType != null && m.HoseType.Contains(hoseType))
                );
            
            if (standard != null)
            {
                return Json(new { 
                    success = true, 
                    data = new {
                        hoseType = standard.HoseType,
                        dimensi = standard.Dimensi,
                        innerTube = standard.InnerTube,
                        outerCover = standard.OuterCover,
                        toleranceInner = standard.ToleranceInner,
                        toleranceOuter = standard.ToleranceOuter,
                        tebalInner = standard.TebalInner,
                        tebalTotal = standard.TebalTotal,
                        yarn = "ARAMID", 
                        customer = standard.Customer,
                        docNo = standard.DocumentNumber,
                        excelId = standard.ExcelId,
                        bypass = "2.0 mm" 
                    }
                });
            }
            return Json(new { success = false, message = "Standard not found in SPS Masterlist: " + hoseType });
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
                DimensionReport? report;
                if (data.ReportId > 0)
                {
                    report = await _context.DimensionReports
                        .Include(r => r.Measurements)
                        .FirstOrDefaultAsync(r => r.Id == data.ReportId);
                }
                else
                {
                    // Create new report if it doesn't exist
                    report = new DimensionReport
                    {
                        DocumentNumber = "DIM-" + DateTime.Now.ToString("yyyyMMdd-HHmm"),
                        ProductionDate = DateTime.Now,
                        Status = "ACTIVE",
                        CreatedDate = DateTime.Now,
                        CreatedBy = "QC Operator"
                    };
                    _context.DimensionReports.Add(report);
                    await _context.SaveChangesAsync();
                }

                if (report == null) return Json(new { success = false, message = "Report not found" });

                // Update Report fields
                report.HoseType = data.HoseType ?? "";
                report.DimensionDisplay = data.DimensionDisplay ?? "";
                report.VinCode = data.VinCode ?? "";
                report.ActualLength = data.ActualLength ?? "";
                report.QtyOk = data.QtyOk;
                report.NgDimension = data.NgDimension;
                report.NgVisual = data.NgVisual;
                report.Remark = data.Remark ?? "";
                report.ByPass = data.ByPass ?? "";
                report.CustomerName = data.CustomerName ?? "-";
                report.Yarn = data.Yarn ?? "";

                // Clear and Re-add measurements for simplicity
                if (report.Measurements != null)
                {
                    _context.DimensionMeasurements.RemoveRange(report.Measurements);
                }
                
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
                return Json(new { success = true, reportId = report.Id });
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
