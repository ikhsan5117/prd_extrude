using System.ComponentModel.DataAnnotations;

namespace VelastoProductionSystem.Models
{
    public class SpsImportTrialRow
    {
        public int Id { get; set; }

        [MaxLength(64)]
        public string BatchId { get; set; } = string.Empty;

        [MaxLength(256)]
        public string SourceFileName { get; set; } = string.Empty;

        [MaxLength(128)]
        public string SourceSheet { get; set; } = string.Empty;

        [MaxLength(64)]
        public string DetectedFormat { get; set; } = string.Empty;

        [MaxLength(32)]
        public string ExcelId { get; set; } = string.Empty;

        [MaxLength(128)]
        public string ItemCode { get; set; } = string.Empty;

        [MaxLength(128)]
        public string? Machine { get; set; }

        [MaxLength(128)]
        public string? DocumentNumber { get; set; }

        [MaxLength(128)]
        public string? Customer { get; set; }

        [MaxLength(256)]
        public string? HoseType { get; set; }

        [MaxLength(256)]
        public string? Dimensi { get; set; }

        public int SourceRowIndex { get; set; }

        public bool ExistsInProduction { get; set; }

        public DateTime ImportedAt { get; set; }

        [MaxLength(64)]
        public string ImportedBy { get; set; } = string.Empty;
    }
}
