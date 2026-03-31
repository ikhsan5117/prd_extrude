using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Controllers
{
    public class DailyPlanExecutionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DailyPlanExecutionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Start()
        {
            ViewBag.Machines = await _context.Machines.Select(m => m.MachineName).Distinct().ToListAsync();
            ViewBag.Shifts = await _context.ShiftMasters.OrderBy(x => x.ShiftName).ToListAsync();
            return View();
        }

        public async Task<IActionResult> Index()
        {
            // Auto repair missing tables
            await _context.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DailyPlanExecutions]') AND type in (N'U'))
            BEGIN
            CREATE TABLE [dbo].[DailyPlanExecutions](
                [Id] [int] IDENTITY(1,1) NOT NULL,
                [MachineName] [nvarchar](max) NULL,
                [ExecutionDate] [datetime2](7) NOT NULL,
                [Shift] [nvarchar](max) NULL,
                [GroupName] [nvarchar](max) NULL,
                [Pic1] [nvarchar](max) NULL,
                [Pic2] [nvarchar](max) NULL,
                [LineStopNote] [nvarchar](max) NULL,
                [LineStopMinutes] [int] NULL,
                [StartMesin] [nvarchar](max) NULL,
                [FinishMesin] [nvarchar](max) NULL,
                [CreatedAt] [datetime2](7) NOT NULL,
                [UpdatedAt] [datetime2](7) NULL,
             CONSTRAINT [PK_DailyPlanExecutions] PRIMARY KEY CLUSTERED ([Id] ASC)
            )
            END

            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DailyPlanActivities]') AND type in (N'U'))
            BEGIN
            CREATE TABLE [dbo].[DailyPlanActivities](
                [Id] [int] IDENTITY(1,1) NOT NULL,
                [DailyPlanExecutionId] [int] NOT NULL,
                [PartName1] [nvarchar](max) NULL,
                [PartName2] [nvarchar](max) NULL,
                [PlanQty] [int] NULL,
                [PlanDurationMinutes] [int] NULL,
                [PlanStart] [nvarchar](max) NULL,
                [PlanEnd] [nvarchar](max) NULL,
                [ActualQty] [int] NULL,
                [ActualDurationMinutes] [int] NULL,
                [ActualStart] [nvarchar](max) NULL,
                [ActualEnd] [nvarchar](max) NULL,
                [Remarks] [nvarchar](max) NULL,
                [OrderIndex] [int] NOT NULL,
             CONSTRAINT [PK_DailyPlanActivities] PRIMARY KEY CLUSTERED ([Id] ASC),
             CONSTRAINT [FK_DailyPlanActivities_DailyPlanExecutions_DailyPlanExecutionId] FOREIGN KEY([DailyPlanExecutionId]) REFERENCES [dbo].[DailyPlanExecutions] ([Id]) ON DELETE CASCADE
            )
            END
            ");

            var data = await _context.DailyPlanExecutions
                .OrderByDescending(x => x.ExecutionDate)
                .ThenBy(x => x.Shift)
                .ToListAsync();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrLoad(DateTime executionDate, string shift, string machineName, string groupName)
        {
            // 1. Cek apakah sudah ada
            var existing = await _context.DailyPlanExecutions
                .Include(x => x.Activities)
                .FirstOrDefaultAsync(x => x.ExecutionDate.Date == executionDate.Date 
                                       && x.Shift == shift 
                                       && x.MachineName == machineName);

            if (existing != null)
            {
                return RedirectToAction(nameof(Execution), new { id = existing.Id });
            }

            // 2. Jika belum, tarik dari PlanningMasters
            // Format pencarian: "SELASA, 31 MARET 2026 SHIFT 1"
            var culture = new System.Globalization.CultureInfo("id-ID");
            var datePart = executionDate.ToString("dd MMMM yyyy", culture).ToUpper(); // "31 MARET 2026"
            
            var plans = await _context.PlanningMasters
                .Where(x => x.MachineName == machineName)
                .OrderBy(x => x.Id)
                .ToListAsync();
                
            // Pencarian fleksibel: harus ada tanggal (misal "31 MARET 2026") DAN harus ada Shift (misal "SHIFT 1")
            var matchedPlans = plans.Where(x => 
                !string.IsNullOrEmpty(x.DateShiftString) && 
                x.DateShiftString.ToUpper().Contains(datePart) && 
                x.DateShiftString.ToUpper().Contains(shift.ToUpper())
            ).ToList();
            
            // Fallback: Jika hari ini kosong, jangan load sembarang data (agar operator tidak bingung)
            // Namun untuk pertama kali, jika user memaksa buat tanpa plan, izinkan tapi list activities kosong

            var newExec = new DailyPlanExecution
            {
                MachineName = machineName,
                ExecutionDate = executionDate.Date,
                Shift = shift,
                GroupName = groupName,
                CreatedAt = DateTime.Now
            };

            _context.DailyPlanExecutions.Add(newExec);
            await _context.SaveChangesAsync();

            int order = 1;
            foreach (var p in matchedPlans)
            {
                if (string.IsNullOrEmpty(p.PartName1) && string.IsNullOrEmpty(p.PartName2)) continue;
                
                int? pMenit = null;
                if (int.TryParse(p.Menit?.Replace("Menit", "").Trim(), out int m)) pMenit = m;

                var act = new DailyPlanActivity
                {
                    DailyPlanExecutionId = newExec.Id,
                    PartName1 = p.PartName1, // Misal "NA2140"
                    PartName2 = p.Kode,      // Kita simpan Kode (77259-BZ120) di PartName2 agar muncul di form tablet
                    PlanQty = p.PlanTargetPcs,
                    PlanDurationMinutes = pMenit,
                    PlanStart = p.WaktuMulai,
                    PlanEnd = p.WaktuSelesai,
                    OrderIndex = order++
                };
                _context.DailyPlanActivities.Add(act);
            }
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Data Planning hasil import berhasil di-generate!";
            return RedirectToAction(nameof(Execution), new { id = newExec.Id });
        }

        public async Task<IActionResult> Execution(int id)
        {
            var data = await _context.DailyPlanExecutions
                .Include(x => x.Activities)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (data == null) return NotFound();

            data.Activities = data.Activities.OrderBy(x => x.OrderIndex).ToList();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveExecution(DailyPlanExecution model, List<DailyPlanActivity> activities)
        {
            var dbModel = await _context.DailyPlanExecutions
                .Include(x => x.Activities)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (dbModel == null) return NotFound();

            dbModel.Pic1 = model.Pic1;
            dbModel.Pic2 = model.Pic2;
            dbModel.LineStopNote = model.LineStopNote;
            dbModel.LineStopMinutes = model.LineStopMinutes;
            dbModel.StartMesin = model.StartMesin;
            dbModel.FinishMesin = model.FinishMesin;
            dbModel.UpdatedAt = DateTime.Now;

            foreach (var act in activities)
            {
                var dbAct = dbModel.Activities.FirstOrDefault(a => a.Id == act.Id);
                if (dbAct != null)
                {
                    dbAct.ActualQty = act.ActualQty;
                    dbAct.ActualStart = act.ActualStart;
                    dbAct.ActualEnd = act.ActualEnd;
                    dbAct.Remarks = act.Remarks;
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Laporan Aktual berhasil disimpan!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.DailyPlanExecutions.FindAsync(id);
            if (item != null)
            {
                _context.DailyPlanExecutions.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Data Laporan berhasil dihapus.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
