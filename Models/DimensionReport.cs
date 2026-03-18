using System.ComponentModel.DataAnnotations;

namespace VelastoProductionSystem.Models
{
    public class DimensionReport
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "No. Dokumen")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Tanggal Produksi")]
        public DateTime ProductionDate { get; set; } = DateTime.Now;

        [Display(Name = "Shift")]
        public string? Shift { get; set; }

        [Display(Name = "Customer")]
        public string? CustomerName { get; set; }

        [Display(Name = "Tipe Hose")]
        public string? HoseType { get; set; }

        [Display(Name = "Dimensi")]
        public string? DimensionDisplay { get; set; }

        [Display(Name = "Yarn/Wire")]
        public string? Yarn { get; set; }

        // Production Evaluation
        public string? VinCode { get; set; }
        public string? StandardLength { get; set; }
        public string? ActualLength { get; set; }
        public int QtyTarget { get; set; }
        public int QtyOk { get; set; }
        public int NgDimension { get; set; }
        public int NgVisual { get; set; }
        public string? Remark { get; set; }
        public string? ByPass { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = "ACTIVE"; // ACTIVE, COMPLETED

        [Display(Name = "Diperiksa Oleh")]
        public string? CheckedBy { get; set; }

        [Display(Name = "Dibuat Oleh")]
        public string? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<DimensionMeasurement> Measurements { get; set; } = new List<DimensionMeasurement>();
    }
}
