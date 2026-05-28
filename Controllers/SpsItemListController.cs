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
using VelastoProductionSystem.Services;

namespace VelastoProductionSystem.Controllers
{
    public class SpsItemListController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IApprovalService _approvalService;

        public SpsItemListController(ApplicationDbContext context, IApprovalService approvalService)
        {
            _context = context;
            _approvalService = approvalService;
        }

        private IActionResult RedirectToApprovalRequest(ApprovalActionType actionType, string targetKey, string returnUrl)
        {
            TempData["ErrorMessage"] = "Aksi ini membutuhkan approval admin. Kirim request approval terlebih dahulu.";
            return RedirectToAction("Request", "Approval", new
            {
                actionType,
                targetKey,
                returnUrl
            });
        }

        // GET: SpsItemList
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 50)
        {
            if (page < 1) page = 1;
            if (pageSize <= 0) pageSize = 50;

            var query = _context.SpsItemLists.Include(s => s.SpsNoDoc).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchUpper = search.ToUpper();
                query = query.Where(s => 
                    (s.ItemList != null && s.ItemList.ToUpper().Contains(searchUpper)) ||
                    (s.DocumentNumber != null && s.DocumentNumber.ToUpper().Contains(searchUpper))
                );
            }

            var totalCount = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
            if (page > totalPages) page = totalPages;

            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;

            var rows = await query
                .OrderBy(s => s.ItemList)
                .ThenBy(s => s.DocumentNumber)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(rows);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadExcel(string? search)
        {
            var query = _context.SpsItemLists
                .Include(s => s.SpsNoDoc)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchUpper = search.ToUpper();
                query = query.Where(s =>
                    (s.ItemList != null && s.ItemList.ToUpper().Contains(searchUpper)) ||
                    (s.DocumentNumber != null && s.DocumentNumber.ToUpper().Contains(searchUpper))
                );
            }

            var rows = await query
                .OrderBy(s => s.ItemList)
                .ThenBy(s => s.DocumentNumber)
                .ToListAsync();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("SPS Item List");

            var headers = new[] { "No", "ID", "Item List", "No. Document", "Machine", "Hose Type" };
            for (var i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            using (var range = worksheet.Cells[1, 1, 1, headers.Length])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(44, 62, 80));
                range.Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            var rowIndex = 2;
            var runningNo = 1;
            foreach (var item in rows)
            {
                worksheet.Cells[rowIndex, 1].Value = runningNo;
                worksheet.Cells[rowIndex, 2].Value = item.Id;
                worksheet.Cells[rowIndex, 3].Value = item.ItemList;
                worksheet.Cells[rowIndex, 4].Value = item.DocumentNumber;
                worksheet.Cells[rowIndex, 5].Value = item.SpsNoDoc?.MachineCode ?? item.SpsNoDoc?.Machine;
                worksheet.Cells[rowIndex, 6].Value = item.SpsNoDoc?.HoseType;
                rowIndex++;
                runningNo++;
            }

            worksheet.Cells.AutoFitColumns();

            var stream = new MemoryStream(package.GetAsByteArray());
            var fileName = $"Sps_ItemList_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
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
                if (_approvalService.IsRequesterRole())
                {
                    var targetKey = $"{spsItemList.DocumentNumber}|{spsItemList.ItemList}";
                    var allowed = await _approvalService.HasConsumableApprovalAsync(ApprovalActionType.SpsItemCreate, targetKey);
                    if (!allowed)
                    {
                        return RedirectToApprovalRequest(
                            ApprovalActionType.SpsItemCreate,
                            targetKey,
                            Url.Action(nameof(Create), "SpsItemList") ?? "/SpsItemList/Create");
                    }
                }

                _context.Add(spsItemList);
                await _context.SaveChangesAsync();

                await _approvalService.ConsumeApprovalAsync(ApprovalActionType.SpsItemCreate, $"{spsItemList.DocumentNumber}|{spsItemList.ItemList}");
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
                    if (_approvalService.IsRequesterRole())
                    {
                        var targetKey = spsItemList.Id.ToString();
                        var allowed = await _approvalService.HasConsumableApprovalAsync(ApprovalActionType.SpsItemEdit, targetKey);
                        if (!allowed)
                        {
                            return RedirectToApprovalRequest(
                                ApprovalActionType.SpsItemEdit,
                                targetKey,
                                Url.Action(nameof(Edit), "SpsItemList", new { id = spsItemList.Id }) ?? $"/SpsItemList/Edit/{spsItemList.Id}");
                        }
                    }

                    _context.Update(spsItemList);
                    await _context.SaveChangesAsync();

                    await _approvalService.ConsumeApprovalAsync(ApprovalActionType.SpsItemEdit, spsItemList.Id.ToString());
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
                return Json(new { success = false, message = "File tidak valid" });
            }

            if (_approvalService.IsRequesterRole())
            {
                var targetKey = file.FileName;
                var allowed = await _approvalService.HasConsumableApprovalAsync(ApprovalActionType.SpsItemImportTemplate, targetKey);
                if (!allowed)
                {
                    var requestUrl = Url.Action("Request", "Approval", new
                    {
                        actionType = ApprovalActionType.SpsItemImportTemplate,
                        targetKey,
                        returnUrl = Url.Action(nameof(Index), "SpsItemList")
                    }) ?? "/Approval/Request";

                    return Json(new
                    {
                        success = false,
                        message = $"Import template Item List membutuhkan approval admin. <a href='{requestUrl}'>Klik di sini untuk kirim request approval</a>."
                    });
                }
            }

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                
                if (worksheet == null)
                {
                    return Json(new { success = false, message = "Worksheet tidak ditemukan" });
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

                await _approvalService.ConsumeApprovalAsync(ApprovalActionType.SpsItemImportTemplate, file.FileName);

                if (errorCount > 0)
                {
                    return Json(new
                    {
                        success = true,
                        message = $"Berhasil import {importedCount} data, namun ada {errorCount} error.",
                        imported = importedCount,
                        errors = errorCount,
                        errorDetails = errors.Take(10).ToList()
                    });
                }

                return Json(new
                {
                    success = true,
                    message = $"Berhasil import {importedCount} Item List baru!",
                    imported = importedCount,
                    errors = 0
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        private bool SpsItemListExists(int id)
        {
            return _context.SpsItemLists.Any(e => e.Id == id);
        }
    }
}
