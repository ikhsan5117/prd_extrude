using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Controllers
{
    public class NowProducingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NowProducingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: NowProducing
        public async Task<IActionResult> Index()
        {
            var productions = await _context.NowProducings
                .OrderByDescending(n => n.ProductionDate)
                .ToListAsync();
            return View(productions);
        }

        // GET: NowProducing/Current - Menampilkan produksi yang sedang berjalan
        public async Task<IActionResult> Current()
        {
            var currentProduction = await _context.NowProducings
                .Where(n => n.ProductionEndTime == null)
                .OrderByDescending(n => n.ProductionDate)
                .FirstOrDefaultAsync();

            // Pass null to view - view handles empty state with a nice UI
            return View(currentProduction);
        }

        // GET: NowProducing/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nowProducing = await _context.NowProducings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nowProducing == null)
            {
                return NotFound();
            }

            return View(nowProducing);
        }

        // GET: NowProducing/Create
        public IActionResult Create()
        {
            var model = new NowProducing
            {
                ProductionDate = DateTime.Today,
                ProductionStartTime = DateTime.Now
            };
            return View(model);
        }

        // POST: NowProducing/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductionDate,HoseType,Class,Dimension,Yarn,MaterialInner,MaterialInnerLotNo,MaterialInnerSG,MaterialMiddle,MaterialMiddleLotNo,MaterialMiddleSG,MaterialOuter,MaterialOuterLotNo,MaterialOuterSG,DandoriStartProdTime,DandoriEndProdTime,ProductionStartTime,CHS2LTargetWaktu,CHS2LTargetDH,CHS2LTargetMaterial,CHS2LTargetBenang,CHS3LTargetWaktu,CHS3LTargetDH,CHS3LTargetMaterial,CHS3LTargetBenang,SPVCheck")] NowProducing nowProducing)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nowProducing);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Produksi baru berhasil dimulai!";
                return RedirectToAction(nameof(Current));
            }
            return View(nowProducing);
        }

        // GET: NowProducing/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nowProducing = await _context.NowProducings.FindAsync(id);
            if (nowProducing == null)
            {
                return NotFound();
            }
            return View(nowProducing);
        }

        // POST: NowProducing/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductionDate,HoseType,Class,Dimension,Yarn,MaterialInner,MaterialInnerLotNo,MaterialInnerSG,MaterialMiddle,MaterialMiddleLotNo,MaterialMiddleSG,MaterialOuter,MaterialOuterLotNo,MaterialOuterSG,DandoriStartProdTime,DandoriEndProdTime,ProductionStartTime,ProductionEndTime,CHS2LTargetWaktu,CHS2LTargetDH,CHS2LTargetMaterial,CHS2LTargetBenang,CHS3LTargetWaktu,CHS3LTargetDH,CHS3LTargetMaterial,CHS3LTargetBenang,SPVCheck")] NowProducing nowProducing)
        {
            if (id != nowProducing.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nowProducing);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Data produksi berhasil diupdate!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NowProducingExists(nowProducing.Id))
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
            return View(nowProducing);
        }

        // POST: NowProducing/Complete/5 - Mengakhiri produksi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var nowProducing = await _context.NowProducings.FindAsync(id);
            if (nowProducing == null)
            {
                return NotFound();
            }

            nowProducing.ProductionEndTime = DateTime.Now;
            _context.Update(nowProducing);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Produksi berhasil diselesaikan!";

            return RedirectToAction(nameof(Index));
        }

        // GET: NowProducing/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nowProducing = await _context.NowProducings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nowProducing == null)
            {
                return NotFound();
            }

            return View(nowProducing);
        }

        // POST: NowProducing/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nowProducing = await _context.NowProducings.FindAsync(id);
            if (nowProducing != null)
            {
                _context.NowProducings.Remove(nowProducing);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Data produksi berhasil dihapus!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool NowProducingExists(int id)
        {
            return _context.NowProducings.Any(e => e.Id == id);
        }

        // GET: NowProducing/Print/5 - Cetak form Now Producing
        public async Task<IActionResult> Print(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nowProducing = await _context.NowProducings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nowProducing == null)
            {
                return NotFound();
            }

            return View(nowProducing);
        }
    }
}
