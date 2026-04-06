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
            // Trying to sync with ShiftMaster if available, otherwise using the 07:30/19:30 logic from Management UI
            var shifts = _context.ShiftMasters.ToList();
            var now = DateTime.Now.TimeOfDay;

            if (shifts.Any())
            {
                var sortedShifts = shifts
                    .Select(s => {
                        TimeSpan.TryParse(s.StartTime, out var ts);
                        return new { Name = s.ShiftName, Start = ts };
                    })
                    .OrderBy(s => s.Start)
                    .ToList();

                for (int i = 0; i < sortedShifts.Count; i++)
                {
                    var current = sortedShifts[i];
                    var next = (i + 1 < sortedShifts.Count) ? sortedShifts[i + 1] : sortedShifts[0];

                    if (current.Start < next.Start)
                    {
                        if (now >= current.Start && now < next.Start) return current.Name;
                    }
                    else // Over midnight case
                    {
                        if (now >= current.Start || now < next.Start) return current.Name;
                    }
                }
            }

            // Fallback to the 07:30/19:30 configuration shown in the image
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;
            var totalMinutes = (hour * 60) + minute;

            // Shift 1 start 07:30 (450 mins)
            // Shift 2 start 19:30 (1170 mins)
            if (totalMinutes >= 450 && totalMinutes < 1170) return "Shift 1";
            return "Shift 2";
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
                        itemList = standard.ItemList,
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

        // GET: /Dimensi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var report = await _context.DimensionReports
                .Include(r => r.Measurements)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null) return NotFound();
            return View(report);
        }

        // GET: /Dimensi/Edit/5
        public IActionResult Edit(int id)
        {
            return RedirectToAction(nameof(App), new { id });
        }

        // GET: /Dimensi/Create
        public IActionResult Create()
        {
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var report = await _context.DimensionReports
                .Include(r => r.Measurements)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null) return NotFound();

            if (report.Measurements != null)
            {
                _context.DimensionMeasurements.RemoveRange(report.Measurements);
            }

            _context.DimensionReports.Remove(report);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(History));
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
                        CreatedBy = "QC Operator",
                        Shift = data.Shift ?? GetCurrentShift()
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
                report.Shift = data.Shift ?? report.Shift ?? GetCurrentShift();

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
                            TimeSection = string.IsNullOrWhiteSpace(mData.TimeSection) ? DateTime.Now.ToString("HH:mm") : mData.TimeSection,
                            Frequency = string.IsNullOrWhiteSpace(mData.Frequency) ? "30m Sekali" : mData.Frequency,
                            StandardDimension = mData.StandardDimension,
                            Initial = mData.Initial,
                            R1 = decimal.TryParse(mData.Reading1, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var r1) ? r1 : null,
                            R2 = decimal.TryParse(mData.Reading2, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var r2) ? r2 : null,
                            R3 = decimal.TryParse(mData.Reading3, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var r3) ? r3 : null,
                            R4 = decimal.TryParse(mData.Reading4, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var r4) ? r4 : null,
                            R5 = decimal.TryParse(mData.Reading5, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var r5) ? r5 : null,
                            Status = string.IsNullOrWhiteSpace(mData.Status) ? "OK" : mData.Status,
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
            if (saveResult is JsonResult { Value: { } } jr && (bool)((dynamic)jr.Value).success) {
                // success
            } else {
                return saveResult;
            }

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
