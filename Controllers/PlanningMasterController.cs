using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;
using ExcelDataReader;
using System.Data;
using System.Globalization;

namespace VelastoProductionSystem.Controllers
{
    public class PlanningMasterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ElwpDbContext _elwpContext;



        public PlanningMasterController(ApplicationDbContext context, ElwpDbContext elwpContext)
        {
            _context = context;
            _elwpContext = elwpContext;
        }

        [HttpGet]
        public async Task<IActionResult> DiagMachines()
        {
            try
            {
                var list = await _elwpContext.ElwpMachines.OrderBy(x => x.Id).ToListAsync();
                return Json(list);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DiagMesinSchema()
        {
            try
            {
                var columns = new List<string>();
                var conn = _elwpContext.Database.GetDbConnection();
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT COLUMN_NAME + ' (' + DATA_TYPE + ')'
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = 'produksi' AND TABLE_NAME = 'tb_elwp_produksi_mesins'
                    ORDER BY ORDINAL_POSITION";
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                    columns.Add(reader.GetString(0));
                await conn.CloseAsync();
                return Json(columns);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DiagMesinToday()
        {
            try
            {
                var targetDate = DateTime.Today;
                var dateStart = targetDate.Date;
                var dateEnd = dateStart.AddDays(1);

                var query = await _elwpContext.ElwpPlannings
                    .Where(x => x.TanggalPlanning >= dateStart && x.TanggalPlanning < dateEnd)
                    .GroupBy(x => x.MesinId)
                    .Select(g => new { MesinId = g.Key, RowCount = g.Count() })
                    .ToListAsync();

                return Json(new { 
                    date = targetDate.ToString("yyyy-MM-dd"),
                    summary = query 
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DiagAreaToday()
        {
            try
            {
                var targetDate = DateTime.Today;
                var dateStart = targetDate.Date;
                var dateEnd = dateStart.AddDays(1);

                var query = await _elwpContext.ElwpPlannings
                    .Where(x => x.TanggalPlanning >= dateStart && x.TanggalPlanning < dateEnd)
                    .GroupBy(x => x.AreaId)
                    .Select(g => new { AreaId = g.Key, RowCount = g.Count() })
                    .ToListAsync();

                return Json(new { 
                    date = targetDate.ToString("yyyy-MM-dd"),
                    summary = query 
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public async Task<IActionResult> Index()
        {
            // Auto-repair missing table just in case EF Migrations failed to create it
            await _context.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PlanningMasters]') AND type in (N'U'))
            BEGIN
            CREATE TABLE [dbo].[PlanningMasters](
                [Id] [int] IDENTITY(1,1) NOT NULL,
                [MachineName] [nvarchar](max) NULL,
                [DateShiftString] [nvarchar](max) NULL,
                [PartName1] [nvarchar](max) NULL,
                [PartName2] [nvarchar](max) NULL,
                [Compound] [nvarchar](max) NULL,
                [CompoundInner] [nvarchar](max) NULL,
                [CompoundOuter] [nvarchar](max) NULL,
                [CompoundCombo] [nvarchar](max) NULL,
                [Length] [nvarchar](max) NULL,
                [Kode] [nvarchar](max) NULL,
                [PlanTargetPcs] [int] NULL,
                [Menit] [nvarchar](max) NULL,
                [WaktuMulai] [nvarchar](max) NULL,
                [WaktuSelesai] [nvarchar](max) NULL,
                [CtAwal] [nvarchar](max) NULL,
                [CtMinus20] [nvarchar](max) NULL,
                [NeedKgInner] [nvarchar](max) NULL,
                [NeedKgOuter] [nvarchar](max) NULL,
                [CreatedAt] [datetime2](7) NOT NULL,
             CONSTRAINT [PK_PlanningMasters] PRIMARY KEY CLUSTERED ([Id] ASC)
            )
            END

            -- Add missing columns if they don't exist
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PlanningMasters]') AND name = 'CtAwal')
                ALTER TABLE [dbo].[PlanningMasters] ADD [CtAwal] [nvarchar](max) NULL;
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PlanningMasters]') AND name = 'CtMinus20')
                ALTER TABLE [dbo].[PlanningMasters] ADD [CtMinus20] [nvarchar](max) NULL;
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PlanningMasters]') AND name = 'NeedKgInner')
                ALTER TABLE [dbo].[PlanningMasters] ADD [NeedKgInner] [nvarchar](max) NULL;
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PlanningMasters]') AND name = 'NeedKgOuter')
                ALTER TABLE [dbo].[PlanningMasters] ADD [NeedKgOuter] [nvarchar](max) NULL;
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PlanningMasters]') AND name = 'CompoundInner')
                ALTER TABLE [dbo].[PlanningMasters] ADD [CompoundInner] [nvarchar](max) NULL;
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PlanningMasters]') AND name = 'CompoundOuter')
                ALTER TABLE [dbo].[PlanningMasters] ADD [CompoundOuter] [nvarchar](max) NULL;
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PlanningMasters]') AND name = 'CompoundCombo')
                ALTER TABLE [dbo].[PlanningMasters] ADD [CompoundCombo] [nvarchar](max) NULL;
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PlanningMasters]') AND name = 'Length')
                ALTER TABLE [dbo].[PlanningMasters] ADD [Length] [nvarchar](max) NULL;
            ");

            // Load Dynamic Config from DB
            ViewBag.Machines = await _context.Machines.Select(m => m.MachineName).Distinct().ToListAsync();
            ViewBag.Shifts   = await _context.ShiftMasters.OrderBy(x => x.ShiftName).ToListAsync();

            var data = await _context.PlanningMasters.OrderByDescending(x => x.Id).ToListAsync();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> ClearData()
        {
            _context.PlanningMasters.RemoveRange(_context.PlanningMasters);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Semua data Planning telah dikosongkan.";
            return RedirectToAction(nameof(Index));
        }

        // ─── Sync dari ELWP_PRD: Ambil data hari ini ────────────────────────
        [HttpPost]
        public async Task<IActionResult> SyncFromElwp(string? syncDate)
        {
            try
            {
                // Tentukan tanggal yang akan diambil (default: hari ini)
                var targetDate = DateTime.Today;
                if (!string.IsNullOrWhiteSpace(syncDate) && DateTime.TryParse(syncDate, out var parsedDate))
                    targetDate = parsedDate.Date;

                var dateStart = new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, 0, 0, 0);
                var dateEnd   = dateStart.AddDays(1);

                // Ambil dari ELWP_PRD
                List<ElwpPlanning> elwpRows;
                try
                {
                    elwpRows = await _elwpContext.ElwpPlannings
                        .Where(x => x.TanggalPlanning >= dateStart && x.TanggalPlanning < dateEnd)
                        .OrderBy(x => x.MesinId)
                        .ThenBy(x => x.Shift)
                        .ThenBy(x => x.Id)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Gagal koneksi ke database ELWP: " + ex.Message;
                    return RedirectToAction(nameof(Index));
                }

                if (!elwpRows.Any())
                {
                    TempData["ErrorMessage"] = $"Tidak ada data planning di ELWP untuk tanggal {targetDate:dd MMMM yyyy}.";
                    return RedirectToAction(nameof(Index));
                }

                // Hapus data hari yang sama di lokal agar tidak duplikat
                var culture = new System.Globalization.CultureInfo("id-ID");
                var d1Str = targetDate.ToString("d MMMM yyyy", culture).ToUpper();
                var d2Str = targetDate.ToString("dd MMMM yyyy", culture).ToUpper();

                var existingToday = _context.PlanningMasters
                    .Where(x => !string.IsNullOrEmpty(x.DateShiftString) &&
                                (x.DateShiftString.ToUpper().Contains(d1Str) ||
                                 x.DateShiftString.ToUpper().Contains(d2Str)))
                    .ToList();

                if (existingToday.Any())
                    _context.PlanningMasters.RemoveRange(existingToday);

                await _context.SaveChangesAsync();

                // Ambil daftar nama mesin dari ELWP untuk lookup
                var machinesMap = await _elwpContext.ElwpMachines
                    .ToDictionaryAsync(x => x.Id, x => $"{(x.KodeMesin ?? x.Id.ToString())} - {x.NamaMesin}");

                // Kelompokkan per Mesin + Shift lalu import
                var grouped = elwpRows
                    .GroupBy(x => new { x.MesinId, x.Shift })
                    .OrderBy(g => g.Key.MesinId)
                    .ThenBy(g => g.Key.Shift);

                int insertedCount = 0;
                var syncAt = DateTime.Now;

                foreach (var grp in grouped)
                {
                    var machName = machinesMap.TryGetValue(grp.Key.MesinId ?? 0, out var mn)
                        ? mn
                        : $"Mesin-{grp.Key.MesinId}";

                    // Format: SELASA, 8 APRIL 2026 SHIFT 1  (format tgl Indonesia)
                    var tglFormatted = targetDate.ToString("dddd, d MMMM yyyy", culture).ToUpper();
                    // Normalisasi shift: "Shift 1" -> "1"
                    var shiftClean = (grp.Key.Shift ?? "1").ToUpper().Replace("SHIFT", "").Trim();
                    var dateShiftStr = $"{tglFormatted} SHIFT {shiftClean}";

                    // Cari data Part Master untuk kalkulasi jam & menit
                    TimeSpan lastEnd = new TimeSpan(7, 30, 0);

                    foreach (var row in grp.OrderBy(x => x.Id))
                    {
                        if (string.IsNullOrWhiteSpace(row.KodeItem)) continue;

                        var pm = await _context.PartMasters
                            .FirstOrDefaultAsync(x => x.PartCode == row.KodeItem.Trim());

                        var planQty = row.QtyPlanning;
                        // Hitung menit dari CT*Qty / 60, atau dari LoadingTimeHours kalau CT tidak ada
                        int durationMins = 0;
                        if (pm != null && planQty.HasValue && pm.CtAwal.HasValue)
                            durationMins = (int)(planQty.Value * pm.CtAwal.Value / 60);
                        else if (row.LoadingTimeHours.HasValue)
                            durationMins = (int)(row.LoadingTimeHours.Value * 60);

                        var startTime = lastEnd;
                        var endTime   = startTime.Add(TimeSpan.FromMinutes(durationMins));
                        lastEnd = endTime;

                        var newPlan = new PlanningMaster
                        {
                            MachineName     = machName,
                            DateShiftString = dateShiftStr,
                            CreatedAt       = syncAt,
                            PartName1       = row.KodeItem.Trim(),
                            PartName2       = row.PartName ?? "",
                            Kode            = row.PnSap ?? "",
                            PlanTargetPcs   = planQty,
                            Compound        = pm?.CompoundCombo ?? "",
                            CompoundInner   = pm?.CompoundInner ?? "",
                            CompoundOuter   = pm?.CompoundOuter ?? "",
                            Length          = pm?.Length ?? "",
                            CtAwal          = pm?.CtAwal?.ToString("G29") ?? "",
                            CtMinus20       = pm?.CtMinus20?.ToString("G29") ?? "",
                            NeedKgInner     = pm?.NeedKgInner?.ToString("G29") ?? "",
                            NeedKgOuter     = pm?.NeedKgOuter?.ToString("G29") ?? "",
                            Menit           = durationMins.ToString(),
                            WaktuMulai      = startTime.ToString(@"hh\:mm"),
                            WaktuSelesai    = endTime.ToString(@"hh\:mm")
                        };

                        _context.PlanningMasters.Add(newPlan);
                        insertedCount++;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"✅ Berhasil sinkronisasi {insertedCount} item planning dari ELWP untuk {targetDate:dd MMMM yyyy}!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error saat sync: " + (ex.InnerException?.Message ?? ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> SavePlan([FromBody] List<PlanningMaster> rows)
        {
            if (rows == null || !rows.Any())
                return Json(new { success = false, error = "Sistem gagal menerima data dari browser. Cek koneksi atau coba lagi." });

            try
            {
                // Bersihkan baris yang mungkin kosong (Activity/PartName1 kosong)
                var validRows = rows.Where(x => !string.IsNullOrWhiteSpace(x.PartName1)).ToList();
                if (!validRows.Any())
                    return Json(new { success = false, error = "Tidak ada baris aktivitas yang valid untuk disimpan." });

                foreach (var row in validRows)
                {
                    row.CreatedAt = DateTime.Now;
                    _context.PlanningMasters.Add(row);
                }
                
                await _context.SaveChangesAsync();
                return Json(new { success = true, count = validRows.Count });
            }
            catch (Exception ex)
            {
                // Log inner exception if exists for better debugging
                string msg = ex.InnerException?.Message ?? ex.Message;
                return Json(new { success = false, error = "DB Error: " + msg });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Pilih file Excel terlebih dahulu.";
                return RedirectToAction(nameof(Index));
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            try
            {
                int insertedCount = 0;

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet();
                        string[] targetSheets = { "DL1", "DL2", "DL3" };
                        
                        // Cari sheet yang cocok, atau ambil sheet pertama jika tidak ada yang cocok
                        var tablesToProcess = result.Tables.Cast<DataTable>()
                            .Where(t => targetSheets.Any(x => t.TableName.ToUpper().Contains(x)))
                            .ToList();

                        if (!tablesToProcess.Any() && result.Tables.Count > 0)
                        {
                            tablesToProcess.Add(result.Tables[0]); // Fallback ke sheet pertama
                        }

                        foreach (DataTable table in tablesToProcess)
                        {
                            string sheetName = table.TableName.ToUpper();
                            string currentMachineName = sheetName.Contains("DL1") ? "DL01 engine" : 
                                                        sheetName.Contains("DL2") ? "DL02 engine" : 
                                                        sheetName.Contains("DL3") ? "DL03 engine" : "DL01 engine";

                            string currentDateShift = "";
                            TimeSpan lastEndTime = new TimeSpan(7, 30, 0); 

                            for (int r = 0; r < table.Rows.Count; r++)
                            {
                                var col0 = table.Rows[r][0]?.ToString()?.Trim();
                                var col1 = table.Rows[r][1]?.ToString()?.Trim();

                                // Deteksi baris Tanggal/Shift (Cari teks SHIFT atau TANGGAL)
                                if (string.IsNullOrEmpty(col0) && !string.IsNullOrEmpty(col1) && 
                                   (col1.ToUpper().Contains("SHIFT") || col1.ToUpper().Contains("TANGGAL") || col1.ToUpper().Contains("RABU") || col1.ToUpper().Contains("KAMIS") || col1.ToUpper().Contains("SENIN") || col1.ToUpper().Contains("SELASA")))
                                {
                                    currentDateShift = col1;
                                    lastEndTime = new TimeSpan(7, 30, 0); // Default

                                    // CEK KE DATABASE: Apakah sudah ada planning untuk tanggal/shift ini?
                                    // Jika ada, ambil jam selesainya yang paling terakhir.
                                    var existingRecords = await _context.PlanningMasters
                                        .Where(x => x.MachineName == currentMachineName && x.DateShiftString == currentDateShift)
                                        .OrderByDescending(x => x.Id)
                                        .ToListAsync();

                                    if (existingRecords.Any())
                                    {
                                        var lastRec = existingRecords.First();
                                        if (TimeSpan.TryParse(lastRec.WaktuSelesai, out TimeSpan ts))
                                        {
                                            lastEndTime = ts;
                                        }
                                    }
                                    continue;
                                }

                                // Jika A berisi angka (misal "1", "2") dianggap baris data
                                if (!string.IsNullOrEmpty(col0) && int.TryParse(col0, out _))
                                {
                                    int totalCols = table.Columns.Count;
                                    string GetV(int idx) => (idx >= 0 && idx < totalCols) ? table.Rows[r][idx]?.ToString()?.Trim() ?? "" : "";

                                    var partCodeFromExcel = GetV(1); 
                                    var qtyStr = GetV(5); 
                                    if (string.IsNullOrEmpty(qtyStr)) qtyStr = GetV(2); 

                                    int? planQty = int.TryParse(qtyStr?.Replace(",", "").Replace(".", ""), out int q) ? q : (int?)null;
                                    var pm = await _context.PartMasters.FirstOrDefaultAsync(x => x.PartCode == partCodeFromExcel);

                                    // Hitung Menit
                                    int durationMinutes = (pm != null && planQty.HasValue) 
                                                ? (int)(planQty.Value * (pm.CtAwal ?? 0) / 60) 
                                                : (int.TryParse(GetV(7), out int m) ? m : 0);

                                    // LOGIKA AUTO-SCHEDULE
                                    var startTime = lastEndTime;
                                    var endTime = startTime.Add(TimeSpan.FromMinutes(durationMinutes));
                                    lastEndTime = endTime; // Untuk baris berikutnya

                                    var newPlan = new PlanningMaster
                                    {
                                        MachineName = currentMachineName,
                                        DateShiftString = currentDateShift,
                                        CreatedAt = DateTime.Now,
                                        PartName1 = partCodeFromExcel,
                                        PartName2 = pm?.PartNumber ?? GetV(2),
                                        PlanTargetPcs = planQty,
                                        Compound  = pm?.CompoundCombo ?? GetV(3),
                                        Length    = pm?.Length ?? GetV(4),
                                        CtAwal    = pm?.CtAwal?.ToString("G29"),
                                        CtMinus20 = pm?.CtMinus20?.ToString("G29"),
                                        NeedKgInner = pm?.NeedKgInner?.ToString("G29"),
                                        NeedKgOuter = pm?.NeedKgOuter?.ToString("G29"),
                                        Menit = durationMinutes.ToString(),
                                        WaktuMulai = startTime.ToString(@"hh\:mm"),
                                        WaktuSelesai = endTime.ToString(@"hh\:mm")
                                    };

                                    _context.PlanningMasters.Add(newPlan);
                                    insertedCount++;
                                }
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Sukses Scan File! Berhasil mengekstrak {insertedCount} baris dan menjadwalkan jam secara otomatis.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Terjadi kesalahan saat scanning: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
