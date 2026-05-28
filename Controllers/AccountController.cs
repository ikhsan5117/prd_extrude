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
            // Jika sudah ada session, redirect sesuai role
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                if (HttpContext.Session.GetString("IsAdmin") == "true")
                    return RedirectToAction("ChartAnalysis", "ProductionReport");
                return RedirectToAction("Create", "ProductionReport");
            }

            ViewBag.Users = await _context.ElwpUsers
                .Where(u => u.IsActive && u.AreaId == 1) // Hanya area Extrude
                .OrderBy(u => u.FullName)
                .ToListAsync();

            ViewBag.Machines = await _context.ElwpMachines
                .Where(m => m.IsActive && m.AreaId == 1 && m.KodeMesin != "DL01" && m.KodeMesin != "DL02")
                .OrderBy(m => m.KodeMesin)
                .ToListAsync();

            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(int? userId, int? machineId, string password, string? adminUsername)
        {
            // --- ADMIN LOGIN ---
            if (!string.IsNullOrEmpty(adminUsername))
            {
                string lowerAdmin = adminUsername.ToLower();
                string userRole = "SUPERADMIN";
                string displayName = "Administrator";

                if (lowerAdmin == "admin")
                {
                    if (password != "admin123!")
                    {
                        ModelState.AddModelError("", "Password Super Admin salah!");
                        await PrepareLoginViewData();
                        return View();
                    }
                    userRole = "SUPERADMIN";
                    displayName = "Super Admin";
                }
                else if (lowerAdmin == "adm_prod")
                {
                    if (password != "produksi123!")
                    {
                        ModelState.AddModelError("", "Password Admin Produksi salah!");
                        await PrepareLoginViewData();
                        return View();
                    }
                    userRole = "ADMIN_PROD";
                    displayName = "Admin Produksi";
                }
                else if (lowerAdmin == "adm_eng")
                {
                    if (password != "engineering123!")
                    {
                        ModelState.AddModelError("", "Password Admin Engineering salah!");
                        await PrepareLoginViewData();
                        return View();
                    }
                    userRole = "ADMIN_ENG";
                    displayName = "Admin Engineering";
                }
                else if (lowerAdmin == "eng_temp")
                {
                    if (password != "engtemp123!")
                    {
                        ModelState.AddModelError("", "Password Engineering sementara salah!");
                        await PrepareLoginViewData();
                        return View();
                    }

                    // Temporary engineering requester account (non-admin) for approval flow testing.
                    string engMachineName = "ENG";
                    int engMachineId = 0;

                    if (machineId.HasValue && machineId > 0)
                    {
                        var selectedMachine = await _context.ElwpMachines.FindAsync(machineId.Value);
                        if (selectedMachine != null)
                        {
                            engMachineName = selectedMachine.KodeMesin ?? "ENG";
                            engMachineId = selectedMachine.Id;
                        }
                    }

                    HttpContext.Session.SetString("UserName", "Engineering Temporary");
                    HttpContext.Session.SetString("IsAdmin", "false");
                    HttpContext.Session.SetString("UserRole", "ENGINEERING");
                    HttpContext.Session.SetString("MachineName", engMachineName);
                    HttpContext.Session.SetInt32("MachineId", engMachineId);

                    return RedirectToAction("Index", "SpsMaster");
                }
                else
                {
                    ModelState.AddModelError("", "Username Admin tidak dikenal!");
                    await PrepareLoginViewData();
                    return View();
                }

                // Jika admin tidak memilih mesin, default ke "ADMIN"
                string machineName = "ADMIN";
                int machineIdValue = 0;

                if (machineId.HasValue && machineId > 0)
                {
                    var selectedMachine = await _context.ElwpMachines.FindAsync(machineId.Value);
                    if (selectedMachine != null)
                    {
                        machineName = selectedMachine.KodeMesin ?? "ADMIN";
                        machineIdValue = selectedMachine.Id;
                    }
                }

                HttpContext.Session.SetString("UserName", displayName);
                HttpContext.Session.SetString("IsAdmin", "true");
                HttpContext.Session.SetString("UserRole", userRole);
                HttpContext.Session.SetString("MachineName", machineName);
                HttpContext.Session.SetInt32("MachineId", machineIdValue);

                return RedirectToAction("ChartAnalysis", "ProductionReport");
            }

            // --- OPERATOR LOGIN ---
            if (!userId.HasValue || !machineId.HasValue)
            {
                ModelState.AddModelError("", "Pilih User dan Mesin terlebih dahulu.");
                await PrepareLoginViewData();
                return View();
            }

            // Validasi password operator
            if (string.IsNullOrEmpty(password) || password.ToLower() != "extrude123!")
            {
                ModelState.AddModelError("", "Password salah! (Hint: extrude123!)");
                await PrepareLoginViewData();
                return View();
            }

            var user = await _context.ElwpUsers.FindAsync(userId.Value);
            var machine = await _context.ElwpMachines.FindAsync(machineId.Value);

            if (user == null || machine == null)
            {
                ModelState.AddModelError("", "User atau Mesin tidak valid.");
                await PrepareLoginViewData();
                return View();
            }

            var resolvedUserName = user.FullName ?? user.Username ?? string.Empty;
            var isTrialEngineering =
                resolvedUserName.Contains("TRIAL", StringComparison.OrdinalIgnoreCase) ||
                (user.Username?.Contains("TRIAL", StringComparison.OrdinalIgnoreCase) ?? false);

            var operatorRole = isTrialEngineering ? "ENGINEERING" : "OPERATOR";

            // Simpan ke Session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", resolvedUserName);
            HttpContext.Session.SetString("IsAdmin", "false");
            HttpContext.Session.SetString("UserRole", operatorRole);
            HttpContext.Session.SetInt32("MachineId", machine.Id);
            HttpContext.Session.SetString("MachineName", machine.KodeMesin ?? machine.NamaMesin ?? "");

            if (isTrialEngineering)
            {
                return RedirectToAction("Index", "SpsMaster");
            }

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
                .Where(m => m.IsActive && m.AreaId == 1 && m.KodeMesin != "DL01" && m.KodeMesin != "DL02")
                .OrderBy(m => m.KodeMesin)
                .ToListAsync();
        }
    }
}
