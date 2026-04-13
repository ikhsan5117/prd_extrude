using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            // Pastikan report exist
            var report = _context.DimensionReports.FirstOrDefault(r => r.HoseType == "NA2060");
            if (report == null)
            {
                report = new DimensionReport {
                    DocumentNumber = "DIM-NA2060-REF",
                    ProductionDate = DateTime.Now,
                    HoseType = "NA2060",
                    DimensionDisplay = "Thickness",
                    CustomerName = "VELASTO CORP",
                    Status = "COMPLETED",
                    CreatedDate = DateTime.Now,
                    CreatedBy = "System Seed",
                    Shift = "Shift 1",
                    QtyTarget = 100,
                    QtyOk = 95
                };
                _context.DimensionReports.Add(report);
                _context.SaveChanges();
            }

            var random = new Random();
            
            // Buat data simulasi untuk beberapa kriteria sekaligus
            var criteriaTypes = new[] { 
                new { Name = "Inner Tube", Target = 3.6m },
                new { Name = "Spiral Pitch", Target = 56.0m },
                new { Name = "Tebal Total", Target = 5.2m }
            };

            // Hapus data lama agar tidak duplikat
            var oldData = _context.DimensionMeasurements.Where(m => m.DimensionReportId == report.Id).ToList();
            if (oldData.Any()) _context.DimensionMeasurements.RemoveRange(oldData);

            foreach (var type in criteriaTypes)
            {
                for (int i = 54; i >= 1; i--)
                {
                    var dt = DateTime.Now.AddMinutes(-i * 30);
                    
                    var reading = new DimensionMeasurement {
                        DimensionReportId = report.Id,
                        PointName = type.Name,
                        TimeSection = dt.ToString("HH:mm"),
                        Frequency = "30m Sekali",
                        RecordedTime = dt,
                        // Simulasi nilai yang fluktuasi sedikit
                        R1 = type.Target + (decimal)((random.NextDouble() - 0.5) * (double)type.Target * 0.05),
                        R2 = type.Target + (decimal)((random.NextDouble() - 0.5) * (double)type.Target * 0.05),
                        Status = "OK"
                    };
                    _context.DimensionMeasurements.Add(reading);
                }
            }
            
            _context.SaveChanges();
            return Content("Data simulasi (Thickness, Diameter, & Spiral Pitch) untuk 'NA2060' berhasil ditambahkan! Silakan refresh dashboard.");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult GetSpcData(string productCode = "", string criteria = "Thickness", string timeRange = "30")
        {
            // Update to use DimensionMeasurements instead of ProductionReadings
            var query = _context.DimensionMeasurements
                .Include(r => r.DimensionReport)
                .AsQueryable();
            
            if (!string.IsNullOrEmpty(productCode)) {
                query = query.Where(r => r.DimensionReport != null && (r.DimensionReport.HoseType == productCode || r.DimensionReport.VinCode == productCode));
            }

            // Batas waktu
            if (int.TryParse(timeRange, out int days)) {
                var cutoff = DateTime.Now.AddDays(-days);
                query = query.Where(r => r.RecordedTime >= cutoff);
            }

            string pointNameFilter = criteria;

            // Apply criteria filtering consistently, even for "All Products"
            if (criteria == "InnerThickness" || criteria == "Thickness") {
                pointNameFilter = "Inner Tube";
                query = query.Where(r => r.PointName.Contains("Thickness") || r.PointName.Contains("Inner") || r.PointName.Contains("Tebal"));
            } else if (criteria == "Diameter") {
                pointNameFilter = "Inner Diameter";
                query = query.Where(r => r.PointName.Contains("Diameter") || r.PointName.Contains("Inner Tube"));
            } else if (criteria == "TotalThickness") {
                pointNameFilter = "Tebal Total";
                query = query.Where(r => r.PointName.Contains("Total") || r.PointName.Contains("Outer") || r.PointName.Contains("Tebal"));
            } else if (criteria == "SpiralPitch") {
                pointNameFilter = "Spiral Pitch";
                query = query.Where(r => r.PointName.Contains("Spiral") || r.PointName.Contains("Pitch"));
            } else if (!string.IsNullOrEmpty(criteria)) {
                query = query.Where(r => r.PointName.Contains(criteria));
            }

            var readings = query
                .OrderBy(r => r.RecordedTime)
                .ToList();

            // Ekstrak nilai berdasarkan kriteria (R1 to R5)
            List<decimal> valuesA = new List<decimal>();
            List<decimal> valuesB = new List<decimal>();
            
            foreach (var r in readings) {
                // Dimensi usually has R1-R5. For SPC we can take R1 as main series
                if (r.R1.HasValue) valuesA.Add(r.R1.Value);
                if (r.R2.HasValue) valuesB.Add(r.R2.Value);
            }

            var allValues = valuesA.Concat(valuesB).ToList();
            if (!allValues.Any()) {
                // Jika tidak ada R1/R2, coba cek R1 saja
                if (valuesA.Any()) allValues = valuesA;
                else return Json(new { success = false, message = "No data available for criteria: " + pointNameFilter });
            }

            // Kalkulasi Statistik
            double avg = (double)allValues.Average();
            double sumOfSquares = allValues.Sum(v => Math.Pow((double)v - avg, 2));
            double stdDev = Math.Sqrt(sumOfSquares / (allValues.Count > 1 ? allValues.Count - 1 : 1));
            if (stdDev == 0) stdDev = 0.001; 

            // Ambil data SPS untuk toleransi (MasterlistSpsDoubleLayers)
            var sps = _context.MasterlistSpsDoubleLayers
                .FirstOrDefault(s => s.HoseType == productCode || s.ItemList == productCode);

            double target = avg;
            double usl = target + 0.4;
            double lsl = target - 0.4;

            if (sps != null) {
                if (criteria == "InnerThickness" || pointNameFilter == "Inner Tube") {
                    if (decimal.TryParse(sps.TebalInner, out var t)) target = (double)t;
                    if (decimal.TryParse(sps.ToleranceInner, out var tol)) { usl = target + (double)tol; lsl = target - (double)tol; }
                } else if (criteria == "TotalThickness" || pointNameFilter == "Tebal Total") {
                    if (decimal.TryParse(sps.TebalTotal, out var t)) target = (double)t;
                    if (decimal.TryParse(sps.ToleranceOuter, out var tol)) { usl = target + (double)tol; lsl = target - (double)tol; }
                } else if (criteria == "Diameter" || pointNameFilter == "Inner Diameter") {
                    if (decimal.TryParse(sps.InnerTube, out var t)) target = (double)t;
                    if (decimal.TryParse(sps.ToleranceInner, out var tol)) { usl = target + (double)tol; lsl = target - (double)tol; }
                } else if (criteria == "SpiralPitch") {
                    // Spiral pitch logic
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
                    labels = readings.Select(r => r.RecordedTime.ToString("dd/MM HH:mm")),
                    seriesA = valuesA,
                    seriesB = valuesB,
                    uslLine = usl,
                    lslLine = lsl,
                    targetLine = target,
                    totalCount = allValues.Count
                }
            });
        }

        public IActionResult GetSpcProducts()
        {
            // Hanya ambil produk yang benar-benar sudah ada datanya di laporan dimensi
            var products = _context.DimensionReports
                .Where(p => !string.IsNullOrEmpty(p.HoseType))
                .Select(p => new { value = p.HoseType, text = p.HoseType })
                .Distinct()
                .OrderBy(p => p.text)
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
