using Microsoft.AspNetCore.Mvc;
using VelastoProductionSystem.Models;
using VelastoProductionSystem.Services;
using System.Reflection;
using System.Text.Json;
using System.Globalization;

namespace VelastoProductionSystem.Controllers
{
    public class ApprovalController : Controller
    {
        private readonly IApprovalService _approvalService;

        public ApprovalController(IApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        [HttpGet]
        [ActionName("Request")]
        public IActionResult RequestForm(ApprovalActionType actionType, string? targetKey, string? returnUrl)
        {
            if (!_approvalService.IsRequesterRole())
            {
                TempData["ErrorMessage"] = "Role Anda tidak memerlukan request approval.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ActionType = actionType;
            ViewBag.TargetKey = targetKey ?? string.Empty;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ActionName("Request")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitRequest(ApprovalActionType actionType, string? targetKey, string? requestComment, string? returnUrl, string? submitMode)
        {
            if (!_approvalService.IsRequesterRole())
            {
                TempData["ErrorMessage"] = "Role Anda tidak memerlukan request approval.";
                return RedirectToAction("Index", "Home");
            }

            var isDraft = string.Equals(submitMode, "draft", StringComparison.OrdinalIgnoreCase);

            if (isDraft)
            {
                var draft = await _approvalService.SaveDraftRequestAsync(actionType, targetKey ?? string.Empty, requestComment, returnUrl);
                TempData["SuccessMessage"] = $"Draft request tersimpan (Code: {draft.RequestCode}). Bisa dilanjutkan kapan saja.";
                return RedirectToAction(nameof(MyRequests));
            }

            if (string.IsNullOrWhiteSpace(requestComment))
            {
                TempData["ErrorMessage"] = "Pesan ke admin wajib diisi.";
                return RedirectToAction("Request", new { actionType, targetKey, returnUrl });
            }

            var request = await _approvalService.CreateOrReusePendingRequestAsync(actionType, targetKey ?? string.Empty, requestComment, returnUrl);
            TempData["SuccessMessage"] = $"Request approval terkirim (Code: {request.RequestCode}). Tunggu review admin.";
            return RedirectToAction(nameof(MyRequests));
        }

        [HttpGet]
        public async Task<IActionResult> MyRequests()
        {
            if (!_approvalService.IsRequesterRole())
            {
                TempData["ErrorMessage"] = "Halaman ini khusus requester approval.";
                return RedirectToAction("Index", "Home");
            }

            var rows = await _approvalService.GetMyRequestsAsync();
            return View(rows);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resubmit(int id, string? requestComment)
        {
            var result = await _approvalService.ResubmitAsync(id, requestComment ?? string.Empty);
            TempData[result.ok ? "SuccessMessage" : "ErrorMessage"] = result.message;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Inbox(string? status)
        {
            if (!_approvalService.IsApproverRole())
            {
                TempData["ErrorMessage"] = "Halaman ini khusus approver admin.";
                return RedirectToAction("Index", "Home");
            }

            var rows = await _approvalService.GetInboxAsync(status);
            ViewBag.StatusFilter = status;
            return View(rows);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var row = await _approvalService.GetByIdAsync(id);
            if (row == null)
            {
                return NotFound();
            }

            var isOwner = string.Equals(row.RequesterUserName, HttpContext.Session.GetString("UserName"), StringComparison.OrdinalIgnoreCase);
            var canOpen = isOwner || (_approvalService.IsApproverRole() && row.Status != ApprovalRequestStatus.Draft);

            if (!canOpen)
            {
                TempData["ErrorMessage"] = "Anda tidak punya akses ke request ini.";
                return RedirectToAction("Index", "Home");
            }

            return View(row);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePayloadDraftData(int id, string? saveMode = null)
        {
            var row = await _approvalService.GetByIdAsync(id);
            if (row == null)
            {
                return NotFound();
            }

            var currentUser = HttpContext.Session.GetString("UserName") ?? string.Empty;
            var isOwner = string.Equals(row.RequesterUserName, currentUser, StringComparison.OrdinalIgnoreCase);
            if (!isOwner)
            {
                TempData["ErrorMessage"] = "Anda tidak punya akses untuk mengubah data request ini.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (row.Status != ApprovalRequestStatus.Draft && row.Status != ApprovalRequestStatus.RevisionRequired)
            {
                TempData["ErrorMessage"] = "Data hanya bisa diubah saat status Draft atau Revision Required.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (string.IsNullOrWhiteSpace(row.PayloadJson))
            {
                TempData["ErrorMessage"] = "Payload detail tidak tersedia untuk diedit.";
                return RedirectToAction(nameof(Details), new { id });
            }

            string updatedPayload;

            if (row.ActionType == ApprovalActionType.SpsItemCreate || row.ActionType == ApprovalActionType.SpsItemEdit)
            {
                var model = JsonSerializer.Deserialize<SpsItemList>(row.PayloadJson) ?? new SpsItemList();
                ApplyStringPayload(model, Request.Form, "DocumentNumber", "ItemList");
                updatedPayload = JsonSerializer.Serialize(model);
            }
            else if (row.ActionType == ApprovalActionType.SpsDocumentCreate || row.ActionType == ApprovalActionType.SpsDocumentEdit)
            {
                var model = JsonSerializer.Deserialize<SpsMaster>(row.PayloadJson) ?? new SpsMaster();
                ApplyGenericPayload(model, Request.Form);
                updatedPayload = JsonSerializer.Serialize(model);
            }
            else
            {
                TempData["ErrorMessage"] = "Jenis request ini belum mendukung edit payload dari detail.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await _approvalService.SaveDraftRequestAsync(
                row.ActionType,
                row.TargetKey,
                row.RequestComment,
                row.ReturnUrl,
                updatedPayload,
                row.Id);

            TempData["SuccessMessage"] = "Perubahan data diajukan berhasil disimpan.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string? approverComment)
        {
            var result = await _approvalService.ApproveAsync(id, approverComment);
            TempData[result.ok ? "SuccessMessage" : "ErrorMessage"] = result.message;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? approverComment)
        {
            var result = await _approvalService.RejectAsync(id, approverComment);
            TempData[result.ok ? "SuccessMessage" : "ErrorMessage"] = result.message;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalReject(int id, string? approverComment)
        {
            var result = await _approvalService.FinalRejectAsync(id, approverComment);
            TempData[result.ok ? "SuccessMessage" : "ErrorMessage"] = result.message;
            return RedirectToAction(nameof(Details), new { id });
        }

        private static void ApplyStringPayload(SpsItemList model, IFormCollection form, params string[] fields)
        {
            var map = typeof(SpsItemList)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

            foreach (var field in fields)
            {
                if (!map.TryGetValue(field, out var prop) || !prop.CanWrite)
                {
                    continue;
                }

                var key = "payload_" + field;
                if (!form.TryGetValue(key, out var value))
                {
                    continue;
                }

                var normalized = value.ToString().Trim();
                prop.SetValue(model, normalized);
            }
        }

        private static void ApplyGenericPayload<T>(T model, IFormCollection form)
        {
            var map = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

            foreach (var formItem in form)
            {
                if (!formItem.Key.StartsWith("payload_", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var propertyName = formItem.Key.Substring("payload_".Length);
                if (!map.TryGetValue(propertyName, out var prop) || !prop.CanWrite)
                {
                    continue;
                }

                var raw = formItem.Value.ToString().Trim();
                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                if (targetType == typeof(string))
                {
                    prop.SetValue(model, raw);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(raw) && Nullable.GetUnderlyingType(prop.PropertyType) != null)
                {
                    prop.SetValue(model, null);
                    continue;
                }

                if (targetType == typeof(bool))
                {
                    SetTypedValue(model, prop, raw, ParseBool(raw));
                    continue;
                }

                if (targetType == typeof(decimal))
                {
                    if (TryParseDecimal(raw, out var decimalValue))
                    {
                        SetTypedValue(model, prop, raw, decimalValue);
                    }
                    continue;
                }

                if (targetType == typeof(double))
                {
                    if (double.TryParse(raw, NumberStyles.Any, CultureInfo.CurrentCulture, out var doubleValue)
                        || double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out doubleValue))
                    {
                        SetTypedValue(model, prop, raw, doubleValue);
                    }
                    continue;
                }

                if (targetType == typeof(float))
                {
                    if (float.TryParse(raw, NumberStyles.Any, CultureInfo.CurrentCulture, out var floatValue)
                        || float.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out floatValue))
                    {
                        SetTypedValue(model, prop, raw, floatValue);
                    }
                    continue;
                }

                if (targetType == typeof(int))
                {
                    if (int.TryParse(raw, NumberStyles.Any, CultureInfo.CurrentCulture, out var intValue)
                        || int.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out intValue))
                    {
                        SetTypedValue(model, prop, raw, intValue);
                    }
                    continue;
                }

                if (targetType == typeof(long))
                {
                    if (long.TryParse(raw, NumberStyles.Any, CultureInfo.CurrentCulture, out var longValue)
                        || long.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out longValue))
                    {
                        SetTypedValue(model, prop, raw, longValue);
                    }
                    continue;
                }
            }
        }

        private static void SetTypedValue<T>(T model, PropertyInfo prop, string raw, object value)
        {
            var nullableType = Nullable.GetUnderlyingType(prop.PropertyType);
            if (string.IsNullOrWhiteSpace(raw) && nullableType != null)
            {
                prop.SetValue(model, null);
                return;
            }

            prop.SetValue(model, value);
        }

        private static bool TryParseDecimal(string raw, out decimal value)
        {
            if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.CurrentCulture, out value))
            {
                return true;
            }

            if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return true;
            }

            return decimal.TryParse(raw.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        private static bool ParseBool(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return false;
            }

            var normalized = raw.Trim().ToLowerInvariant();
            return normalized is "1" or "true" or "active" or "yes" or "ya";
        }
    }
}
