using Microsoft.AspNetCore.Mvc;
using VelastoProductionSystem.Models;
using VelastoProductionSystem.Services;

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
        public async Task<IActionResult> SubmitRequest(ApprovalActionType actionType, string? targetKey, string? requestComment, string? returnUrl)
        {
            if (!_approvalService.IsRequesterRole())
            {
                TempData["ErrorMessage"] = "Role Anda tidak memerlukan request approval.";
                return RedirectToAction("Index", "Home");
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

            var canOpen = _approvalService.IsApproverRole() ||
                          string.Equals(row.RequesterUserName, HttpContext.Session.GetString("UserName"), StringComparison.OrdinalIgnoreCase);

            if (!canOpen)
            {
                TempData["ErrorMessage"] = "Anda tidak punya akses ke request ini.";
                return RedirectToAction("Index", "Home");
            }

            return View(row);
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
    }
}
