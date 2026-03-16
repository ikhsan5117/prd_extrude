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

        public IActionResult Privacy()
        {
            return View();
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
