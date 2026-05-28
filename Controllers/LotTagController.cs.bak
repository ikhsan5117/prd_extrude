using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Controllers
{
    public class LotTagController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LotTagController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LotTag
        public async Task<IActionResult> Index()
        {
            var lotTags = await _context.LotTags
                .Include(l => l.ProductionReport)
                .OrderByDescending(l => l.CreatedDate)
                .ToListAsync();
            return View(lotTags);
        }

        // GET: LotTag/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lotTag = await _context.LotTags
                .Include(l => l.ProductionReport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lotTag == null)
            {
                return NotFound();
            }

            return View(lotTag);
        }

        // GET: LotTag/Create
        public IActionResult Create()
        {
            ViewBag.ProductionReports = new SelectList(
                _context.ProductionReports.Where(p => p.Status != "Draft"),
                "Id",
                "DocumentNumber");

            var model = new LotTag
            {
                Plant = "2504 PT Velasto Mfg Fac 4 - Tango",
                Status = "Created",
                CreatedDate = DateTime.Now,
                PrintCount = 0
            };

            // Generate Lot Tag Number
            model.LotTagNumber = GenerateLotTagNumber();

            return View(model);
        }

        // POST: LotTag/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LotTagNumber,Plant,PartNumber,PartDescription,TargetQty,LotPackaging,CompoundCode,BomText,CompoundQty,NoLot,DaftarKomponen,CreatedBy,ProductionReportId")] LotTag lotTag)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    lotTag.CreatedDate = DateTime.Now;
                    lotTag.Status = "Created";
                    lotTag.PrintCount = 0;
                    lotTag.Barcode = lotTag.PartNumber;

                    _context.Add(lotTag);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Lot Tag berhasil dibuat!";
                    return RedirectToAction(nameof(Details), new { id = lotTag.Id });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Gagal menyimpan data: " + ex.Message);
            }

            ViewBag.ProductionReports = new SelectList(
                _context.ProductionReports.Where(p => p.Status != "Draft"),
                "Id",
                "DocumentNumber",
                lotTag.ProductionReportId);

            return View(lotTag);
        }

        // GET: LotTag/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lotTag = await _context.LotTags.FindAsync(id);
            if (lotTag == null)
            {
                return NotFound();
            }

            ViewBag.ProductionReports = new SelectList(
                _context.ProductionReports.Where(p => p.Status != "Draft"),
                "Id",
                "DocumentNumber",
                lotTag.ProductionReportId);

            return View(lotTag);
        }

        // POST: LotTag/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LotTagNumber,Plant,PartNumber,PartDescription,TargetQty,ActualQty,LotPackaging,CompoundCode,BomText,CompoundQty,NoLot,DaftarKomponen,SubconCheck,MesinCheck,TanggalCheck,QtyOK,QtyNG,NGReason,Status,ProductionReportId")] LotTag lotTag)
        {
            if (id != lotTag.Id)
            {
                return NotFound();
            }

            var existingLotTag = await _context.LotTags.FindAsync(id);
            if (existingLotTag == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update only modified fields to preserve CreatedDate, CreatedBy, etc.
                    existingLotTag.Plant = lotTag.Plant;
                    existingLotTag.PartNumber = lotTag.PartNumber;
                    existingLotTag.PartDescription = lotTag.PartDescription;
                    existingLotTag.TargetQty = lotTag.TargetQty;
                    existingLotTag.ActualQty = lotTag.ActualQty;
                    existingLotTag.LotPackaging = lotTag.LotPackaging;
                    existingLotTag.CompoundCode = lotTag.CompoundCode;
                    existingLotTag.BomText = lotTag.BomText;
                    existingLotTag.CompoundQty = lotTag.CompoundQty;
                    existingLotTag.NoLot = lotTag.NoLot;
                    existingLotTag.DaftarKomponen = lotTag.DaftarKomponen;
                    existingLotTag.SubconCheck = lotTag.SubconCheck;
                    existingLotTag.MesinCheck = lotTag.MesinCheck;
                    existingLotTag.TanggalCheck = lotTag.TanggalCheck;
                    existingLotTag.QtyOK = lotTag.QtyOK;
                    existingLotTag.QtyNG = lotTag.QtyNG;
                    existingLotTag.NGReason = lotTag.NGReason;
                    existingLotTag.Status = lotTag.Status;
                    existingLotTag.ProductionReportId = lotTag.ProductionReportId;
                    
                    // Update barcode based on PartNumber
                    existingLotTag.Barcode = lotTag.PartNumber;

                    _context.Update(existingLotTag);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Lot Tag berhasil diupdate!";
                    return RedirectToAction(nameof(Details), new { id = existingLotTag.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LotTagExists(lotTag.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Gagal mengupdate data: " + ex.Message);
                }
            }

            ViewBag.ProductionReports = new SelectList(
                _context.ProductionReports.Where(p => p.Status != "Draft"),
                "Id",
                "DocumentNumber",
                lotTag.ProductionReportId);

            return View(lotTag);
        }

        // GET: LotTag/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lotTag = await _context.LotTags
                .Include(l => l.ProductionReport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lotTag == null)
            {
                return NotFound();
            }

            return View(lotTag);
        }

        // POST: LotTag/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lotTag = await _context.LotTags.FindAsync(id);
            if (lotTag != null)
            {
                _context.LotTags.Remove(lotTag);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Lot Tag berhasil dihapus!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LotTagExists(int id)
        {
            return _context.LotTags.Any(e => e.Id == id);
        }

        // GET: LotTag/Print/5 - Cetak Lot Tag
        public async Task<IActionResult> Print(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lotTag = await _context.LotTags
                .Include(l => l.ProductionReport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lotTag == null)
            {
                return NotFound();
            }

            // Update print count dan tanggal
            lotTag.PrintCount++;
            lotTag.LastPrintedDate = DateTime.Now;
            if (lotTag.PrintedDate == null)
            {
                lotTag.PrintedDate = DateTime.Now;
            }
            _context.Update(lotTag);
            await _context.SaveChangesAsync();

            return View(lotTag);
        }

        // Helper method untuk generate Lot Tag Number
        private string GenerateLotTagNumber()
        {
            // Format: VH + YYMMDDhhmmss (e.g., VH2512042942)
            return "VH" + DateTime.Now.ToString("yyMMddHHmmss");
        }

        // GET: LotTag/UpdateQC/5 - Update Quality Check
        public async Task<IActionResult> UpdateQC(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lotTag = await _context.LotTags
                .Include(l => l.ProductionReport)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lotTag == null)
            {
                return NotFound();
            }

            return View(lotTag);
        }

        // POST: LotTag/UpdateQC/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQC(int id, [Bind("Id,SubconCheck,MesinCheck,TanggalCheck,QtyOK,QtyNG,NGReason,ActualQty")] LotTag lotTag)
        {
            if (id != lotTag.Id)
            {
                return NotFound();
            }

            var existingLotTag = await _context.LotTags.FindAsync(id);
            if (existingLotTag == null)
            {
                return NotFound();
            }

            // Update only QC fields
            existingLotTag.SubconCheck = lotTag.SubconCheck;
            existingLotTag.MesinCheck = lotTag.MesinCheck;
            existingLotTag.TanggalCheck = lotTag.TanggalCheck;
            existingLotTag.QtyOK = lotTag.QtyOK;
            existingLotTag.QtyNG = lotTag.QtyNG;
            existingLotTag.NGReason = lotTag.NGReason;
            existingLotTag.ActualQty = lotTag.ActualQty;

            // Update status jika sudah check
            if (lotTag.TanggalCheck != null)
            {
                existingLotTag.Status = "Completed";
            }

            try
            {
                _context.Update(existingLotTag);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Quality Check berhasil diupdate!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LotTagExists(lotTag.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Details), new { id = existingLotTag.Id });
        }
    }
}
