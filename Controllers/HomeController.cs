using Microsoft.AspNetCore.Mvc;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;
using System.Diagnostics;

namespace VelastoProductionSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Dashboard dengan statistik produksi
            ViewBag.TotalProductions = _context.ProductionReports.Count();
            ViewBag.ActiveProductions = _context.ProductionReports
                .Where(p => p.Status == "InProgress").Count();
            ViewBag.TodayProductions = _context.ProductionReports
                .Where(p => p.ProductionDate.Date == DateTime.Today).Count();
            ViewBag.TotalLotTags = _context.LotTags.Count();

            return View();
        }

        public IActionResult SeedSpcData()
        {
            // Ensure report with hose type DEMO exist
            var report = _context.ProductionReports.FirstOrDefault(r => r.HoseType == "DEMO-PART");
            if (report == null)
            {
                report = new ProductionReport {
                    DocumentNumber = "DEMO-DOC-01",
                    ProductionDate = DateTime.Today,
                    HoseType = "DEMO-PART",
                    CustomerName = "VELASTO CORP",
                    Status = "COMPLETED",
                    QtyTarget = 100,
                    QtyOk = 95,
                    NgDimension = 3,
                    NgVisual = 2
                };
                _context.ProductionReports.Add(report);
                _context.SaveChanges();
            }

            // Create SPS for demo if none
            var sps = _context.StandardParameterSettings.FirstOrDefault(s => s.ProductCode == "DEMO-PART");
            if (sps == null)
            {
                sps = new StandardParameterSetting {
                    ProductCode = "DEMO-PART",
                    ItemList = "DEMO-PART",
                    TubeDie = 1.6m,
                    Tol_TubeDie = 0.2m,
                    OuterDie = 2.8m,
                    Tol_OuterDie = 0.3m,
                    InnerDie = 8.8m,
                    ToleranceDie = 0.4m,
                    SpiralPitch = 14.0m,
                    Tol_SpiralPitch = 0.5m,
                    EffectiveDate = DateTime.Today
                };
                _context.StandardParameterSettings.Add(sps);
                _context.SaveChanges();
            }

            // Dates 1, 2, 3, 4, 5 and today
            var random = new Random();
            var days = new[] { 1, 2, 3, 4, 5, DateTime.Today.Day };
            
            foreach (var day in days)
            {
                var dt = new DateTime(DateTime.Today.Year, DateTime.Today.Month, day, 10, 0, 0);
                
                // Add 2 random readings per day
                for (int i = 0; i < 2; i++)
                {
                    var reading = new ProductionReading {
                        ProductionReportId = report.Id,
                        ReadingTime = dt.AddHours(i * 3),
                        InnerDiameter = 8.0m + (decimal)(random.NextDouble() * 1.5),
                        InnerThicknessX = 1.0m + (decimal)(random.NextDouble() * 0.8),
                        InnerThicknessY = 1.0m + (decimal)(random.NextDouble() * 0.8),
                        TotalThicknessX = 2.0m + (decimal)(random.NextDouble() * 1.2),
                        TotalThicknessY = 2.0m + (decimal)(random.NextDouble() * 1.2),
                        SpiralPitch = 10.0m + (decimal)(random.NextDouble() * 5.0),
                        RecordedBy = "System Seed"
                    };
                    _context.ProductionReadings.Add(reading);
                }
            }
            
            _context.SaveChanges();
            return Content("Data berhasil ditambahkan untuk 'DEMO-PART'. Silakan refresh dashboard dan pilih produk 'DEMO-PART' atau 'Semua Produk'.");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult GetSpcData(string productCode = "", string criteria = "InnerThickness", string timeRange = "30")
        {
            // Ambil data untuk dimensi yang dipilih
            // Filter berdasarkan produk jika produk dipilih (NA2060 dll)
            var query = _context.ProductionReadings.AsQueryable();
            
            if (!string.IsNullOrEmpty(productCode)) {
                query = query.Where(r => r.ProductionReport != null && (r.ProductionReport.HoseType == productCode || r.ProductionReport.VinCode == productCode));
            }

            // Batas waktu
            if (int.TryParse(timeRange, out int days)) {
                var cutoff = DateTime.Now.AddDays(-days);
                query = query.Where(r => r.ReadingTime >= cutoff);
            }

            var readings = query.OrderBy(r => r.ReadingTime).ToList();

            // Ekstrak nilai berdasarkan kriteria
            List<decimal> valuesA = new List<decimal>();
            List<decimal> valuesB = new List<decimal>();
            
            foreach (var r in readings) {
                if (criteria == "InnerThickness") {
                    if (r.InnerThicknessX.HasValue) valuesA.Add(r.InnerThicknessX.Value);
                    if (r.InnerThicknessY.HasValue) valuesB.Add(r.InnerThicknessY.Value);
                } else if (criteria == "TotalThickness") {
                    if (r.TotalThicknessX.HasValue) valuesA.Add(r.TotalThicknessX.Value);
                    if (r.TotalThicknessY.HasValue) valuesB.Add(r.TotalThicknessY.Value);
                } else if (criteria == "Diameter") {
                    if (r.InnerDiameter.HasValue) valuesA.Add(r.InnerDiameter.Value);
                } else if (criteria == "SpiralPitch") {
                   if (r.SpiralPitch.HasValue) valuesA.Add(r.SpiralPitch.Value);
                }
            }

            var allValues = valuesA.Concat(valuesB).ToList();
            if (!allValues.Any()) {
                // Return empty if no data
                return Json(new { success = false, message = "No data available for criteria" });
            }

            // Kalkulasi Statistik
            double avg = (double)allValues.Average();
            double sumOfSquares = allValues.Sum(v => Math.Pow((double)v - avg, 2));
            double stdDev = Math.Sqrt(sumOfSquares / allValues.Count);
            if (stdDev == 0) stdDev = 0.001; 

            // Ambil data SPS untuk toleransi asli
            var sps = _context.StandardParameterSettings
                .FirstOrDefault(s => s.ProductCode == productCode || s.ItemList == productCode);

            double target = avg;
            double usl = target + 0.4;
            double lsl = target - 0.4;

            if (sps != null) {
                if (criteria == "InnerThickness") {
                    target = (double)sps.TubeDie;
                    usl = target + (double)sps.Tol_TubeDie;
                    lsl = target - (double)sps.Tol_TubeDie;
                } else if (criteria == "TotalThickness") {
                    target = (double)sps.OuterDie;
                    usl = target + (double)sps.Tol_OuterDie;
                    lsl = target - (double)sps.Tol_OuterDie;
                } else if (criteria == "Diameter") {
                    target = (double)sps.InnerDie;
                    usl = target + (double)sps.ToleranceDie;
                    lsl = target - (double)sps.ToleranceDie;
                } else if (criteria == "SpiralPitch") {
                    target = (double)(sps.SpiralPitch);
                    usl = target + (double)sps.Tol_SpiralPitch;
                    lsl = target - (double)sps.Tol_SpiralPitch;
                }
            }

            // CP & CPK
            double cp = (usl - lsl) / (6 * stdDev);
            double cpu = (usl - avg) / (3 * stdDev);
            double cpl = (avg - lsl) / (3 * stdDev);
            double cpk = Math.Min(cpu, cpl);

            return Json(new {
                success = true,
                summary = new {
                    avg = Math.Round(avg, 3),
                    stdDev = Math.Round(stdDev, 3),
                    cp = Math.Round(cp, 2),
                    cpk = Math.Round(cpk, 2),
                    target = Math.Round(target, 2),
                    usl = usl,
                    lsl = lsl,
                    statusText = cpk > 1.33 ? "Excellent" : (cpk > 1.0 ? "Capable" : "Not Capable")
                },
                chart = new {
                    labels = readings.Select(r => r.ReadingTime.ToString("dd/MM HH:mm")),
                    seriesA = valuesA,
                    seriesB = valuesB,
                    uslLine = usl,
                    lslLine = lsl,
                    targetLine = target
                }
            });
        }

        public IActionResult GetSpcProducts()
        {
            var products = _context.PartMasters
                .Select(p => new { value = p.PartCode, text = p.PartCode + " - " + p.Description })
                .Distinct()
                .ToList();
            return Json(products);
        }

        public IActionResult GetDashboardPieData()
        {
            var today = DateTime.Today;
            var todayReports = _context.ProductionReports
                .Where(p => p.ProductionDate.Date == today)
                .ToList();

            int totalOk = todayReports.Sum(r => r.QtyOk);
            int totalNgDimension = todayReports.Sum(r => r.NgDimension);
            int totalNgVisual = todayReports.Sum(r => r.NgVisual);

            // Jika tidak ada data hari ini, ambil data total untuk demo
            if (totalOk + totalNgDimension + totalNgVisual == 0)
            {
                var allReports = _context.ProductionReports.ToList();
                totalOk = allReports.Sum(r => r.QtyOk);
                totalNgDimension = allReports.Sum(r => r.NgDimension);
                totalNgVisual = allReports.Sum(r => r.NgVisual);
            }

            return Json(new {
                labels = new[] { "Units OK", "NG Dimension", "NG Visual" },
                datasets = new[] {
                    new {
                        data = new[] { totalOk, totalNgDimension, totalNgVisual },
                        backgroundColor = new[] { "#10b981", "#f59e0b", "#ef4444" },
                        borderWidth = 0
                    }
                }
            });
        }

        public IActionResult GetDashboardData()
        {
            // Ambil data produksi terakhir untuk trend
            // Kita ambil data dari ProductionReadings yang digabung dengan ProductionReport
            var readings = _context.ProductionReadings
                .OrderByDescending(r => r.ReadingTime)
                .Take(20)
                .Select(r => new {
                    time = r.ReadingTime.ToString("HH:mm"),
                    temp = r.HeadTempInner,
                    speed = r.HoseSpeed,
                    thickness = r.InnerDiameter
                })
                .ToList();

            readings.Reverse(); // Urutkan dari terlama ke terbaru untuk chart

            return Json(new {
                labels = readings.Select(r => r.time),
                datasets = new object[] {
                    new {
                        label = "Head Temp Inner (°C)",
                        data = readings.Select(r => (decimal?)r.temp),
                        borderColor = "#3b82f6",
                        backgroundColor = "rgba(59, 130, 246, 0.1)",
                    },
                    new {
                        label = "Hose Speed (m/min)",
                        data = readings.Select(r => r.speed),
                        borderColor = "#10b981",
                        backgroundColor = "rgba(16, 185, 129, 0.1)",
                    }
                }
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
