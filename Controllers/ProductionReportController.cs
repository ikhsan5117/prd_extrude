using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;
using VelastoProductionSystem.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace VelastoProductionSystem.Controllers
{
    public class ProductionReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ElwpDbContext _elwpContext;
        private readonly IHubContext<DashboardHub> _hubContext;

        public ProductionReportController(ApplicationDbContext context, ElwpDbContext elwpContext, IHubContext<DashboardHub> hubContext)
        {
            _context = context;
            _elwpContext = elwpContext;
            _hubContext = hubContext;
        }

        // GET: ProductionReport (Monitoring List)
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToAction("Login", "Account");
            }
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
                    Shift = "---",
                    CreatedBy = HttpContext.Session.GetString("UserName") ?? "Operator",
                    MachineName = HttpContext.Session.GetString("MachineName")
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

            return Json(new { success = false, message = "Protocol Not Found" });
        }

        // GET: ProductionReport/GetTodaysSummary
        // Returns today's submitted data for the operator's machine (used by FAB popup, read-only)
        [HttpGet]
        public async Task<IActionResult> GetTodaysSummary()
        {
            var machineName = HttpContext.Session.GetString("MachineName");
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "true";
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // Query ProductionReports for today
            var paramQuery = _context.ProductionReports
                .Where(r => r.CreatedDate >= today && r.CreatedDate < tomorrow);

            // Query DimensionReports for today
            var dimQuery = _context.DimensionReports
                .Where(r => r.CreatedDate >= today && r.CreatedDate < tomorrow);

            // Filter by machine for non-admin
            if (!isAdmin && !string.IsNullOrEmpty(machineName))
            {
                paramQuery = paramQuery.Where(r => r.MachineName != null && r.MachineName.Contains(machineName));
                dimQuery   = dimQuery.Where(r => r.MachineName != null && r.MachineName.Contains(machineName));
            }

            var paramReports = await paramQuery
                .OrderByDescending(r => r.CreatedDate)
                .Select(r => new {
                    r.Id, r.HoseType, r.VinCode, r.Shift, r.CreatedBy, r.Status,
                    r.InnerMaterial, r.OuterMaterial, r.Yarn,
                    r.StandardLength, r.ActualLength,
                    r.QtyTarget, r.QtyOk, r.NgDimension, r.NgVisual, r.Remark,
                    r.MachineName
                }).ToListAsync();

            var dimReports = await dimQuery
                .OrderByDescending(r => r.CreatedDate)
                .Select(r => new {
                    r.Id, r.HoseType, r.VinCode, r.Shift, r.CreatedBy, r.Status,
                    r.DocumentNumber, r.DimensionDisplay,
                    r.StandardLength, r.ActualLength,
                    r.QtyTarget, r.QtyOk, r.NgDimension, r.NgVisual, r.Remark,
                    r.MachineName
                }).ToListAsync();

            return Json(new {
                machine = machineName ?? (isAdmin ? "ALL" : "—"),
                parameterReports = paramReports,
                dimensionReports = dimReports
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetPlanningItems(DateTime date, string shift, string? machine = null)

        {
            var cultureID = new System.Globalization.CultureInfo("id-ID");
            var cultureEN = new System.Globalization.CultureInfo("en-US");
            
            // Ambil nama mesin dari session jika tidak di-pass via parameter (non-admin)
            var sessionMachine = HttpContext.Session.GetString("MachineName");
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "true";
            var machineFilter = machine ?? sessionMachine;
            
            // 1. PRIORITAS: Ambil Live dari ELWP_PRD (Source of Truth Utama)
            var start = date.Date;
            var end = start.AddDays(1);

            var elwpQuery = from p in _elwpContext.ElwpPlannings
                             join m in _elwpContext.ElwpMachines on p.MesinId equals m.Id into machineJoin
                             from m in machineJoin.DefaultIfEmpty()
                             where p.TanggalPlanning >= start && p.TanggalPlanning < end
                             select new { p, MachineName = m != null ? m.NamaMesin : "UNKNOWN", MachineCode = m != null ? m.KodeMesin : "" };

            var elwpRows = await elwpQuery.ToListAsync();

            if (elwpRows.Any())
            {
                var shiftRaw = (shift ?? "").Trim().ToUpper();
                if (shiftRaw.StartsWith("SHIFT ")) shiftRaw = shiftRaw.Substring(6).Trim();
                
                // Filter berdasarkan shift
                var matchedShift = elwpRows.Where(x => (x.p.Shift ?? "").ToUpper().Contains(shiftRaw)).ToList();
                if (matchedShift.Any()) elwpRows = matchedShift;

                // Filter berdasarkan mesin - SELALU diapply untuk non-admin
                // jika operator memiliki session mesin, hanya tampilkan planning untuk mesin tersebut
                if (!isAdmin && !string.IsNullOrEmpty(machineFilter))
                {
                    elwpRows = elwpRows.Where(x =>
                        (x.MachineName ?? "").ToUpper().Contains(machineFilter.ToUpper()) ||
                        (x.MachineCode ?? "").ToUpper().Contains(machineFilter.ToUpper())
                    ).ToList();
                    // Tidak ada fallback ke semua data - operator hanya boleh lihat mesinnya sendiri
                }

                var items = elwpRows.Select(x => new {
                    itemCode = x.p.KodeItem,
                    itemName = (x.p.PartName ?? "#N/A") + (string.IsNullOrEmpty(x.p.PnSap) ? "" : " | " + x.p.PnSap),
                    machineName = x.MachineName,
                    machineCode = x.MachineCode,
                    dateShift = $"{(x.p.TanggalPlanning?.ToString("dddd, d MMMM yyyy", cultureID) ?? "").ToUpper()} SHIFT {x.p.Shift}",
                    date = x.p.TanggalPlanning?.ToString("yyyy-MM-dd"),
                    shift = x.p.Shift,
                    isFiltered = !isAdmin && !string.IsNullOrEmpty(machineFilter)
                }).OrderBy(p => p.itemName).ToList();

                return Json(items);
            }

            // 2. FALLBACK: Jika di ELWP kosong (data manual/lama), ambil dari Cache Lokal
            var dayStr1 = date.Day.ToString();
            var dayStr2 = date.Day.ToString("00");
            var months = new List<string> {
                date.ToString("MMMM", cultureID).ToUpper(), date.ToString("MMM", cultureID).ToUpper(),
                date.ToString("MMMM", cultureEN).ToUpper(), date.ToString("MMM", cultureEN).ToUpper()
            }.Distinct().ToList();

            var yearFull = date.Year.ToString();
            var yearShort = date.ToString("yy");

            var itemRows = await _context.PlanningMasters
                .Where(p => p.DateShiftString != null)
                .Select(p => new { p.Id, p.PartName1, p.PartName2, p.Kode, p.DateShiftString })
                .ToListAsync();

            var filtered = itemRows.Where(p => {
                var ds = (p.DateShiftString ?? "").ToUpper();
                bool hasDayMatch = ds.Contains(dayStr1) || ds.Contains(dayStr2);
                bool hasMonthMatch = months.Any(m => ds.Contains(m));
                bool hasYearMatch = ds.Contains(yearFull) || ds.Contains(yearShort);
                return hasDayMatch && hasMonthMatch && hasYearMatch;
            }).OrderByDescending(x => x.Id).ToList();

            if (!filtered.Any())
            {
                filtered = itemRows.OrderByDescending(x => x.Id).Take(50).ToList();
            }

            var result = filtered.Select(p => {
                var parsed = ParseDateShiftString(p.DateShiftString);
                return new {
                    itemCode = p.PartName1, 
                    itemName = (p.PartName2 ?? "") + (string.IsNullOrEmpty(p.Kode) ? "" : " | " + p.Kode),
                    dateShift = p.DateShiftString,
                    date = parsed.date,
                    shift = parsed.shift
                };
            }).Distinct().OrderBy(p => p.itemName).ToList();

            return Json(result);
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
        [HttpPost]
        public async Task<IActionResult> SaveWithReadings([FromBody] ProductionReportSaveDto dto)
        {
            if (dto == null) return Json(new { success = false, message = "Invalid data payload" });

            try
            {
                var report = new ProductionReport
                {
                    DocumentNumber = dto.DocumentNumber ?? "AUTO",
                    RevisionNumber = dto.RevisionNumber,
                    ProductionDate = DateTime.TryParse(dto.ProductionDate, out var dt) ? dt : DateTime.Now,
                    Shift = dto.Shift,
                    CustomerName = dto.CustomerName,
                    HoseType = dto.HoseType,
                    MachineName = HttpContext.Session.GetString("MachineName"),
                    InnerMaterial = dto.InnerMaterial,
                    InnerMaterialActual = dto.InnerMaterialActual,
                    InnerMaterialLotNo = dto.InnerMaterialLotNo,
                    OuterMaterial = dto.OuterMaterial,
                    OuterMaterialActual = dto.OuterMaterialActual,
                    OuterMaterialLotNo = dto.OuterMaterialLotNo,
                    Yarn = dto.Yarn,
                    YarnActual = dto.YarnActual,
                    YarnLotNo = dto.YarnLotNo,
                    DAI_Awal = dto.DAI_Awal,
                    DAI_Akhir = dto.DAI_Akhir,
                    DAC_Awal = dto.DAC_Awal,
                    DAC_Akhir = dto.DAC_Akhir,
                    DRAI_Awal = dto.DRAI_Awal,
                    DRAI_Akhir = dto.DRAI_Akhir,
                    DRAC_Awal = dto.DRAC_Awal,
                    DRAC_Akhir = dto.DRAC_Akhir,
                    CreatedBy = dto.CreatedBy ?? "Operator",
                    CreatedDate = DateTime.Now,
                    Status = "COMPLETED",
                    StandardParameterSettingId = dto.StandardParameterSettingId,
                    NippleDieOK = dto.NippleDieOK, NippleDieInitial = dto.NippleDieInitial, NippleDieFinal = dto.NippleDieFinal,
                    TubeDieOK = dto.TubeDieOK, TubeDieInitial = dto.TubeDieInitial, TubeDieFinal = dto.TubeDieFinal,
                    MiddleDieOK = dto.MiddleDieOK, MiddleDieInitial = dto.MiddleDieInitial, MiddleDieFinal = dto.MiddleDieFinal,
                    CoverDieOK = dto.CoverDieOK, CoverDieInitial = dto.CoverDieInitial, CoverDieFinal = dto.CoverDieFinal,
                    SpacerDieOK = dto.SpacerDieOK, SpacerDieInitial = dto.SpacerDieInitial, SpacerDieFinal = dto.SpacerDieFinal,
                    ToleranceDieOK = dto.ToleranceDieOK, ToleranceInitial = dto.ToleranceInitial, ToleranceFinal = dto.ToleranceFinal,
                    MeshInner10Before = dto.MeshInner10Before, MeshInner40Before = dto.MeshInner40Before,
                    MeshOuter10Before = dto.MeshOuter10Before, MeshOuter40Before = dto.MeshOuter40Before,
                    MeshInnerCheck = dto.MeshInnerCheck, MeshOuterCheck = dto.MeshOuterCheck,
                    EmbossMarkContent = dto.EmbossMarkContent,
                    EmbossMarkDate = DateTime.TryParse(dto.EmbossMarkDate, out var ed) ? ed : null,
                    QcCond = dto.QcCond, QcSurf = dto.QcSurf, QcRes = dto.QcRes
                };

                _context.ProductionReports.Add(report);
                await _context.SaveChangesAsync(); // Save first to get the ID

                // Batch insert Material Lots + Readings in a single round-trip
                if (dto.MaterialLots != null && dto.MaterialLots.Count > 0)
                {
                    var lots = dto.MaterialLots.Select(lotDto => new ProductionMaterialLot
                    {
                        ProductionReportId = report.Id,
                        LayerType    = lotDto.LayerType,
                        MaterialName = lotDto.MaterialName,
                        MaterialActual = lotDto.MaterialActual,
                        LotNumber    = lotDto.LotNumber,
                        SGValue      = lotDto.SGValue
                    });
                    _context.ProductionMaterialLots.AddRange(lots);
                }

                if (dto.Readings != null && dto.Readings.Count > 0)
                {
                    var readings = dto.Readings.Select(r => new ProductionReading
                    {
                        ProductionReportId  = report.Id,
                        ReadingTime         = DateTime.TryParse(r.ReadingTime, out var rt) ? rt : DateTime.Now,
                        RecordedBy          = r.RecordedBy ?? report.CreatedBy,

                        HeadTempInner       = int.TryParse(r.HeadTempInner,       out var hti) ? hti : (int?)null,
                        Cylinder1TempInner  = int.TryParse(r.Cylinder1TempInner,  out var c1i) ? c1i : (int?)null,
                        Cylinder2TempInner  = int.TryParse(r.Cylinder2TempInner,  out var c2i) ? c2i : (int?)null,
                        Cylinder3TempInner  = int.TryParse(r.Cylinder3TempInner,  out var c3i) ? c3i : (int?)null,
                        ScrewTempInner      = int.TryParse(r.ScrewTempInner,      out var sti) ? sti : (int?)null,
                        ScrewSpeedInner     = ParseDecimal(r.ScrewSpeedInner),
                        FeedRollRatioInner  = int.TryParse(r.FeedRollRatioInner,  out var fri) ? fri : (int?)null,
                        PressureInner       = ParseDecimal(r.PressureInner),

                        HeadTempOuter       = int.TryParse(r.HeadTempOuter,       out var hto) ? hto : (int?)null,
                        Cylinder1TempOuter  = int.TryParse(r.Cylinder1TempOuter,  out var c1o) ? c1o : (int?)null,
                        Cylinder2TempOuter  = int.TryParse(r.Cylinder2TempOuter,  out var c2o) ? c2o : (int?)null,
                        Cylinder3TempOuter  = int.TryParse(r.Cylinder3TempOuter,  out var c3o) ? c3o : (int?)null,
                        ScrewTempOuter      = int.TryParse(r.ScrewTempOuter,      out var sto) ? sto : (int?)null,
                        ScrewSpeedOuter     = ParseDecimal(r.ScrewSpeedOuter),
                        FeedRollRatioOuter  = int.TryParse(r.FeedRollRatioOuter,  out var fro) ? fro : (int?)null,
                        PressureOuter       = ParseDecimal(r.PressureOuter),

                        SpiralSpeed         = ParseDecimal(r.SpiralSpeed),
                        SpiralPitchSetting  = ParseDecimal(r.SpiralPitchSetting),
                        SpiralPitchDisplay  = ParseDecimal(r.SpiralPitchDisplay),
                        PresetValue         = ParseDecimal(r.PresetValue),
                        ControlValue        = ParseDecimal(r.ControlValue),
                        HoseSpeed           = ParseDecimal(r.HoseSpeed),
                        TakeupConveyorSpeed = ParseDecimal(r.TakeupConveyorSpeed),
                        CoolConveyorSpeed   = ParseDecimal(r.CoolConveyorSpeed),
                        ConveyorRatio       = r.ConveyorRatio,
                        UnsmoothSurface     = ParseDecimal(r.UnsmoothSurface),
                        ChillerWaterTemp    = ParseDecimal(r.ChillerWaterTemp),
                        CaterpillarGap      = ParseDecimal(r.CaterpillarGap)
                    });
                    _context.ProductionReadings.AddRange(readings);
                }

                // Single bulk save for lots + readings
                await _context.SaveChangesAsync();

                // Notify dashboard via SignalR (fire-and-forget, don't await to block response)
                _ = _hubContext.Clients.All.SendAsync("ReceiveUpdate");

                return Json(new { success = true, id = report.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWithReadings([FromBody] ProductionReportSaveDto dto)
        {
            if (dto == null || dto.Id == 0) return Json(new { success = false, message = "Invalid data payload" });

            try
            {
                var report = await _context.ProductionReports
                    .Include(p => p.ProductionReadings)
                    .FirstOrDefaultAsync(m => m.Id == dto.Id);

                if (report == null) return Json(new { success = false, message = "Report not found" });

                // Update common fields
                report.DocumentNumber = dto.DocumentNumber ?? report.DocumentNumber;
                report.RevisionNumber = dto.RevisionNumber;
                if (DateTime.TryParse(dto.ProductionDate, out var dt)) report.ProductionDate = dt;
                report.Shift = dto.Shift;
                report.CustomerName = dto.CustomerName;
                report.HoseType = dto.HoseType;
                report.StandardParameterSettingId = dto.StandardParameterSettingId;

                report.InnerMaterial = dto.InnerMaterial;
                report.InnerMaterialActual = dto.InnerMaterialActual;
                report.InnerMaterialLotNo = dto.InnerMaterialLotNo;
                report.OuterMaterial = dto.OuterMaterial;
                report.OuterMaterialActual = dto.OuterMaterialActual;
                report.OuterMaterialLotNo = dto.OuterMaterialLotNo;
                report.Yarn = dto.Yarn;
                report.YarnActual = dto.YarnActual;
                report.YarnLotNo = dto.YarnLotNo;

                report.DAI_Awal = dto.DAI_Awal;
                report.DAI_Akhir = dto.DAI_Akhir;
                report.DAC_Awal = dto.DAC_Awal;
                report.DAC_Akhir = dto.DAC_Akhir;
                report.DRAI_Awal = dto.DRAI_Awal;
                report.DRAI_Akhir = dto.DRAI_Akhir;
                report.DRAC_Awal = dto.DRAC_Awal;
                report.DRAC_Akhir = dto.DRAC_Akhir;

                report.NippleDieOK = dto.NippleDieOK; report.NippleDieInitial = dto.NippleDieInitial; report.NippleDieFinal = dto.NippleDieFinal;
                report.TubeDieOK = dto.TubeDieOK; report.TubeDieInitial = dto.TubeDieInitial; report.TubeDieFinal = dto.TubeDieFinal;
                report.MiddleDieOK = dto.MiddleDieOK; report.MiddleDieInitial = dto.MiddleDieInitial; report.MiddleDieFinal = dto.MiddleDieFinal;
                report.CoverDieOK = dto.CoverDieOK; report.CoverDieInitial = dto.CoverDieInitial; report.CoverDieFinal = dto.CoverDieFinal;
                report.SpacerDieOK = dto.SpacerDieOK; report.SpacerDieInitial = dto.SpacerDieInitial; report.SpacerDieFinal = dto.SpacerDieFinal;
                report.ToleranceDieOK = dto.ToleranceDieOK; report.ToleranceInitial = dto.ToleranceInitial; report.ToleranceFinal = dto.ToleranceFinal;

                report.MeshInner10Before = dto.MeshInner10Before; report.MeshInner40Before = dto.MeshInner40Before;
                report.MeshOuter10Before = dto.MeshOuter10Before; report.MeshOuter40Before = dto.MeshOuter40Before;
                report.MeshInnerCheck = dto.MeshInnerCheck; report.MeshOuterCheck = dto.MeshOuterCheck;
                report.EmbossMarkContent = dto.EmbossMarkContent;
                if (DateTime.TryParse(dto.EmbossMarkDate, out var ed)) report.EmbossMarkDate = ed;
                report.QcCond = dto.QcCond; report.QcSurf = dto.QcSurf; report.QcRes = dto.QcRes;

                // Sync Initial instrument parameters from the first reading if they are empty
                if (dto.Readings != null && dto.Readings.Count > 0)
                {
                    var r0 = dto.Readings[0];
                    report.InitHeadTempInner = decimal.TryParse(r0.HeadTempInner, out var it1) ? it1 : report.InitHeadTempInner;
                    report.InitCylinder1TempInner = decimal.TryParse(r0.Cylinder1TempInner, out var it2) ? it2 : report.InitCylinder1TempInner;
                    report.InitCylinder2TempInner = decimal.TryParse(r0.Cylinder2TempInner, out var it3) ? it3 : report.InitCylinder2TempInner;
                    report.InitCylinder3TempInner = decimal.TryParse(r0.Cylinder3TempInner, out var it4) ? it4 : report.InitCylinder3TempInner;
                    report.InitScrewTempInner = decimal.TryParse(r0.ScrewTempInner, out var it5) ? it5 : report.InitScrewTempInner;
                    report.InitScrewSpeedInner = decimal.TryParse(r0.ScrewSpeedInner, out var it6) ? it6 : report.InitScrewSpeedInner;
                    report.InitFeedRollRatioInner = decimal.TryParse(r0.FeedRollRatioInner, out var it7) ? it7 : report.InitFeedRollRatioInner;
                    report.InitPressureInner = decimal.TryParse(r0.PressureInner, out var it8) ? it8 : report.InitPressureInner;

                    report.InitHeadTempOuter = decimal.TryParse(r0.HeadTempOuter, out var ot1) ? ot1 : report.InitHeadTempOuter;
                    report.InitCylinder1TempOuter = decimal.TryParse(r0.Cylinder1TempOuter, out var ot2) ? ot2 : report.InitCylinder1TempOuter;
                    report.InitCylinder2TempOuter = decimal.TryParse(r0.Cylinder2TempOuter, out var ot3) ? ot3 : report.InitCylinder2TempOuter;
                    report.InitCylinder3TempOuter = decimal.TryParse(r0.Cylinder3TempOuter, out var ot4) ? ot4 : report.InitCylinder3TempOuter;
                    report.InitScrewTempOuter = decimal.TryParse(r0.ScrewTempOuter, out var ot5) ? ot5 : report.InitScrewTempOuter;
                    report.InitScrewSpeedOuter = decimal.TryParse(r0.ScrewSpeedOuter, out var ot6) ? ot6 : report.InitScrewSpeedOuter;
                    report.InitFeedRollRatioOuter = decimal.TryParse(r0.FeedRollRatioOuter, out var ot7) ? ot7 : report.InitFeedRollRatioOuter;
                    report.InitPressureOuter = decimal.TryParse(r0.PressureOuter, out var ot8) ? ot8 : report.InitPressureOuter;

                    report.InitSpiralSpeed = decimal.TryParse(r0.SpiralSpeed, out var m1) ? m1 : report.InitSpiralSpeed;
                    report.InitSpiralPitchSetting = decimal.TryParse(r0.SpiralPitchSetting, out var m2) ? m2 : report.InitSpiralPitchSetting;
                    report.InitHoseSpeed = decimal.TryParse(r0.HoseSpeed, out var m3) ? m3 : report.InitHoseSpeed;
                    report.InitConveyorRatio = decimal.TryParse(r0.ConveyorRatio, out var m4) ? m4 : report.InitConveyorRatio;
                }

                // Update Readings: Remove old and add new (simplest for Edit)
                // Alternatively, match IDs if complexity is needed, but for 1-5 readings, clear & reload is safer
                _context.ProductionReadings.RemoveRange(report.ProductionReadings);

                if (dto.Readings != null)
                {
                    foreach (var r in dto.Readings)
                    {
                        var reading = new ProductionReading
                        {
                            ProductionReportId = report.Id,
                            ReadingTime = DateTime.TryParse(r.ReadingTime, out var rt) ? rt : DateTime.Now,
                            RecordedBy = r.RecordedBy ?? report.CreatedBy ?? "Operator",
                            
                            HeadTempInner = int.TryParse(r.HeadTempInner, out var hti) ? hti : (int?)null,
                            Cylinder1TempInner = int.TryParse(r.Cylinder1TempInner, out var c1i) ? c1i : (int?)null,
                            Cylinder2TempInner = int.TryParse(r.Cylinder2TempInner, out var c2i) ? c2i : (int?)null,
                            Cylinder3TempInner = int.TryParse(r.Cylinder3TempInner, out var c3i) ? c3i : (int?)null,
                            ScrewTempInner = int.TryParse(r.ScrewTempInner, out var sti) ? sti : (int?)null,
                            ScrewSpeedInner = ParseDecimal(r.ScrewSpeedInner),
                            FeedRollRatioInner = int.TryParse(r.FeedRollRatioInner, out var fri) ? fri : (int?)null,
                            PressureInner = ParseDecimal(r.PressureInner),

                            HeadTempOuter = int.TryParse(r.HeadTempOuter, out var hto) ? hto : (int?)null,
                            Cylinder1TempOuter = int.TryParse(r.Cylinder1TempOuter, out var c1o) ? c1o : (int?)null,
                            Cylinder2TempOuter = int.TryParse(r.Cylinder2TempOuter, out var c2o) ? c2o : (int?)null,
                            Cylinder3TempOuter = int.TryParse(r.Cylinder3TempOuter, out var c3o) ? c3o : (int?)null,
                            ScrewTempOuter = int.TryParse(r.ScrewTempOuter, out var sto) ? sto : (int?)null,
                            ScrewSpeedOuter = ParseDecimal(r.ScrewSpeedOuter),
                            FeedRollRatioOuter = int.TryParse(r.FeedRollRatioOuter, out var fro) ? fro : (int?)null,
                            PressureOuter = ParseDecimal(r.PressureOuter),

                            SpiralSpeed = ParseDecimal(r.SpiralSpeed),
                            SpiralPitchSetting = ParseDecimal(r.SpiralPitchSetting),
                            SpiralPitchDisplay = ParseDecimal(r.SpiralPitchDisplay),
                            PresetValue = ParseDecimal(r.PresetValue),
                            ControlValue = ParseDecimal(r.ControlValue),
                            HoseSpeed = ParseDecimal(r.HoseSpeed),
                            TakeupConveyorSpeed = ParseDecimal(r.TakeupConveyorSpeed),
                            CoolConveyorSpeed = ParseDecimal(r.CoolConveyorSpeed),
                            ConveyorRatio = r.ConveyorRatio,
                            UnsmoothSurface = ParseDecimal(r.UnsmoothSurface),
                            ChillerWaterTemp = ParseDecimal(r.ChillerWaterTemp),
                            CaterpillarGap = ParseDecimal(r.CaterpillarGap)
                        };
                        _context.ProductionReadings.Add(reading);
                    }
                }

                await _context.SaveChangesAsync();
                
                // Notify via SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveUpdate");

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private decimal? ParseDecimal(string? val)
        {
            if (string.IsNullOrEmpty(val)) return null;
            if (decimal.TryParse(val.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var res)) return res;
            return null;
        }

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
                    
                    // Notify via SignalR
                    await _hubContext.Clients.All.SendAsync("ReceiveUpdate");
                    
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

            var report = await _context.ProductionReports
                .Include(p => p.StandardParameterSetting)
                .Include(p => p.ProductionReadings)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (report == null) return NotFound();

            ViewBag.StandardParameterSettings = new SelectList(
                await _context.StandardParameterSettings.Where(s => s.IsActive)
                    .Select(s => new { Id = s.Id, Display = "[" + s.ItemList + "] - " + s.HoseType })
                    .ToListAsync(),
                "Id", "Display", report.StandardParameterSettingId
            );

            return View(report);
        }

        // POST: ProductionReport/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductionReport report)
        {
            if (id != report.Id) return NotFound();

            ModelState.Remove("CreatedBy");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("Status");

            if (ModelState.IsValid)
            {
                try
                {
                    // Load existing from DB to preserve immutable fields
                    var existing = await _context.ProductionReports.FindAsync(id);
                    if (existing == null) return NotFound();

                    // === EDITABLE FIELDS ONLY ===
                    // Header
                    existing.DocumentNumber = report.DocumentNumber;
                    existing.ProductionDate = report.ProductionDate;
                    existing.RevisionNumber = report.RevisionNumber;
                    existing.Shift = report.Shift;
                    existing.CustomerName = report.CustomerName;
                    existing.HoseType = report.HoseType;
                    existing.Dimension = report.Dimension;
                    existing.StandardParameterSettingId = report.StandardParameterSettingId;

                    // Standards (from SPS)
                    existing.InnerMaterial = report.InnerMaterial;
                    existing.OuterMaterial = report.OuterMaterial;
                    existing.Yarn = report.Yarn;

                    // Material Actual + Lot
                    existing.InnerMaterialActual = report.InnerMaterialActual;
                    existing.InnerMaterialLotNo = report.InnerMaterialLotNo;
                    existing.InnerMaterialSG = report.InnerMaterialSG;
                    existing.OuterMaterialActual = report.OuterMaterialActual;
                    existing.OuterMaterialLotNo = report.OuterMaterialLotNo;
                    existing.OuterMaterialSG = report.OuterMaterialSG;
                    existing.YarnActual = report.YarnActual;
                    existing.YarnLotNo = report.YarnLotNo;

                    // Die Verification
                    existing.NippleDieOK = report.NippleDieOK;
                    existing.NippleDieInitial = report.NippleDieInitial;
                    existing.NippleDieFinal = report.NippleDieFinal;
                    existing.TubeDieOK = report.TubeDieOK;
                    existing.TubeDieInitial = report.TubeDieInitial;
                    existing.TubeDieFinal = report.TubeDieFinal;
                    existing.MiddleDieOK = report.MiddleDieOK;
                    existing.MiddleDieInitial = report.MiddleDieInitial;
                    existing.MiddleDieFinal = report.MiddleDieFinal;
                    existing.CoverDieOK = report.CoverDieOK;
                    existing.CoverDieInitial = report.CoverDieInitial;
                    existing.CoverDieFinal = report.CoverDieFinal;
                    existing.SpacerDieOK = report.SpacerDieOK;
                    existing.SpacerDieInitial = report.SpacerDieInitial;
                    existing.SpacerDieFinal = report.SpacerDieFinal;
                    existing.ToleranceDieOK = report.ToleranceDieOK;
                    existing.ToleranceInitial = report.ToleranceInitial;
                    existing.ToleranceFinal = report.ToleranceFinal;

                    // Mesh & Emboss
                    existing.MeshInner10Before = report.MeshInner10Before;
                    existing.MeshInner40Before = report.MeshInner40Before;
                    existing.MeshOuter10Before = report.MeshOuter10Before;
                    existing.MeshOuter40Before = report.MeshOuter40Before;
                    existing.MeshInnerCheck = report.MeshInnerCheck;
                    existing.MeshOuterCheck = report.MeshOuterCheck;
                    existing.EmbossMarkContent = report.EmbossMarkContent;
                    existing.EmbossMarkDate = report.EmbossMarkDate;

                    // QC
                    existing.QcCond = report.QcCond;
                    existing.QcSurf = report.QcSurf;
                    existing.QcRes = report.QcRes;

                    // Waste / Dandori
                    existing.DAI_Awal = report.DAI_Awal;
                    existing.DAI_Akhir = report.DAI_Akhir;
                    existing.DAC_Awal = report.DAC_Awal;
                    existing.DAC_Akhir = report.DAC_Akhir;
                    existing.DRAI_Awal = report.DRAI_Awal;
                    existing.DRAI_Akhir = report.DRAI_Akhir;
                    existing.DRAC_Awal = report.DRAC_Awal;
                    existing.DRAC_Akhir = report.DRAC_Akhir;

                    // Production Summary
                    existing.QtyTarget = report.QtyTarget;
                    existing.QtyOk = report.QtyOk;
                    existing.NgDimension = report.NgDimension;
                    existing.NgVisual = report.NgVisual;
                    existing.VinCode = report.VinCode;
                    existing.StandardLength = report.StandardLength;
                    existing.ActualLength = report.ActualLength;
                    existing.Remark = report.Remark;

                    // Operator (allow update)
                    if (!string.IsNullOrWhiteSpace(report.CreatedBy))
                        existing.CreatedBy = report.CreatedBy;

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

            ViewBag.StandardParameterSettings = new SelectList(
                await _context.StandardParameterSettings.Where(s => s.IsActive)
                    .Select(s => new { Id = s.Id, Display = "[" + s.ItemList + "] - " + s.HoseType })
                    .ToListAsync(),
                "Id", "Display", report.StandardParameterSettingId
            );
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
