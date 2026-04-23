using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;
using VelastoProductionSystem.Hubs;

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
            ViewBag.ApiKey = configuredKey;
            ViewBag.IngestUrl = $"{Request.Scheme}://{Request.Host}/api/sensor-readings/ingest";
            ViewBag.BatchUrl  = $"{Request.Scheme}://{Request.Host}/api/sensor-readings/ingest/batch";

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
            _ = _hubContext.Clients.All.SendAsync("ReceiveUpdate");

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
                _ = _hubContext.Clients.All.SendAsync("ReceiveUpdate");
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
