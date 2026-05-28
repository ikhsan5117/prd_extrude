using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using OfficeOpenXml;

namespace VelastoProductionSystem.Controllers
{
    public class SpsItemListController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SpsItemListController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SpsItemList
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.SpsItemLists.Include(s => s.SpsNoDoc).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchUpper = search.ToUpper();
                query = query.Where(s => 
                    (s.ItemList != null && s.ItemList.ToUpper().Contains(searchUpper)) ||
                    (s.DocumentNumber != null && s.DocumentNumber.ToUpper().Contains(searchUpper))
                );
            }

            ViewBag.Search = search;
            return View(await query.ToListAsync());
        }

        // GET: SpsItemList/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spsItemList = await _context.SpsItemLists
                .Include(s => s.SpsNoDoc)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (spsItemList == null)
            {
                return NotFound();
            }

            return View(spsItemList);
        }

        // GET: SpsItemList/Create
        public IActionResult Create()
        {
            ViewData["DocumentNumber"] = new SelectList(_context.SpsNoDocs, "DocumentNumber", "DocumentNumber");
            return View();
        }

        // POST: SpsItemList/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ItemList,DocumentNumber")] SpsItemList spsItemList)
        {
            if (ModelState.IsValid)
            {
                _context.Add(spsItemList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DocumentNumber"] = new SelectList(_context.SpsNoDocs, "DocumentNumber", "DocumentNumber", spsItemList.DocumentNumber);
            return View(spsItemList);
        }

        // GET: SpsItemList/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spsItemList = await _context.SpsItemLists.FindAsync(id);
            if (spsItemList == null)
            {
                return NotFound();
            }
            ViewData["DocumentNumber"] = new SelectList(_context.SpsNoDocs, "DocumentNumber", "DocumentNumber", spsItemList.DocumentNumber);
            return View(spsItemList);
        }

        // POST: SpsItemList/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ItemList,DocumentNumber")] SpsItemList spsItemList)
        {
            if (id != spsItemList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(spsItemList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpsItemListExists(spsItemList.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DocumentNumber"] = new SelectList(_context.SpsNoDocs, "DocumentNumber", "DocumentNumber", spsItemList.DocumentNumber);
            return View(spsItemList);
        }

        // GET: SpsItemList/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spsItemList = await _context.SpsItemLists
                .Include(s => s.SpsNoDoc)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (spsItemList == null)
            {
                return NotFound();
            }

            return View(spsItemList);
        }

        // POST: SpsItemList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var spsItemList = await _context.SpsItemLists.FindAsync(id);
            if (spsItemList != null)
            {
                _context.SpsItemLists.Remove(spsItemList);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return Json(new { success = false, message = "Tidak ada Item List yang dipilih" });
                }

                var recordsToDelete = await _context.SpsItemLists
                    .Where(s => ids.Contains(s.Id))
                    .ToListAsync();

                if (!recordsToDelete.Any())
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _context.SpsItemLists.RemoveRange(recordsToDelete);
                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = $"Berhasil menghapus {recordsToDelete.Count} Item List",
                    deletedCount = recordsToDelete.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = $"Terjadi kesalahan: {ex.Message}" 
                });
            }
        }

        [HttpGet]
        public IActionResult DownloadTemplateItemList()
        {
            try
            {
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Template ItemList");
                
                worksheet.Cells[1, 1].Value = "ITEM LIST";
                worksheet.Cells[1, 2].Value = "NO.DOC";
                
                using (var range = worksheet.Cells[1, 1, 1, 2])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }
                
                worksheet.Cells[2, 1].Value = "NA5030";
                worksheet.Cells[2, 2].Value = "SOP/PROD/HOSE/24/10/038";
                
                worksheet.Cells[4, 1].Value = "Contoh Pengisian:";
                worksheet.Cells[4, 1].Style.Font.Italic = true;
                worksheet.Cells[4, 1].Style.Font.Color.SetColor(System.Drawing.Color.Gray);
                worksheet.Cells[5, 1].Value = "Row 2: HN3040 | SOP/PROD/HOSE/24/10/030";
                worksheet.Cells[5, 1].Style.Font.Size = 9;
                worksheet.Cells[5, 1].Style.Font.Color.SetColor(System.Drawing.Color.Gray);
                worksheet.Cells[6, 1].Value = "Row 3: NA2670 | SOP/PROD/HOSE/24/10/039";
                worksheet.Cells[6, 1].Style.Font.Size = 9;
                worksheet.Cells[6, 1].Style.Font.Color.SetColor(System.Drawing.Color.Gray);
                
                worksheet.Cells.AutoFitColumns();
                
                var stream = new MemoryStream(package.GetAsByteArray());
                var fileName = $"Template_ItemList_{DateTime.Now:yyyyMMdd}.xlsx";
                
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportItemListOnly(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "File tidak valid";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                
                if (worksheet == null)
                {
                    TempData["ErrorMessage"] = "Worksheet tidak ditemukan";
                    return RedirectToAction(nameof(Index));
                }

                int rowCount = worksheet.Dimension?.Rows ?? 0;
                int importedCount = 0;
                int errorCount = 0;
                var errors = new List<string>();

                var header1 = worksheet.Cells[1, 1].Text?.Trim().ToUpper() ?? "";
                var header2 = worksheet.Cells[1, 2].Text?.Trim().ToUpper() ?? "";
                
                bool isNewFormat = header1.Contains("ITEM") && (header2.Contains("DOC") || header2.Contains("NO"));
                int startRow = 2;

                for (int row = startRow; row <= rowCount; row++)
                {
                    try
                    {
                        var col1Value = worksheet.Cells[row, 1].Text?.Trim();
                        var col2Value = worksheet.Cells[row, 2].Text?.Trim();

                        if (string.IsNullOrWhiteSpace(col1Value) || string.IsNullOrWhiteSpace(col2Value)) continue;

                        string itemList = isNewFormat ? col1Value : col2Value;
                        string documentNumber = isNewFormat ? col2Value : col1Value;

                        var spsNoDoc = await _context.SpsNoDocs.FirstOrDefaultAsync(s => s.DocumentNumber == documentNumber);
                        if (spsNoDoc != null)
                        {
                            var existingItem = await _context.SpsItemLists
                                .FirstOrDefaultAsync(i => i.DocumentNumber == spsNoDoc.DocumentNumber && i.ItemList == itemList);
                            
                            if (existingItem == null)
                            {
                                _context.SpsItemLists.Add(new SpsItemList
                                {
                                    DocumentNumber = spsNoDoc.DocumentNumber,
                                    ItemList = itemList
                                });
                                importedCount++;
                            }
                        }
                        else
                        {
                            errors.Add($"Row {row}: Document '{documentNumber}' tidak ditemukan");
                            errorCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {row}: {ex.Message}");
                        errorCount++;
                    }
                }

                if (importedCount > 0)
                {
                    await _context.SaveChangesAsync();
                }

                if (errorCount > 0)
                {
                    TempData["ErrorMessage"] = $"Berhasil import {importedCount} data, namun ada {errorCount} error: " + string.Join(", ", errors.Take(3));
                }
                else
                {
                    TempData["SuccessMessage"] = $"Berhasil import {importedCount} Item List baru!";
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool SpsItemListExists(int id)
        {
            return _context.SpsItemLists.Any(e => e.Id == id);
        }
    }
}
