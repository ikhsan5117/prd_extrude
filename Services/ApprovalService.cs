using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Services
{
    public class ApprovalService : IApprovalService
    {
        private static readonly string[] ApproverRoles = { "SUPERADMIN" };
        private static readonly string[] RequesterRoles = { "ADMIN_ENG", "OPERATOR", "ENGINEERING" };

        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApprovalService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private string CurrentUserName => _httpContextAccessor.HttpContext?.Session.GetString("UserName") ?? "Unknown";
        private string CurrentUserRole => (_httpContextAccessor.HttpContext?.Session.GetString("UserRole") ?? string.Empty).ToUpperInvariant();

        public bool IsRequesterRole()
        {
            return RequesterRoles.Contains(CurrentUserRole, StringComparer.OrdinalIgnoreCase);
        }

        public bool IsApproverRole()
        {
            return ApproverRoles.Contains(CurrentUserRole, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<bool> HasConsumableApprovalAsync(ApprovalActionType actionType, string targetKey)
        {
            if (!IsRequesterRole())
            {
                return true;
            }

            targetKey = NormalizeTargetKey(targetKey);
            var now = DateTime.UtcNow;
            var request = await _context.ApprovalRequests
                .AsNoTracking()
                .Where(r => r.RequesterUserName == CurrentUserName
                            && r.ActionType == actionType
                            && r.TargetKey == targetKey
                            && r.Status == ApprovalRequestStatus.Approved
                            && (r.TokenExpiresAt == null || r.TokenExpiresAt > now))
                .OrderByDescending(r => r.ReviewedAt)
                .FirstOrDefaultAsync();

            return request != null;
        }

        public async Task ConsumeApprovalAsync(ApprovalActionType actionType, string targetKey)
        {
            if (!IsRequesterRole())
            {
                return;
            }

            targetKey = NormalizeTargetKey(targetKey);
            var now = DateTime.UtcNow;
            var request = await _context.ApprovalRequests
                .Where(r => r.RequesterUserName == CurrentUserName
                            && r.ActionType == actionType
                            && r.TargetKey == targetKey
                            && r.Status == ApprovalRequestStatus.Approved
                            && (r.TokenExpiresAt == null || r.TokenExpiresAt > now))
                .OrderByDescending(r => r.ReviewedAt)
                .FirstOrDefaultAsync();

            if (request == null)
            {
                return;
            }

            var previousStatus = request.Status;
            request.Status = ApprovalRequestStatus.Consumed;
            request.ConsumedAt = now;
            request.UpdatedAt = now;

            _context.ApprovalRequestLogs.Add(new ApprovalRequestLog
            {
                ApprovalRequestId = request.Id,
                FromStatus = previousStatus,
                ToStatus = ApprovalRequestStatus.Consumed,
                Comment = "Approval token consumed by execution",
                ActorUserName = CurrentUserName,
                ActorRole = CurrentUserRole,
                CreatedAt = now
            });

            await _context.SaveChangesAsync();
        }

        public async Task<ApprovalRequest> CreateOrReusePendingRequestAsync(ApprovalActionType actionType, string targetKey, string requestComment, string? returnUrl = null)
        {
            targetKey = NormalizeTargetKey(targetKey);
            requestComment = (requestComment ?? string.Empty).Trim();
            var now = DateTime.UtcNow;

            var existingPending = await _context.ApprovalRequests
                .FirstOrDefaultAsync(r => r.RequesterUserName == CurrentUserName
                                          && r.ActionType == actionType
                                          && r.TargetKey == targetKey
                                          && r.Status == ApprovalRequestStatus.Pending);

            if (existingPending != null)
            {
                if (!string.IsNullOrWhiteSpace(requestComment))
                {
                    existingPending.RequestComment = requestComment;
                    existingPending.UpdatedAt = now;
                    existingPending.ReturnUrl = returnUrl;
                    _context.ApprovalRequestLogs.Add(new ApprovalRequestLog
                    {
                        ApprovalRequestId = existingPending.Id,
                        FromStatus = ApprovalRequestStatus.Pending,
                        ToStatus = ApprovalRequestStatus.Pending,
                        Comment = "Requester updated pending request comment",
                        ActorUserName = CurrentUserName,
                        ActorRole = CurrentUserRole,
                        CreatedAt = now
                    });
                    await _context.SaveChangesAsync();
                }

                return existingPending;
            }

            var request = new ApprovalRequest
            {
                RequestCode = $"APR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}",
                ActionType = actionType,
                TargetKey = targetKey,
                RequestComment = requestComment,
                ReturnUrl = returnUrl,
                RequesterUserName = CurrentUserName,
                RequesterRole = CurrentUserRole,
                Status = ApprovalRequestStatus.Pending,
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.ApprovalRequests.Add(request);
            await _context.SaveChangesAsync();

            _context.ApprovalRequestLogs.Add(new ApprovalRequestLog
            {
                ApprovalRequestId = request.Id,
                FromStatus = null,
                ToStatus = ApprovalRequestStatus.Pending,
                Comment = requestComment,
                ActorUserName = CurrentUserName,
                ActorRole = CurrentUserRole,
                CreatedAt = now
            });

            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<List<ApprovalRequest>> GetMyRequestsAsync()
        {
            return await _context.ApprovalRequests
                .AsNoTracking()
                .Where(r => r.RequesterUserName == CurrentUserName)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ApprovalRequest>> GetInboxAsync(string? status = null)
        {
            var query = _context.ApprovalRequests.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ApprovalRequestStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(x => x.Status == parsedStatus);
            }

            return await query
                .OrderBy(r => r.Status == ApprovalRequestStatus.Pending ? 0 : 1)
                .ThenByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> CountInboxPendingAsync()
        {
            if (!IsApproverRole())
            {
                return 0;
            }

            return await _context.ApprovalRequests
                .AsNoTracking()
                .CountAsync(r => r.Status == ApprovalRequestStatus.Pending);
        }

        public async Task<int> CountMyOpenRequestsAsync()
        {
            if (!IsRequesterRole())
            {
                return 0;
            }

            return await _context.ApprovalRequests
                .AsNoTracking()
                .CountAsync(r => r.RequesterUserName == CurrentUserName &&
                                 (r.Status == ApprovalRequestStatus.Pending || r.Status == ApprovalRequestStatus.Rejected));
        }

        public async Task<ApprovalRequest?> GetByIdAsync(int id)
        {
            return await _context.ApprovalRequests
                .Include(r => r.Logs.OrderByDescending(l => l.CreatedAt))
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<(bool ok, string message)> ApproveAsync(int id, string? comment)
        {
            if (!IsApproverRole())
            {
                return (false, "Anda tidak memiliki akses untuk approve.");
            }

            var request = await _context.ApprovalRequests.FirstOrDefaultAsync(r => r.Id == id);
            if (request == null)
            {
                return (false, "Request approval tidak ditemukan.");
            }

            if (request.Status != ApprovalRequestStatus.Pending)
            {
                return (false, "Hanya request Pending yang bisa di-approve.");
            }

            var now = DateTime.UtcNow;
            var previousStatus = request.Status;
            request.Status = ApprovalRequestStatus.Approved;
            request.ApproverUserName = CurrentUserName;
            request.ApproverRole = CurrentUserRole;
            request.ApproverComment = (comment ?? string.Empty).Trim();
            request.ReviewedAt = now;
            request.UpdatedAt = now;
            request.ApprovalToken = Guid.NewGuid().ToString("N");
            request.TokenExpiresAt = now.AddHours(24);

            _context.ApprovalRequestLogs.Add(new ApprovalRequestLog
            {
                ApprovalRequestId = request.Id,
                FromStatus = previousStatus,
                ToStatus = ApprovalRequestStatus.Approved,
                Comment = request.ApproverComment,
                ActorUserName = CurrentUserName,
                ActorRole = CurrentUserRole,
                CreatedAt = now
            });

            await _context.SaveChangesAsync();
            return (true, "Request berhasil di-approve.");
        }

        public async Task<(bool ok, string message)> RejectAsync(int id, string? comment)
        {
            if (!IsApproverRole())
            {
                return (false, "Anda tidak memiliki akses untuk reject.");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                return (false, "Komentar reject wajib diisi.");
            }

            var request = await _context.ApprovalRequests.FirstOrDefaultAsync(r => r.Id == id);
            if (request == null)
            {
                return (false, "Request approval tidak ditemukan.");
            }

            if (request.Status != ApprovalRequestStatus.Pending)
            {
                return (false, "Hanya request Pending yang bisa di-reject.");
            }

            var now = DateTime.UtcNow;
            var previousStatus = request.Status;
            request.Status = ApprovalRequestStatus.Rejected;
            request.ApproverUserName = CurrentUserName;
            request.ApproverRole = CurrentUserRole;
            request.ApproverComment = comment.Trim();
            request.ReviewedAt = now;
            request.UpdatedAt = now;
            request.ApprovalToken = null;
            request.TokenExpiresAt = null;

            _context.ApprovalRequestLogs.Add(new ApprovalRequestLog
            {
                ApprovalRequestId = request.Id,
                FromStatus = previousStatus,
                ToStatus = ApprovalRequestStatus.Rejected,
                Comment = request.ApproverComment,
                ActorUserName = CurrentUserName,
                ActorRole = CurrentUserRole,
                CreatedAt = now
            });

            await _context.SaveChangesAsync();
            return (true, "Request berhasil di-reject.");
        }

        public async Task<(bool ok, string message)> ResubmitAsync(int id, string comment)
        {
            if (!IsRequesterRole())
            {
                return (false, "Hanya requester yang bisa submit ulang.");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                return (false, "Komentar perbaikan wajib diisi.");
            }

            var request = await _context.ApprovalRequests.FirstOrDefaultAsync(r => r.Id == id);
            if (request == null)
            {
                return (false, "Request approval tidak ditemukan.");
            }

            if (!string.Equals(request.RequesterUserName, CurrentUserName, StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Anda tidak bisa submit ulang request milik user lain.");
            }

            if (request.Status != ApprovalRequestStatus.Rejected)
            {
                return (false, "Hanya request Rejected yang bisa submit ulang.");
            }

            var now = DateTime.UtcNow;
            var previousStatus = request.Status;
            request.Status = ApprovalRequestStatus.Pending;
            request.RequestComment = comment.Trim();
            request.ApproverComment = null;
            request.ApproverRole = null;
            request.ApproverUserName = null;
            request.ReviewedAt = null;
            request.UpdatedAt = now;
            request.ApprovalToken = null;
            request.TokenExpiresAt = null;

            _context.ApprovalRequestLogs.Add(new ApprovalRequestLog
            {
                ApprovalRequestId = request.Id,
                FromStatus = previousStatus,
                ToStatus = ApprovalRequestStatus.Pending,
                Comment = request.RequestComment,
                ActorUserName = CurrentUserName,
                ActorRole = CurrentUserRole,
                CreatedAt = now
            });

            await _context.SaveChangesAsync();
            return (true, "Request berhasil dikirim ulang ke admin.");
        }

        private static string NormalizeTargetKey(string targetKey)
        {
            if (string.IsNullOrWhiteSpace(targetKey))
            {
                return "GENERAL";
            }

            return targetKey.Trim().ToUpperInvariant();
        }
    }
}
