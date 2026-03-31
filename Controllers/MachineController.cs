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

        private async Task EnsureTableExists()
        {
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
        }
    }
}
