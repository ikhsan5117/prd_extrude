using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Controllers
{
    public class DimensionReadingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DimensionReadingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DimensionReading
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.DimensionReadings
                .Include(d => d.ProductionReport)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d =>
                    (d.ProductionReport != null && d.ProductionReport.HoseType != null && d.ProductionReport.HoseType.Contains(search)) ||
                    (d.PartNumber != null && d.PartNumber.Contains(search)) ||
                    (d.VinCode != null && d.VinCode.Contains(search)));
            }

            var readings = await query
                .OrderByDescending(d => d.ReadingTime)
                .ToListAsync();

            ViewBag.Search = search;
            return View(readings);
        }

        // GET: DimensionReading/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var reading = await _context.DimensionReadings
                .Include(d => d.ProductionReport)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reading == null) return NotFound();
            return View(reading);
        }

        // GET: DimensionReading/Create
        public IActionResult Create(int? productionReportId)
        {
            ViewBag.ProductionReports = new SelectList(
                _context.ProductionReports
                    .Where(p => p.Status == "NOW PRODUCING")
                    .OrderByDescending(p => p.ProductionDate)
                    .Select(p => new { p.Id, Name = $"{p.DocumentNumber} - {p.HoseType} ({p.ProductionDate:dd MMM yyyy})" }),
                "Id", "Name", productionReportId);

            var model = new DimensionReading
            {
                ReadingTime = DateTime.Now,
                ProductionReportId = productionReportId ?? 0
            };
            return View(model);
        }

        // POST: DimensionReading/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DimensionReading reading)
        {
            ModelState.Remove("ProductionReport");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(reading);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Data dimensi berhasil disimpan!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    var msg = ex.InnerException?.Message ?? ex.Message;
                    ModelState.AddModelError("", "Gagal simpan: " + msg);
                }
            }

            ViewBag.ProductionReports = new SelectList(
                _context.ProductionReports
                    .OrderByDescending(p => p.ProductionDate)
                    .Select(p => new { p.Id, Name = $"{p.DocumentNumber} - {p.HoseType} ({p.ProductionDate:dd MMM yyyy})" }),
                "Id", "Name", reading.ProductionReportId);

            return View(reading);
        }

        // GET: DimensionReading/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var reading = await _context.DimensionReadings.FindAsync(id);
            if (reading == null) return NotFound();

            ViewBag.ProductionReports = new SelectList(
                _context.ProductionReports
                    .OrderByDescending(p => p.ProductionDate)
                    .Select(p => new { p.Id, Name = $"{p.DocumentNumber} - {p.HoseType} ({p.ProductionDate:dd MMM yyyy})" }),
                "Id", "Name", reading.ProductionReportId);

            return View(reading);
        }

        // POST: DimensionReading/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DimensionReading reading)
        {
            if (id != reading.Id) return NotFound();

            ModelState.Remove("ProductionReport");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reading);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Data dimensi berhasil diupdate!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DimensionReadings.Any(e => e.Id == reading.Id)) return NotFound();
                    throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Gagal update: " + ex.Message);
                }
            }

            ViewBag.ProductionReports = new SelectList(
                _context.ProductionReports
                    .OrderByDescending(p => p.ProductionDate)
                    .Select(p => new { p.Id, Name = $"{p.DocumentNumber} - {p.HoseType} ({p.ProductionDate:dd MMM yyyy})" }),
                "Id", "Name", reading.ProductionReportId);

            return View(reading);
        }

        // POST: DimensionReading/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reading = await _context.DimensionReadings.FindAsync(id);
            if (reading != null)
            {
                _context.DimensionReadings.Remove(reading);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Data dimensi berhasil dihapus!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
