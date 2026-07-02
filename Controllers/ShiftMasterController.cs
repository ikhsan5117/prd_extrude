using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Controllers
{
    public class ShiftMasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShiftMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ShiftMaster
        public async Task<IActionResult> Index()
        {
            var shifts = await _context.ShiftMasters
                .OrderBy(s => s.ShiftName)
                .ToListAsync();
            return View(shifts);
        }

        // GET: ShiftMaster/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shiftMaster = await _context.ShiftMasters.FindAsync(id);
            if (shiftMaster == null)
            {
                return NotFound();
            }
            return View(shiftMaster);
        }

        // POST: ShiftMaster/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ShiftName,StartTime")] ShiftMaster shiftMaster)
        {
            if (id != shiftMaster.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.ShiftMasters.FindAsync(id);
                    if (existing != null)
                    {
                        existing.ShiftName = shiftMaster.ShiftName;
                        existing.StartTime = shiftMaster.StartTime;
                        existing.UpdatedAt = DateTime.Now;
                        _context.Update(existing);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Shift settings updated successfully!";
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShiftMasterExists(shiftMaster.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(shiftMaster);
        }

        // API Endpoint for frontend to fetch shift timings globally
        [HttpGet]
        public async Task<IActionResult> GetShifts()
        {
            var shifts = await _context.ShiftMasters
                .Select(s => new { s.ShiftName, s.StartTime })
                .ToListAsync();
            return Json(shifts);
        }

        private bool ShiftMasterExists(int id)
        {
            return _context.ShiftMasters.Any(e => e.Id == id);
        }
    }
}
