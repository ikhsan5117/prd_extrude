using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Controllers
{
    public class ExtrudeUserController : Controller
    {
        private readonly ElwpDbContext _context;

        public ExtrudeUserController(ElwpDbContext context)
        {
            _context = context;
        }

        // GET: ExtrudeUser
        public async Task<IActionResult> Index()
        {
            // Filter hanya area Extrude (ID 1)
            var users = await _context.ElwpUsers
                .Include(u => u.Area)
                .Where(u => u.AreaId == 1) 
                .ToListAsync();
            
            return View(users);
        }

        // GET: ExtrudeUser/Create
        public IActionResult Create()
        {
            return View(new ElwpUser { AreaId = 1, IsActive = true, PlantId = 1 });
        }

        // POST: ExtrudeUser/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ElwpUser user)
        {
            if (ModelState.IsValid)
            {
                user.CreatedAt = DateTime.Now;
                _context.Add(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User Extrude berhasil ditambahkan!";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: ExtrudeUser/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.ElwpUsers.FindAsync(id);
            if (user == null || user.AreaId != 1) return NotFound();

            return View(user);
        }

        // POST: ExtrudeUser/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ElwpUser user)
        {
            if (id != user.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.ElwpUsers.FindAsync(id);
                    if (existingUser == null) return NotFound();

                    existingUser.FullName = user.FullName;
                    existingUser.Username = user.Username;
                    existingUser.NPK = user.NPK;
                    existingUser.Email = user.Email;
                    existingUser.Role = user.Role;
                    existingUser.IsActive = user.IsActive;

                    if (!string.IsNullOrEmpty(user.PasswordHash))
                    {
                        existingUser.PasswordHash = user.PasswordHash; // Note: In production use hashing
                    }

                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Data user berhasil diperbarui!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // POST: ExtrudeUser/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.ElwpUsers.FindAsync(id);
            if (user != null && user.AreaId == 1)
            {
                _context.ElwpUsers.Remove(user);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "User berhasil dihapus!" });
            }
            return Json(new { success = false, message = "User tidak ditemukan!" });
        }

        private bool UserExists(int id)
        {
            return _context.ElwpUsers.Any(e => e.Id == id);
        }
    }
}
