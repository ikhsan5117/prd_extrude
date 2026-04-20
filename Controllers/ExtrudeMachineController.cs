using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Controllers
{
    public class ExtrudeMachineController : Controller
    {
        private readonly ElwpDbContext _context;

        public ExtrudeMachineController(ElwpDbContext context)
        {
            _context = context;
        }

        // GET: ExtrudeMachine
        public async Task<IActionResult> Index()
        {
            // Filter area Extrude (ID 1) dan TPE (ID 10)
            var machines = await _context.ElwpMachines
                .Include(m => m.Area)
                .Where(m => m.AreaId == 1 || m.AreaId == 10)
                .OrderBy(m => m.AreaId)
                .ThenBy(m => m.KodeMesin)
                .ToListAsync();
            
            return View(machines);
        }

        // GET: ExtrudeMachine/Create
        public IActionResult Create()
        {
            return View(new ElwpMachine { AreaId = 1, PlantId = 1, IsActive = true });
        }

        // POST: ExtrudeMachine/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ElwpMachine machine)
        {
            if (ModelState.IsValid)
            {
                machine.CreatedAt = DateTime.Now;
                _context.Add(machine);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Mesin baru berhasil ditambahkan!";
                return RedirectToAction(nameof(Index));
            }
            return View(machine);
        }

        // GET: ExtrudeMachine/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var machine = await _context.ElwpMachines.FindAsync(id);
            if (machine == null) return NotFound();

            return View(machine);
        }

        // POST: ExtrudeMachine/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ElwpMachine machine)
        {
            if (id != machine.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.ElwpMachines.FindAsync(id);
                    if (existing == null) return NotFound();

                    existing.KodeMesin = machine.KodeMesin;
                    existing.NamaMesin = machine.NamaMesin;
                    existing.Keterangan = machine.Keterangan;
                    existing.AreaId = machine.AreaId;
                    existing.IsActive = machine.IsActive;
                    existing.Kapasitas = machine.Kapasitas;
                    existing.RequiredManPower = machine.RequiredManPower;
                    existing.UpdatedAt = DateTime.Now;

                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Data mesin berhasil diperbarui!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MachineExists(machine.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(machine);
        }

        // POST: ExtrudeMachine/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var machine = await _context.ElwpMachines.FindAsync(id);
            if (machine != null)
            {
                _context.ElwpMachines.Remove(machine);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Mesin berhasil dihapus!" });
            }
            return Json(new { success = false, message = "Mesin tidak ditemukan!" });
        }

        private bool MachineExists(int id)
        {
            return _context.ElwpMachines.Any(e => e.Id == id);
        }
    }
}
