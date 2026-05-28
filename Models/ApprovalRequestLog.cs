using System.ComponentModel.DataAnnotations;

namespace VelastoProductionSystem.Models
{
    public class ApprovalRequestLog
    {
        public int Id { get; set; }

        public int ApprovalRequestId { get; set; }

        public ApprovalRequest? ApprovalRequest { get; set; }

        public ApprovalRequestStatus? FromStatus { get; set; }

        public ApprovalRequestStatus ToStatus { get; set; }

        [MaxLength(1024)]
        public string? Comment { get; set; }

        [Required]
        [MaxLength(150)]
        public string ActorUserName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string ActorRole { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
