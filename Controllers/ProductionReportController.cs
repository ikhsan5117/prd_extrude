using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Controllers
{
    public class ProductionReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductionReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductionReport (Monitoring List)
        public async Task<IActionResult> Index()
        {
            var reports = await _context.ProductionReports
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
            return View(reports);
        }

        // GET: ProductionReport/Details/5 (The Monitoring Dashboard - Gambar 2 & 5)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var report = await _context.ProductionReports
                .Include(p => p.StandardParameterSetting)
                .Include(p => p.ProductionReadings)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (report == null) return NotFound();

            return View(report);
        }

        // GET: ProductionReport/Create (The "NOW I'M PRODUCE" Form - Gambar 1)
        public IActionResult Create()
        {
            return View(new ProductionReport 
            { 
                ProductionDate = DateTime.Today,
                DocumentNumber = "VI-SOP-PROD-131",
                Status = "NOW PRODUCING"
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
                report.CreatedDate = DateTime.Now;
                report.Status = "NOW PRODUCING";
                _context.Add(report);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Produksi Dimulai! Silakan monitor di Dashboard.";
                return RedirectToAction(nameof(Details), new { id = report.Id });
            }
            return View(report);
        }

        [HttpPost]
        public async Task<IActionResult> FinishProduction(int id, int qtyOk, int ngDim, int ngVis)
        {
            var report = await _context.ProductionReports.FindAsync(id);
            if (report != null)
            {
                report.QtyOk = qtyOk;
                report.NgDimension = ngDim;
                report.NgVisual = ngVis;
                report.Status = "COMPLETED";
                report.ProductionEndTime = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Produksi Selesai!";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetMasterByItem(string itemCode)
        {
            var master = await _context.MasterlistSpsDoubleLayers
                .FirstOrDefaultAsync(m => m.ItemList == itemCode);
            return Json(master);
        }
    }
}
