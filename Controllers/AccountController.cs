using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ElwpDbContext _context;

        public AccountController(ElwpDbContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public async Task<IActionResult> Login()
        {
            // Jika sudah ada session, tampilkan status saat ini (opsional) atau redirect
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Users = await _context.ElwpUsers
                .Where(u => u.IsActive && u.AreaId == 1) // Hanya area Extrude
                .OrderBy(u => u.FullName)
                .ToListAsync();

            ViewBag.Machines = await _context.ElwpMachines
                .Where(m => m.IsActive && (m.AreaId == 1 || m.AreaId == 10))
                .OrderBy(m => m.KodeMesin)
                .ToListAsync();

            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(int userId, int machineId, string password)
        {
            // Validasi password (case-insensitive extrude123!)
            if (string.IsNullOrEmpty(password) || password.ToLower() != "extrude123!")
            {
                ModelState.AddModelError("", "Password salah! (Hint: extrude123!)");
                await PrepareLoginViewData();
                return View();
            }

            var user = await _context.ElwpUsers.FindAsync(userId);
            var machine = await _context.ElwpMachines.FindAsync(machineId);

            if (user == null || machine == null)
            {
                ModelState.AddModelError("", "User atau Mesin tidak valid.");
                await PrepareLoginViewData();
                return View();
            }

            // Simpan ke Session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.FullName ?? user.Username ?? "");
            HttpContext.Session.SetInt32("MachineId", machine.Id);
            HttpContext.Session.SetString("MachineName", machine.KodeMesin ?? machine.NamaMesin ?? "");

            // Langsung arahkan ke menu utama produksi (Form Parameter Setting)
            return RedirectToAction("Create", "ProductionReport");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        private async Task PrepareLoginViewData()
        {
            ViewBag.Users = await _context.ElwpUsers
                .Where(u => u.IsActive && u.AreaId == 1)
                .OrderBy(u => u.FullName)
                .ToListAsync();

            ViewBag.Machines = await _context.ElwpMachines
                .Where(m => m.IsActive && (m.AreaId == 1 || m.AreaId == 10))
                .OrderBy(m => m.KodeMesin)
                .ToListAsync();
        }
    }
}
