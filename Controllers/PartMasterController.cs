using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExcelDataReader;
using System.Data;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Controllers
{
    public class PartMasterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PartMasterController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            await EnsureTableExists();
            var parts = await _context.PartMasters
                .OrderBy(x => x.PartCode)
                .ToListAsync();
            return View(parts);
        }

        // ─── API: Lookup part by PartCode atau PartNumber (for auto-fill in planning) ───
        [HttpGet]
        public async Task<IActionResult> Lookup(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Json(null);

            string search = q.Trim();

            // Coba exact match dulu (case-insensitive)
            var part = await _context.PartMasters
                .Where(x => (x.PartCode != null && x.PartCode.ToLower() == search.ToLower()) || 
                            (x.PartNumber != null && x.PartNumber.ToLower() == search.ToLower()))
                .FirstOrDefaultAsync();

            // Jika gagal, coba yang diawali dengan search
            if (part == null)
            {
                part = await _context.PartMasters
                    .Where(x => (x.PartCode != null && x.PartCode.Contains(search)) || 
                                (x.PartNumber != null && x.PartNumber.Contains(search)))
                    .FirstOrDefaultAsync();
            }

            if (part == null)
                return Json(null);

            return Json(new
            {
                partCode      = part.PartCode,
                partNumber    = part.PartNumber,
                description   = part.Description,
                diameter      = part.Diameter,
                length        = part.Length,
                compoundInner = part.CompoundInner,
                compoundOuter = part.CompoundOuter,
                compoundCombo = part.CompoundCombo,
                needKgInner   = part.NeedKgInner,
                needKgOuter   = part.NeedKgOuter,
                secPerPcs     = part.SecPerPcs,
                ctAwal        = part.CtAwal,
                ctMinus20     = part.CtMinus20
            });
        }

        // ─── API: Search suggestions ───
        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return Json(new List<object>());

            var results = await _context.PartMasters
                .Where(x => (x.PartCode != null && x.PartCode.Contains(q)) ||
                            (x.PartNumber != null && x.PartNumber.Contains(q)) ||
                            (x.Description != null && x.Description.Contains(q)))
                .Take(10)
                .Select(x => new { x.PartCode, x.PartNumber, x.Description, x.CtAwal })
                .ToListAsync();

            return Json(results);
        }

        // ─── Import DL0 dari Excel ───
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
                _context.PartMasters.RemoveRange(_context.PartMasters);
                await _context.SaveChangesAsync();

                int insertedCount = 0;

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet();

                        // Cari sheet DL0
                        DataTable? dl0 = null;
                        foreach (DataTable t in result.Tables)
                        {
                            if (t.TableName.Trim().Equals("DL0", StringComparison.OrdinalIgnoreCase))
                            { dl0 = t; break; }
                        }

                        if (dl0 == null)
                        {
                            TempData["ErrorMessage"] = "Sheet 'DL0' tidak ditemukan di file Excel ini.";
                            return RedirectToAction(nameof(Index));
                        }

                        // Header ada di baris ke-5 (index 5), data mulai baris 6
                        int totalCols = dl0.Columns.Count;
                        string GetV(DataRow r, int idx) =>
                            (idx >= 0 && idx < totalCols) ? r[idx]?.ToString()?.Trim() ?? "" : "";

                        for (int i = 6; i < dl0.Rows.Count; i++)
                        {
                            var row = dl0.Rows[i];
                            var partCode = GetV(row, 1);

                            // Skip baris kosong atau header ulang
                            if (string.IsNullOrEmpty(partCode)) continue;
                            if (partCode.Equals("PART CODE", StringComparison.OrdinalIgnoreCase)) continue;

                            decimal? ParseDec(string s) =>
                                decimal.TryParse(s.Replace(",", "."),
                                    System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : null;

                            var part = new PartMaster
                            {
                                PartCode      = partCode,
                                PartNumber    = GetV(row, 2),
                                Description   = GetV(row, 3),
                                Length        = GetV(row, 4),
                                Diameter      = GetV(row, 5),
                                CompoundInner = GetV(row, 6),
                                CompoundOuter = GetV(row, 7),
                                CompoundCombo = GetV(row, 8),
                                NeedKgInner   = ParseDec(GetV(row, 9)),
                                NeedKgOuter   = ParseDec(GetV(row, 10)),
                                SecPerPcs     = ParseDec(GetV(row, 11)), // Col L (index 11)
                                CtMinus20     = ParseDec(GetV(row, 14)), // Col O (index 14)
                                CtAwal        = ParseDec(GetV(row, 15)), // Col P (index 15)
                                Senin         = GetV(row, 16),
                                Selasa        = GetV(row, 17),
                                Rabu          = GetV(row, 18),
                                Kamis         = GetV(row, 19),
                                Jumat         = GetV(row, 20),
                                Sabtu         = GetV(row, 21),
                                Minggu        = GetV(row, 22),
                                ImportedAt    = DateTime.Now
                            };

                            _context.PartMasters.Add(part);
                            insertedCount++;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Sukses! {insertedCount} part berhasil diimport dari sheet DL0.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Gagal import: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ClearData()
        {
            _context.PartMasters.RemoveRange(_context.PartMasters);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Semua data Part Master telah dikosongkan.";
            return RedirectToAction(nameof(Index));
        }

        private async Task EnsureTableExists()
        {
            // Cek apakah tabel ada dengan kolom NeedKgInner bertipe string (schema lama)
            // Menggunakan INFORMATION_SCHEMA agar lebih reliabel lintas versi SQL Server
            await _context.Database.ExecuteSqlRawAsync(@"
            IF EXISTS (
                SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'PartMasters' AND COLUMN_NAME = 'NeedKgInner' 
                AND DATA_TYPE IN ('nvarchar', 'varchar', 'text')
            )
            BEGIN
                DROP TABLE [dbo].[PartMasters]
            END
            ");

            await _context.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PartMasters]') AND type in (N'U'))
            BEGIN
            CREATE TABLE [dbo].[PartMasters](
                [Id] [int] IDENTITY(1,1) NOT NULL,
                [PartCode] [nvarchar](max) NULL,
                [PartNumber] [nvarchar](max) NULL,
                [Description] [nvarchar](max) NULL,
                [Length] [nvarchar](max) NULL,
                [Diameter] [nvarchar](max) NULL,
                [CompoundInner] [nvarchar](max) NULL,
                [CompoundOuter] [nvarchar](max) NULL,
                [CompoundCombo] [nvarchar](max) NULL,
                [NeedKgInner] [decimal](18,6) NULL,
                [NeedKgOuter] [decimal](18,6) NULL,
                [SecPerPcs] [decimal](18,4) NULL,
                [CtMinus20] [decimal](18,4) NULL,
                [CtAwal] [decimal](18,4) NULL,
                [Senin] [nvarchar](20) NULL,
                [Selasa] [nvarchar](20) NULL,
                [Rabu] [nvarchar](20) NULL,
                [Kamis] [nvarchar](20) NULL,
                [Jumat] [nvarchar](20) NULL,
                [Sabtu] [nvarchar](20) NULL,
                [Minggu] [nvarchar](20) NULL,
                [ImportedAt] [datetime2](7) NOT NULL,
             CONSTRAINT [PK_PartMasters] PRIMARY KEY CLUSTERED ([Id] ASC)
            )
            END
            ");
        }
    }
}
