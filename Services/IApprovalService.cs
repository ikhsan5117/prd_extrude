using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Services
{
    public interface IApprovalService
    {
        bool IsRequesterRole();
        bool IsApproverRole();
        Task<bool> HasConsumableApprovalAsync(ApprovalActionType actionType, string targetKey);
        Task ConsumeApprovalAsync(ApprovalActionType actionType, string targetKey);
        Task<ApprovalRequest> CreateOrReusePendingRequestAsync(ApprovalActionType actionType, string targetKey, string requestComment, string? returnUrl = null, string? payloadJson = null);
        Task<List<ApprovalRequest>> GetMyRequestsAsync();
        Task<List<ApprovalRequest>> GetInboxAsync(string? status = null);
        Task<int> CountInboxPendingAsync();
        Task<int> CountMyOpenRequestsAsync();
        Task<ApprovalRequest?> GetByIdAsync(int id);
        Task<(bool ok, string message)> ApproveAsync(int id, string? comment);
        Task<(bool ok, string message)> RejectAsync(int id, string? comment);
        Task<(bool ok, string message)> ResubmitAsync(int id, string comment);
    }
}
