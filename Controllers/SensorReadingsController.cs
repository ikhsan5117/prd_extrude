using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;
using VelastoProductionSystem.Hubs;
using OfficeOpenXml;

namespace VelastoProductionSystem.Controllers
{
    public class SensorReadingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<DashboardHub> _hubContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SensorReadingsController> _logger;

        // Mapping: metric_type -> nama field di ProductionReading
        private static readonly Dictionary<string, string> MetricToFieldMap =
            new(StringComparer.OrdinalIgnoreCase)
        {
            { "outer_diameter",     "ControlValue" },
            { "inner_diameter",     "InnerDiameter" },
            { "hose_speed",         "HoseSpeed" },
            { "head_temp_inner",    "HeadTempInner" },
            { "head_temp_outer",    "HeadTempOuter" },
            { "chiller_water_temp", "ChillerWaterTemp" },
            { "screw_speed_inner",  "ScrewSpeedInner" },
            { "screw_speed_outer",  "ScrewSpeedOuter" },
            { "pressure_inner",     "PressureInner" },
            { "pressure_outer",     "PressureOuter" },
            { "spiral_pitch",       "SpiralPitchDisplay" },
            { "caterpillar_gap",    "CaterpillarGap" },
        };

        // Metrik dimensi yang di-mirror ke DimensionMeasurements (untuk SPC Chart)
        private static readonly Dictionary<string, string> DimensionMetricToPointName =
            new(StringComparer.OrdinalIgnoreCase)
        {
            { "outer_diameter", "Outer Diameter (Sensor)" },
            { "inner_diameter", "Inner Diameter (Sensor)" },
            { "spiral_pitch",   "Spiral Pitch (Sensor)" },
        };

        // Satuan default per metrik
        private static readonly Dictionary<string, string> DefaultUnits =
            new(StringComparer.OrdinalIgnoreCase)
        {
            { "outer_diameter",     "mm" },
            { "inner_diameter",     "mm" },
            { "hose_speed",         "m/min" },
            { "head_temp_inner",    "°C" },
            { "head_temp_outer",    "°C" },
            { "chiller_water_temp", "°C" },
            { "screw_speed_inner",  "rpm" },
            { "screw_speed_outer",  "rpm" },
            { "pressure_inner",     "MPa" },
            { "pressure_outer",     "MPa" },
            { "spiral_pitch",       "mm" },
            { "caterpillar_gap",    "mm" },
        };

        public SensorReadingsController(
            ApplicationDbContext context,
            IHubContext<DashboardHub> hubContext,
            IConfiguration configuration,
            ILogger<SensorReadingsController> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _configuration = configuration;
            _logger = logger;
        }

        // ═══════════════════════════════════════════════════════════════
        //  MVC PAGE — Monitor UI
        // ═══════════════════════════════════════════════════════════════

        // GET /SensorReadings/Monitor
        public IActionResult Monitor()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                return RedirectToAction("Login", "Account");

            var configuredKey = _configuration["SensorApi:ApiKey"] ?? "VS-SENSOR-KEY-2026";
            ViewBag.ApiKey    = configuredKey;
            ViewBag.IngestUrl = $"{Request.Scheme}://{Request.Host}/api/sensor-readings/ingest";
            ViewBag.BatchUrl  = $"{Request.Scheme}://{Request.Host}/api/sensor-readings/ingest/batch";

