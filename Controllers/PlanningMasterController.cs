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

        public async Task<IActionResult> Index(DateTime? filterDate, string? query, int? machineId)
        {
            // Fetch only Extrude machines (starting with EXT-) for the filter dropdown
            var machines = await _elwpContext.ElwpMachines
                .Where(m => m.KodeMesin != null && m.KodeMesin.ToUpper().StartsWith("EXT"))
                .OrderBy(m => m.KodeMesin)
                .Select(m => new { Id = m.Id, Display = (m.KodeMesin ?? m.Id.ToString()) + " - " + m.NamaMesin })
                .ToListAsync();
            ViewBag.ExtrudeMachines = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(machines, "Id", "Display", machineId);

            var machinesMap = await _elwpContext.ElwpMachines
                .ToDictionaryAsync(x => x.Id, x => $"{(x.KodeMesin ?? x.Id.ToString())} - {x.NamaMesin}");

            var elwpQuery = _elwpContext.ElwpPlannings.Where(x => x.AreaId == 1).AsQueryable();

            if (filterDate.HasValue)
            {
                var start = filterDate.Value.Date;
                var end = start.AddDays(1);
                elwpQuery = elwpQuery.Where(x => x.TanggalPlanning >= start && x.TanggalPlanning < end);
            }

            if (machineId.HasValue)
            {
                elwpQuery = elwpQuery.Where(x => x.MesinId == machineId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                var normalizedQuery = query.Trim().ToLower();
                elwpQuery = elwpQuery.Where(x =>
                    (x.KodeItem != null && x.KodeItem.ToLower().Contains(normalizedQuery)) ||
                    (x.PartName != null && x.PartName.ToLower().Contains(normalizedQuery)) ||
                    (x.PnSap != null && x.PnSap.ToLower().Contains(normalizedQuery)) ||
                    (x.Shift != null && x.Shift.ToLower().Contains(normalizedQuery)));
            }

            var elwpRows = await elwpQuery
                .OrderByDescending(x => x.TanggalPlanning)
                .ThenBy(x => x.MesinId)
                .ThenBy(x => x.Shift)
                .ThenBy(x => x.Id)
                .ToListAsync();

            var data = elwpRows.Select(x => new ElwpPlanningDisplay
            {
                Id = x.Id,
                DateValue = x.TanggalPlanning,
                DateString = x.TanggalPlanning.HasValue ? x.TanggalPlanning.Value.ToString("dddd, d MMMM yyyy") : "",
                MachineName = x.MesinId.HasValue && machinesMap.TryGetValue(x.MesinId.Value, out var name) ? name : x.MesinId?.ToString() ?? "-",
                Shift = x.Shift ?? "-",
                KodeItem = x.KodeItem,
                PartName = x.PartName,
                PnSap = x.PnSap,
                QtyPlanning = x.QtyPlanning
            }).ToList();

            ViewBag.FilterDate = filterDate;
            ViewBag.Query = query;
            ViewBag.MachineId = machineId;
            ViewBag.RecordCount = data.Count;

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ExportCsv(DateTime? filterDate, string? query, int? machineId)
        {
            var machinesMap = await _elwpContext.ElwpMachines
                .ToDictionaryAsync(x => x.Id, x => $"{(x.KodeMesin ?? x.Id.ToString())} - {x.NamaMesin}");

            var elwpQuery = _elwpContext.ElwpPlannings.AsQueryable();

            if (filterDate.HasValue)
            {
                var start = filterDate.Value.Date;
                var end = start.AddDays(1);
                elwpQuery = elwpQuery.Where(x => x.TanggalPlanning >= start && x.TanggalPlanning < end);
            }

            if (machineId.HasValue)
            {
                elwpQuery = elwpQuery.Where(x => x.MesinId == machineId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                var normalizedQuery = query.Trim().ToLower();
                elwpQuery = elwpQuery.Where(x =>
                    (x.KodeItem != null && x.KodeItem.ToLower().Contains(normalizedQuery)) ||
                    (x.PartName != null && x.PartName.ToLower().Contains(normalizedQuery)) ||
                    (x.PnSap != null && x.PnSap.ToLower().Contains(normalizedQuery)) ||
                    (x.Shift != null && x.Shift.ToLower().Contains(normalizedQuery)) ||
                    (x.MesinId != null && x.MesinId.Value.ToString().Contains(normalizedQuery)));
            }

            var elwpRows = await elwpQuery
                .OrderByDescending(x => x.TanggalPlanning)
                .ThenBy(x => x.MesinId)
                .ThenBy(x => x.Shift)
                .ThenBy(x => x.Id)
                .ToListAsync();

            var csvBuilder = new System.Text.StringBuilder();
            csvBuilder.AppendLine("Tanggal,Mesin,Shift,Kode Item,Part Name,PN SAP,Target (pcs)");

            foreach (var row in elwpRows)
            {
                var date = row.TanggalPlanning.HasValue ? row.TanggalPlanning.Value.ToString("yyyy-MM-dd") : "";
                var machine = row.MesinId.HasValue && machinesMap.TryGetValue(row.MesinId.Value, out var name) ? name : row.MesinId?.ToString() ?? "-";
                var shift = row.Shift?.Replace(",", " ") ?? "";
                var kodeItem = row.KodeItem?.Replace(",", " ") ?? "";
                var partName = row.PartName?.Replace(",", " ") ?? "";
                var pnSap = row.PnSap?.Replace(",", " ") ?? "";
                var qty = row.QtyPlanning?.ToString() ?? "";
                csvBuilder.AppendLine($"{date},{machine},{shift},{kodeItem},{partName},{pnSap},{qty}");
            }

            var fileName = $"ELWP_Planning_{DateTime.Now:yyyyMMddHHmmss}.csv";
            var bytes = System.Text.Encoding.UTF8.GetBytes(csvBuilder.ToString());
            return File(bytes, "text/csv", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel(DateTime? filterDate, string? query, int? machineId)
        {
            var machinesMap = await _elwpContext.ElwpMachines
                .ToDictionaryAsync(x => x.Id, x => $"{(x.KodeMesin ?? x.Id.ToString())} - {x.NamaMesin}");

            var elwpQuery = _elwpContext.ElwpPlannings.AsQueryable();

            if (filterDate.HasValue)
            {
                var start = filterDate.Value.Date;
                var end = start.AddDays(1);
                elwpQuery = elwpQuery.Where(x => x.TanggalPlanning >= start && x.TanggalPlanning < end);
            }
            if (machineId.HasValue)
                elwpQuery = elwpQuery.Where(x => x.MesinId == machineId.Value);
            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim().ToLower();
                elwpQuery = elwpQuery.Where(x =>
                    (x.KodeItem != null && x.KodeItem.ToLower().Contains(q)) ||
                    (x.PartName != null && x.PartName.ToLower().Contains(q)) ||
                    (x.PnSap != null && x.PnSap.ToLower().Contains(q)) ||
                    (x.Shift != null && x.Shift.ToLower().Contains(q)));
            }

            var rows = await elwpQuery
                .OrderByDescending(x => x.TanggalPlanning)
                .ThenBy(x => x.MesinId)
                .ThenBy(x => x.Shift)
                .ToListAsync();

            using var package = new OfficeOpenXml.ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Planning Master");

            // Title row
            ws.Cells[1, 1].Value = "PLANNING MASTER - ELWP Production System";
            ws.Cells[1, 1, 1, 7].Merge = true;
            ws.Cells[1, 1].Style.Font.Bold = true;
            ws.Cells[1, 1].Style.Font.Size = 14;
            ws.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(15, 25, 35));
            ws.Cells[1, 1].Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(0, 255, 136));

            // Sub-title
            ws.Cells[2, 1].Value = $"Export Date: {DateTime.Now:dd MMM yyyy HH:mm}";
            ws.Cells[2, 1, 2, 7].Merge = true;
            ws.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells[2, 1].Style.Font.Italic = true;
            ws.Cells[2, 1].Style.Font.Color.SetColor(System.Drawing.Color.Gray);

            // Header row
            var headers = new[] { "No", "Tanggal", "Mesin", "Shift", "Kode Item", "Part Name", "PN SAP", "Target (pcs)" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cells[4, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.Color.SetColor(System.Drawing.Color.White);
                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(10, 20, 30));
                cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.FromArgb(0, 180, 100));
                cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }

            // Data rows
            int row = 5;
            int no = 1;
            foreach (var r in rows)
            {
                var date = r.TanggalPlanning.HasValue ? r.TanggalPlanning.Value.ToString("dd/MM/yyyy") : "";
                var machine = r.MesinId.HasValue && machinesMap.TryGetValue(r.MesinId.Value, out var mname) ? mname : r.MesinId?.ToString() ?? "-";
                bool isOdd = (no % 2 == 0);
                var bgColor = isOdd ? System.Drawing.Color.FromArgb(20, 28, 38) : System.Drawing.Color.FromArgb(25, 34, 46);

                var rowData = new object[] { no, date, machine, r.Shift ?? "-", r.KodeItem ?? "-", r.PartName ?? "-", r.PnSap ?? "-", r.QtyPlanning ?? 0 };
                for (int col = 1; col <= rowData.Length; col++)
                {
                    var cell = ws.Cells[row, col];
                    cell.Value = rowData[col - 1];
                    cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(bgColor);
                    cell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(220, 220, 220));
                    cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Hair, System.Drawing.Color.FromArgb(50, 60, 70));
                    cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                }
                // Highlight Kode Item column
                ws.Cells[row, 5].Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(100, 200, 255));
                ws.Cells[row, 5].Style.Font.Bold = true;
                // Highlight qty
                ws.Cells[row, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells[row, 8].Style.Font.Bold = true;

                row++;
                no++;
            }

            // Auto fit columns
            ws.Column(1).Width = 6;
            ws.Column(2).Width = 18;
            ws.Column(3).Width = 32;
            ws.Column(4).Width = 10;
            ws.Column(5).Width = 14;
            ws.Column(6).Width = 50;
            ws.Column(7).Width = 24;
            ws.Column(8).Width = 14;
            ws.Row(4).Height = 20;

            var fileName = $"ELWP_Planning_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var content = package.GetAsByteArray();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
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
                        .Where(x => x.AreaId == 1) // Filter Extrude Only
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
                            CompoundMiddle  = pm?.CompoundMiddle ?? "",
                            CompoundOuter   = pm?.CompoundOuter ?? "",
                            Length          = pm?.Length ?? "",
                            CtAwal          = pm?.CtAwal?.ToString("G29") ?? "",
                            CtMinus20       = pm?.CtMinus20?.ToString("G29") ?? "",
                            NeedKgInner     = pm?.NeedKgInner?.ToString("G29") ?? "",
                            NeedKgMiddle    = pm?.NeedKgMiddle?.ToString("G29") ?? "",
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
                                        NeedKgMiddle = pm?.NeedKgMiddle?.ToString("G29"),
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
