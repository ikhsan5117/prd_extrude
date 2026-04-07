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

        // ── API: Search Part Codes for picker modal ──────────────────────────
        [HttpGet]
        public async Task<IActionResult> SearchPartCodes(string q)
        {
            var query = _context.PartMasters.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                var qUp = q.ToUpper();
                query = query.Where(p =>
                    (p.PartCode != null && p.PartCode.ToUpper().Contains(qUp)) ||
                    (p.PartNumber != null && p.PartNumber.ToUpper().Contains(qUp)) ||
                    (p.Description != null && p.Description.ToUpper().Contains(qUp)));
            }
            var results = await query
                .OrderBy(p => p.PartCode)
                .Take(30)
                .Select(p => new {
                    p.PartCode,
                    p.PartNumber,
                    p.Description,
                    p.Length,
                    p.SecPerPcs,
                    p.CtAwal
                })
                .ToListAsync();
            return Json(results);
        }

        // ── API: Search Available Planning Batches for selection ───────────────
        [HttpGet]
        public async Task<IActionResult> GetAvailablePlanningBatches(DateTime d, string s, string m)
        {
            var culture = new System.Globalization.CultureInfo("id-ID");
            var d1 = d.ToString("d MMMM yyyy", culture).ToUpper();
            var d2 = d.ToString("dd MMMM yyyy", culture).ToUpper();
            
            var cleanShift = (s ?? "").ToUpper().Replace("SHIFT", "").Trim();
            var target1 = $", {d1} SHIFT {cleanShift}"; 
            var target2 = $", {d2} SHIFT {cleanShift}";

            var matches = await _context.PlanningMasters
                .Where(x => x.MachineName == m &&
                            !string.IsNullOrEmpty(x.DateShiftString) &&
                            (x.DateShiftString.ToUpper().Contains(target1) || x.DateShiftString.ToUpper().Contains(target2)))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            if (!matches.Any()) return Json(new List<object>());

            // Group by CreatedAt (tolerance 10s)
            var batches = new List<dynamic>();
            DateTime? lastTime = null;
            var currentBatch = new List<PlanningMaster>();

            foreach (var item in matches.OrderBy(x => x.CreatedAt)) // Ascending for easier grouping
            {
                if (lastTime == null || Math.Abs((item.CreatedAt - lastTime.Value).TotalSeconds) <= 10)
                {
                    currentBatch.Add(item);
                    if (lastTime == null) lastTime = item.CreatedAt;
                }
                else
                {
                    batches.Add(new { 
                        Time = lastTime.Value.ToString("HH:mm"), 
                        FullTime = lastTime.Value.ToString("o"),
                        Count = currentBatch.Count,
                        Items = currentBatch.Select(x => new { 
                            Id = x.Id, 
                            Text = (x.PartName1 ?? x.Kode) + (string.IsNullOrEmpty(x.PartName2) ? "" : " / " + x.PartName2) 
                        }).ToList()
                    });
                    currentBatch = new List<PlanningMaster> { item };
                    lastTime = item.CreatedAt;
                }
            }
            if (currentBatch.Any())
            {
                batches.Add(new { 
                    Time = lastTime.Value.ToString("HH:mm"), 
                    FullTime = lastTime.Value.ToString("o"),
                    Count = currentBatch.Count,
                    Items = currentBatch.Select(x => new { 
                        Id = x.Id, 
                        Text = (x.PartName1 ?? x.Kode) + (string.IsNullOrEmpty(x.PartName2) ? "" : " / " + x.PartName2) 
                    }).ToList()
                });
            }

            return Json(batches.OrderByDescending(x => x.FullTime));
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
        public async Task<IActionResult> CreateOrLoad(DateTime executionDate, string shift, string machineName, string groupName, DateTime? batchTime, int? planningId)
        {
            // 1. Cek apakah sudah ada
            var existing = await _context.DailyPlanExecutions
                .Include(x => x.Activities)
                .FirstOrDefaultAsync(x => x.ExecutionDate.Date == executionDate.Date 
                                       && x.Shift == shift 
                                       && x.MachineName == machineName);

            if (existing != null)
            {
                // Jika sudah ada tapi activities kosong, atau batchTime/planningId diberikan (force reload)
                if (existing.Activities.Count == 0 || batchTime != null || planningId != null)
                {
                    if (batchTime != null || planningId != null)
                    {
                        _context.DailyPlanActivities.RemoveRange(existing.Activities);
                        await _context.SaveChangesAsync();
                        await LoadActivitiesFromPlanning(existing, executionDate, shift, machineName, batchTime, planningId);
                    }
                    else if (existing.Activities.Count == 0)
                    {
                        await LoadActivitiesFromPlanning(existing, executionDate, shift, machineName, null, null);
                    }
                }
                return RedirectToAction(nameof(Execution), new { id = existing.Id });
            }

            // 2. Buat baru & tarik dari PlanningMasters
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

            await LoadActivitiesFromPlanning(newExec, executionDate, shift, machineName, batchTime, planningId);

            TempData["SuccessMessage"] = "Data Planning berhasil di-generate!";
            return RedirectToAction(nameof(Execution), new { id = newExec.Id });
        }

        // ─── Helper: muat activities dari PlanningMasters ───────────────────
        private async Task LoadActivitiesFromPlanning(DailyPlanExecution exec, DateTime executionDate, string shift, string machineName, DateTime? batchTime, int? planningId)
        {
            var culture = new System.Globalization.CultureInfo("id-ID");
            var d1 = executionDate.ToString("d MMMM yyyy", culture).ToUpper();
            var d2 = executionDate.ToString("dd MMMM yyyy", culture).ToUpper();
            
            // Bersihkan shift agar tidak double kata "SHIFT"
            var cleanShift = (shift ?? "").ToUpper().Replace("SHIFT", "").Trim();
            
            var target1 = $", {d1} SHIFT {cleanShift}"; 
            var target2 = $", {d2} SHIFT {cleanShift}";

            // 1. Cari semua yang cocok
            var query = _context.PlanningMasters
                .Where(x => x.MachineName == machineName &&
                            !string.IsNullOrEmpty(x.DateShiftString) &&
                            (x.DateShiftString.ToUpper().Contains(target1) || x.DateShiftString.ToUpper().Contains(target2)));

            var allMatching = await query.OrderByDescending(x => x.Id).ToListAsync();

            if (!allMatching.Any()) return;

            // 2. Filter (Satu Part tertentu ATAU Satu Batch tertentu)
            List<PlanningMaster> matchedPlans;
            if (planningId.HasValue)
            {
                matchedPlans = allMatching.Where(x => x.Id == planningId.Value).ToList();
            }
            else
            {
                var targetTime = batchTime ?? allMatching.First().CreatedAt;
                matchedPlans = allMatching
                    .Where(x => Math.Abs((x.CreatedAt - targetTime).TotalSeconds) <= 10) 
                    .OrderBy(x => x.Id)
                    .ToList();
            }



            int order = 1;
            foreach (var p in matchedPlans)
            {
                var s1 = p.PartName1?.ToUpper() ?? "";
                var s2 = p.PartName2?.ToUpper() ?? "";
                var sk = p.Kode?.ToUpper() ?? "";

                if (s1.Contains("ISTIRAHAT") || s2.Contains("ISTIRAHAT") || sk.Contains("ISTIRAHAT")) continue;

                bool isDandoriRow = s1.Contains("DANDORI") || s2.Contains("DANDORI") || sk.Contains("DANDORI");

                var act = new DailyPlanActivity
                {
                    DailyPlanExecutionId = exec.Id,
                    PartName1 = isDandoriRow ? "DANDORI" : (p.PartName1 ?? p.Kode), 
                    PartName2 = isDandoriRow ? "" : (p.PartName1 == null ? "" : p.Kode),      
                    PlanQty   = p.PlanTargetPcs,
                    PlanDurationMinutes = int.TryParse(p.Menit?.Replace("Menit","").Trim(), out int m) ? m : (int?)null,
                    PlanStart = p.WaktuMulai,
                    PlanEnd   = p.WaktuSelesai,
                    OrderIndex = order++
                };
                _context.DailyPlanActivities.Add(act);
            }

            if (matchedPlans.Any())
                await _context.SaveChangesAsync();
        }

        // ─── Regenerate activities untuk existing execution ──────────────────
        [HttpPost]
        public async Task<IActionResult> RegenerateActivities(int id)
        {
            var exec = await _context.DailyPlanExecutions
                .Include(x => x.Activities)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (exec == null) return NotFound();

            // Hapus activities lama
            _context.DailyPlanActivities.RemoveRange(exec.Activities);
            await _context.SaveChangesAsync();

            // Reload dari planning
            await LoadActivitiesFromPlanning(exec, exec.ExecutionDate, exec.Shift ?? "", exec.MachineName ?? "", null, null);

            TempData["SuccessMessage"] = "Data aktivitas berhasil di-reload dari Planning Master!";
            return RedirectToAction(nameof(Execution), new { id });
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
                    dbAct.PartName1     = act.PartName1;            // VIN Code (bisa dikoreksi operator)
                    dbAct.ActualQty     = act.ActualQty;
                    dbAct.ActualDurationMinutes = act.ActualDurationMinutes;
                    dbAct.ActualStart   = act.ActualStart;
                    dbAct.ActualEnd     = act.ActualEnd;
                    dbAct.Remarks       = act.Remarks;
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