            return View();
        }

        // ═══════════════════════════════════════════════════════════════
        //  MVC PAGE — Sensor Simulator
        //  GET /SensorReadings/Simulator
        // ═══════════════════════════════════════════════════════════════

        public async Task<IActionResult> Simulator()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                return RedirectToAction("Login", "Account");

            var configuredKey = _configuration["SensorApi:ApiKey"] ?? "VS-SENSOR-KEY-2026";
            ViewBag.ApiKey    = configuredKey;
            ViewBag.IngestUrl = $"{Request.Scheme}://{Request.Host}/api/sensor-readings/ingest";

            // Ambil semua SPS untuk ditampilkan di simulator (join ke ItemList)
            var spsList = await _context.SpsItemLists
                .Include(i => i.SpsNoDoc)
                .Select(s => new {
                    Id = s.DocumentNumber,
                    s.SpsNoDoc!.DocumentNumber,
                    s.SpsNoDoc.No,
                    ItemList = s.ItemList,
                    s.SpsNoDoc.MachineCode,
                    s.SpsNoDoc.Customer,
                    // Standard parameter fields (_Asli = nilai target)
                    s.SpsNoDoc.OdSensor_Asli,    s.SpsNoDoc.OdSensor_Max,    s.SpsNoDoc.OdSensor_Min,
                    s.SpsNoDoc.HoseSpeed_Asli,   s.SpsNoDoc.HoseSpeed_Max,   s.SpsNoDoc.HoseSpeed_Min,
                    s.SpsNoDoc.HeadTemp1_Asli,   s.SpsNoDoc.HeadTemp1_Max,   s.SpsNoDoc.HeadTemp1_Min,
                    s.SpsNoDoc.HeadTemp2_Asli,   s.SpsNoDoc.HeadTemp2_Max,   s.SpsNoDoc.HeadTemp2_Min,
                    s.SpsNoDoc.ScrewSpeed1_Asli, s.SpsNoDoc.ScrewSpeed1_Max, s.SpsNoDoc.ScrewSpeed1_Min,
                    s.SpsNoDoc.ScrewSpeed2_Asli, s.SpsNoDoc.ScrewSpeed2_Max, s.SpsNoDoc.ScrewSpeed2_Min,
                    s.SpsNoDoc.Pressure1_Asli,   s.SpsNoDoc.Pressure1_Max,   s.SpsNoDoc.Pressure1_Min,
                    s.SpsNoDoc.Pressure2_Asli,   s.SpsNoDoc.Pressure2_Max,   s.SpsNoDoc.Pressure2_Min,
                    s.SpsNoDoc.SpiralPitchSetting_Asli, s.SpsNoDoc.SpiralPitchSetting_Max, s.SpsNoDoc.SpiralPitchSetting_Min,
                    s.SpsNoDoc.CaterpillarGap_Asli, s.SpsNoDoc.CaterpillarGap_Max, s.SpsNoDoc.CaterpillarGap_Min,
                    s.SpsNoDoc.ChillerWaterTemp_Asli, s.SpsNoDoc.ChillerWaterTemp_Max, s.SpsNoDoc.ChillerWaterTemp_Min,
                    s.SpsNoDoc.InnerTarget, s.SpsNoDoc.InnerMin, s.SpsNoDoc.InnerMax,
                    s.SpsNoDoc.ToleranceInner_Asli, s.SpsNoDoc.ToleranceSpiralPitch_Asli
                })
                .ToListAsync();

            var elwpContext = HttpContext.RequestServices.GetRequiredService<ElwpDbContext>();
            var machines = await elwpContext.ElwpMachines
                .Where(m => m.IsActive && m.AreaId == 1 && m.KodeMesin != "DL01" && m.KodeMesin != "DL02")
                .OrderBy(m => m.KodeMesin)
                .ToListAsync();

            ViewBag.Machines = machines;

            var jsonOpts = new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase };
            ViewBag.SpsList = System.Text.Json.JsonSerializer.Serialize(spsList, jsonOpts);

            return View();
        }

        // ═══════════════════════════════════════════════════════════════
        //  API — INGEST SINGLE (sensor device → aplikasi)
        //  POST /api/sensor-readings/ingest
        //  Header: X-Api-Key: {configured_key}
        // ═══════════════════════════════════════════════════════════════

        [HttpPost]
        [Route("api/sensor-readings/ingest")]
        public async Task<IActionResult> Ingest([FromBody] SensorPayloadDto? payload)
        {
            if (!ValidateApiKey(Request))
                return Unauthorized(new { success = false, message = "Invalid or missing API key. Use header: X-Api-Key" });

            if (payload == null)
                return BadRequest(new { success = false, message = "Request body kosong atau format JSON tidak valid." });

            if (string.IsNullOrWhiteSpace(payload.DeviceId))
                return BadRequest(new { success = false, message = "DeviceId wajib diisi." });

            if (string.IsNullOrWhiteSpace(payload.MetricType))
                return BadRequest(new { success = false, message = "MetricType wajib diisi." });

            var sensorTime = payload.Timestamp ?? DateTime.Now;

            // Tolak data lebih dari 5 menit di masa depan
            if (sensorTime > DateTime.Now.AddMinutes(5))
                return BadRequest(new { success = false, message = "Timestamp terlalu jauh di masa depan (max +5 menit)." });

            // Tolak data lebih dari 24 jam ke belakang
            if (sensorTime < DateTime.Now.AddHours(-24))
                return BadRequest(new { success = false, message = "Timestamp terlalu lama (max 24 jam ke belakang)." });

            // Dedup: tolak payload identik dalam 1 detik yang sama
            var idempotencyKey = $"{payload.DeviceId}|{sensorTime:yyyyMMddHHmmss}|{payload.MetricType.ToLower()}";
            var alreadyExists = await _context.SensorIngestLogs
                .AnyAsync(l => l.IdempotencyKey == idempotencyKey);

            if (alreadyExists)
                return Ok(new { success = true, status = "DUPLICATE", message = "Data sudah tersimpan sebelumnya." });

            // Cari ProductionReport aktif berdasarkan ReportId atau MachineCode
            ProductionReport? report = null;
            if (payload.ReportId.HasValue && payload.ReportId > 0)
            {
                report = await _context.ProductionReports.FindAsync(payload.ReportId.Value);
            }
            else if (!string.IsNullOrEmpty(payload.MachineCode))
            {
                report = await _context.ProductionReports
                    .Where(r => r.MachineName != null &&
                                r.MachineName.Contains(payload.MachineCode) &&
                                (r.Status == "NOW PRODUCING" || r.Status == "ACTIVE"))
                    .OrderByDescending(r => r.CreatedDate)
                    .FirstOrDefaultAsync();
            }

            var metricNorm = payload.MetricType.ToLower().Trim();
            var unitFinal  = payload.Unit ?? (DefaultUnits.TryGetValue(metricNorm, out var u) ? u : null);

            var log = new SensorIngestLog
            {
                DeviceId          = payload.DeviceId.Trim(),
                MachineCode       = (payload.MachineCode ?? string.Empty).Trim(),
                ProductionReportId = report?.Id,
                SensorTimestamp   = sensorTime,
                MetricType        = metricNorm,
                MetricValue       = payload.MetricValue,
                Unit              = unitFinal,
                Quality           = payload.Quality ?? "OK",
                Status            = "OK",
                IdempotencyKey    = idempotencyKey,
                IngestedAt        = DateTime.Now
            };
            _context.SensorIngestLogs.Add(log);

            // Mirror metrik dimensi ke DimensionMeasurements (baca SPC dari HomeController)
            if (DimensionMetricToPointName.TryGetValue(metricNorm, out var pointName) && report != null)
            {
                var dimReport = await _context.DimensionReports
                    .Where(d => d.MachineName != null &&
                                d.MachineName.Contains(payload.MachineCode ?? "") &&
                                d.Status == "ACTIVE")
                    .OrderByDescending(d => d.CreatedDate)
                    .FirstOrDefaultAsync();

                if (dimReport != null)
                {
                    _context.DimensionMeasurements.Add(new DimensionMeasurement
                    {
                        DimensionReportId  = dimReport.Id,
                        PointName          = pointName,
                        TimeSection        = sensorTime.ToString("HH:mm"),
                        Frequency          = "Auto (Sensor)",
                        StandardDimension  = "-",
                        Initial            = "SENSOR",
                        R1                 = payload.MetricValue,
                        Status             = (payload.Quality == "OK") ? "OK" : "NG",
                        RecordedTime       = sensorTime
                    });
                }
            }

            await _context.SaveChangesAsync();

            // ── Broadcast SignalR: event khusus sensor + generic refresh ──
            await _hubContext.Clients.All.SendAsync("SensorDataReceived", new
            {
                deviceId    = log.DeviceId,
                machineCode = log.MachineCode,
                metricType  = log.MetricType,
                metricValue = log.MetricValue,
                unit        = log.Unit,
                quality     = log.Quality,
                timestamp   = log.SensorTimestamp.ToString("HH:mm:ss"),
                reportId    = report?.Id
            });
            // _ = _hubContext.Clients.All.SendAsync("ReceiveUpdate"); // DISABLED: Avoid full-page reloads on every sensor tick

            _logger.LogInformation("Sensor ingest OK: {Device} {Metric}={Value}{Unit} @ {Time}",
                log.DeviceId, log.MetricType, log.MetricValue, log.Unit, sensorTime);

            return Ok(new
            {
                success  = true,
                logId    = log.Id,
                reportId = report?.Id,
                message  = "Data berhasil disimpan."
            });
        }

        // ═══════════════════════════════════════════════════════════════
        //  API — INGEST BATCH
        //  POST /api/sensor-readings/ingest/batch
        // ═══════════════════════════════════════════════════════════════

        [HttpPost]
        [Route("api/sensor-readings/ingest/batch")]
        public async Task<IActionResult> IngestBatch([FromBody] SensorBatchPayloadDto? batch)
        {
            if (!ValidateApiKey(Request))
                return Unauthorized(new { success = false, message = "Invalid or missing API key." });

            if (batch == null || string.IsNullOrWhiteSpace(batch.DeviceId))
                return BadRequest(new { success = false, message = "DeviceId wajib diisi." });

            if (batch.Metrics == null || batch.Metrics.Count == 0)
                return BadRequest(new { success = false, message = "Metrics array kosong." });

            int saved = 0, dups = 0, errors = 0;

            foreach (var metric in batch.Metrics)
            {
                if (string.IsNullOrWhiteSpace(metric.MetricType)) { errors++; continue; }

                var sensorTime      = metric.Timestamp ?? DateTime.Now;
                var metricNorm      = metric.MetricType.ToLower().Trim();
                var idempotencyKey  = $"{batch.DeviceId}|{sensorTime:yyyyMMddHHmmss}|{metricNorm}";

                var exists = await _context.SensorIngestLogs
                    .AnyAsync(l => l.IdempotencyKey == idempotencyKey);

                if (exists) { dups++; continue; }

                var unitFinal = metric.Unit ?? (DefaultUnits.TryGetValue(metricNorm, out var u) ? u : null);

                _context.SensorIngestLogs.Add(new SensorIngestLog
                {
                    DeviceId          = batch.DeviceId.Trim(),
                    MachineCode       = (batch.MachineCode ?? string.Empty).Trim(),
                    ProductionReportId = batch.ReportId,
                    SensorTimestamp   = sensorTime,
                    MetricType        = metricNorm,
                    MetricValue       = metric.MetricValue,
                    Unit              = unitFinal,
                    Quality           = metric.Quality ?? "OK",
                    Status            = "OK",
                    IdempotencyKey    = idempotencyKey,
                    IngestedAt        = DateTime.Now
                });
                saved++;
            }

            if (saved > 0)
            {
                await _context.SaveChangesAsync();
                // _ = _hubContext.Clients.All.SendAsync("ReceiveUpdate"); // DISABLED: Avoid full-page reloads on every sensor tick
            }

            return Ok(new { success = true, saved, duplicates = dups, errors });
        }

        // ═══════════════════════════════════════════════════════════════
        //  API — LIVE DATA (untuk Dashboard AJAX)
        //  GET /api/sensor-readings/live?machineCode=EXT-01&metricType=outer_diameter&last=50
        // ═══════════════════════════════════════════════════════════════

        [HttpGet]
        [Route("api/sensor-readings/live")]
        public async Task<IActionResult> GetLive(
            string? machineCode  = null,
            string? metricType   = null,
            int last             = 50)
        {
            var query = _context.SensorIngestLogs
                .Where(l => l.Status == "OK")
                .AsQueryable();

            if (!string.IsNullOrEmpty(machineCode))
                query = query.Where(l => l.MachineCode == machineCode);

            if (!string.IsNullOrEmpty(metricType))
                query = query.Where(l => l.MetricType == metricType.ToLower());

            var data = await query
                .OrderByDescending(l => l.SensorTimestamp)
                .Take(last)
                .Select(l => new
                {
                    l.Id,
                    l.DeviceId,
                    l.MachineCode,
                    l.MetricType,
                    l.MetricValue,
                    l.Unit,
                    l.Quality,
                    Timestamp     = l.SensorTimestamp.ToString("HH:mm:ss"),
                    TimestampFull = l.SensorTimestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    IngestedAt    = l.IngestedAt.ToString("HH:mm:ss"),
                    l.Status
                })
                .ToListAsync();

            // Kembalikan urutan chronologis untuk chart
            return Json(data.OrderBy(d => d.TimestampFull).ToList());
        }

        // ═══════════════════════════════════════════════════════════════
        //  EXPORT — LIVE FEED TO EXCEL
        //  GET /SensorReadings/ExportLiveExcel?machineCode=EXT-01&metricType=outer_diameter&last=50
        // ═══════════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> ExportLiveExcel(
            string? machineCode = null,
            string? metricType = null,
            int last = 50)
        {
            var safeLast = Math.Clamp(last, 1, 1000);

            var query = _context.SensorIngestLogs
                .Where(l => l.Status == "OK")
                .AsQueryable();

            if (!string.IsNullOrEmpty(machineCode))
                query = query.Where(l => l.MachineCode == machineCode);

            if (!string.IsNullOrEmpty(metricType))
                query = query.Where(l => l.MetricType == metricType.ToLower());

            var rows = await query
                .OrderByDescending(l => l.SensorTimestamp)
                .Take(safeLast)
                .Select(l => new
                {
                    l.DeviceId,
                    l.MachineCode,
                    l.MetricType,
                    l.MetricValue,
                    l.Unit,
                    l.Quality,
                    SensorTime = l.SensorTimestamp,
                    IngestedAt = l.IngestedAt,
                    l.Status
                })
                .ToListAsync();

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Sensor Live Feed");

            ws.Cells[1, 1].Value = "No";
            ws.Cells[1, 2].Value = "Device";
            ws.Cells[1, 3].Value = "Machine";
            ws.Cells[1, 4].Value = "Metric";
            ws.Cells[1, 5].Value = "Value";
            ws.Cells[1, 6].Value = "Unit";
            ws.Cells[1, 7].Value = "Quality";
            ws.Cells[1, 8].Value = "Sensor Time";
            ws.Cells[1, 9].Value = "Ingested";
            ws.Cells[1, 10].Value = "Status";

            for (var i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                var row = i + 2;
                ws.Cells[row, 1].Value = i + 1;
                ws.Cells[row, 2].Value = r.DeviceId;
                ws.Cells[row, 3].Value = r.MachineCode;
                ws.Cells[row, 4].Value = r.MetricType;
                ws.Cells[row, 5].Value = r.MetricValue;
                ws.Cells[row, 6].Value = r.Unit;
                ws.Cells[row, 7].Value = r.Quality;
                ws.Cells[row, 8].Value = r.SensorTime.ToString("yyyy-MM-dd HH:mm:ss");
                ws.Cells[row, 9].Value = r.IngestedAt.ToString("yyyy-MM-dd HH:mm:ss");
                ws.Cells[row, 10].Value = r.Status;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            var machineTag = string.IsNullOrWhiteSpace(machineCode) ? "ALL" : machineCode;
            var metricTag = string.IsNullOrWhiteSpace(metricType) ? "ALL" : metricType;
            var fileName = $"sensor-live-{machineTag}-{metricTag}-{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            var bytes = package.GetAsByteArray();
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // ═══════════════════════════════════════════════════════════════
        //  API — DEVICE STATUS (online/offline per device)
        //  GET /api/sensor-readings/device-status
        // ═══════════════════════════════════════════════════════════════

        [HttpGet]
        [Route("api/sensor-readings/device-status")]
        public async Task<IActionResult> GetDeviceStatus()
        {
            // Periksa device aktif dalam 1 jam terakhir
            var lookback = DateTime.Now.AddHours(-1);
            var onlineCutoff = DateTime.Now.AddSeconds(-30); // 30 detik = online threshold

            var groups = await _context.SensorIngestLogs
                .Where(l => l.IngestedAt >= lookback)
                .GroupBy(l => new { l.DeviceId, l.MachineCode })
                .Select(g => new
                {
                    g.Key.DeviceId,
                    g.Key.MachineCode,
                    LastSeen   = g.Max(l => l.IngestedAt),
                    TotalToday = g.Count(l => l.IngestedAt.Date == DateTime.Today),
                    LastMetric = g.OrderByDescending(l => l.SensorTimestamp)
                                  .Select(l => l.MetricType).First(),
                    LastValue  = g.OrderByDescending(l => l.SensorTimestamp)
                                  .Select(l => l.MetricValue).First(),
                    LastUnit   = g.OrderByDescending(l => l.SensorTimestamp)
                                  .Select(l => l.Unit).First()
                })
                .ToListAsync();

            var result = groups.Select(d => new
            {
                d.DeviceId,
                d.MachineCode,
                IsOnline        = d.LastSeen >= onlineCutoff,
                LastSeen        = d.LastSeen.ToString("HH:mm:ss"),
                LastSeenSeconds = (int)(DateTime.Now - d.LastSeen).TotalSeconds,
                d.TotalToday,
                d.LastMetric,
                d.LastValue,
                d.LastUnit
            });

            return Json(result);
        }

        // ═══════════════════════════════════════════════════════════════
        //  API — METRICS SUMMARY per mesin, dalam N menit terakhir
        //  GET /api/sensor-readings/metrics-summary?machineCode=EXT-01&minutes=30
        // ═══════════════════════════════════════════════════════════════

        [HttpGet]
        [Route("api/sensor-readings/metrics-summary")]
        public async Task<IActionResult> GetMetricsSummary(
            string? machineCode = null,
            int     minutes     = 30)
        {
            var cutoff = DateTime.Now.AddMinutes(-minutes);
            var query  = _context.SensorIngestLogs
                .Where(l => l.SensorTimestamp >= cutoff && l.Status == "OK");

            if (!string.IsNullOrEmpty(machineCode))
                query = query.Where(l => l.MachineCode == machineCode);

            var groups = await query
                .GroupBy(l => new { l.MetricType, l.Unit })
                .Select(g => new
                {
                    g.Key.MetricType,
                    g.Key.Unit,
                    Latest = g.OrderByDescending(l => l.SensorTimestamp)
                               .Select(l => l.MetricValue).First(),
                    Min   = g.Min(l => l.MetricValue),
                    Max   = g.Max(l => l.MetricValue),
                    Avg   = g.Average(l => l.MetricValue),
                    Count = g.Count()
                })
                .ToListAsync();

            return Json(groups);
        }

        // ═══════════════════════════════════════════════════════════════
        //  API — SPARKLINE DATA untuk chart mini di monitor
        //  GET /api/sensor-readings/sparkline?deviceId=X&metricType=outer_diameter&points=20
        // ═══════════════════════════════════════════════════════════════

        [HttpGet]
        [Route("api/sensor-readings/sparkline")]
        public async Task<IActionResult> GetSparkline(
            string? deviceId   = null,
            string? machineCode = null,
            string? metricType = "outer_diameter",
            int     points     = 20)
        {
            var query = _context.SensorIngestLogs
                .Where(l => l.Status == "OK" && l.MetricType == metricType!.ToLower())
                .AsQueryable();

            if (!string.IsNullOrEmpty(deviceId))
                query = query.Where(l => l.DeviceId == deviceId);
            if (!string.IsNullOrEmpty(machineCode))
                query = query.Where(l => l.MachineCode == machineCode);

            var data = await query
                .OrderByDescending(l => l.SensorTimestamp)
                .Take(points)
                .Select(l => new
                {
                    x = l.SensorTimestamp.ToString("HH:mm:ss"),
                    y = l.MetricValue,
                    q = l.Quality
                })
                .ToListAsync();

            return Json(data.OrderBy(d => d.x).ToList());
        }

        // ═══════════════════════════════════════════════════════════════
        //  MVC PAGE — Analysis Chart UI
        //  GET /SensorReadings/Analysis
        // ═══════════════════════════════════════════════════════════════

        public async Task<IActionResult> Analysis()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                return RedirectToAction("Login", "Account");

            // Ambil daftar machine code yang ada di sensor log
            var machines = await _context.SensorIngestLogs
                .Where(l => l.MachineCode != null && l.MachineCode != "")
                .Select(l => l.MachineCode!)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();

            // Ambil semua SPS dari SpsNoDocs (join ke ItemList untuk ambil ItemList)
            // Hanya ambil yang punya DocumentNumber valid (bukan strip '-')
            var spsList = await _context.SpsItemLists
                .Include(i => i.SpsNoDoc)
                .Where(s => s.SpsNoDoc != null && s.SpsNoDoc.DocumentNumber != null && s.SpsNoDoc.DocumentNumber != "-")
                .Select(s => new {
                    Id = s.DocumentNumber,
                    s.SpsNoDoc!.DocumentNumber,
                    s.SpsNoDoc.No,
                    ItemList = s.ItemList,
                    s.SpsNoDoc.MachineCode,
                    s.SpsNoDoc.Customer,
                    s.SpsNoDoc.OdSensor_Asli, s.SpsNoDoc.OdSensor_Max, s.SpsNoDoc.OdSensor_Min,
                    s.SpsNoDoc.ControlValue_Asli, s.SpsNoDoc.ControlValue_Max, s.SpsNoDoc.ControlValue_Min,
                    s.SpsNoDoc.ToleranceInner_Min, s.SpsNoDoc.ToleranceInner_Asli, s.SpsNoDoc.ToleranceInner_Max,
                    s.SpsNoDoc.ToleranceOuter_Min, s.SpsNoDoc.ToleranceOuter_Asli, s.SpsNoDoc.ToleranceOuter_Max,
                    s.SpsNoDoc.ToleranceOuter,
                    s.SpsNoDoc.ToleranceInner,
                    s.SpsNoDoc.HoseSpeed_Asli, s.SpsNoDoc.HoseSpeed_Max, s.SpsNoDoc.HoseSpeed_Min,
                    s.SpsNoDoc.HeadTemp1_Asli, s.SpsNoDoc.HeadTemp1_Max, s.SpsNoDoc.HeadTemp1_Min,
                    s.SpsNoDoc.HeadTemp2_Asli, s.SpsNoDoc.HeadTemp2_Max, s.SpsNoDoc.HeadTemp2_Min,
                    s.SpsNoDoc.ScrewSpeed1_Asli, s.SpsNoDoc.ScrewSpeed1_Max, s.SpsNoDoc.ScrewSpeed1_Min,
                    s.SpsNoDoc.ScrewSpeed2_Asli, s.SpsNoDoc.ScrewSpeed2_Max, s.SpsNoDoc.ScrewSpeed2_Min,
                    s.SpsNoDoc.Pressure1_Asli, s.SpsNoDoc.Pressure1_Max, s.SpsNoDoc.Pressure1_Min,
                    s.SpsNoDoc.Pressure2_Asli, s.SpsNoDoc.Pressure2_Max, s.SpsNoDoc.Pressure2_Min,
                    s.SpsNoDoc.SpiralPitchSetting_Asli, s.SpsNoDoc.SpiralPitchSetting_Max, s.SpsNoDoc.SpiralPitchSetting_Min,
                    s.SpsNoDoc.ToleranceSpiralPitch,
                    s.SpsNoDoc.CaterpillarGap_Asli, s.SpsNoDoc.CaterpillarGap_Max, s.SpsNoDoc.CaterpillarGap_Min,
                    s.SpsNoDoc.ChillerWaterTemp_Asli, s.SpsNoDoc.ChillerWaterTemp_Max, s.SpsNoDoc.ChillerWaterTemp_Min,
                    s.SpsNoDoc.InnerTarget,
                    s.SpsNoDoc.InnerMin,
                    s.SpsNoDoc.InnerMax
                })
                .ToListAsync();

            ViewBag.Machines = machines;
            var jsonOpts = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };
            ViewBag.SpsList = System.Text.Json.JsonSerializer.Serialize(spsList, jsonOpts);
            return View();
        }

        // ═══════════════════════════════════════════════════════════════
        //  API — ANALYSIS DATA (sensor aktual + standar untuk chart)
        //  GET /api/sensor-readings/analysis-data
        //      ?machineCode=EXT-01&metricType=outer_diameter&startDate=2026-05-01&endDate=2026-05-02
        // ═══════════════════════════════════════════════════════════════

        [HttpGet]
        [Route("api/sensor-readings/analysis-data")]
        public async Task<IActionResult> GetAnalysisData(
            string? machineCode = null,
            string? metricType  = "outer_diameter",
            int     hours       = 8,
            DateTime? startDate = null,
            DateTime? endDate   = null,
            string? startTime   = "00:00", // Format HH:mm
            string? endTime     = "23:59", // Format HH:mm
            string? spsId       = null)
        {
            var now       = DateTime.Now;
            var fromTime  = startDate ?? now.AddHours(-hours);
            var toTime    = endDate ?? now;

            // Gabungkan Tanggal dengan Jam jika disediakan
            if (startDate.HasValue && !string.IsNullOrWhiteSpace(startTime))
            {
                if (TimeSpan.TryParse(startTime, out var startTs))
                {
                    fromTime = startDate.Value.Date.Add(startTs);
                }
            }

            if (endDate.HasValue && !string.IsNullOrWhiteSpace(endTime))
            {
                if (TimeSpan.TryParse(endTime, out var endTs))
                {
                    toTime = endDate.Value.Date.Add(endTs);
                }
            }
            else if (endDate.HasValue && endDate.Value.TimeOfDay == TimeSpan.Zero)
            {
                // Fallback: Jika input tanggal tanpa jam (00:00:00), anggap sampai akhir hari.
                toTime = endDate.Value.Date.AddDays(1).AddTicks(-1);
            }

            if (fromTime > toTime)
                return BadRequest("Rentang waktu tidak valid (Waktu awal > Waktu akhir).");

            var metricKey = (metricType ?? "outer_diameter").ToLower().Trim();

            // ── 1. Ambil data sensor aktual ──────────────────────────
            var metricKeys = new List<string> { metricKey };
            if (metricKey == "outer_diameter") metricKeys.Add("diameter");
            if (metricKey == "inner_diameter") metricKeys.Add("inner_dia");

            var query = _context.SensorIngestLogs
                .Where(l => l.Status == "OK"
                         && metricKeys.Contains(l.MetricType)
                         && l.SensorTimestamp >= fromTime
                         && l.SensorTimestamp <= toTime);

            if (!string.IsNullOrEmpty(machineCode))
                query = query.Where(l => l.MachineCode == machineCode);

            var sensorData = await query
                .OrderBy(l => l.SensorTimestamp)
                .Select(l => new
                {
                    x       = l.SensorTimestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    y       = l.MetricValue,
                    quality = l.Quality,
                    device  = l.DeviceId
                })
                .ToListAsync();

            // ── 2. Ambil standar — prioritas: spsId, lalu machineCode ─
            SpsNoDoc? spsNew = null;
            SpsNoDoc? spsOld = null;

            if (!string.IsNullOrWhiteSpace(spsId))
            {
                spsNew = await _context.SpsNoDocs
                    .FirstOrDefaultAsync(s => s.DocumentNumber == spsId);

                if (spsNew == null)
                {
                    spsNew = await _context.SpsNoDocs
                        .FirstOrDefaultAsync(s => s.DocumentNumber != null && s.DocumentNumber.Contains(spsId));
                }
            }
            else if (!string.IsNullOrEmpty(machineCode))
            {
                spsNew = await _context.SpsNoDocs
                    .Where(s => s.MachineCode == machineCode)
                    .OrderByDescending(s => s.DocumentNumber)
                    .FirstOrDefaultAsync();
                
                if (spsNew == null)
                    spsOld = await _context.SpsNoDocs
                        .Where(s => s.MachineCode == machineCode)
                        .OrderByDescending(s => s.DocumentNumber)
                        .FirstOrDefaultAsync();
            }

            object? standard = null;
            if (spsNew != null)      standard = BuildStandardFromNew(metricKey, spsNew);
            else if (spsOld != null) standard = BuildStandardFromOld(metricKey, spsOld);

            // ── 3. Hitung statistik ──────────────────────────────────
            object? stats = null;
            if (sensorData.Any())
            {
                var vals = sensorData.Select(d => d.y).ToList();
                var avg  = vals.Average();
                var stdDev = vals.Count > 1
                    ? Math.Sqrt(vals.Sum(v => Math.Pow((double)(v - (decimal)avg), 2)) / (vals.Count - 1))
                    : 0;
                stats = new
                {
                    count  = vals.Count,
                    min    = vals.Min(),
                    max    = vals.Max(),
                    avg    = Math.Round(avg, 4),
                    stdDev = Math.Round((decimal)stdDev, 4),
                    range  = vals.Max() - vals.Min()
                };
            }

            return Json(new
            {
                metricType = metricKey,
                machineCode,
                hours,
                rangeStart = fromTime,
                rangeEnd   = toTime,
                rangeLabel = $"{fromTime:dd/MM/yyyy HH:mm} - {toTime:dd/MM/yyyy HH:mm}",
                sensorData,
                standard,
                stats
            });
        }

        /// <summary>Petakan metricType ke field standar di SpsMaster (BARU - Min/Asli/Max)</summary>
        private static object? BuildStandardFromNew(string metricKey, SpsNoDoc sps)
        {
            decimal? target = null;
            decimal? ucl    = null;
            decimal? lcl    = null;
            string   label  = metricKey;
            string   unit   = "";

            switch (metricKey)
            {
                case "outer_diameter":
                    target = sps.ToleranceOuter_Asli ?? sps.OdSensor_Asli ?? sps.ControlValue_Asli;
                    ucl    = sps.ToleranceOuter_Max  ?? sps.OdSensor_Max  ?? sps.ControlValue_Max;
                    lcl    = sps.ToleranceOuter_Min  ?? sps.OdSensor_Min  ?? sps.ControlValue_Min;
                    
                    // Fallback to parsed text tolerance if explicit min/max are empty.
                    if (ucl == null && target != null)
                    {
                        var tol = sps.ToleranceOuter_Asli
                               ?? TryParseTolerance(sps.ToleranceOuter)
                               ?? TryParseTolerance(sps.OdSensor)
                               ?? TryParseTolerance(sps.ControlValue);
                        if (tol != null) { ucl = target + tol; lcl = target - tol; }
                    }
                    
                    label = "Outer Diameter"; unit = "mm"; break;
                case "inner_diameter":
                    target = sps.ToleranceInner_Asli ?? TryParseFirst(sps.InnerTarget);
                    ucl    = sps.ToleranceInner_Max ?? TryParseFirst(sps.InnerMax) ?? TryParseFirst(sps.InnerUCL);
                    lcl    = sps.ToleranceInner_Min ?? TryParseFirst(sps.InnerMin) ?? TryParseFirst(sps.InnerLCL);

                    // Fallback to parsed text tolerance if explicit min/max are empty
                    if (ucl == null && target != null)
                    {
                        var tol = sps.ToleranceInner_Asli ?? TryParseTolerance(sps.ToleranceInner);
                        if (tol != null)
                        {
                            ucl = target + tol;
                            lcl = target - tol;
                        }
                    }
                    label = "Inner Diameter"; unit = "mm"; break;
                case "hose_speed":
                    target = sps.HoseSpeed_Asli; ucl = sps.HoseSpeed_Max; lcl = sps.HoseSpeed_Min;
                    label = "Hose Speed"; unit = "m/min"; break;
                case "head_temp_inner":
                    target = sps.HeadTemp1_Asli; ucl = sps.HeadTemp1_Max; lcl = sps.HeadTemp1_Min;
                    label = "Head Temp Inner"; unit = "\u00b0C"; break;
                case "head_temp_outer":
                    target = sps.HeadTemp2_Asli; ucl = sps.HeadTemp2_Max; lcl = sps.HeadTemp2_Min;
                    label = "Head Temp Outer"; unit = "\u00b0C"; break;
                case "screw_speed_inner":
                    target = sps.ScrewSpeed1_Asli; ucl = sps.ScrewSpeed1_Max; lcl = sps.ScrewSpeed1_Min;
                    label = "Screw Speed Inner"; unit = "rpm"; break;
                case "screw_speed_outer":
                    target = sps.ScrewSpeed2_Asli; ucl = sps.ScrewSpeed2_Max; lcl = sps.ScrewSpeed2_Min;
                    label = "Screw Speed Outer"; unit = "rpm"; break;
                case "pressure_inner":
                    target = sps.Pressure1_Asli; ucl = sps.Pressure1_Max; lcl = sps.Pressure1_Min;
                    label = "Pressure Inner"; unit = "MPa"; break;
                case "pressure_outer":
                    target = sps.Pressure2_Asli; ucl = sps.Pressure2_Max; lcl = sps.Pressure2_Min;
                    label = "Pressure Outer"; unit = "MPa"; break;
                case "spiral_pitch":
                    target = sps.SpiralPitchSetting_Asli; ucl = sps.SpiralPitchSetting_Max; lcl = sps.SpiralPitchSetting_Min;
                    
                    // Fallback to ToleranceSpiralPitch (string parsing)
                    if (ucl == null && target != null && !string.IsNullOrEmpty(sps.ToleranceSpiralPitch))
                    {
                        var tol = TryParseTolerance(sps.ToleranceSpiralPitch);
                        if (tol != null) { ucl = target + tol; lcl = target - tol; }
                    }
                    label = "Spiral Pitch"; unit = "mm"; break;
                case "caterpillar_gap":
                    target = sps.CaterpillarGap_Asli; ucl = sps.CaterpillarGap_Max; lcl = sps.CaterpillarGap_Min;
                    label = "Caterpillar Gap"; unit = "mm"; break;
                case "chiller_water_temp":
                    target = sps.ChillerWaterTemp_Asli; ucl = sps.ChillerWaterTemp_Max; lcl = sps.ChillerWaterTemp_Min;
                    label = "Chiller Water Temp"; unit = "\u00b0C"; break;
            }

            if (target == null) return null;

            return new {
                target, ucl, lcl, label, unit,
                documentNumber = sps.DocumentNumber,
                itemList       = sps.DocumentNumber,
                machineCode    = sps.MachineCode,
            };
        }

        /// <summary>Petakan metricType ke field standar di SpsNoDoc (LAMA - legacy fallback)</summary>
        private static object? BuildStandardFromOld(string metricKey, SpsNoDoc sps)
        {
            decimal? target    = null;
            decimal? ucl       = null;
            decimal? lcl       = null;
            decimal? tolerance = null;
            string   label     = metricKey;
            string   unit      = "";

            switch (metricKey)
            {
                case "outer_diameter":
                    target    = TryParseFirst(sps.ToleranceOuter) ?? TryParseFirst(sps.OdSensor) ?? TryParseFirst(sps.ControlValue);
                    tolerance = TryParseTolerance(sps.ToleranceOuter)
                             ?? TryParseTolerance(sps.OdSensor)
                             ?? TryParseTolerance(sps.ControlValue);
                    label = "Outer Diameter"; unit = "mm";
                    break;
                case "inner_diameter":
                    target    = TryParseFirst(sps.ToleranceInner) ?? TryParseFirst(sps.InnerTarget);
                    tolerance = TryParseTolerance(sps.ToleranceInner) ?? TryParseTolerance(sps.InnerTol);
                    // Jika ada UCL/LCL eksplisit, gunakan langsung
                    if (sps.InnerUCL != null)
                        ucl = TryParseFirst(sps.InnerUCL);
                    if (sps.InnerLCL != null)
                        lcl = TryParseFirst(sps.InnerLCL);
                    label = "Inner Diameter"; unit = "mm";
                    break;
                case "hose_speed":
                    target    = TryParseFirst(sps.HoseSpeed);
                    tolerance = TryParseTolerance(sps.HoseSpeed);
                    label = "Hose Speed"; unit = "m/min";
                    break;
                case "head_temp_inner":
                    target    = TryParseFirst(sps.HeadTemp1);
                    tolerance = TryParseTolerance(sps.HeadTemp1) ?? 5;
                    label = "Head Temp Inner"; unit = "°C";
                    break;
                case "head_temp_outer":
                    target    = TryParseFirst(sps.HeadTemp2);
                    tolerance = TryParseTolerance(sps.HeadTemp2) ?? 5;
                    label = "Head Temp Outer"; unit = "°C";
                    break;
                case "screw_speed_inner":
                    target    = TryParseFirst(sps.ScrewSpeed1);
                    tolerance = TryParseTolerance(sps.ScrewSpeed1);
                    label = "Screw Speed Inner"; unit = "rpm";
                    break;
                case "screw_speed_outer":
                    target    = TryParseFirst(sps.ScrewSpeed2);
                    tolerance = TryParseTolerance(sps.ScrewSpeed2);
                    label = "Screw Speed Outer"; unit = "rpm";
                    break;
                case "pressure_inner":
                    target    = TryParseFirst(sps.Pressure1);
                    tolerance = TryParseTolerance(sps.Pressure1);
                    label = "Pressure Inner"; unit = "MPa";
                    break;
                case "pressure_outer":
                    target    = TryParseFirst(sps.Pressure2);
                    tolerance = TryParseTolerance(sps.Pressure2);
                    label = "Pressure Outer"; unit = "MPa";
                    break;
                case "spiral_pitch":
                    target    = TryParseFirst(sps.SpiralPitchSetting);
                    tolerance = TryParseTolerance(sps.ToleranceSpiralPitch);
                    label = "Spiral Pitch"; unit = "mm";
                    break;
                case "caterpillar_gap":
                    target    = TryParseFirst(sps.CaterpillarGap);
                    label = "Caterpillar Gap"; unit = "mm";
                    break;
                case "chiller_water_temp":
                    target    = TryParseFirst(sps.ChillerWaterTemp);
                    tolerance = 2;
                    label = "Chiller Water Temp"; unit = "°C";
                    break;
            }

            if (target == null) return null;

            // UCL/LCL: gunakan nilai eksplisit jika ada, fallback ke target±tolerance
            ucl ??= tolerance.HasValue ? target + tolerance : (decimal?)null;
            lcl ??= tolerance.HasValue ? target - tolerance : (decimal?)null;

            return new
            {
                target,
                ucl,
                lcl,
                tolerance,
                label,
                unit,
                documentNumber = sps.DocumentNumber,
                itemList       = sps.DocumentNumber,
                machineCode    = sps.MachineCode,
            };
        }

        /// <summary>Parse angka pertama dari string (mis. "25.4", "Ø 8.0±0.2mm", "3.5-4.5", "25.4±0.5")</summary>
        private static decimal? TryParseFirst(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            // Cari angka pertama (termasuk desimal) di mana saja dalam string
            var m = System.Text.RegularExpressions.Regex.Match(input,
                @"\d+([.,]\d+)?");
            if (!m.Success) return null;
            var clean = m.Value.Replace(',', '.');
            return decimal.TryParse(clean, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var val) ? val : null;
        }

        /// <summary>
        /// Parse toleransi dari string SPS.
        /// Contoh: "10.3±0.1" => 0.1, "±0.2" => 0.2, "0.1" => 0.1
        /// </summary>
        private static decimal? TryParseTolerance(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            var matches = System.Text.RegularExpressions.Regex.Matches(input, @"\d+([.,]\d+)?");
            if (matches.Count == 0) return null;

            // Format umum SPS: "target±tol" => ambil angka kedua sebagai toleransi.
            if (input.Contains('±') && matches.Count >= 2)
            {
                var tolText = matches[1].Value.Replace(',', '.');
                if (decimal.TryParse(tolText, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var tolVal))
                {
                    return tolVal;
                }
            }

            // Fallback: gunakan angka pertama (mis. "±0.1" atau "0.1").
            var first = matches[0].Value.Replace(',', '.');
            return decimal.TryParse(first, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var val) ? val : null;
        }

        // ═══════════════════════════════════════════════════════════════
        //  PRIVATE HELPERS
        // ═══════════════════════════════════════════════════════════════

        private bool ValidateApiKey(HttpRequest request)
        {
            var configuredKey = _configuration["SensorApi:ApiKey"];

            // Jika tidak dikonfigurasi (development), izinkan semua
            if (string.IsNullOrEmpty(configuredKey)) return true;

            var headerKey = request.Headers["X-Api-Key"].FirstOrDefault();
            var queryKey  = request.Query["apiKey"].FirstOrDefault();

            return headerKey == configuredKey || queryKey == configuredKey;
        }
    }
}
