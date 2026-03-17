using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExcelDataReader;
using System.Data;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;
using System.Text.RegularExpressions;

namespace VelastoProductionSystem.Controllers
{
    public class MasterlistSpsDoubleLayersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MasterlistSpsDoubleLayersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.MasterlistSpsDoubleLayers.ToListAsync());
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
            if (file == null || file.Length == 0) return RedirectToAction(nameof(Index));

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
                        
                        // 1. Pilih Sheet Berbasis Konten (Cari yang ada VHFUNC)
                        DataTable dataTable = result.Tables[0];
                        foreach(DataTable table in result.Tables) {
                            bool hasLongItems = false;
                            for (int r = 0; r < Math.Min(20, table.Rows.Count); r++) {
                                for (int c = 0; c < table.Columns.Count; c++) {
                                    if (table.Rows[r][c]?.ToString()?.Contains("VHFUNC") == true) {
                                        hasLongItems = true; break;
                                    }
                                }
                                if (hasLongItems) break;
                            }
                            if (hasLongItems) { dataTable = table; break; }
                        }

                        _context.MasterlistSpsDoubleLayers.RemoveRange(_context.MasterlistSpsDoubleLayers);
                        await _context.SaveChangesAsync();

                        int totalCols = dataTable.Columns.Count;
                        string GetV(DataRow r, int idx) => (idx >= 0 && idx < totalCols) ? r[idx]?.ToString()?.Trim() ?? "" : "";

                        // 2. Deteksi Kolom ITEM
                        int targetItemCol = 50; 
                        for (int r = 0; r < Math.Min(10, dataTable.Rows.Count); r++) {
                            for (int c = 0; c < totalCols; c++) {
                                string val = dataTable.Rows[r][c]?.ToString()?.ToUpper() ?? "";
                                if (val == "51" || val == "ITEM") { targetItemCol = c; break; }
                            }
                        }

                        int insertedCount = 0;
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            var row = dataTable.Rows[i];
                            string idExcel = GetV(row, 0);

                            if (string.IsNullOrEmpty(idExcel) || 
                                idExcel.Equals("ID", StringComparison.OrdinalIgnoreCase) || 
                                idExcel.Equals("ID EXCEL", StringComparison.OrdinalIgnoreCase) ||
                                !idExcel.Contains("-") || // Data rows should have ID like 1-0, 2-0
                                GetV(row, 5).Contains("Customer", StringComparison.OrdinalIgnoreCase) ||
                                idExcel.Length > 20) continue;

                            string rawItemList = GetV(row, targetItemCol);
                            string machineCode = GetV(row, targetItemCol + 1);

                            // 3. LOGIKA SPLITTING: Pecah berdasarkan koma
                            var items = rawItemList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                   .Select(s => s.Trim())
                                                   .ToList();

                            // Jika kosong, minimal kita buat 1 baris
                            if (items.Count == 0) items.Add("");

                            foreach (var item in items)
                            {
                                var newItem = new MasterlistSpsDoubleLayer
                                {
                                    ExcelId = idExcel,
                                    No = GetV(row, 1),
                                    Machine = GetV(row, 2),
                                    DocumentNumber = GetV(row, 3),
                                    RevisionNumber = GetV(row, 4),
                                    Customer = GetV(row, 5),
                                    RevisionDate = GetV(row, 6),
                                    Formulasi = GetV(row, 7),
                                    HoseType = GetV(row, 8),
                                    Dimensi = GetV(row, 9),
                                    Material = GetV(row, 10),
                                    InnerTube = GetV(row, 11),
                                    OuterCover = GetV(row, 12),
                                    Yarn = "", // Not found in Excel file
                                    UseLimitsInner = GetV(row, 13),
                                    UseLimitsOuter = GetV(row, 14),
                                    Nipple = GetV(row, 15),
                                    TubeDie = GetV(row, 16),
                                    CoverDie = GetV(row, 17),
                                    MeshScreen1 = GetV(row, 18),
                                    MeshScreen2 = GetV(row, 19),
                                    HeadTemp1 = GetV(row, 20),
                                    HeadTemp2 = GetV(row, 21),
                                    Cylinder1_1 = GetV(row, 22),
                                    Cylinder1_2 = GetV(row, 23),
                                    Cylinder2_1 = GetV(row, 24),
                                    Cylinder2_2 = GetV(row, 25),
                                    Feed1 = GetV(row, 26),
                                    Feed2 = GetV(row, 27),
                                    ScrewTemp1 = GetV(row, 28),
                                    ScrewTemp2 = GetV(row, 29),
                                    ScrewSpeed1 = GetV(row, 30),
                                    ScrewSpeed2 = GetV(row, 31),
                                    Pressure1 = GetV(row, 32),
                                    Pressure2 = GetV(row, 33),
                                    AmMeter = GetV(row, 34),
                                    OdSensor = GetV(row, 35),
                                    MarkingSort = GetV(row, 36),
                                    TextMarkingMaterial = GetV(row, 37),
                                    MarkingColour = GetV(row, 38),
                                    ChillerWaterTemp = GetV(row, 39),
                                    CuttingSpeed = GetV(row, 40),
                                    TakeUpConveyorSpeed = GetV(row, 41),
                                    ToleranceInner = GetV(row, 42),
                                    ToleranceOuter = GetV(row, 43),
                                    TebalInner = GetV(row, 44),
                                    TebalOuter = GetV(row, 45),
                                    TebalTotal = GetV(row, 46),
                                    SelisihTebal = GetV(row, 47),
                                    ItemList = item, // Simpan satu per satu
                                    MachineCode = machineCode
                                };

                                _context.MasterlistSpsDoubleLayers.Add(newItem);
                                insertedCount++;
                            }
                        }
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = $"Berhasil! Data telah dipecah menjadi {insertedCount} baris barcode.";
                    }
                }
            }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
            return RedirectToAction(nameof(Index));
        }
    }
}
