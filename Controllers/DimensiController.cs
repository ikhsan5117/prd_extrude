using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;
using System.Text.Json;

using VelastoProductionSystem.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace VelastoProductionSystem.Controllers
{
    public class DimensiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ElwpDbContext _elwpContext;
        private readonly IHubContext<DashboardHub> _hubContext;

        public DimensiController(ApplicationDbContext context, ElwpDbContext elwpContext, IHubContext<DashboardHub> hubContext)
        {
            _context = context;
            _elwpContext = elwpContext;
            _hubContext = hubContext;
        }

        // GET: /Dimensi/Index
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToAction("Login", "Account");
            }

            // Ambil Kode Item dari ELWP langsung (hari ini), agar selalu up-to-date
            var today = DateTime.Today;
            ViewBag.ItemCodes = _elwpContext.ElwpPlannings
                .Where(p => !string.IsNullOrEmpty(p.KodeItem)
                         && p.TanggalPlanning >= today
                         && p.TanggalPlanning < today.AddDays(1)
                         && p.AreaId == 1)
                .Select(p => p.KodeItem)
                .Distinct()
                .OrderBy(p => p)
                .ToList();

            var report = new DimensionReport
            {
                DocumentNumber = "",
                ProductionDate = DateTime.Now,
                Shift = GetCurrentShift(),
                Status = "DRAFT",
                CreatedBy = HttpContext.Session.GetString("UserName") ?? "QC Operator",
                MachineName = HttpContext.Session.GetString("MachineName")
            };
            return View("App", report);
        }

        private string GetCurrentShift()
        {
            try
            {
                var shifts = _context.ShiftMasters.ToList();
                if (shifts == null || !shifts.Any()) 
                {
                    return DateTime.Now.Hour >= 7 && DateTime.Now.Hour < 19 ? "Shift 1" : "Shift 2";
                }

                var now = DateTime.Now.TimeOfDay;

                var shiftInfos = shifts.Select(s => {
                    TimeSpan.TryParse(s.StartTime ?? "07:30", out var start);
                    return new { Name = s.ShiftName ?? "Shift", Start = start };
                }).OrderBy(s => s.Start).ToList();

                for (int i = 0; i < shiftInfos.Count; i++)
                {
                    var current = shiftInfos[i];
                    var next = (i + 1 < shiftInfos.Count) ? shiftInfos[i + 1] : shiftInfos[0];

                    if (current.Start < next.Start) {
                        if (now >= current.Start && now < next.Start) return current.Name;
                    } else {
                        if (now >= current.Start || now < next.Start) return current.Name;
                    }
                }
                return shiftInfos.FirstOrDefault()?.Name ?? "Shift 1";
            }
            catch { return "Shift 1"; }
        }

        [HttpGet]
        public async Task<JsonResult> GetSpsStandard(string hoseType, string? machineCode)
        {
            if (string.IsNullOrEmpty(hoseType)) return Json(new { success = false, message = "Input empty" });

            var sanitized = hoseType.Trim().ToUpper();
            var sanitizedMachine = string.IsNullOrEmpty(machineCode) ? null : machineCode.Trim().ToUpper();

            // 1. Try MasterlistSpsDoubleLayers FIRST - this contains the full names like VHFUNC-...
            var query = _context.MasterlistSpsDoubleLayers.AsQueryable();
            
            if (!string.IsNullOrEmpty(sanitizedMachine))
            {
                // filter by machine if provided
                query = query.Where(m => m.MachineCode != null && m.MachineCode.ToUpper() == sanitizedMachine);
            }

            var standard = await query.FirstOrDefaultAsync(m => 
                (m.HoseType != null && m.HoseType.Trim().ToUpper() == sanitized) || 
                (m.ItemList != null && m.ItemList.Trim().ToUpper() == sanitized) ||
                (m.ExcelId != null && m.ExcelId.Trim().ToUpper() == sanitized) || 
                (m.DocumentNumber != null && m.DocumentNumber.Trim().ToUpper() == sanitized) ||
                (m.ItemList != null && m.ItemList.ToUpper().Contains(sanitized)) ||
                (m.HoseType != null && m.HoseType.ToUpper().Contains(sanitized))
            );
            
            // Fallback: If no machine-specific SPS found, search without machine filter
            if (standard == null && !string.IsNullOrEmpty(sanitizedMachine))
            {
                standard = await _context.MasterlistSpsDoubleLayers.FirstOrDefaultAsync(m => 
                    (m.HoseType != null && m.HoseType.Trim().ToUpper() == sanitized) || 
                    (m.ItemList != null && m.ItemList.Trim().ToUpper() == sanitized) ||
                    (m.ExcelId != null && m.ExcelId.Trim().ToUpper() == sanitized) || 
                    (m.DocumentNumber != null && m.DocumentNumber.Trim().ToUpper() == sanitized) ||
                    (m.ItemList != null && m.ItemList.ToUpper().Contains(sanitized)) ||
                    (m.HoseType != null && m.HoseType.ToUpper().Contains(sanitized))
                );
            }

            if (standard != null)
            {
                return Json(new { 
                    success = true, 
                    data = new {
                        hoseType = standard.HoseType,
                        dimensi = standard.Dimensi,
                        cuttingLength = standard.CuttingSpeed, // Map CuttingSpeed to length if applicable
                        innerTube = standard.InnerTube,
                        outerCover = standard.OuterCover,
                        toleranceInner = standard.ToleranceInner,
                        toleranceOuter = standard.ToleranceOuter,
                        tebalInner = standard.TebalInner,
                        tebalInnerMiddle = standard.TebalInnerMiddle,
                        tebalTotal = standard.TebalTotal,
                        yarn = standard.Material ?? "ARAMID", 
                        customer = standard.Customer,
                        docNo = standard.DocumentNumber,
                        revNo = standard.RevisionNumber,
                        excelId = standard.ExcelId,
                        itemList = standard.ItemList,
                        toleranceSpiralPitch = standard.ToleranceSpiralPitch,
                        bypass = "2.0 mm" 
                    }
                });
            }

            // 2. Fallback to StandardParameterSettings (SPS Parameter)
            var sps = await _context.StandardParameterSettings
                .FirstOrDefaultAsync(s => 
                    (s.ItemList != null && s.ItemList.Trim().ToUpper() == sanitized) || 
                    (s.ProductCode != null && s.ProductCode.Trim().ToUpper() == sanitized) ||
                    (s.HoseType != null && s.HoseType.Trim().ToUpper() == sanitized) ||
                    (s.ItemList != null && s.ItemList.ToUpper().Contains(sanitized)) ||
                    (s.HoseType != null && s.HoseType.ToUpper().Contains(sanitized)) ||
                    (s.ProductCode != null && s.ProductCode.ToUpper().Contains(sanitized)));

            if (sps != null)
            {
                return Json(new { 
                    success = true, 
                    data = new {
                        hoseType = sps.HoseType,
                        dimensi = sps.Diameter,
                        innerTube = sps.InnerMaterial,
                        outerCover = sps.OuterMaterial,
                        toleranceInner = sps.ToleranceDie.ToString("G29"),
                        toleranceOuter = sps.Tol_CoverDie.ToString("G29"),
                        tebalInner = sps.TubeDie.ToString("G29"),
                        tebalTotal = sps.CoverDie.ToString("G29"),
                        yarn = sps.YarnType ?? "GENERAL",
                        customer = sps.CustomerName,
                        docNo = sps.DocumentNumber,
                        revNo = sps.RevisionNumber,
                        itemList = sps.ItemList,
                        toleranceSpiralPitch = sps.SpiralPitch.ToString("G29"),
                        bypass = "2.0 mm" 
                    }
                });
            }

            return Json(new { success = false, message = "Standard not found: " + hoseType });
        }

        public IActionResult App()
        {
            return View(new DimensionReport { Measurements = new List<DimensionMeasurement>() });
        }

        public async Task<IActionResult> History()
        {
            var reports = await _context.DimensionReports
                .OrderByDescending(d => d.CreatedDate)
                .ToListAsync();
            return View(reports);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var report = await _context.DimensionReports
                .Include(r => r.Measurements)
                .Include(r => r.Summaries)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (report == null) return NotFound();

            ViewBag.Sps = await _context.StandardParameterSettings
                .FirstOrDefaultAsync(s => s.ProductCode == report.HoseType);
            ViewBag.Master = await _context.MasterlistSpsDoubleLayers
                .FirstOrDefaultAsync(m => m.HoseType == report.HoseType || m.ItemList == report.HoseType);

            return View(report);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var report = await _context.DimensionReports
                .Include(r => r.Measurements)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (report == null) return NotFound();
            return View("Edit", report);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var report = await _context.DimensionReports
                .Include(r => r.Measurements)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (report != null)
            {
                if (report.Measurements != null) _context.DimensionMeasurements.RemoveRange(report.Measurements);
                _context.DimensionReports.Remove(report);
                await _context.SaveChangesAsync();

                // Notify via SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveUpdate");
            }
            return RedirectToAction(nameof(History));
        }

        [HttpPost]
        public async Task<IActionResult> SaveData()
        {
            try {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                var data = JsonSerializer.Deserialize<DimensionReportAppData>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (data == null) return Json(new { success = false, message = "JSON Error" });
                
                var result = await ProcessSaveInternal(data);
                
                // Notify via SignalR for real-time dashboard updates
                await _hubContext.Clients.All.SendAsync("ReceiveUpdate");
                
                return Json(result);
            } catch (Exception ex) {
                return Json(new { success = false, message = "System Error: " + ex.Message });
            }
        }

        private async Task<object> ProcessSaveInternal(DimensionReportAppData data)
        {
            try
            {
                DimensionReport? report = null;
                if (data.ReportId > 0)
                {
                    report = await _context.DimensionReports
                        .Include(r => r.Measurements)
                        .FirstOrDefaultAsync(r => r.Id == data.ReportId);
                }

                if (report == null)
                {
                    report = new DimensionReport
                    {
                        DocumentNumber = "DIM-" + DateTime.Now.ToString("yyyyMMdd-HHmm"),
                        ProductionDate = DateTime.Now,
                        Status = "ACTIVE",
                        CreatedDate = DateTime.Now,
                        CreatedBy = HttpContext.Session.GetString("UserName") ?? "QC Operator",
                        MachineName = HttpContext.Session.GetString("MachineName"),
                        Shift = !string.IsNullOrEmpty(data.Shift) ? data.Shift : GetCurrentShift()
                    };
                    _context.DimensionReports.Add(report);
                    await _context.SaveChangesAsync();
                }

                report.HoseType = data.HoseType ?? "";
                report.DimensionDisplay = data.DimensionDisplay ?? "";
                report.VinCode = data.VinCode ?? "";
                report.StandardLength = data.StandardLength ?? "";
                report.ActualLength = data.ActualLength ?? "";
                report.QtyTarget = data.QtyTarget ?? 0;
                report.QtyOk = data.QtyOk ?? 0;
                report.NgDimension = data.NgDimension ?? 0;
                report.NgVisual = data.NgVisual ?? 0;
                report.Remark = data.Remark ?? "";
                report.ByPass = data.ByPass ?? "";
                report.CustomerName = data.CustomerName ?? "-";
                report.Yarn = data.Yarn ?? "";
                report.Shift = !string.IsNullOrEmpty(data.Shift) ? data.Shift : (report.Shift ?? GetCurrentShift());

                var existing = _context.DimensionMeasurements.Where(m => m.DimensionReportId == report.Id).ToList();
                if (existing.Any()) _context.DimensionMeasurements.RemoveRange(existing);
                
                if (data.DimensionReadings != null)
                {
                    foreach (var mData in data.DimensionReadings)
                    {
                        if (mData == null) continue;
                        var reading = new DimensionMeasurement
                        {
                            DimensionReportId = report.Id,
                            PointName = mData.PointName ?? "Point",
                            TimeSection = string.IsNullOrWhiteSpace(mData.TimeSection) ? DateTime.Now.ToString("HH:mm") : mData.TimeSection,
                            Frequency = string.IsNullOrWhiteSpace(mData.Frequency) ? "30m Sekali" : mData.Frequency,
                            StandardDimension = mData.StandardDimension ?? "-",
                            Initial = mData.Initial ?? "",
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

                // Process Summaries
                var existingSummaries = _context.DimensionSummaries.Where(s => s.DimensionReportId == report.Id).ToList();
                if (existingSummaries.Any()) _context.DimensionSummaries.RemoveRange(existingSummaries);

                if (data.ProductionDataSummaries != null)
                {
                    foreach (var sData in data.ProductionDataSummaries)
                    {
                        if (sData == null) continue;
                        var summary = new DimensionSummary
                        {
                            DimensionReportId = report.Id,
                            PartNumber = sData.PartNumber,
                            VinCode = sData.VinCode,
                            StandardLength = sData.StandardLength,
                            ActualLength = sData.ActualLength,
                            QtyTarget = sData.QtyTarget ?? 0,
                            QtyOk = sData.QtyOk ?? 0,
                            NgDimension = sData.NgDimension ?? 0,
                            NgVisual = sData.NgVisual ?? 0
                        };
                        _context.DimensionSummaries.Add(summary);
                    }
                }

                await _context.SaveChangesAsync();
                return new { success = true, reportId = report.Id };
            }
            catch (Exception ex)
            {
                return new { success = false, message = "Internal Error: " + ex.Message };
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReport()
        {
            try {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                var data = JsonSerializer.Deserialize<DimensionReportAppData>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (data == null) return Json(new { success = false, message = "JSON Error" });

                var saveResult = await ProcessSaveInternal(data);
                if (saveResult is { } sr && (bool)((dynamic)sr).success) 
                {
                    var report = await _context.DimensionReports.FindAsync(data.ReportId);
                    if (report != null)
                    {
                        report.Status = "COMPLETED";
                        await _context.SaveChangesAsync();
                        
                        // Notify via SignalR
                        await _hubContext.Clients.All.SendAsync("ReceiveUpdate");
                    }
                    return Json(new { success = true, redirectUrl = "/Dimensi/Index" });
                }
                return Json(saveResult);
            } catch (Exception ex) {
                return Json(new { success = false, message = "Submit Error: " + ex.Message });
            }
        }
    }
}
