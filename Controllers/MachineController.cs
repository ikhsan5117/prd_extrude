using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Controllers
{
    public class MachineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MachineController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            await EnsureTableExists();
            await EnsureDefaultShifts();
            
            ViewBag.Shifts = await _context.ShiftMasters.OrderBy(x => x.ShiftName).ToListAsync();
            var machines = await _context.Machines.OrderBy(x => x.Line).ThenBy(x => x.MachineCode).ToListAsync();
            return View(machines);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Machine());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Machine model)
        {
            if (!ModelState.IsValid) return View(model);

            model.CreatedAt = DateTime.Now;
            _context.Machines.Add(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Mesin '{model.MachineName}' berhasil ditambahkan.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var machine = await _context.Machines.FindAsync(id);
            if (machine == null) return NotFound();
            return View(machine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Machine model)
        {
            if (!ModelState.IsValid) return View(model);

            model.UpdatedAt = DateTime.Now;
            _context.Machines.Update(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Data mesin '{model.MachineName}' berhasil diperbarui.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var machine = await _context.Machines.FindAsync(id);
            if (machine != null)
            {
                _context.Machines.Remove(machine);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Data mesin berhasil dihapus.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var machine = await _context.Machines.FindAsync(id);
            if (machine != null)
            {
                machine.Status = status;
                machine.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true, status = status });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateShift(int id, string startTime)
        {
            var shift = await _context.ShiftMasters.FindAsync(id);
            if (shift != null)
            {
                shift.StartTime = startTime;
                shift.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AddShift(string name, string startTime)
        {
            var shift = new ShiftMaster { ShiftName = name, StartTime = startTime };
            _context.ShiftMasters.Add(shift);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task EnsureDefaultShifts()
        {
            if (!await _context.ShiftMasters.AnyAsync())
            {
                _context.ShiftMasters.AddRange(new List<ShiftMaster> {
                    new ShiftMaster { ShiftName = "SHIFT 1", StartTime = "07:00" },
                    new ShiftMaster { ShiftName = "SHIFT 2", StartTime = "15:30" },
                    new ShiftMaster { ShiftName = "SHIFT 3", StartTime = "22:30" }
                });
                await _context.SaveChangesAsync();
            }
        }

        private async Task EnsureTableExists()
        {
            // First ensure Machine table
            await _context.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Machines]') AND type in (N'U'))
            BEGIN
            CREATE TABLE [dbo].[Machines](
                [Id] [int] IDENTITY(1,1) NOT NULL,
                [MachineCode] [nvarchar](max) NOT NULL,
                [MachineName] [nvarchar](max) NOT NULL,
                [Line] [nvarchar](max) NULL,
                [MachineType] [nvarchar](max) NULL,
                [Brand] [nvarchar](max) NULL,
                [YearMade] [nvarchar](max) NULL,
                [SerialNumber] [nvarchar](max) NULL,
                [Status] [nvarchar](max) NOT NULL DEFAULT 'AKTIF',
                [CapacityPerHour] [int] NULL,
                [LastMaintenanceDate] [datetime2](7) NULL,
                [NextMaintenanceDate] [datetime2](7) NULL,
                [Notes] [nvarchar](max) NULL,
                [CreatedAt] [datetime2](7) NOT NULL,
                [UpdatedAt] [datetime2](7) NULL,
             CONSTRAINT [PK_Machines] PRIMARY KEY CLUSTERED ([Id] ASC)
            )
            END
            ");

            // Ensure ShiftMaster table
            await _context.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ShiftMasters]') AND type in (N'U'))
            BEGIN
            CREATE TABLE [dbo].[ShiftMasters](
                [Id] [int] IDENTITY(1,1) NOT NULL,
                [ShiftName] [nvarchar](max) NOT NULL,
                [StartTime] [nvarchar](max) NOT NULL,
                [CreatedAt] [datetime2](7) NOT NULL,
                [UpdatedAt] [datetime2](7) NULL,
             CONSTRAINT [PK_ShiftMasters] PRIMARY KEY CLUSTERED ([Id] ASC)
            )
            END
            ");
        }
    }
}
