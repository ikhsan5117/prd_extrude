using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Helpers;
using VelastoProductionSystem.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

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

        public async Task<ApprovalRequest> SaveDraftRequestAsync(ApprovalActionType actionType, string targetKey, string? requestComment, string? returnUrl = null, string? payloadJson = null, int? sourceRequestId = null)
        {
            targetKey = NormalizeTargetKey(targetKey);
            requestComment = (requestComment ?? string.Empty).Trim();
            var now = DateTime.UtcNow;

            if (sourceRequestId.HasValue)
            {
                var sourceRequest = await _context.ApprovalRequests
                    .FirstOrDefaultAsync(r => r.Id == sourceRequestId.Value
                                              && r.RequesterUserName == CurrentUserName
                                              && r.ActionType == actionType);

                if (sourceRequest != null)
                {
                    sourceRequest.TargetKey = targetKey;
                    sourceRequest.RequestComment = requestComment;
                    sourceRequest.ReturnUrl = returnUrl;
                    if (!string.IsNullOrWhiteSpace(payloadJson))
                    {
                        sourceRequest.PayloadJson = payloadJson;
                    }
                    sourceRequest.UpdatedAt = now;

                    _context.ApprovalRequestLogs.Add(new ApprovalRequestLog
                    {
                        ApprovalRequestId = sourceRequest.Id,
                        FromStatus = sourceRequest.Status,
                        ToStatus = sourceRequest.Status,
                        Comment = sourceRequest.Status == ApprovalRequestStatus.RevisionRequired
                            ? "Requester updated draft revision"
                            : "Requester updated draft",
                        ActorUserName = CurrentUserName,
                        ActorRole = CurrentUserRole,
                        CreatedAt = now
                    });

                    await _context.SaveChangesAsync();
                    return sourceRequest;
                }
            }

            var existingDraft = await _context.ApprovalRequests
                .FirstOrDefaultAsync(r => r.RequesterUserName == CurrentUserName
                                          && r.ActionType == actionType
                                          && r.TargetKey == targetKey
                                          && r.Status == ApprovalRequestStatus.Draft);

            if (existingDraft != null)
            {
                existingDraft.RequestComment = requestComment;
                existingDraft.ReturnUrl = returnUrl;
                if (!string.IsNullOrWhiteSpace(payloadJson))
                {
                    existingDraft.PayloadJson = payloadJson;
                }
                existingDraft.UpdatedAt = now;

                _context.ApprovalRequestLogs.Add(new ApprovalRequestLog
                {
                    ApprovalRequestId = existingDraft.Id,
                    FromStatus = ApprovalRequestStatus.Draft,
                    ToStatus = ApprovalRequestStatus.Draft,
                    Comment = "Requester updated draft",
                    ActorUserName = CurrentUserName,
                    ActorRole = CurrentUserRole,
                    CreatedAt = now
                });

                await _context.SaveChangesAsync();
                return existingDraft;
            }

            var existingRevisionRequired = await _context.ApprovalRequests
                .FirstOrDefaultAsync(r => r.RequesterUserName == CurrentUserName
                                          && r.ActionType == actionType
                                          && r.TargetKey == targetKey
                                          && r.Status == ApprovalRequestStatus.RevisionRequired);

            if (existingRevisionRequired != null)
            {
                existingRevisionRequired.RequestComment = requestComment;
                existingRevisionRequired.ReturnUrl = returnUrl;
                if (!string.IsNullOrWhiteSpace(payloadJson))
                {
                    existingRevisionRequired.PayloadJson = payloadJson;
                }
                existingRevisionRequired.UpdatedAt = now;

                _context.ApprovalRequestLogs.Add(new ApprovalRequestLog
                {
                    ApprovalRequestId = existingRevisionRequired.Id,
                    FromStatus = ApprovalRequestStatus.RevisionRequired,
                    ToStatus = ApprovalRequestStatus.RevisionRequired,
                    Comment = "Requester updated revision draft",
                    ActorUserName = CurrentUserName,
                    ActorRole = CurrentUserRole,
                    CreatedAt = now
                });

                await _context.SaveChangesAsync();
                return existingRevisionRequired;
            }

            var request = new ApprovalRequest
            {
                RequestCode = $"APR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}",
                ActionType = actionType,
                TargetKey = targetKey,
                RequestComment = requestComment,
                ReturnUrl = returnUrl,
                PayloadJson = payloadJson,
                RequesterUserName = CurrentUserName,
                RequesterRole = CurrentUserRole,
                Status = ApprovalRequestStatus.Draft,
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.ApprovalRequests.Add(request);
            await _context.SaveChangesAsync();

            _context.ApprovalRequestLogs.Add(new ApprovalRequestLog
            {
                ApprovalRequestId = request.Id,
                FromStatus = null,
                ToStatus = ApprovalRequestStatus.Draft,
                Comment = string.IsNullOrWhiteSpace(requestComment) ? "Draft disimpan" : requestComment,
                ActorUserName = CurrentUserName,
                ActorRole = CurrentUserRole,
                CreatedAt = now
            });

            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<ApprovalRequest> CreateOrReusePendingRequestAsync(ApprovalActionType actionType, string targetKey, string requestComment, string? returnUrl = null, string? payloadJson = null, int? sourceRequestId = null)
        {
            targetKey = NormalizeTargetKey(targetKey);
            requestComment = (requestComment ?? string.Empty).Trim();
            var now = DateTime.UtcNow;

            // --- PRIORITY: if sourceRequestId is given, find that exact Draft record and promote it ---
            if (sourceRequestId.HasValue)
            {
                var sourceRequest = await _context.ApprovalRequests
                    .FirstOrDefaultAsync(r => r.Id == sourceRequestId.Value
                                              && r.RequesterUserName == CurrentUserName
                                              && r.ActionType == actionType
                                              && (r.Status == ApprovalRequestStatus.Draft || r.Status == ApprovalRequestStatus.RevisionRequired));

                if (sourceRequest != null)
                {
                    // If already Pending, just update comment & payload then return it
                    if (sourceRequest.Status == ApprovalRequestStatus.Pending)
                    {
                        if (!string.IsNullOrWhiteSpace(requestComment))
                        {
                            sourceRequest.RequestComment = requestComment;
                            sourceRequest.UpdatedAt = now;
                            sourceRequest.ReturnUrl = returnUrl;
                            if (!string.IsNullOrWhiteSpace(payloadJson)) sourceRequest.PayloadJson = payloadJson;
                            await _context.SaveChangesAsync();
                        }
                        return sourceRequest;
                    }

                    var previousStatus = sourceRequest.Status;
                    sourceRequest.Status = ApprovalRequestStatus.Pending;
                    sourceRequest.TargetKey = targetKey;  // update in case key changed
                    sourceRequest.RequestComment = requestComment;
                    sourceRequest.ReturnUrl = returnUrl;
                    if (!string.IsNullOrWhiteSpace(payloadJson)) sourceRequest.PayloadJson = payloadJson;
                    sourceRequest.UpdatedAt = now;
                    sourceRequest.ApproverComment = null;
                    sourceRequest.ApproverRole = null;
                    sourceRequest.ApproverUserName = null;
                    sourceRequest.ReviewedAt = null;
                    sourceRequest.ApprovalToken = null;
                    sourceRequest.TokenExpiresAt = null;

                    _context.ApprovalRequestLogs.Add(new ApprovalRequestLog
                    {
                        ApprovalRequestId = sourceRequest.Id,
                        FromStatus = previousStatus,
                        ToStatus = ApprovalRequestStatus.Pending,
                        Comment = string.IsNullOrWhiteSpace(requestComment) ? "Requester submit draft" : requestComment,
                        ActorUserName = CurrentUserName,
                        ActorRole = CurrentUserRole,
                        CreatedAt = now
                    });

                    await _context.SaveChangesAsync();
                    return sourceRequest;
                }
            }

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
                    if (!string.IsNullOrWhiteSpace(payloadJson))
                    {
                        existingPending.PayloadJson = payloadJson;
                    }
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

            var existingDraft = await _context.ApprovalRequests
                .FirstOrDefaultAsync(r => r.RequesterUserName == CurrentUserName
                                          && r.ActionType == actionType
                                          && r.TargetKey == targetKey
                                          && r.Status == ApprovalRequestStatus.Draft);

            if (existingDraft != null)
            {
                var previousStatus = existingDraft.Status;
                existingDraft.Status = ApprovalRequestStatus.Pending;
                existingDraft.RequestComment = requestComment;
                existingDraft.ReturnUrl = returnUrl;
                if (!string.IsNullOrWhiteSpace(payloadJson))
                {
                    existingDraft.PayloadJson = payloadJson;
                }
                existingDraft.UpdatedAt = now;
                existingDraft.ApproverComment = null;
                existingDraft.ApproverRole = null;
                existingDraft.ApproverUserName = null;
                existingDraft.ReviewedAt = null;
                existingDraft.ApprovalToken = null;
                existingDraft.TokenExpiresAt = null;

                _context.ApprovalRequestLogs.Add(new ApprovalRequestLog
                {
                    ApprovalRequestId = existingDraft.Id,
                    FromStatus = previousStatus,
                    ToStatus = ApprovalRequestStatus.Pending,
                    Comment = string.IsNullOrWhiteSpace(requestComment) ? "Requester submit draft" : requestComment,
                    ActorUserName = CurrentUserName,
                    ActorRole = CurrentUserRole,
                    CreatedAt = now
                });

                await _context.SaveChangesAsync();
                return existingDraft;
            }

            var request = new ApprovalRequest
            {
                RequestCode = $"APR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}",
                ActionType = actionType,
                TargetKey = targetKey,
                RequestComment = requestComment,
                ReturnUrl = returnUrl,
                PayloadJson = payloadJson,
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
            var query = _context.ApprovalRequests
                .AsNoTracking()
                .Where(x => x.Status != ApprovalRequestStatus.Draft);

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
                                 (r.Status == ApprovalRequestStatus.Draft || r.Status == ApprovalRequestStatus.Pending || r.Status == ApprovalRequestStatus.RevisionRequired));
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

            var executionMessage = await TryExecuteApprovedRequestAsync(request);
            if (!string.IsNullOrWhiteSpace(executionMessage))
            {
                request.ApproverComment = string.IsNullOrWhiteSpace(request.ApproverComment)
                    ? executionMessage
                    : $"{request.ApproverComment}\n{executionMessage}";
            }

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

        private async Task<string?> TryExecuteApprovedRequestAsync(ApprovalRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PayloadJson))
            {
                return null;
            }

            switch (request.ActionType)
            {
                case ApprovalActionType.SpsDocumentCreate:
                    {
                        var model = JsonSerializer.Deserialize<SpsMaster>(request.PayloadJson);
                        if (model == null || string.IsNullOrWhiteSpace(model.DocumentNumber))
                        {
                            return "Payload SPS document tidak valid.";
                        }

                        if (await _context.SpsNoDocs.AnyAsync(s => s.DocumentNumber == model.DocumentNumber))
                        {
                            return $"Dokumen '{model.DocumentNumber}' sudah ada, create dilewati.";
                        }

                        var createdDoc = SpsMapper.ToSpsNoDoc(model);
                        createdDoc.IsActive = true;
                        _context.SpsNoDocs.Add(createdDoc);
                        return $"Dokumen '{model.DocumentNumber}' berhasil dibuat otomatis dari request.";
                    }
                case ApprovalActionType.SpsDocumentEdit:
                    {
                        var model = JsonSerializer.Deserialize<SpsMaster>(request.PayloadJson);
                        if (model == null || string.IsNullOrWhiteSpace(model.DocumentNumber))
                        {
                            return "Payload revisi SPS document tidak valid.";
                        }

                        var targetKey = (request.TargetKey ?? model.DocumentNumber).Trim();
                        if (string.IsNullOrWhiteSpace(targetKey))
                        {
                            return "Target dokumen revisi tidak valid.";
                        }

                        var sourceDoc = await _context.SpsNoDocs
                            .Include(s => s.ItemLists)
                            .FirstOrDefaultAsync(s => s.DocumentNumber.ToUpper() == targetKey.ToUpper());

                        if (sourceDoc == null)
                        {
                            return $"Dokumen sumber '{targetKey}' tidak ditemukan, revisi dilewati.";
                        }

                        string nextDocumentNumber;
                        string? nextRevisionNumber = null;

                        if (!string.Equals(model.DocumentNumber, targetKey, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(model.DocumentNumber))
                        {
                            // Isi manual oleh user
                            nextDocumentNumber = model.DocumentNumber.Trim();
                            nextRevisionNumber = model.RevisionNumber?.Trim();
                        }
                        else 
                        {
                            // Generate otomatis format /01, /02
                            var revisionInfo = await GenerateNextRevisionDocumentNumberAsync(sourceDoc.DocumentNumber);
                            nextDocumentNumber = revisionInfo.DocNumber;
                            nextRevisionNumber = revisionInfo.RevNumber.ToString();
                        }

                        if (await _context.SpsNoDocs.AnyAsync(s => s.DocumentNumber == nextDocumentNumber))
                        {
                            return $"Dokumen revisi '{nextDocumentNumber}' sudah ada, revisi dilewati.";
                        }

                        var revisedDoc = SpsMapper.ToSpsNoDoc(model);
                        revisedDoc.DocumentNumber = nextDocumentNumber;
                        revisedDoc.IsActive = true;
                        revisedDoc.RevisionNumber = nextRevisionNumber;

                        sourceDoc.IsActive = false;
                        _context.SpsNoDocs.Add(revisedDoc);

                        var itemLists = sourceDoc.ItemLists
                            .Where(i => !string.IsNullOrWhiteSpace(i.ItemList))
                            .Select(i => i.ItemList!.Trim())
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToList();

                        foreach (var itemList in itemLists)
                        {
                            _context.SpsItemLists.Add(new SpsItemList
                            {
                                DocumentNumber = nextDocumentNumber,
                                ItemList = itemList
                            });
                        }

                        return $"Revisi berhasil: '{sourceDoc.DocumentNumber}' dinonaktifkan, dokumen aktif baru '{nextDocumentNumber}' dibuat otomatis.";
                    }
                case ApprovalActionType.SpsItemCreate:
                    {
                        var model = JsonSerializer.Deserialize<SpsItemList>(request.PayloadJson);
                        if (model == null || string.IsNullOrWhiteSpace(model.ItemList) || string.IsNullOrWhiteSpace(model.DocumentNumber))
                        {
                            return "Payload Item List tidak valid.";
                        }

                        var existing = await _context.SpsItemLists.FirstOrDefaultAsync(i => i.DocumentNumber == model.DocumentNumber && i.ItemList == model.ItemList);
                        if (existing != null)
                        {
                            return $"Item '{model.ItemList}' pada doc '{model.DocumentNumber}' sudah ada, create dilewati.";
                        }

                        _context.SpsItemLists.Add(new SpsItemList
                        {
                            DocumentNumber = model.DocumentNumber,
                            ItemList = model.ItemList
                        });
                        return $"Item List '{model.ItemList}' berhasil dibuat otomatis dari request.";
                    }
                case ApprovalActionType.SpsItemEdit:
                    {
                        var model = JsonSerializer.Deserialize<SpsItemList>(request.PayloadJson);
                        if (model == null || string.IsNullOrWhiteSpace(model.ItemList) || string.IsNullOrWhiteSpace(model.DocumentNumber))
                        {
                            return "Payload revisi Item List tidak valid.";
                        }

                        if (!int.TryParse(request.TargetKey, out var id))
                        {
                            return "Target ID revisi Item List tidak valid.";
                        }

                        var existingItem = await _context.SpsItemLists.FirstOrDefaultAsync(i => i.Id == id);
                        if (existingItem == null)
                        {
                            return $"Item List dengan ID '{id}' tidak ditemukan, revisi dilewati.";
                        }

                        var oldItemList = existingItem.ItemList;
                        existingItem.ItemList = model.ItemList;
                        existingItem.DocumentNumber = model.DocumentNumber;

                        return $"Revisi Item List berhasil: '{oldItemList}' diubah menjadi '{model.ItemList}'.";
                    }
                default:
                    return null;
            }
        }

        public async Task<(bool ok, string message)> RejectAsync(int id, string? comment)
        {
            if (!IsApproverRole())
            {
                return (false, "Anda tidak memiliki akses untuk meminta revisi.");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                return (false, "Catatan revisi wajib diisi.");
            }

            var request = await _context.ApprovalRequests.FirstOrDefaultAsync(r => r.Id == id);
            if (request == null)
            {
                return (false, "Request approval tidak ditemukan.");
            }

            if (request.Status != ApprovalRequestStatus.Pending)
            {
                return (false, "Hanya request Pending yang bisa diberi status Revision Required.");
            }

            var now = DateTime.UtcNow;
            var previousStatus = request.Status;
            request.Status = ApprovalRequestStatus.RevisionRequired;
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
                ToStatus = ApprovalRequestStatus.RevisionRequired,
                Comment = request.ApproverComment,
                ActorUserName = CurrentUserName,
                ActorRole = CurrentUserRole,
                CreatedAt = now
            });

            await _context.SaveChangesAsync();
            return (true, "Request ditandai Revision Required.");
        }

        public async Task<(bool ok, string message)> FinalRejectAsync(int id, string? comment)
        {
            if (!IsApproverRole())
            {
                return (false, "Anda tidak memiliki akses untuk reject final.");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                return (false, "Alasan reject wajib diisi.");
            }

            var request = await _context.ApprovalRequests.FirstOrDefaultAsync(r => r.Id == id);
            if (request == null)
            {
                return (false, "Request approval tidak ditemukan.");
            }

            if (request.Status != ApprovalRequestStatus.Pending)
            {
                return (false, "Hanya request Pending yang bisa di-reject final.");
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
            return (true, "Request berhasil di-reject final.");
        }

        public async Task<(bool ok, string message)> ResubmitAsync(int id, string comment)
        {
            if (!IsRequesterRole())
            {
                return (false, "Hanya requester yang bisa kirim ulang request revisi.");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                return (false, "Catatan perbaikan wajib diisi.");
            }

            var request = await _context.ApprovalRequests.FirstOrDefaultAsync(r => r.Id == id);
            if (request == null)
            {
                return (false, "Request approval tidak ditemukan.");
            }

            if (!string.Equals(request.RequesterUserName, CurrentUserName, StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Anda tidak bisa kirim ulang request milik user lain.");
            }

            if (request.Status != ApprovalRequestStatus.RevisionRequired)
            {
                return (false, "Hanya request dengan status Revision Required yang bisa dikirim ulang.");
            }

            var now = DateTime.UtcNow;
            var previousStatus = request.Status;
            request.Status = ApprovalRequestStatus.Pending;
            request.RequestComment = comment.Trim();
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
            return (true, "Perbaikan berhasil dikirim ulang ke admin.");
        }

        private static string NormalizeTargetKey(string targetKey)
        {
            if (string.IsNullOrWhiteSpace(targetKey))
            {
                return "GENERAL";
            }

            return targetKey.Trim().ToUpperInvariant();
        }

        private async Task<(string DocNumber, int RevNumber)> GenerateNextRevisionDocumentNumberAsync(string sourceDocumentNumber)
        {
            var sourceDoc = await _context.SpsNoDocs.AsNoTracking().FirstOrDefaultAsync(s => s.DocumentNumber == sourceDocumentNumber);
            string baseDocument = sourceDocumentNumber;
            
            // Jika dokumen asal sudah memiliki nomor revisi, berarti ini adalah turunan dari base document.
            // Kita potong bagian '/XX' di belakangnya untuk mendapatkan base document aslinya.
            if (sourceDoc != null && !string.IsNullOrWhiteSpace(sourceDoc.RevisionNumber))
            {
                int slashIndex = sourceDocumentNumber.LastIndexOf('/');
                if (slashIndex > 0)
                {
                    baseDocument = sourceDocumentNumber.Substring(0, slashIndex);
                }
            }

            var basePrefix = baseDocument + "/";

            // Cari semua dokumen yang sama persis dengan base (revisi 0) atau berawalan base/
            var candidates = await _context.SpsNoDocs
                .AsNoTracking()
                .Where(s => s.DocumentNumber == baseDocument || s.DocumentNumber.StartsWith(basePrefix))
                .Select(s => new { s.DocumentNumber, s.RevisionNumber })
                .ToListAsync();

            var maxRevision = 0;
            foreach (var candidate in candidates)
            {
                if (int.TryParse(candidate.RevisionNumber, out var revNum))
                {
                    if (revNum > maxRevision) maxRevision = revNum;
                }
            }

            int nextRev = maxRevision + 1;
            // Format angka revisi jadi 2 digit (01, 02, dst)
            return ($"{baseDocument}/{nextRev:D2}", nextRev);
        }
    }
}
