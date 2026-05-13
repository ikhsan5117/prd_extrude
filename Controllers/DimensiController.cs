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
                         && p.TanggalPlanning < today.AddDays(1))
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
                ItemCode = "",
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

            IQueryable<MasterlistSpsDoubleLayer> ApplyMasterlistMatch(IQueryable<MasterlistSpsDoubleLayer> q)
            {
                return q
                    .Where(m =>
                        (m.HoseType != null && m.HoseType.Trim().ToUpper() == sanitized) ||
                        (m.ItemList != null && m.ItemList.Trim().ToUpper() == sanitized) ||
                        (m.ExcelId != null && m.ExcelId.Trim().ToUpper() == sanitized) ||
                        (m.DocumentNumber != null && m.DocumentNumber.Trim().ToUpper() == sanitized) ||
                        (m.ItemList != null && m.ItemList.ToUpper().Contains(sanitized)) ||
                        (m.HoseType != null && m.HoseType.ToUpper().Contains(sanitized))
                    )
                    .OrderByDescending(m => m.ItemList != null && m.ItemList.Trim().ToUpper() == sanitized)
                    .ThenByDescending(m => m.HoseType != null && m.HoseType.Trim().ToUpper() == sanitized)
                    .ThenByDescending(m => m.ExcelId != null && m.ExcelId.Trim().ToUpper() == sanitized)
                    .ThenByDescending(m => m.DocumentNumber != null && m.DocumentNumber.Trim().ToUpper() == sanitized)
                    .ThenByDescending(m => m.ItemList != null && m.ItemList.ToUpper().Contains(sanitized))
                    .ThenByDescending(m => m.HoseType != null && m.HoseType.ToUpper().Contains(sanitized))
                    .ThenByDescending(m => m.Id);
            }

            var standard = await ApplyMasterlistMatch(query).FirstOrDefaultAsync();
            
            // Fallback: If no machine-specific SPS found, search without machine filter
            if (standard == null && !string.IsNullOrEmpty(sanitizedMachine))
            {
                standard = await ApplyMasterlistMatch(_context.MasterlistSpsDoubleLayers).FirstOrDefaultAsync();
            }

            if (standard != null)
            {
                return Json(new { 
                    success = true, 
                    data = new {
                        hoseType = standard.HoseType,
                        dimensi = standard.Dimensi,
                        cuttingLength = standard.CuttingSpeed, 
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
                        spiralPitchSetting = standard.SpiralPitchSetting,
                        spiralPitchDisplay = standard.SpiralPitchDisplay,
                        innerTol = standard.InnerTol,
                        thickTol = standard.ThickTol,
                        totalTol = standard.TotalTol,
                        bypass = "2.0 mm" 
                    }
                });
            }

            // 2. Fallback to SpsMasters (SPS Parameter)
            var sps = await _context.SpsMasters
                .FirstOrDefaultAsync(s => 
                    (s.ItemList != null && s.ItemList.Trim().ToUpper() == sanitized) || 
                    (s.HoseType != null && s.HoseType.Trim().ToUpper() == sanitized) ||
                    (s.ItemList != null && s.ItemList.ToUpper().Contains(sanitized)) ||
                    (s.HoseType != null && s.HoseType.ToUpper().Contains(sanitized)));

            if (sps != null)
            {
                return Json(new { 
                    success = true, 
                    data = new {
                        hoseType = sps.HoseType,
                        dimensi = sps.Dimensi,
                        innerTube = sps.InnerTube,
                        outerCover = sps.OuterCover,
                        toleranceInner = sps.InnerTol,
                        toleranceOuter = sps.TotalTol,
                        tebalInner = sps.InnerTarget,
                        tebalTotal = sps.TotalTarget,
                        yarn = sps.Yarn ?? "GENERAL",
                        customer = sps.Customer,
                        docNo = sps.DocumentNumber,
                        revNo = sps.RevisionNumber,
                        itemList = sps.ItemList,
                        toleranceSpiralPitch = sps.ToleranceSpiralPitch,
                        spiralPitchSetting = sps.SpiralPitchSetting,
                        spiralPitchDisplay = sps.SpiralPitchDisplay,
                        innerTol = sps.InnerTol,
                        thickTol = sps.ThickTol,
                        totalTol = sps.TotalTol,
                        bypass = "2.0 mm" 
                    }
                });
            }

            return Json(new { success = false, message = "Standard not found: " + hoseType });
        }

        public IActionResult App()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToAction("Login", "Account");
            }
            return View(new DimensionReport { Measurements = new List<DimensionMeasurement>() });
        }

        public async Task<IActionResult> History()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToAction("Login", "Account");
            }
            var reports = await _context.DimensionReports
                .OrderByDescending(d => d.CreatedDate)
                .ToListAsync();
            return View(reports);
        }

        [HttpGet]
        public async Task<IActionResult> ExportHistoryExcel(string? searchType = null, string? query = null, string? startDate = null, string? endDate = null)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToAction("Login", "Account");
            }

            var q = _context.DimensionReports
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(startDate) && DateTime.TryParse(startDate, out var sDate))
            {
                q = q.Where(r => r.CreatedDate >= sDate.Date);
            }

            if (!string.IsNullOrWhiteSpace(endDate) && DateTime.TryParse(endDate, out var eDate))
            {
                q = q.Where(r => r.CreatedDate <= eDate.Date.AddDays(1).AddTicks(-1));
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                var s = query.Trim().ToUpper();
                var mode = (searchType ?? "").Trim().ToLower();

                if (mode == "planning")
                {
                    q = q.Where(r =>
                        (r.ItemCode != null && r.ItemCode.ToUpper().Contains(s))
                    );
                }
                else if (mode == "operator")
                {
                    q = q.Where(r => r.CreatedBy != null && r.CreatedBy.ToUpper().Contains(s));
                }
                else if (mode == "docvin")
                {
                    q = q.Where(r =>
                        (r.DocumentNumber != null && r.DocumentNumber.ToUpper().Contains(s)) ||
                        (r.VinCode != null && r.VinCode.ToUpper().Contains(s))
                    );
                }
                else
                {
                    q = q.Where(r =>
                        (r.DocumentNumber != null && r.DocumentNumber.ToUpper().Contains(s)) ||
                        (r.VinCode != null && r.VinCode.ToUpper().Contains(s)) ||
                        (r.ItemCode != null && r.ItemCode.ToUpper().Contains(s)) ||
                        (r.CreatedBy != null && r.CreatedBy.ToUpper().Contains(s)) ||
                        (r.HoseType != null && r.HoseType.ToUpper().Contains(s))
                    );
                }
            }

            var rows = await q
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();

            using var package = new OfficeOpenXml.ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Dimension History");

            ws.Cells[1, 1].Value = "DIMENSION HISTORY EXPORT";
            ws.Cells[1, 1, 1, 11].Merge = true;
            ws.Cells[1, 1].Style.Font.Bold = true;
            ws.Cells[1, 1].Style.Font.Size = 14;
            ws.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(15, 23, 42));
            ws.Cells[1, 1].Style.Font.Color.SetColor(System.Drawing.Color.White);

            ws.Cells[2, 1].Value = $"Exported at: {DateTime.Now:dd MMM yyyy HH:mm} | Total rows: {rows.Count}";
            ws.Cells[2, 1, 2, 11].Merge = true;
            ws.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells[2, 1].Style.Font.Color.SetColor(System.Drawing.Color.Gray);

            var headers = new[]
            {
                "Document", "Date", "Planning", "Input By", "Machine", "Product Info", "Passed", "Failed", "VIN", "Shift", "Customer"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cells[4, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.Color.SetColor(System.Drawing.Color.White);
                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(30, 64, 175));
                cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.FromArgb(59, 130, 246));
                cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }

            int row = 5;
            foreach (var item in rows)
            {
                ws.Cells[row, 1].Value = (item.DocumentNumber ?? "").Replace("#", "");
                ws.Cells[row, 2].Value = item.CreatedDate.ToString("dd MMM yyyy HH:mm");
                var planningDisplay = (item.ItemCode ?? string.Empty).Trim();
                var productInfoDisplay = (item.HoseType ?? string.Empty).Trim();
                ws.Cells[row, 3].Value = string.IsNullOrWhiteSpace(planningDisplay) ? "-" : planningDisplay;
                ws.Cells[row, 4].Value = item.CreatedBy ?? "SYSTEM";
                ws.Cells[row, 5].Value = item.MachineName ?? "-";
                ws.Cells[row, 6].Value = string.IsNullOrWhiteSpace(productInfoDisplay) ? "UNDEFINED_PART" : productInfoDisplay;
                ws.Cells[row, 7].Value = item.QtyOk;
                ws.Cells[row, 8].Value = item.NgDimension;
                ws.Cells[row, 9].Value = item.VinCode ?? "-";
                ws.Cells[row, 10].Value = item.Shift ?? "-";
                ws.Cells[row, 11].Value = item.CustomerName ?? "UNKNOWN_CLIENT";

                for (int col = 1; col <= 11; col++)
                {
                    ws.Cells[row, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Hair, System.Drawing.Color.FromArgb(148, 163, 184));
                }

                row++;
            }

            ws.Column(1).Width = 24;
            ws.Column(2).Width = 20;
            ws.Column(3).Width = 14;
            ws.Column(4).Width = 16;
            ws.Column(5).Width = 14;
            ws.Column(6).Width = 28;
            ws.Column(7).Width = 12;
            ws.Column(8).Width = 12;
            ws.Column(9).Width = 16;
            ws.Column(10).Width = 10;
            ws.Column(11).Width = 20;

            var fileName = $"DimensionHistory_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(
                package.GetAsByteArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToAction("Login", "Account");
            }
            if (id == null) return NotFound();
            var report = await _context.DimensionReports
                .Include(r => r.Measurements)
                .Include(r => r.Summaries)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (report == null) return NotFound();

            // Fetch specific SPS record. Prioritize SpsId saved in the report.
            if (report.SpsId > 0)
            {
                ViewBag.Sps = await _context.SpsMasters.FirstOrDefaultAsync(s => s.Id == report.SpsId);
            }

            // Fallback: If SpsId lookup failed or is 0, match by ItemCode + DocNumber for accuracy
            if (ViewBag.Sps == null)
            {
                ViewBag.Sps = await _context.SpsMasters
                    .OrderByDescending(s => s.Id)
                    .FirstOrDefaultAsync(s => s.ItemList == report.ItemCode && (report.DocumentNumber == null || s.DocumentNumber == report.DocumentNumber));
            }

            // Fetch Masterlist data as fallback - Match by ItemCode/DocNumber for better accuracy than just HoseType
            ViewBag.Master = await _context.MasterlistSpsDoubleLayers
                .OrderByDescending(m => m.Id)
                .FirstOrDefaultAsync(m => (m.ItemList != null && m.ItemList.Contains(report.ItemCode ?? "")) || m.DocumentNumber == report.DocumentNumber);

            return View(report);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToAction("Login", "Account");
            }
            var report = await _context.DimensionReports
                .Include(r => r.Measurements)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (report == null) return NotFound();

            // Fetch specific SPS record. Prioritize SpsId saved in the report.
            if (report.SpsId > 0)
            {
                ViewBag.Sps = await _context.SpsMasters.FirstOrDefaultAsync(s => s.Id == report.SpsId);
            }

            if (ViewBag.Sps == null)
            {
                ViewBag.Sps = await _context.SpsMasters
                    .OrderByDescending(s => s.Id)
                    .FirstOrDefaultAsync(s => s.ItemList == report.ItemCode && (report.DocumentNumber == null || s.DocumentNumber == report.DocumentNumber));
            }

            // Fetch Masterlist data as fallback
            ViewBag.Master = await _context.MasterlistSpsDoubleLayers
                .OrderByDescending(m => m.Id)
                .FirstOrDefaultAsync(m => (m.ItemList != null && m.ItemList.Contains(report.ItemCode ?? "")) || m.DocumentNumber == report.DocumentNumber);

            return View("Edit", report);
        }

        // AJAX API: Search SPS Items for Autocomplete
        [HttpGet]
        public async Task<IActionResult> SearchSpsItems(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return Json(new List<string>());
            
            var sanitizedQuery = query.Trim().ToUpper();
            
            // Get unique ItemList values from both tables
            var spsItems = await _context.SpsMasters
                .Where(s => s.ItemList != null && s.ItemList.ToUpper().Contains(sanitizedQuery))
                .Select(s => new { ItemList = s.ItemList, HoseType = s.HoseType })
                .Distinct()
                .Take(10)
                .ToListAsync();
            
            var masterItems = await _context.MasterlistSpsDoubleLayers
                .Where(m => m.ItemList != null && m.ItemList.ToUpper().Contains(sanitizedQuery))
                .Select(m => new { ItemList = m.ItemList, HoseType = m.HoseType })
                .Distinct()
                .Take(10)
                .ToListAsync();
            
            // Combine and deduplicate
            var results = spsItems.Concat(masterItems)
                .GroupBy(x => x.ItemList)
                .Select(g => g.First())
                .OrderBy(x => x.ItemList)
                .Take(10)
                .Select(x => new { 
                    itemCode = x.ItemList, 
                    hoseType = x.HoseType ?? "" 
                })
                .ToList();
            
            return Json(results);
        }

        // AJAX API: Get SPS by Item Code (Scanner)
        [HttpGet]
        public async Task<IActionResult> GetSpsByItem(string itemCode)
        {
            if (string.IsNullOrWhiteSpace(itemCode)) return BadRequest();
            
            var sanitizedCode = itemCode.Trim().ToUpper();
            
            // Use SpsMasters for detailed lookup
            var sps = await _context.SpsMasters
                .FirstOrDefaultAsync(s => 
                    (s.ItemList != null && s.ItemList.ToUpper().Contains(sanitizedCode)));
            
            if (sps != null) return Json(sps);

            // Fallback to MasterlistSpsDoubleLayers (Brief)
            var master = await _context.MasterlistSpsDoubleLayers
                .Where(m => m.ItemList != null && m.ItemList.ToUpper().Contains(sanitizedCode))
                .OrderByDescending(m => m.Id)
                .FirstOrDefaultAsync();

            if (master != null)
            {
                return Json(new {
                    Id = (int?)null,
                    Source = "Masterlist",
                    MasterlistId = master.Id,
                    ItemList = master.ItemList,
                    HoseType = master.HoseType,
                    CustomerName = master.Customer,
                    DocumentNumber = master.DocumentNumber,
                    InnerMaterial = master.InnerTube,
                    MiddleMaterial = master.MiddleTube,
                    OuterMaterial = master.OuterCover,
                    YarnType = master.Material,
                    LayerType = !string.IsNullOrWhiteSpace(master.MiddleTube) ? "CHS 3 Layer" : "CHS 2 Layer",
                    InnerDie = master.Nipple,
                    TubeDie = master.TubeDie,
                    MiddleDie = master.MiddleDie,
                    CoverDie = master.CoverDie,
                    SpacerDie = master.SpacerDie,
                    ToleranceDie = master.ADistance
                });
            }

            return Json(new { success = false, message = "Protocol Not Found" });
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
                Console.WriteLine($">>> SIGNALR: Broadcasting 'ReceiveUpdate' from Dimensi module at {DateTime.Now}");
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

                // Log SpsId before update
                Console.WriteLine($"🔍 DEBUG - Before Update: SpsId = {report.SpsId}, Incoming = {data.SpsId}");
                
                report.HoseType = data.HoseType ?? "";
                report.ItemCode = data.ItemCode ?? "";
                report.SpsId = data.SpsId;
                report.MachineName = data.MachineName ?? report.MachineName;
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
                
                // Explicitly mark entity as modified to ensure EF tracks changes
                _context.Entry(report).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                
                // Log SpsId after update
                Console.WriteLine($"💾 DEBUG - After Update: SpsId = {report.SpsId}, State = {_context.Entry(report).State}");

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
                            R6 = decimal.TryParse(mData.Reading6, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var r6) ? r6 : null,
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
