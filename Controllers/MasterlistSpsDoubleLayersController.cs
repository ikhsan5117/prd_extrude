using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExcelDataReader;
using System.Data;
using System.Text;
using System.Globalization;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Controllers
{
    public class MasterlistSpsDoubleLayersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ElwpDbContext _elwpContext;
        private readonly ILogger<MasterlistSpsDoubleLayersController> _logger;

        public MasterlistSpsDoubleLayersController(ApplicationDbContext context, ElwpDbContext elwpContext, ILogger<MasterlistSpsDoubleLayersController> logger)
        {
            _context = context;
            _elwpContext = elwpContext;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Ambil semua data SPS (tanpa filter ketat yang menyembunyikan data)
            var data = await _context.MasterlistSpsDoubleLayers.ToListAsync();

            // Ambil daftar mesin yang benar-benar ada di data SPS untuk dropdown filter
            var machinesInData = data
                .Select(m => m.Machine)
                .Where(m => !string.IsNullOrEmpty(m))
                .Distinct()
                .OrderBy(m => m)
                .ToList();

            // Juga tambahkan MachineCode jika Machine-nya kosong tapi MC ada
            var codesInData = data
                .Where(m => string.IsNullOrEmpty(m.Machine) && !string.IsNullOrEmpty(m.MachineCode))
                .Select(m => m.MachineCode)
                .Distinct()
                .ToList();

            ViewBag.AuthorizedMachines = machinesInData.Concat(codesInData).Distinct().OrderBy(x => x).ToList();
            
            // Tambahkan daftar untuk autocomplete (hilangkan dash sesuai request)
            ViewBag.ItemList = data.Select(m => m.ItemList != null ? m.ItemList.Replace("-", "") : null)
                .Where(i => !string.IsNullOrEmpty(i))
                .Distinct()
                .OrderBy(i => i)
                .ToList();
            ViewBag.DocList = data.Select(m => m.DocumentNumber).Where(d => !string.IsNullOrEmpty(d)).Distinct().OrderBy(d => d).ToList();

            return View(data);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.MasterlistSpsDoubleLayers
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (item == null) return NotFound();

            return View(item);
        }

        // GET: MasterlistSpsDoubleLayers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MasterlistSpsDoubleLayers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MasterlistSpsDoubleLayer item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Data SPS berhasil ditambahkan!";
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: MasterlistSpsDoubleLayers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.MasterlistSpsDoubleLayers.FindAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: MasterlistSpsDoubleLayers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MasterlistSpsDoubleLayer item)
        {
            if (id != item.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Data SPS berhasil diperbarui!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // POST: MasterlistSpsDoubleLayers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.MasterlistSpsDoubleLayers.FindAsync(id);
            if (item != null)
            {
                _context.MasterlistSpsDoubleLayers.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Data SPS berhasil dihapus!";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: MasterlistSpsDoubleLayers/DeleteMultiple
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple([FromBody] int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return Json(new { success = false, message = "Tidak ada data yang dipilih." });
            }

            try
            {
                var itemsToDelete = await _context.MasterlistSpsDoubleLayers
                    .Where(m => ids.Contains(m.Id))
                    .ToListAsync();

                if (itemsToDelete.Any())
                {
                    _context.MasterlistSpsDoubleLayers.RemoveRange(itemsToDelete);
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true, message = $"{itemsToDelete.Count} data berhasil dihapus." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Terjadi kesalahan: " + ex.Message });
            }
        }

        private bool ItemExists(int id)
        {
            return _context.MasterlistSpsDoubleLayers.Any(e => e.Id == id);
        }

        // GET: MasterlistSpsDoubleLayers/ExportExcel
        [HttpGet]
        public async Task<IActionResult> ExportExcel()
        {
            var data = await _context.MasterlistSpsDoubleLayers
                .OrderBy(m => m.DocumentNumber)
                .ThenBy(m => m.ItemList)
                .ToListAsync();

            using var package = new OfficeOpenXml.ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("SPS Master");

            // --- Title ---
            ws.Cells[1, 1].Value = "SPS MASTER - Standard Parameter Setting";
            ws.Cells[1, 1, 1, 20].Merge = true;
            ws.Cells[1, 1].Style.Font.Bold = true;
            ws.Cells[1, 1].Style.Font.Size = 14;
            ws.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(15, 25, 45));
            ws.Cells[1, 1].Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(100, 160, 255));

            ws.Cells[2, 1].Value = $"Export Date: {DateTime.Now:dd MMM yyyy HH:mm} | Total: {data.Count} records";
            ws.Cells[2, 1, 2, 20].Merge = true;
            ws.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Cells[2, 1].Style.Font.Italic = true;
            ws.Cells[2, 1].Style.Font.Color.SetColor(System.Drawing.Color.Gray);

            // --- Headers ---
            var headers = new[]
            {
                "No", "No. Doc", "Item List", "Hose Type", "Machine", "Dimensi",
                "Mat. Inner", "Mat. Outer", "Mat. Middle",
                "Head Temp 1", "Cyl1-1", "Cyl2-1", "Cyl3-1", "Screw Temp 1", "Screw Speed 1", "Pressure 1",
                "Head Temp 2", "Cyl1-2", "Cyl2-2", "Cyl3-2", "Screw Temp 2", "Screw Speed 2", "Pressure 2",
                "Head Temp 3", "Cyl1-3", "Cyl2-3", "Cyl3-3", "Screw Temp 3", "Screw Speed 3", "Pressure 3",
                "Hose Speed", "Takeup Speed", "Chiller Temp",
                "Tol Inner", "Tol Outer", "Tebal Inner", "Tebal Outer", "Tebal Total",
                "Nipple Die", "Tube Die", "Cover Die", "Spacer Die",
                "Customer", "Rev. Number", "Rev. Date"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cells[4, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.Color.SetColor(System.Drawing.Color.White);
                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(13, 60, 120));
                cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.FromArgb(60, 100, 200));
                cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                cell.Style.WrapText = false;
            }

            // --- Data rows ---
            int row = 5;
            int no = 1;
            foreach (var m in data)
            {
                bool isOdd = (no % 2 == 0);
                var bgColor = isOdd
                    ? System.Drawing.Color.FromArgb(18, 25, 38)
                    : System.Drawing.Color.FromArgb(23, 32, 50);

                var values = new object[]
                {
                    no, m.DocumentNumber ?? "", m.ItemList ?? "", m.HoseType ?? "", m.Machine ?? "", m.Dimensi ?? "",
                    m.InnerTube ?? "", m.OuterCover ?? "", m.MiddleTube ?? "",
                    m.HeadTemp1 ?? "", m.Cylinder1_1 ?? "", m.Cylinder2_1 ?? "", m.Cylinder3_1 ?? "", m.ScrewTemp1 ?? "", m.ScrewSpeed1 ?? "", m.Pressure1 ?? "",
                    m.HeadTemp2 ?? "", m.Cylinder1_2 ?? "", m.Cylinder2_2 ?? "", m.Cylinder3_2 ?? "", m.ScrewTemp2 ?? "", m.ScrewSpeed2 ?? "", m.Pressure2 ?? "",
                    m.HeadTemp3 ?? "", m.Cylinder1_3 ?? "", m.Cylinder2_3 ?? "", m.Cylinder3_3 ?? "", m.ScrewTemp3 ?? "", m.ScrewSpeed3 ?? "", m.Pressure3 ?? "",
                    m.HoseSpeed ?? "", m.TakeUpConveyorSpeed ?? "", m.ChillerWaterTemp ?? "",
                    m.ToleranceInner ?? "", m.ToleranceOuter ?? "", m.TebalInner ?? "", m.TebalOuter ?? "", m.TebalTotal ?? "",
                    m.Nipple ?? "", m.TubeDie ?? "", m.CoverDie ?? "", m.SpacerDie ?? "",
                    m.Customer ?? "", m.RevisionNumber ?? "", m.RevisionDate ?? ""
                };

                for (int col = 1; col <= values.Length; col++)
                {
                    var cell = ws.Cells[row, col];
                    cell.Value = values[col - 1];
                    cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(bgColor);
                    cell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(210, 210, 210));
                    cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Hair, System.Drawing.Color.FromArgb(40, 55, 80));
                    cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                }
                // Highlight Item List & Doc columns
                ws.Cells[row, 2].Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(120, 180, 255));
                ws.Cells[row, 3].Style.Font.Bold = true;
                ws.Cells[row, 3].Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(100, 220, 255));

                row++;
                no++;
            }

            // Column widths
            ws.Column(1).Width = 5;
            ws.Column(2).Width = 20;
            ws.Column(3).Width = 14;
            ws.Column(4).Width = 22;
            ws.Column(5).Width = 12;
            ws.Column(6).Width = 10;
            for (int c = 7; c <= headers.Length; c++) ws.Column(c).Width = 13;
            ws.Row(4).Height = 22;

            var fileName = $"SPS_Master_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(package.GetAsByteArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // GET: MasterlistSpsDoubleLayers/DownloadTemplate
        public IActionResult DownloadTemplate(string type = "3L")
        {
            try
            {
                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("SPS Parameter Setting");
                    
                    bool is3Layer = (type == "3L");
                    bool is2Layer = (type == "2L");
                    bool isDoubleLayer = (type == "DL");
                    
                    // Determine column count and Item List position
                    int itemColIndex;
                    string titleText;
                    
                    if (is3Layer) {
                        itemColIndex = 108;
                        titleText = "PARAMETER SETTING CHS 3 LAYER DIGITALISASI";
                    } else if (is2Layer) {
                        itemColIndex = 90;
                        titleText = "PARAMETER SETTING CHS 2 LAYER DIGITALISASI";
                    } else { // Double Layer (Legacy/Non-CHS)
                        itemColIndex = 51;
                        titleText = "PARAMETER SETTING DOUBLE LAYER (NON-CHS)";
                    }
                    
                    // ROW 1: Title
                    worksheet.Cells[1, 2].Value = titleText;
                    worksheet.Cells[1, 2].Style.Font.Bold = true;
                    worksheet.Cells[1, 2].Style.Font.Size = 14;
                    
                    // ROW 2-5: Additional info (optional, can be left empty)
                    worksheet.Cells[2, 2].Value = "Velasto Production System";
                    
                    // ROW 6: Column Headers (Index Row)
                    // This helps users know which column is which during data entry
                    for (int i = 1; i <= itemColIndex; i++)
                    {
                        worksheet.Cells[6, i].Value = i.ToString();
                        worksheet.Cells[6, i].Style.Font.Bold = true;
                        worksheet.Cells[6, i].Style.Font.Size = 8;
                        worksheet.Cells[6, i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[6, i].Style.Fill.BackgroundColor.SetColor(255, 211, 211, 211); // LightGray ARGB
                    }
                    
                    // ROW 5: Header Labels (Detailed column names)
                    SetHeaderLabel(worksheet, 5, 1, "ID");
                    SetHeaderLabel(worksheet, 5, 2, "No");
                    SetHeaderLabel(worksheet, 5, 3, "Machine");
                    SetHeaderLabel(worksheet, 5, 4, "No. Doc");
                    SetHeaderLabel(worksheet, 5, 5, "");
                    SetHeaderLabel(worksheet, 5, 6, "");
                    SetHeaderLabel(worksheet, 5, 7, "No. Rev");
                    SetHeaderLabel(worksheet, 5, 8, "Customer");
                    SetHeaderLabel(worksheet, 5, 9, "Rev. Date");
                    SetHeaderLabel(worksheet, 5, 10, "Formulasi");
                    SetHeaderLabel(worksheet, 5, 11, "Hose Type");
                    SetHeaderLabel(worksheet, 5, 12, "Dimensi");
                    SetHeaderLabel(worksheet, 5, 13, "Material");
                    SetHeaderLabel(worksheet, 5, 14, "Inner Tube");
                    
                    if (is3Layer)
                    {
                        SetHeaderLabel(worksheet, 5, 15, "Middle Tube");
                        SetHeaderLabel(worksheet, 5, 16, "Outer Cover");
                        SetHeaderLabel(worksheet, 5, 17, "Use Limits Inner");
                        SetHeaderLabel(worksheet, 5, 18, "Use Limits Middle");
                        SetHeaderLabel(worksheet, 5, 19, "Use Limits Outer");
                        SetHeaderLabel(worksheet, 5, 20, "Yarn");
                        SetHeaderLabel(worksheet, 5, 21, "Tension Yarn Inner");
                        SetHeaderLabel(worksheet, 5, 22, "Tension Yarn Outer");
                        SetHeaderLabel(worksheet, 5, 23, "Nipple");
                        SetHeaderLabel(worksheet, 5, 24, "Tube Die");
                        SetHeaderLabel(worksheet, 5, 25, "Middle Die");
                        SetHeaderLabel(worksheet, 5, 26, "Cover Die");
                        SetHeaderLabel(worksheet, 5, 27, "Spacer");
                        SetHeaderLabel(worksheet, 5, 28, "A Distance");
                        SetHeaderLabel(worksheet, 5, 29, "Mesh Dim 1");
                        SetHeaderLabel(worksheet, 5, 30, "Mesh Screen 1");
                        SetHeaderLabel(worksheet, 5, 31, "Mesh Dim 2");
                        SetHeaderLabel(worksheet, 5, 32, "Mesh Screen 2");
                        SetHeaderLabel(worksheet, 5, 33, "Mesh Dim 3");
                        SetHeaderLabel(worksheet, 5, 34, "Mesh Screen 3");
                        SetHeaderLabel(worksheet, 5, 35, "Head Temp 1");
                        SetHeaderLabel(worksheet, 5, 36, "Cylinder 1-1");
                        SetHeaderLabel(worksheet, 5, 37, "Cylinder 2-1");
                        SetHeaderLabel(worksheet, 5, 38, "Cylinder 3-1");
                        SetHeaderLabel(worksheet, 5, 39, "Screw Temp 1");
                        SetHeaderLabel(worksheet, 5, 40, "Head Temp 2");
                        SetHeaderLabel(worksheet, 5, 41, "Cylinder 1-2");
                        SetHeaderLabel(worksheet, 5, 42, "Cylinder 2-2");
                        SetHeaderLabel(worksheet, 5, 43, "Cylinder 3-2");
                        SetHeaderLabel(worksheet, 5, 44, "Screw Temp 2");
                        SetHeaderLabel(worksheet, 5, 45, "Head Temp 3");
                        SetHeaderLabel(worksheet, 5, 46, "Cylinder 1-3");
                        SetHeaderLabel(worksheet, 5, 47, "Cylinder 2-3");
                        SetHeaderLabel(worksheet, 5, 48, "Cylinder 3-3");
                        SetHeaderLabel(worksheet, 5, 49, "Screw Temp 3");
                        SetHeaderLabel(worksheet, 5, 50, "Screw Speed 1");
                        SetHeaderLabel(worksheet, 5, 51, "Screw Speed 2");
                        SetHeaderLabel(worksheet, 5, 52, "Screw Speed 3");
                        SetHeaderLabel(worksheet, 5, 53, "Feed Roll Ratio 1");
                        SetHeaderLabel(worksheet, 5, 54, "Feed Roll Ratio 2");
                        SetHeaderLabel(worksheet, 5, 55, "Feed Roll Ratio 3");
                        SetHeaderLabel(worksheet, 5, 56, "Pressure 1");
                        SetHeaderLabel(worksheet, 5, 57, "Pressure 2");
                        SetHeaderLabel(worksheet, 5, 58, "Pressure 3");
                        SetHeaderLabel(worksheet, 5, 59, "Am Meter 1");
                        SetHeaderLabel(worksheet, 5, 60, "Am Meter 2");
                        SetHeaderLabel(worksheet, 5, 61, "Am Meter 3");
                        SetHeaderLabel(worksheet, 5, 62, "Preset Value");
                        SetHeaderLabel(worksheet, 5, 63, "Control Value");
                        SetHeaderLabel(worksheet, 5, 64, "Spiral Pitch Setting");
                        SetHeaderLabel(worksheet, 5, 65, "Spiral Pitch Display");
                        SetHeaderLabel(worksheet, 5, 66, "Spiral Speed");
                        SetHeaderLabel(worksheet, 5, 67, "Hose Speed");
                        SetHeaderLabel(worksheet, 5, 68, "Unsmooth Surface");
                        SetHeaderLabel(worksheet, 5, 69, "Marking Sort");
                        SetHeaderLabel(worksheet, 5, 70, "Text Marking Material");
                        SetHeaderLabel(worksheet, 5, 71, "Marking Colour");
                        SetHeaderLabel(worksheet, 5, 72, "Chiller Water Temp");
                        SetHeaderLabel(worksheet, 5, 73, "Caterpillar Gap");
                        SetHeaderLabel(worksheet, 5, 74, "Take Up Conveyor Speed");
                        SetHeaderLabel(worksheet, 5, 75, "Cool Conveyor Speed");
                        SetHeaderLabel(worksheet, 5, 76, "Cool Conveyor Speed 2");
                        SetHeaderLabel(worksheet, 5, 77, "Conveyor Ratio");
                        SetHeaderLabel(worksheet, 5, 78, "Tolerance Inner");
                        SetHeaderLabel(worksheet, 5, 79, "Tolerance Outer");
                        SetHeaderLabel(worksheet, 5, 80, "Tebal Inner");
                        SetHeaderLabel(worksheet, 5, 81, "Tebal Inner+Middle");
                        SetHeaderLabel(worksheet, 5, 82, "Tebal Total");
                        SetHeaderLabel(worksheet, 5, 83, "Selisih Tebal");
                        // Quality Matrix - Inner (84-95 -> index 89-94 in code = col 90-95)
                        SetHeaderLabel(worksheet, 5, 90, "Inner Target");
                        SetHeaderLabel(worksheet, 5, 91, "Inner Tol");
                        SetHeaderLabel(worksheet, 5, 92, "Inner LCL");
                        SetHeaderLabel(worksheet, 5, 93, "Inner Min");
                        SetHeaderLabel(worksheet, 5, 94, "Inner UCL");
                        SetHeaderLabel(worksheet, 5, 95, "Inner Max");
                        // Quality Matrix - Inner+Mid (96-101 in code = col 96-101)
                        SetHeaderLabel(worksheet, 5, 96, "Inner+Mid Target");
                        SetHeaderLabel(worksheet, 5, 97, "Inner+Mid Tol");
                        SetHeaderLabel(worksheet, 5, 98, "Inner+Mid LCL");
                        SetHeaderLabel(worksheet, 5, 99, "Inner+Mid Min");
                        SetHeaderLabel(worksheet, 5, 100, "Inner+Mid UCL");
                        SetHeaderLabel(worksheet, 5, 101, "Inner+Mid Max");
                        // Quality Matrix - Total (102-107 in code = col 102-107)
                        SetHeaderLabel(worksheet, 5, 102, "Total Target");
                        SetHeaderLabel(worksheet, 5, 103, "Total Tol");
                        SetHeaderLabel(worksheet, 5, 104, "Total LCL");
                        SetHeaderLabel(worksheet, 5, 105, "Total Min");
                        SetHeaderLabel(worksheet, 5, 106, "Total UCL");
                        SetHeaderLabel(worksheet, 5, 107, "Total Max");
                        SetHeaderLabel(worksheet, 5, 108, "Item List");
                    }
                    else if (is2Layer) // 2 Layer
                    {
                        SetHeaderLabel(worksheet, 5, 15, "Outer Cover");
                        SetHeaderLabel(worksheet, 5, 16, "Use Limits Inner");
                        SetHeaderLabel(worksheet, 5, 17, "Use Limits Outer");
                        SetHeaderLabel(worksheet, 5, 18, "Yarn");
                        SetHeaderLabel(worksheet, 5, 19, "Pitch Yarn");
                        SetHeaderLabel(worksheet, 5, 20, "Tension Yarn Inner");
                        SetHeaderLabel(worksheet, 5, 21, "Tension Yarn Outer");
                        SetHeaderLabel(worksheet, 5, 22, "Nipple");
                        SetHeaderLabel(worksheet, 5, 23, "Tube Die");
                        SetHeaderLabel(worksheet, 5, 24, "Middle Die");
                        SetHeaderLabel(worksheet, 5, 25, "Cover Die");
                        SetHeaderLabel(worksheet, 5, 26, "Spacer");
                        SetHeaderLabel(worksheet, 5, 27, "A Distance");
                        SetHeaderLabel(worksheet, 5, 28, "Mesh Dim 1");
                        SetHeaderLabel(worksheet, 5, 29, "Mesh Screen 1");
                        SetHeaderLabel(worksheet, 5, 30, "Mesh Dim 2");
                        SetHeaderLabel(worksheet, 5, 31, "Mesh Screen 2");
                        SetHeaderLabel(worksheet, 5, 32, "Head Temp 1");
                        SetHeaderLabel(worksheet, 5, 33, "Cylinder 1-1");
                        SetHeaderLabel(worksheet, 5, 34, "Cylinder 2-1");
                        SetHeaderLabel(worksheet, 5, 35, "Cylinder 3-1");
                        SetHeaderLabel(worksheet, 5, 36, "Screw Temp 1");
                        SetHeaderLabel(worksheet, 5, 37, "Head Temp 2");
                        SetHeaderLabel(worksheet, 5, 38, "Cylinder 1-2");
                        SetHeaderLabel(worksheet, 5, 39, "Cylinder 2-2");
                        SetHeaderLabel(worksheet, 5, 40, "Cylinder 3-2");
                        SetHeaderLabel(worksheet, 5, 41, "Screw Temp 2");
                        SetHeaderLabel(worksheet, 5, 42, "Screw Speed 1");
                        SetHeaderLabel(worksheet, 5, 43, "Screw Speed 2");
                        SetHeaderLabel(worksheet, 5, 44, "Feed Roll Ratio 1");
                        SetHeaderLabel(worksheet, 5, 45, "Feed Roll Ratio 2");
                        SetHeaderLabel(worksheet, 5, 46, "Pressure 1");
                        SetHeaderLabel(worksheet, 5, 47, "Pressure 2");
                        SetHeaderLabel(worksheet, 5, 48, "Current Value");
                        SetHeaderLabel(worksheet, 5, 49, "Am Meter 1");
                        SetHeaderLabel(worksheet, 5, 50, "Am Meter 2");
                        SetHeaderLabel(worksheet, 5, 51, "Preset Value");
                        SetHeaderLabel(worksheet, 5, 52, "Control Value");
                        SetHeaderLabel(worksheet, 5, 53, "Spiral Pitch Setting");
                        SetHeaderLabel(worksheet, 5, 54, "Spiral Pitch Display");
                        SetHeaderLabel(worksheet, 5, 55, "Spiral Speed");
                        SetHeaderLabel(worksheet, 5, 56, "Hose Speed");
                        SetHeaderLabel(worksheet, 5, 57, "Unsmooth Surface");
                        SetHeaderLabel(worksheet, 5, 58, "Marking Sort");
                        SetHeaderLabel(worksheet, 5, 59, "Text Marking Material");
                        SetHeaderLabel(worksheet, 5, 60, "Marking Colour");
                        SetHeaderLabel(worksheet, 5, 61, "Chiller Water Temp");
                        SetHeaderLabel(worksheet, 5, 62, "Dancer Position");
                        SetHeaderLabel(worksheet, 5, 63, "Caterpillar Gap");
                        SetHeaderLabel(worksheet, 5, 64, "Take Up Conveyor Speed");
                        SetHeaderLabel(worksheet, 5, 65, "Cool Conveyor Speed");
                        SetHeaderLabel(worksheet, 5, 66, "Conveyor Ratio");
                        SetHeaderLabel(worksheet, 5, 67, "Tolerance Inner");
                        SetHeaderLabel(worksheet, 5, 68, "Tolerance Outer");
                        SetHeaderLabel(worksheet, 5, 69, "Tebal Inner");
                        SetHeaderLabel(worksheet, 5, 70, "Tebal Total");
                        SetHeaderLabel(worksheet, 5, 71, "Selisih Tebal");
                        SetHeaderLabel(worksheet, 5, 72, "Thick Target");
                        SetHeaderLabel(worksheet, 5, 73, "Thick Tol");
                        SetHeaderLabel(worksheet, 5, 74, "Thick LCL");
                        SetHeaderLabel(worksheet, 5, 75, "Thick Min");
                        SetHeaderLabel(worksheet, 5, 76, "Thick UCL");
                        SetHeaderLabel(worksheet, 5, 77, "Thick Max");
                        // Quality Matrix - Inner
                        SetHeaderLabel(worksheet, 5, 78, "Inner Target");
                        SetHeaderLabel(worksheet, 5, 79, "Inner Tol");
                        SetHeaderLabel(worksheet, 5, 80, "Inner LCL");
                        SetHeaderLabel(worksheet, 5, 81, "Inner Min");
                        SetHeaderLabel(worksheet, 5, 82, "Inner UCL");
                        SetHeaderLabel(worksheet, 5, 83, "Inner Max");
                        // Quality Matrix - Total
                        SetHeaderLabel(worksheet, 5, 84, "Total Target");
                        SetHeaderLabel(worksheet, 5, 85, "Total Tol");
                        SetHeaderLabel(worksheet, 5, 86, "Total LCL");
                        SetHeaderLabel(worksheet, 5, 87, "Total Min");
                        SetHeaderLabel(worksheet, 5, 88, "Total UCL");
                        SetHeaderLabel(worksheet, 5, 89, "Total Max");
                        SetHeaderLabel(worksheet, 5, 90, "Item List");
                    }
                    else if (isDoubleLayer) // Double Layer (Legacy/Non-CHS)
                    {
                        SetHeaderLabel(worksheet, 5, 15, "Outer Cover");
                        SetHeaderLabel(worksheet, 5, 16, "Use Limits Inner");
                        SetHeaderLabel(worksheet, 5, 17, "Use Limits Outer");
                        SetHeaderLabel(worksheet, 5, 18, "Nipple");
                        SetHeaderLabel(worksheet, 5, 19, "Tube Die");
                        SetHeaderLabel(worksheet, 5, 20, "Cover Die");
                        SetHeaderLabel(worksheet, 5, 21, "Mesh Screen 1");
                        SetHeaderLabel(worksheet, 5, 22, "Mesh Screen 2");
                        SetHeaderLabel(worksheet, 5, 23, "Head Temp 1");
                        SetHeaderLabel(worksheet, 5, 24, "Head Temp 2");
                        SetHeaderLabel(worksheet, 5, 25, "Cylinder 1-1");
                        SetHeaderLabel(worksheet, 5, 26, "Cylinder 1-2");
                        SetHeaderLabel(worksheet, 5, 27, "Cylinder 2-1");
                        SetHeaderLabel(worksheet, 5, 28, "Cylinder 2-2");
                        SetHeaderLabel(worksheet, 5, 29, "Feed 1");
                        SetHeaderLabel(worksheet, 5, 30, "Feed 2");
                        SetHeaderLabel(worksheet, 5, 31, "Screw Temp 1");
                        SetHeaderLabel(worksheet, 5, 32, "Screw Temp 2");
                        SetHeaderLabel(worksheet, 5, 33, "Screw Speed 1");
                        SetHeaderLabel(worksheet, 5, 34, "Screw Speed 2");
                        SetHeaderLabel(worksheet, 5, 35, "Pressure 1");
                        SetHeaderLabel(worksheet, 5, 36, "Pressure 2");
                        SetHeaderLabel(worksheet, 5, 37, "Am Meter");
                        SetHeaderLabel(worksheet, 5, 38, "OD Sensor");
                        SetHeaderLabel(worksheet, 5, 39, "Marking Sort");
                        SetHeaderLabel(worksheet, 5, 40, "Text Marking Material");
                        SetHeaderLabel(worksheet, 5, 41, "Marking Colour");
                        SetHeaderLabel(worksheet, 5, 42, "Chiller Water Temp");
                        SetHeaderLabel(worksheet, 5, 43, "Cutting Speed");
                        SetHeaderLabel(worksheet, 5, 44, "Take Up Conveyor Speed");
                        SetHeaderLabel(worksheet, 5, 45, "Tolerance Inner");
                        SetHeaderLabel(worksheet, 5, 46, "Tolerance Outer");
                        SetHeaderLabel(worksheet, 5, 47, "Tebal Inner");
                        SetHeaderLabel(worksheet, 5, 48, "Tebal Outer");
                        SetHeaderLabel(worksheet, 5, 49, "Tebal Total");
                        SetHeaderLabel(worksheet, 5, 50, "Selisih Tebal");
                        SetHeaderLabel(worksheet, 5, 51, "Item List");
                        SetHeaderLabel(worksheet, 5, 52, "MC (Machine Code)");
                    }
                    
                    // ROW 7: Example Data
                    worksheet.Cells[7, 1].Value = "1-0";
                    worksheet.Cells[7, 2].Value = "1";
                    worksheet.Cells[7, 3].Value = "Non-CHS (DL)";
                    worksheet.Cells[7, 4].Value = "SOP/PROD/HOSE/SPS/24/10/01";
                    worksheet.Cells[7, 7].Value = "0";
                    worksheet.Cells[7, 8].Value = "PT. TMMIN";
                    worksheet.Cells[7, 9].Value = "07-08-17";
                    worksheet.Cells[7, 10].Value = "Hose; Breather; Ø 35 mm x 7.5 mm; TSM1649G-1";
                    worksheet.Cells[7, 11].Value = "Hose, Breather";
                    worksheet.Cells[7, 12].Value = "Ø 35 mm x 7.5 mm";
                    worksheet.Cells[7, 13].Value = "TSM1649G-1";
                    worksheet.Cells[7, 14].Value = "TSM-1649 G";
                    
                    if (is3Layer)
                    {
                        worksheet.Cells[7, 15].Value = "TSM-1649 G-2";
                        worksheet.Cells[7, 16].Value = "TSM-1649 G";
                        worksheet.Cells[7, 23].Value = "11.8 x 11.6";
                        worksheet.Cells[7, 24].Value = "17.85 x 17.85";
                        worksheet.Cells[7, 25].Value = "18.5 x 17.85";
                        worksheet.Cells[7, 26].Value = "23.5 x 23.3";
                        worksheet.Cells[7, 108].Value = "VHFUNC12345,VHFUNC67890";
                    }
                    else if (is2Layer)
                    {
                        worksheet.Cells[7, 15].Value = "TSM-1649 G";
                        worksheet.Cells[7, 22].Value = "11.8 x 11.6";
                        worksheet.Cells[7, 23].Value = "17.85 x 17.85";
                        worksheet.Cells[7, 25].Value = "23.5 x 23.3";
                        worksheet.Cells[7, 90].Value = "VHFUNC12345,VHFUNC67890";
                    }
                    else // Double Layer
                    {
                        worksheet.Cells[7, 15].Value = "TSM-1649 G";
                        worksheet.Cells[7, 18].Value = "11.8 x 11.6";
                        worksheet.Cells[7, 19].Value = "17.85 x 17.85";
                        worksheet.Cells[7, 20].Value = "23.5 x 23.3";
                        worksheet.Cells[7, 51].Value = "VHFUNC12345,VHFUNC67890";
                        worksheet.Cells[7, 52].Value = "DL1";
                    }
                    
                    // Auto-fit columns (wrap in try-catch in case method signature differs)
                    try
                    {
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    }
                    catch
                    {
                        // If AutoFit fails, set a default width
                        worksheet.DefaultColWidth = 12;
                    }
                    
                    // Freeze panes (header rows)
                    worksheet.View.FreezePanes(7, 1);
                    
                    var fileBytes = package.GetAsByteArray();
                    string fileName;
                    if (is3Layer) {
                        fileName = "Template_SPS_3Layer.xlsx";
                    } else if (is2Layer) {
                        fileName = "Template_SPS_2Layer.xlsx";
                    } else {
                        fileName = "Template_SPS_DoubleLayer.xlsx";
                    }
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal membuat template");
                return StatusCode(500, "Gagal membuat template: " + ex.Message);
            }
        }
        
        private void SetHeaderLabel(OfficeOpenXml.ExcelWorksheet ws, int row, int col, string label)
        {
            ws.Cells[row, col].Value = label;
            ws.Cells[row, col].Style.Font.Bold = true;
            ws.Cells[row, col].Style.Font.Size = 9;
            ws.Cells[row, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ws.Cells[row, col].Style.Fill.BackgroundColor.SetColor(255, 68, 114, 196); // Blue header ARGB
            ws.Cells[row, col].Style.Font.Color.SetColor(255, 255, 255, 255); // White ARGB
            ws.Cells[row, col].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        }

        [HttpPost]
        public async Task<IActionResult> ClearData()
        {
            _context.MasterlistSpsDoubleLayers.RemoveRange(_context.MasterlistSpsDoubleLayers);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0) {
                TempData["ErrorMessage"] = "File tidak ditemukan atau kosong";
                return RedirectToAction(nameof(Index));
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet();
                        
                        // Gunakan helper method untuk pilih sheet terbaik
                        DataTable table = SelectBestSheet(result);

                        // Gunakan helper method untuk deteksi format dengan logging
                        bool isDig3L, isDig2L, isLegacy;
                        string detectionLog = DetectFormatWithLogging(table, out isDig3L, out isDig2L, out isLegacy);
                        
                        // Validasi struktur Excel
                        string validationError;
                        if (!ValidateExcelStructure(table, isDig3L, isDig2L, isLegacy, out validationError)) {
                            TempData["ErrorMessage"] = $"Validasi gagal: {validationError}\n\n{detectionLog}";
                            return RedirectToAction(nameof(Index));
                        }

                        // Cek apakah format terdeteksi
                        if (!isDig3L && !isDig2L && !isLegacy) {
                            TempData["ErrorMessage"] = $"Format SPS tidak terdeteksi!\n\n{detectionLog}";
                            return RedirectToAction(nameof(Index));
                        }

                        // 2. Set Indices
                        int idxItem, idxRev, idxCust, idxDate, idxForm, idxType, idxDim, idxMat, idxInner, idxOuter;
                        int idxDoc = 3;
                        int idxMC = isLegacy ? 51 : -1;

                        if (isDig3L) {
                            idxItem = 107;
                            idxRev = 6; idxCust = 7; idxDate = 8; idxForm = 9;
                            idxType = 10; idxDim = 11; idxMat = 12; idxInner = 13; idxOuter = 15;
                        } else if (isDig2L) {
                            idxItem = 89;
                            idxRev = 6; idxCust = 7; idxDate = 8; idxForm = 9;
                            idxType = 10; idxDim = 11; idxMat = 12; idxInner = 13; idxOuter = 14;
                        } else {
                            // Legacy / Non-CHS
                            idxItem = 50;
                            idxRev = 4; idxCust = 5; idxDate = 6; idxForm = 7;
                            idxType = 8; idxDim = 9; idxMat = 10; idxInner = 11; idxOuter = 12;
                        }

                        
                        // 2b. Dynamic Item Column Detection (Fallback)
                        // HANYA jalan jika format TIDAK terdeteksi (untuk safety)
                        // Jika format sudah terdeteksi, gunakan index yang sudah di-set
                        if ((!isDig3L && !isDig2L && !isLegacy) && table.Rows.Count > 5) {
                            _logger.LogWarning("Format tidak terdeteksi, mencoba dynamic item column detection...");
                            for (int c = 0; c < table.Rows[5].ItemArray.Length; c++) {
                                string val = GetV(table.Rows[5], c).ToLower();
                                if (val.Contains("item") || val == "107" || val == "89" || val == "50") {
                                    idxItem = c;
                                    _logger.LogInformation($"Dynamic detection: Item column set to {c}");
                                    break;
                                }
                            }
                        } else if (isDig3L || isDig2L || isLegacy) {
                            _logger.LogInformation($"Format terdeteksi, Item column: {idxItem}");
                        }

                        int startRow = 6;
                        var existingData = await _context.MasterlistSpsDoubleLayers.ToListAsync();
                        var existingMap = existingData
                            .Where(x => !string.IsNullOrWhiteSpace(x.ExcelId) && !string.IsNullOrWhiteSpace(x.ItemList))
                            .GroupBy(x => BuildImportKey(x.ExcelId, x.ItemList))
                            .ToDictionary(g => g.Key, g => g.First());

                        var processedInFile = new HashSet<string>(StringComparer.Ordinal);
                        int newCount = 0, updateCount = 0, skippedEmptyItemCount = 0, skippedDuplicateInFileCount = 0;

                        for (int i = startRow; i < table.Rows.Count; i++)
                        {
                            var row = table.Rows[i];
                            string idExcel = GetV(row, 0);
                            if (string.IsNullOrEmpty(idExcel) || idExcel.Length > 20 || idExcel == "1" || idExcel == "2") continue;

                            string rawItems = GetV(row, idxItem);
                            if (string.IsNullOrWhiteSpace(rawItems)) {
                                skippedEmptyItemCount++;
                                continue;
                            }

                            var itemsList = rawItems.Split(',')
                                .Select(s => s.Trim())
                                .Where(s => !string.IsNullOrEmpty(s))
                                .Select(s => (isDig3L && s.Contains("-")) ? s.Replace("-", "") : s)
                                .ToList();

                            foreach (var itemCode in itemsList)
                            {
                                var key = BuildImportKey(idExcel, itemCode);
                                if (processedInFile.Contains(key)) {
                                    skippedDuplicateInFileCount++;
                                    continue;
                                }
                                processedInFile.Add(key);

                                var target = existingMap.TryGetValue(key, out var existingTarget)
                                    ? existingTarget
                                    : new MasterlistSpsDoubleLayer();
                                bool isNew = (target.Id == 0);

                                target.ExcelId = idExcel;
                                target.No = GetV(row, 1);
                                target.Machine = GetV(row, 2);
                                target.ItemList = itemCode;
                                target.DocumentNumber = GetV(row, idxDoc);
                                target.RevisionNumber = GetV(row, idxRev);
                                target.Customer = GetV(row, idxCust);
                                target.RevisionDate = GetV(row, idxDate);
                                target.Formulasi = GetV(row, idxForm);
                                target.HoseType = GetV(row, idxType);
                                target.Dimensi = GetV(row, idxDim);
                                target.Material = GetV(row, idxMat);
                                target.InnerTube = GetV(row, idxInner);
                                target.OuterCover = GetV(row, idxOuter);
                                target.MiddleTube = isDig3L ? GetV(row, 14) : "";

                                // Use Limits Mapping
                                if (isDig3L) {
                                    target.UseLimitsInner = GetV(row, 16);
                                    target.UseLimitsMiddle = GetV(row, 17);
                                    target.UseLimitsOuter = GetV(row, 18);
                                } else if (isDig2L) {
                                    target.UseLimitsInner = GetV(row, 15);
                                    target.UseLimitsOuter = GetV(row, 16);
                                    target.PitchYarn = GetV(row, 18);
                                } else {
                                    target.UseLimitsInner = GetV(row, 13);
                                    target.UseLimitsOuter = GetV(row, 14);
                                }

                                if (idxMC != -1) target.MachineCode = GetV(row, idxMC);

                                // Dies & Mesh
                                if (isDig3L) {
                                    target.Yarn = GetV(row, 19);
                                    target.TensionYarnInner = GetV(row, 20);
                                    target.TensionYarnOuter = GetV(row, 21);
                                    target.Nipple = GetV(row, 22);
                                    target.TubeDie = GetV(row, 23);
                                    target.MiddleDie = GetV(row, 24);
                                    target.CoverDie = GetV(row, 25);
                                    target.SpacerDie = GetV(row, 26);
                                    target.ADistance = GetV(row, 27);
                                    target.MeshDim1 = GetV(row, 28);
                                    target.MeshScreen1 = GetV(row, 29);
                                    target.MeshDim2 = GetV(row, 30);
                                    target.MeshScreen2 = GetV(row, 31);
                                    target.MeshDim3 = GetV(row, 32);
                                    target.MeshScreen3 = GetV(row, 33);
                                } else if (isDig2L) {
                                    target.Yarn = GetV(row, 17);
                                    target.TensionYarnInner = GetV(row, 19);
                                    target.TensionYarnOuter = GetV(row, 20);
                                    target.Nipple = GetV(row, 21);
                                    target.TubeDie = GetV(row, 22);
                                    target.MiddleDie = GetV(row, 23);
                                    target.CoverDie = GetV(row, 24);
                                    target.SpacerDie = GetV(row, 25);
                                    target.ADistance = GetV(row, 26);
                                    target.MeshDim1 = GetV(row, 27);
                                    target.MeshScreen1 = GetV(row, 28);
                                    target.MeshDim2 = GetV(row, 29);
                                    target.MeshScreen2 = GetV(row, 30);
                                } else {
                                    target.Nipple = GetV(row, 15);
                                    target.TubeDie = GetV(row, 16);
                                    target.CoverDie = GetV(row, 17);
                                    target.MeshScreen1 = GetV(row, 18);
                                    target.MeshScreen2 = GetV(row, 19);
                                    target.ADistance = GetV(row, 27); // Added for Legacy if exists
                                }

                                // Parameters
                                if (isDig3L) {
                                    target.HeadTemp1 = GetV(row, 34); target.Cylinder1_1 = GetV(row, 35);
                                    target.Cylinder2_1 = GetV(row, 36); target.Cylinder3_1 = GetV(row, 37);
                                    target.ScrewTemp1 = GetV(row, 38);
                                    target.HeadTemp2 = GetV(row, 39); target.Cylinder1_2 = GetV(row, 40);
                                    target.Cylinder2_2 = GetV(row, 41); target.Cylinder3_2 = GetV(row, 42);
                                    target.ScrewTemp2 = GetV(row, 43);
                                    target.HeadTemp3 = GetV(row, 44); target.Cylinder1_3 = GetV(row, 45);
                                    target.Cylinder2_3 = GetV(row, 46); target.Cylinder3_3 = GetV(row, 47);
                                    target.ScrewTemp3 = GetV(row, 48);
                                    target.ScrewSpeed1 = GetV(row, 49); target.ScrewSpeed2 = GetV(row, 50);
                                    target.ScrewSpeed3 = GetV(row, 51);
                                    target.FeedRollRatio1 = GetV(row, 52);
                                    target.FeedRollRatio2 = GetV(row, 53);
                                    target.FeedRollRatio3 = GetV(row, 54);
                                    target.Pressure1 = GetV(row, 55); target.Pressure2 = GetV(row, 56);
                                    target.Pressure3 = GetV(row, 57);
                                    target.AmMeter = GetV(row, 58);
                                    target.AmMeter2 = GetV(row, 59);
                                    target.AmMeter3 = GetV(row, 60);
                                    target.PresetValue = GetV(row, 61);
                                    target.ControlValue = GetV(row, 62);
                                    target.SpiralPitchSetting = GetV(row, 63);
                                    target.SpiralPitchDisplay = GetV(row, 64);
                                    target.SpiralSpeed = GetV(row, 65);
                                    target.HoseSpeed = GetV(row, 66);
                                    target.UnsmoothSurface = GetV(row, 67);

                                    target.MarkingSort = GetV(row, 68);
                                    target.TextMarkingMaterial = GetV(row, 69);
                                    target.MarkingColour = GetV(row, 70);
                                    target.ChillerWaterTemp = GetV(row, 71);
                                    target.CaterpillarGap = GetV(row, 72);
                                    target.TakeUpConveyorSpeed = GetV(row, 73);
                                    target.CoolConveyorSpeed = GetV(row, 74);
                                    target.CoolConveyorSpeed2 = GetV(row, 75);
                                    target.ConveyorRatio = GetV(row, 76);
                                    
                                    target.ToleranceInner = GetV(row, 77);
                                    target.ToleranceOuter = GetV(row, 78);
                                    target.TebalInner = GetV(row, 79);
                                    target.TebalInnerMiddle = GetV(row, 80);
                                    target.TebalTotal = GetV(row, 81);
                                    target.SelisihTebal = GetV(row, 82);

                                    // Final Quality Matrix (Inner) - Col 90-95
                                    target.InnerTarget = GetV(row, 89);
                                    target.InnerTol = GetV(row, 90);
                                    target.InnerLCL = GetV(row, 91);
                                    target.InnerMin = GetV(row, 92);
                                    target.InnerUCL = GetV(row, 93);
                                    target.InnerMax = GetV(row, 94);

                                    // Final Quality Matrix (Inner + Middle) - Col 96-101
                                    target.InnerMidTarget = GetV(row, 95);
                                    target.InnerMidTol = GetV(row, 96);
                                    target.InnerMidLCL = GetV(row, 97);
                                    target.InnerMidMin = GetV(row, 98);
                                    target.InnerMidUCL = GetV(row, 99);
                                    target.InnerMidMax = GetV(row, 100);

                                    // Final Quality Matrix (Total Thick) - Col 102-107
                                    target.TotalTarget = GetV(row, 101);
                                    target.TotalTol = GetV(row, 102);
                                    target.TotalLCL = GetV(row, 103);
                                    target.TotalMin = GetV(row, 104);
                                    target.TotalUCL = GetV(row, 105);
                                    target.TotalMax = GetV(row, 106);
                                } else if (isDig2L) {
                                    target.HeadTemp1 = GetV(row, 31); target.Cylinder1_1 = GetV(row, 32);
                                    target.Cylinder2_1 = GetV(row, 33); target.Cylinder3_1 = GetV(row, 34);
                                    target.ScrewTemp1 = GetV(row, 35);
                                    target.HeadTemp2 = GetV(row, 36); target.Cylinder1_2 = GetV(row, 37);
                                    target.Cylinder2_2 = GetV(row, 38); target.Cylinder3_2 = GetV(row, 39);
                                    target.ScrewTemp2 = GetV(row, 40);
                                    target.ScrewSpeed1 = GetV(row, 41); target.ScrewSpeed2 = GetV(row, 42);
                                    target.FeedRollRatio1 = GetV(row, 43);
                                    target.FeedRollRatio2 = GetV(row, 44);
                                    target.Pressure1 = GetV(row, 45); target.Pressure2 = GetV(row, 46);
                                    target.CurrentValue = GetV(row, 47);
                                    target.AmMeter = GetV(row, 48);
                                    target.AmMeter2 = GetV(row, 49);
                                    target.PresetValue = GetV(row, 50);
                                    target.ControlValue = GetV(row, 51);
                                    target.SpiralPitchSetting = GetV(row, 52);
                                    target.SpiralPitchDisplay = GetV(row, 53);
                                    target.SpiralSpeed = GetV(row, 54);
                                    target.HoseSpeed = GetV(row, 55);
                                    target.UnsmoothSurface = GetV(row, 56);

                                    target.MarkingSort = GetV(row, 57);
                                    target.TextMarkingMaterial = GetV(row, 58);
                                    target.MarkingColour = GetV(row, 59);
                                    target.ChillerWaterTemp = GetV(row, 60);
                                    target.DancerPosition = GetV(row, 61);
                                    target.CaterpillarGap = GetV(row, 62);
                                    target.TakeUpConveyorSpeed = GetV(row, 63);
                                    target.CoolConveyorSpeed = GetV(row, 64);
                                    target.ConveyorRatio = GetV(row, 65);

                                    target.ToleranceInner = GetV(row, 66);
                                    target.ToleranceOuter = GetV(row, 67);
                                    target.TebalInner = GetV(row, 68); // Col 69
                                    target.TebalTotal = GetV(row, 69); // Col 70
                                    target.SelisihTebal = GetV(row, 70); // Col 71
                                    target.TebalOuter = null; // Clear if not explicitly in 2L

                                    // Quality Matrix CHS 2L
                                    // Map Col 78-83 (Inner Thickness) to Inner Matrix for UI consistency
                                    target.InnerTarget = GetV(row, 77);
                                    target.InnerTol = GetV(row, 78);
                                    target.InnerLCL = GetV(row, 79);
                                    target.InnerMin = GetV(row, 80);
                                    target.InnerUCL = GetV(row, 81);
                                    target.InnerMax = GetV(row, 82);
                                    
                                    // Move previous matrix (Col 72-77) to Thick fields if needed, or clear
                                    target.ThickTarget = GetV(row, 71);
                                    target.ThickTol = GetV(row, 72);
                                    target.ThickLCL = GetV(row, 73);
                                    target.ThickMin = GetV(row, 74);
                                    target.ThickUCL = GetV(row, 75);
                                    target.ThickMax = GetV(row, 76);

                                    // Total Matrix for 2L - Col 84-89
                                    target.TotalTarget = GetV(row, 83);
                                    target.TotalTol = GetV(row, 84);
                                    target.TotalLCL = GetV(row, 85);
                                    target.TotalMin = GetV(row, 86);
                                    target.TotalUCL = GetV(row, 87);
                                    target.TotalMax = GetV(row, 88);
                                } else {
                                    target.HeadTemp1 = GetV(row, 20); target.HeadTemp2 = GetV(row, 21);
                                    target.Cylinder1_1 = GetV(row, 22); target.Cylinder1_2 = GetV(row, 23);
                                    target.Cylinder2_1 = GetV(row, 24); target.Cylinder2_2 = GetV(row, 25);
                                    target.Feed1 = GetV(row, 26); target.Feed2 = GetV(row, 27);
                                    target.ScrewTemp1 = GetV(row, 28); target.ScrewTemp2 = GetV(row, 29);
                                    target.ScrewSpeed1 = GetV(row, 30); target.ScrewSpeed2 = GetV(row, 31);
                                    target.Pressure1 = GetV(row, 32); target.Pressure2 = GetV(row, 33);
                                    target.AmMeter = GetV(row, 34);
                                    target.OdSensor = GetV(row, 35);

                                    target.MarkingSort = GetV(row, 36);
                                    target.TextMarkingMaterial = GetV(row, 37);
                                    target.MarkingColour = GetV(row, 38);
                                    target.ChillerWaterTemp = GetV(row, 39);
                                    target.CuttingSpeed = GetV(row, 40);
                                    target.TakeUpConveyorSpeed = GetV(row, 41);
                                    target.ToleranceInner = GetV(row, 42);
                                    target.ToleranceOuter = GetV(row, 43);
                                    target.TebalInner = GetV(row, 44);
                                    target.TebalOuter = GetV(row, 45);
                                    target.TebalTotal = GetV(row, 46);
                                    target.SelisihTebal = GetV(row, 47);
                                }

                                if (isNew) { _context.MasterlistSpsDoubleLayers.Add(target); newCount++; }
                                else { _context.MasterlistSpsDoubleLayers.Update(target); updateCount++; }

                                if (isNew) {
                                    existingMap[key] = target;
                                }
                            }
                        }
                        await _context.SaveChangesAsync();
                        
                        string detectedFormat = isDig3L ? "CHS 3 Layer" : (isDig2L ? "CHS 2 Layer" : "Legacy/Non-CHS");
                        string sheetLog = TempData["SheetLog"]?.ToString() ?? "";
                        
                        // Validasi: hanya tampilkan success jika ada data yang masuk
                        if (newCount == 0 && updateCount == 0) {
                            TempData["ErrorMessage"] = $"⚠ Tidak ada data yang diimport!\n" +
                                                      $"Format: {detectedFormat}\n" +
                                                      $"Sheet: {table.TableName}\n" +
                                                      $"Kolom Item: {idxItem}\n" +
                                                      $"Total rows di Excel: {table.Rows.Count}\n" +
                                                      $"Start row: {startRow}\n\n" +
                                                      $"Kemungkinan penyebab:\n" +
                                                      $"- ID Excel kosong atau tidak valid\n" +
                                                      $"- Item List kosong di kolom {idxItem}\n" +
                                                      $"- Data dimulai bukan dari row 6\n" +
                                                      $"- Kolom Item salah (pastikan kolom {idxItem} berisi item codes)";
                            _logger.LogWarning($"Import warning: Format={detectedFormat}, 0 records imported");
                        } else {
                            TempData["SuccessMessage"] = $"✓ Import Berhasil!\n" +
                                                        $"Format: {detectedFormat}\n" +
                                                        $"Sheet: {table.TableName}\n" +
                                                        $"Kolom Item: {idxItem}\n" +
                                                        $"Data baru: {newCount}\n" +
                                                        $"Data diperbarui: {updateCount}\n" +
                                                        $"Baris ITEM kosong di-skip: {skippedEmptyItemCount}\n" +
                                                        $"Duplikat di file di-skip: {skippedDuplicateInFileCount}\n" +
                                                        $"Total: {newCount + updateCount} records";
                            
                            _logger.LogInformation($"Import sukses: {detectedFormat}, {newCount} new, {updateCount} updated");
                        }
                    }
                }
            }
            catch (Exception ex) { 
                _logger.LogError(ex, "Error during ImportExcel");
                TempData["ErrorMessage"] = $"Error: {ex.Message}\n\nDetail: {ex.InnerException?.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcelTrial(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                TempData["ErrorMessage"] = "File uji coba tidak ditemukan.";
                return RedirectToAction(nameof(Index));
            }

            var validFiles = files.Where(f => f != null && f.Length > 0).ToList();
            if (validFiles.Count == 0)
            {
                TempData["ErrorMessage"] = "Semua file uji coba kosong.";
                return RedirectToAction(nameof(Index));
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            int insertedCount = 0;
            int updatedCount = 0;
            int skippedEmptyItemCount = 0;
            int skippedDuplicateInBatchCount = 0;
            int skippedInvalidIdCount = 0;
            var batchId = DateTime.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            var processedInBatch = new HashSet<string>(StringComparer.Ordinal);

            var existingTrialRows = await _context.SpsImportTrialRows.ToListAsync();
            var trialMap = existingTrialRows
                .Where(x => !string.IsNullOrWhiteSpace(x.ExcelId) && !string.IsNullOrWhiteSpace(x.ItemCode))
                .GroupBy(x => BuildImportKey(x.ExcelId, x.ItemCode))
                .ToDictionary(g => g.Key, g => g.First());

            var existingProductionKeys = (await _context.MasterlistSpsDoubleLayers
                    .Where(x => !string.IsNullOrWhiteSpace(x.ExcelId) && !string.IsNullOrWhiteSpace(x.ItemList))
                    .Select(x => new { x.ExcelId, x.ItemList })
                    .ToListAsync())
                .Select(x => BuildImportKey(x.ExcelId, x.ItemList))
                .ToHashSet(StringComparer.Ordinal);

            foreach (var file in validFiles)
            {
                try
                {
                    using var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using var reader = ExcelReaderFactory.CreateReader(stream);
                    var result = reader.AsDataSet();
                    var table = SelectBestSheet(result);

                    bool isDig3L, isDig2L, isLegacy;
                    string detectionLog = DetectFormatWithLogging(table, out isDig3L, out isDig2L, out isLegacy);

                    string validationError;
                    if (!ValidateExcelStructure(table, isDig3L, isDig2L, isLegacy, out validationError))
                    {
                        _logger.LogWarning("Trial import skipped file {FileName}. Validation failed: {ValidationError}. {DetectionLog}", file.FileName, validationError, detectionLog);
                        continue;
                    }

                    if (!isDig3L && !isDig2L && !isLegacy)
                    {
                        _logger.LogWarning("Trial import skipped file {FileName}. Format not detected. {DetectionLog}", file.FileName, detectionLog);
                        continue;
                    }

                    int idxItem, idxCust, idxDim, idxType;
                    int idxDoc = 3;

                    if (isDig3L)
                    {
                        idxItem = 107;
                        idxCust = 7;
                        idxType = 10;
                        idxDim = 11;
                    }
                    else if (isDig2L)
                    {
                        idxItem = 89;
                        idxCust = 7;
                        idxType = 10;
                        idxDim = 11;
                    }
                    else
                    {
                        idxItem = 50;
                        idxCust = 5;
                        idxType = 8;
                        idxDim = 9;
                    }

                    int startRow = 6;
                    string detectedFormat = isDig3L ? "CHS 3 Layer" : (isDig2L ? "CHS 2 Layer" : "Legacy/Non-CHS");

                    for (int i = startRow; i < table.Rows.Count; i++)
                    {
                        var row = table.Rows[i];
                        string idExcel = GetV(row, 0);

                        if (string.IsNullOrWhiteSpace(idExcel) || idExcel.Length > 20 || idExcel == "1" || idExcel == "2")
                        {
                            skippedInvalidIdCount++;
                            continue;
                        }

                        string rawItems = GetV(row, idxItem);
                        if (string.IsNullOrWhiteSpace(rawItems))
                        {
                            skippedEmptyItemCount++;
                            continue;
                        }

                        var itemsList = rawItems.Split(',')
                            .Select(s => s.Trim())
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .Select(s => (isDig3L && s.Contains("-")) ? s.Replace("-", "") : s)
                            .ToList();

                        foreach (var itemCode in itemsList)
                        {
                            var key = BuildImportKey(idExcel, itemCode);
                            if (processedInBatch.Contains(key))
                            {
                                skippedDuplicateInBatchCount++;
                                continue;
                            }

                            processedInBatch.Add(key);

                            var target = trialMap.TryGetValue(key, out var existingTrial)
                                ? existingTrial
                                : new SpsImportTrialRow();

                            bool isNew = target.Id == 0;
                            target.BatchId = batchId;
                            target.SourceFileName = file.FileName;
                            target.SourceSheet = table.TableName;
                            target.DetectedFormat = detectedFormat;
                            target.ExcelId = idExcel.Trim();
                            target.ItemCode = itemCode.Trim();
                            target.Machine = GetV(row, 2);
                            target.DocumentNumber = GetV(row, idxDoc);
                            target.Customer = GetV(row, idxCust);
                            target.HoseType = GetV(row, idxType);
                            target.Dimensi = GetV(row, idxDim);
                            target.SourceRowIndex = i + 1;
                            target.ExistsInProduction = existingProductionKeys.Contains(key);
                            target.ImportedAt = DateTime.UtcNow;
                            target.ImportedBy = User?.Identity?.Name ?? "system";

                            if (isNew)
                            {
                                _context.SpsImportTrialRows.Add(target);
                                insertedCount++;
                                trialMap[key] = target;
                            }
                            else
                            {
                                _context.SpsImportTrialRows.Update(target);
                                updatedCount++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in trial import for file {FileName}", file.FileName);
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"Trial import selesai. Batch: {batchId}\n" +
                $"Jumlah file diproses: {validFiles.Count}\n" +
                $"Insert baru: {insertedCount}\n" +
                $"Update existing trial: {updatedCount}\n" +
                $"Skip ITEM kosong: {skippedEmptyItemCount}\n" +
                $"Skip ID tidak valid: {skippedInvalidIdCount}\n" +
                $"Skip duplikat antar-file: {skippedDuplicateInBatchCount}";

            return RedirectToAction(nameof(Index));
        }

        private string GetV(DataRow row, int colIndex)
        {
            try {
                if (colIndex < 0 || colIndex >= row.Table.Columns.Count) return "";
                return row[colIndex]?.ToString()?.Trim() ?? "";
            } catch { return ""; }
        }

        private static string BuildImportKey(string? excelId, string? itemCode)
        {
            return $"{NormalizeImportToken(excelId)}|{NormalizeImportToken(itemCode)}";
        }

        private static string NormalizeImportToken(string? value)
        {
            return (value ?? string.Empty).Trim().ToUpperInvariant();
        }

        /// <summary>
        /// Pilih sheet terbaik dari Excel berdasarkan nama atau ukuran
        /// </summary>
        private DataTable SelectBestSheet(DataSet result)
        {
            var log = new StringBuilder();
            log.AppendLine("=== SHEET DETECTION ===");
            
            // Priority 1: Sheet dengan nama yang mengandung keyword
            string[] keywords = { "Parameter Setting", "Master 1", "SPS", "Parameter", "Master" };
            
            foreach (DataTable table in result.Tables) {
                log.AppendLine($"Found: '{table.TableName}' ({table.Rows.Count} rows, {table.Columns.Count} cols)");
                
                foreach (var keyword in keywords) {
                    if (table.TableName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) {
                        log.AppendLine($"✓ Selected: {table.TableName} (matched: {keyword})");
                        _logger.LogInformation(log.ToString());
                        TempData["SheetLog"] = log.ToString();
                        return table;
                    }
                }
            }
            
            // Priority 2: Sheet dengan row terbanyak
            var largestSheet = result.Tables.Cast<DataTable>()
                .OrderByDescending(t => t.Rows.Count)
                .FirstOrDefault();
            
            if (largestSheet != null && largestSheet.Rows.Count > 5) {
                log.AppendLine($"✓ Selected largest: {largestSheet.TableName} ({largestSheet.Rows.Count} rows)");
                _logger.LogInformation(log.ToString());
                TempData["SheetLog"] = log.ToString();
                return largestSheet;
            }
            
            // Fallback: Sheet pertama
            var firstSheet = result.Tables[0];
            log.AppendLine($"⚠ Using first sheet: {firstSheet.TableName}");
            _logger.LogWarning(log.ToString());
            TempData["SheetLog"] = log.ToString();
            return firstSheet;
        }

        /// <summary>
        /// Deteksi format SPS dengan logging detail
        /// </summary>
        private string DetectFormatWithLogging(DataTable table, out bool isDig3L, out bool isDig2L, out bool isLegacy)
        {
            isDig3L = isDig2L = isLegacy = false;
            var log = new StringBuilder();
            
            log.AppendLine("=== FORMAT DETECTION ===");
            log.AppendLine($"Sheet: {table.TableName}");
            log.AppendLine($"Rows: {table.Rows.Count}, Columns: {table.Columns.Count}");
            
            // Method 1: Check title in Row 1
            if (table.Rows.Count > 0) {
                string title = GetV(table.Rows[0], 1).ToUpper();
                log.AppendLine($"Title (Row 1, Col B): '{title}'");
                
                if (title.Contains("CHS 3 LAYER") || title.Contains("3 LAYER")) {
                    isDig3L = true;
                    log.AppendLine("✓ PRIMARY: CHS 3 LAYER");
                }
                else if (title.Contains("CHS 2 LAYER") || title.Contains("2 LAYER")) {
                    isDig2L = true;
                    log.AppendLine("✓ PRIMARY: CHS 2 LAYER");
                }
                else if (title.Contains("NON-CHS") || title.Contains("NON CHS")) {
                    isLegacy = true;
                    log.AppendLine("✓ PRIMARY: LEGACY/NON-CHS");
                }
                else {
                    log.AppendLine("⚠ No format found in title");
                }
            }
            
            // Method 2: Fallback - check Item column position
            if (!isDig3L && !isDig2L && !isLegacy && table.Rows.Count > 5) {
                log.AppendLine("Trying fallback detection...");
                
                // Check multiple rows for "Item" indicator
                for (int checkRow = 2; checkRow <= Math.Min(6, table.Rows.Count - 1); checkRow++) {
                    string col107 = GetV(table.Rows[checkRow], 107);
                    string col89 = GetV(table.Rows[checkRow], 89);
                    string col50 = GetV(table.Rows[checkRow], 50);
                    
                    if (!string.IsNullOrEmpty(col107) && (col107.Contains("Item") || col107.Contains("107"))) {
                        isDig3L = true;
                        log.AppendLine($"✓ FALLBACK: CHS 3 LAYER (Row {checkRow}, Col 107: '{col107}')");
                        break;
                    }
                    
                    // New check: if Mesh 3 exists
                    string mesh3Dim = GetV(table.Rows[checkRow], 32);
                    string mesh3Scr = GetV(table.Rows[checkRow], 33);
                    if (!string.IsNullOrEmpty(mesh3Dim) || !string.IsNullOrEmpty(mesh3Scr)) {
                        isDig3L = true;
                        log.AppendLine($"✓ FALLBACK: CHS 3 LAYER (Found Mesh 3 at Row {checkRow})");
                        break;
                    }
                    else if (!string.IsNullOrEmpty(col89) && (col89.Contains("Item") || col89.Contains("89") || col89.Contains("90"))) {
                        isDig2L = true;
                        log.AppendLine($"✓ FALLBACK: CHS 2 LAYER (Row {checkRow}, Col 89: '{col89}')");
                        break;
                    }
                    else if (!string.IsNullOrEmpty(col50) && (col50.Contains("Item") || col50.Contains("ITEM") || col50.Contains("50") || col50.Contains("51"))) {
                        isLegacy = true;
                        log.AppendLine($"✓ FALLBACK: LEGACY (Row {checkRow}, Col 50: '{col50}')");
                        break;
                    }
                }
            }
            
            if (!isDig3L && !isDig2L && !isLegacy) {
                log.AppendLine("❌ FORMAT TIDAK TERDETEKSI!");
                log.AppendLine("Pastikan:");
                log.AppendLine("- Row 1 memiliki title 'CHS 3 LAYER' / 'CHS 2 LAYER' / 'NON-CHS'");
                log.AppendLine("- Atau kolom Item ada di posisi 107/89/50");
            }
            
            string logResult = log.ToString();
            _logger.LogInformation(logResult);
            return logResult;
        }

        /// <summary>
        /// Validasi struktur kolom Excel
        /// </summary>
        private bool ValidateExcelStructure(DataTable table, bool isDig3L, bool isDig2L, bool isLegacy, out string errorMsg)
        {
            errorMsg = "";
            int expectedItemCol = isDig3L ? 107 : (isDig2L ? 89 : 50);
            int minColumns = expectedItemCol + 1;
            
            // Check 1: Jumlah kolom cukup
            if (table.Columns.Count < minColumns) {
                errorMsg = $"❌ Kolom tidak cukup! Expected: {minColumns}, Found: {table.Columns.Count}";
                _logger.LogError(errorMsg);
                return false;
            }
            
            // Check 2: Ada cukup baris data
            if (table.Rows.Count < 7) {
                errorMsg = $"❌ Data terlalu sedikit! Expected: min 7 rows, Found: {table.Rows.Count}";
                _logger.LogError(errorMsg);
                return false;
            }
            
            // Check 3: Kolom kunci tidak kosong
            int testRow = 6; // First data row
            if (testRow < table.Rows.Count) {
                string idExcel = GetV(table.Rows[testRow], 0);
                string machine = GetV(table.Rows[testRow], 2);
                string itemList = GetV(table.Rows[testRow], expectedItemCol);
                
                if (string.IsNullOrEmpty(idExcel) && string.IsNullOrEmpty(itemList)) {
                    errorMsg = $"⚠ Warning: Row {testRow + 1} memiliki ID Excel dan Item List kosong";
                    _logger.LogWarning(errorMsg);
                    // Not a hard fail, just warning
                }
            }
            
            _logger.LogInformation($"✓ Validasi passed: {table.Columns.Count} columns, {table.Rows.Count} rows");
            return true;
        }

        /// <summary>
        /// Preview Excel untuk debugging - API endpoint
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PreviewExcel(IFormFile file)
        {
            if (file == null || file.Length == 0) 
                return Json(new { success = false, message = "File tidak ditemukan" });

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet();
                        
                        // List all sheets
                        var sheets = result.Tables.Cast<DataTable>()
                            .Select(t => new {
                                name = t.TableName,
                                rows = t.Rows.Count,
                                columns = t.Columns.Count
                            }).ToList();
                        
                        // Select best sheet
                        var table = SelectBestSheet(result);
                        
                        // Detect format
                        bool isDig3L, isDig2L, isLegacy;
                        string detectionLog = DetectFormatWithLogging(table, out isDig3L, out isDig2L, out isLegacy);
                        
                        string format = isDig3L ? "CHS 3 LAYER" : 
                                       (isDig2L ? "CHS 2 LAYER" : 
                                       (isLegacy ? "LEGACY" : "UNKNOWN"));
                        
                        // Validate structure
                        string errorMsg;
                        bool isValid = ValidateExcelStructure(table, isDig3L, isDig2L, isLegacy, out errorMsg);
                        
                        int expectedItemCol = isDig3L ? 107 : (isDig2L ? 89 : 50);
                        
                        // Get sample data
                        var sampleData = new List<object>();
                        int startRow = 6;
                        for (int i = startRow; i < Math.Min(startRow + 5, table.Rows.Count); i++) {
                            var row = table.Rows[i];
                            string idExcel = GetV(row, 0);
                            if (string.IsNullOrEmpty(idExcel)) continue;
                            
                            sampleData.Add(new {
                                rowIndex = i + 1,
                                idExcel = idExcel,
                                no = GetV(row, 1),
                                machine = GetV(row, 2),
                                customer = GetV(row, isDig3L || isDig2L ? 6 : 4),
                                itemList = GetV(row, expectedItemCol),
                                dimensi = GetV(row, isDig3L ? 11 : (isDig2L ? 9 : 8))
                            });
                        }
                        
                        return Json(new {
                            success = true,
                            fileName = file.FileName,
                            fileSize = file.Length,
                            sheets = sheets,
                            selectedSheet = table.TableName,
                            detectedFormat = format,
                            isValid = isValid,
                            validationError = errorMsg,
                            detectionLog = detectionLog,
                            expectedItemColumn = expectedItemCol,
                            totalRows = table.Rows.Count,
                            totalColumns = table.Columns.Count,
                            sampleData = sampleData,
                            estimatedRecords = sampleData.Count > 0 ? $"~{table.Rows.Count - 6} records" : "0 records"
                        });
                    }
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error in PreviewExcel");
                return Json(new { success = false, message = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}
