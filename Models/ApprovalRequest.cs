using System.ComponentModel.DataAnnotations;

namespace VelastoProductionSystem.Models
{
    public class ApprovalRequest
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string RequestCode { get; set; } = string.Empty;

        [Required]
        public ApprovalActionType ActionType { get; set; }

        [MaxLength(256)]
        public string TargetKey { get; set; } = string.Empty;

        [MaxLength(1024)]
        public string? RequestComment { get; set; }

        [MaxLength(256)]
        public string? ReturnUrl { get; set; }

        public string? PayloadJson { get; set; }

        [Required]
        [MaxLength(150)]
        public string RequesterUserName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string RequesterRole { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? ApproverUserName { get; set; }

        [MaxLength(50)]
        public string? ApproverRole { get; set; }

        [MaxLength(1024)]
        public string? ApproverComment { get; set; }

        [MaxLength(80)]
        public string? ApprovalToken { get; set; }

        public DateTime? TokenExpiresAt { get; set; }

        [Required]
        public ApprovalRequestStatus Status { get; set; } = ApprovalRequestStatus.Pending;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public DateTime? ConsumedAt { get; set; }

        public ICollection<ApprovalRequestLog> Logs { get; set; } = new List<ApprovalRequestLog>();
    }
}
