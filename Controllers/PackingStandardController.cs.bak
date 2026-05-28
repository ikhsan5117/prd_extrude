using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Controllers
{
    public class PackingStandardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PackingStandardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PackingStandard
        public async Task<IActionResult> Index()
        {
            var standards = await _context.PackingStandards
                .Where(p => p.IsActive)
                .OrderBy(p => p.NACode)
                .ToListAsync();
            return View(standards);
        }

        // GET: PackingStandard/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var standard = await _context.PackingStandards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (standard == null)
            {
                return NotFound();
            }

            return View(standard);
        }

        // GET: PackingStandard/Create
        public IActionResult Create()
        {
            var model = new PackingStandard
            {
                EffectiveDate = DateTime.Today,
                IsActive = true,
                CreatedDate = DateTime.Now
            };
            return View(model);
        }

        // POST: PackingStandard/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NACode,MaterialName,PartNumber,VinCode,Dandori,DH,StdQty,ActualQty,Remarks,EffectiveDate,CreatedBy")] PackingStandard standard)
        {
            if (ModelState.IsValid)
            {
                standard.CreatedDate = DateTime.Now;
                standard.IsActive = true;
                _context.Add(standard);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Packing Standard berhasil dibuat!";
                return RedirectToAction(nameof(Index));
            }
            return View(standard);
        }

        // GET: PackingStandard/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var standard = await _context.PackingStandards.FindAsync(id);
            if (standard == null)
            {
                return NotFound();
            }
            return View(standard);
        }

        // POST: PackingStandard/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NACode,MaterialName,PartNumber,VinCode,Dandori,DH,StdQty,ActualQty,Remarks,EffectiveDate,IsActive")] PackingStandard standard)
        {
            if (id != standard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingStandard = await _context.PackingStandards.FindAsync(id);
                    if (existingStandard != null)
                    {
                        existingStandard.NACode = standard.NACode;
                        existingStandard.MaterialName = standard.MaterialName;
                        existingStandard.PartNumber = standard.PartNumber;
                        existingStandard.VinCode = standard.VinCode;
                        existingStandard.Dandori = standard.Dandori;
                        existingStandard.DH = standard.DH;
                        existingStandard.StdQty = standard.StdQty;
                        existingStandard.ActualQty = standard.ActualQty;
                        existingStandard.Remarks = standard.Remarks;
                        existingStandard.EffectiveDate = standard.EffectiveDate;
                        existingStandard.IsActive = standard.IsActive;

                        _context.Update(existingStandard);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Packing Standard berhasil diupdate!";
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PackingStandardExists(standard.Id))
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
            return View(standard);
        }

        // GET: PackingStandard/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var standard = await _context.PackingStandards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (standard == null)
            {
                return NotFound();
            }

            return View(standard);
        }

        // POST: PackingStandard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var standard = await _context.PackingStandards.FindAsync(id);
            if (standard != null)
            {
                // Soft delete
                standard.IsActive = false;
                _context.Update(standard);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Packing Standard berhasil dihapus!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PackingStandardExists(int id)
        {
            return _context.PackingStandards.Any(e => e.Id == id);
        }

        // GET: PackingStandard/Search - Search by NA Code or Part Number
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction(nameof(Index));
            }

            var standards = await _context.PackingStandards
                .Where(p => p.IsActive && 
                    (p.NACode.Contains(searchTerm) || 
                     p.PartNumber.Contains(searchTerm) ||
                     p.MaterialName.Contains(searchTerm)))
                .OrderBy(p => p.NACode)
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            return View("Index", standards);
        }

        // GET: PackingStandard/Print - Cetak daftar packing standard
        public async Task<IActionResult> Print()
        {
            var standards = await _context.PackingStandards
                .Where(p => p.IsActive)
                .OrderBy(p => p.NACode)
                .ToListAsync();
            return View(standards);
        }
    }
}
