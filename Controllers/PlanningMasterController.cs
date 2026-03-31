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

        public PlanningMasterController(ApplicationDbContext context)
        {
            _context = context;
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
                // Kosongkan data lama agar tidak menumpuk duplikat
                _context.PlanningMasters.RemoveRange(_context.PlanningMasters);
                await _context.SaveChangesAsync();

                int insertedCount = 0;

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet();
                        string[] targetSheets = { "DL1", "DL2", "DL3" };
                        
                        foreach (DataTable table in result.Tables)
                        {
                            string sheetName = table.TableName;
                            if (!targetSheets.Any(x => sheetName.Contains(x))) continue;

                            string currentMachineName = sheetName.Contains("DL1") ? "DL01 engine" : 
                                                        sheetName.Contains("DL2") ? "DL02 engine" : "DL03 engine";

                            string currentDateShift = "";

                            for (int r = 0; r < table.Rows.Count; r++)
                            {
                                var col0 = table.Rows[r][0]?.ToString()?.Trim();
                                var col1 = table.Rows[r][1]?.ToString()?.Trim();

                                // Jika B berisi TANGGAL SHIFT dan A kosong
                                if (string.IsNullOrEmpty(col0) && !string.IsNullOrEmpty(col1) && col1.Contains("SHIFT"))
                                {
                                    currentDateShift = col1;
                                    continue;
                                }

                                // Jika A berisi angka (misal "30") artinya ini baris data!
                                if (!string.IsNullOrEmpty(col0) && int.TryParse(col0, out _))
                                {
                                    // Ambil sesuai Column Index (0-based di pandas)
                                    // Pastikan kita tidak overlap array length
                                    int totalCols = table.Columns.Count;
                                    string GetV(int idx) => (idx >= 0 && idx < totalCols) ? table.Rows[r][idx]?.ToString()?.Trim() ?? "" : "";

                                    var part1 = GetV(1);
                                    var part2 = GetV(2);
                                    var comp = GetV(3);
                                    
                                    var qtyStr = GetV(20);
                                    var durStr = GetV(22);
                                    var startStr = GetV(23);
                                    var endStr = GetV(24);

                                    // Parse jam agar rapi (kadang bertipe datetime)
                                    if (DateTime.TryParse(startStr, out DateTime startTime)) startStr = startTime.ToString("HH:mm");
                                    if (DateTime.TryParse(endStr, out DateTime endTime)) endStr = endTime.ToString("HH:mm");

                                    int? planQty = null;
                                    if (int.TryParse(qtyStr?.Replace(",", "").Replace(".", ""), out int q)) planQty = q;

                                    var newPlan = new PlanningMaster
                                    {
                                        MachineName = currentMachineName,
                                        DateShiftString = currentDateShift,
                                        PartName1 = part1,
                                        PartName2 = part2,
                                        Compound = comp,
                                        PlanTargetPcs = planQty,
                                        Menit = durStr,
                                        WaktuMulai = startStr,
                                        WaktuSelesai = endStr
                                    };

                                    _context.PlanningMasters.Add(newPlan);
                                    insertedCount++;
                                }
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Sukses Scan File! Berhasil mengekstrak {insertedCount} baris jadwal dari Excel ke Database.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Terjadi kesalahan saat scanning: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
