using System.ComponentModel.DataAnnotations;

namespace VelastoProductionSystem.Models
{
    public class DimensionReading
    {
        public int Id { get; set; }

        [Required]
        public int ProductionReportId { get; set; }
        public ProductionReport? ProductionReport { get; set; }

        [Required]
        [Display(Name = "Waktu Pengukuran")]
        public DateTime ReadingTime { get; set; }

        [Display(Name = "Interval (menit)")]
        public int IntervalMinutes { get; set; } = 30;

        [Display(Name = "Frequency")]
        public string Frequency { get; set; } = "30 Menit Sekali";

        // Inner Diameter Readings
        [Display(Name = "Inner Diameter Standard")]
        public string InnerDiameterStandard { get; set; } = string.Empty;

        [Display(Name = "Inner Diameter 1 (mm)")]
        public decimal? InnerDiameter1 { get; set; }

        [Display(Name = "Inner Diameter 2 (mm)")]
        public decimal? InnerDiameter2 { get; set; }

        [Display(Name = "Inner Diameter 3 (mm)")]
        public decimal? InnerDiameter3 { get; set; }

        [Display(Name = "Inner Diameter 4 (mm)")]
        public decimal? InnerDiameter4 { get; set; }

        [Display(Name = "Inner Diameter 5 (mm)")]
        public decimal? InnerDiameter5 { get; set; }

        // Inner Thickness Readings
        [Display(Name = "Inner Thickness Standard")]
        public string InnerThicknessStandard { get; set; } = string.Empty;

        [Display(Name = "Inner Thickness 1 (mm)")]
        public decimal? InnerThickness1 { get; set; }

        [Display(Name = "Inner Thickness 2 (mm)")]
        public decimal? InnerThickness2 { get; set; }

        [Display(Name = "Inner Thickness 3 (mm)")]
        public decimal? InnerThickness3 { get; set; }

        [Display(Name = "Inner Thickness 4 (mm)")]
        public decimal? InnerThickness4 { get; set; }

        [Display(Name = "Inner Thickness 5 (mm)")]
        public decimal? InnerThickness5 { get; set; }

        // Total Thickness Readings
        [Display(Name = "Total Thickness Standard")]
        public string TotalThicknessStandard { get; set; } = string.Empty;

        [Display(Name = "Total Thickness 1 (mm)")]
        public decimal? TotalThickness1 { get; set; }

        [Display(Name = "Total Thickness 2 (mm)")]
        public decimal? TotalThickness2 { get; set; }

        [Display(Name = "Total Thickness 3 (mm)")]
        public decimal? TotalThickness3 { get; set; }

        [Display(Name = "Total Thickness 4 (mm)")]
        public decimal? TotalThickness4 { get; set; }

        [Display(Name = "Total Thickness 5 (mm)")]
        public decimal? TotalThickness5 { get; set; }

        // Spiral Pitch
        [Display(Name = "Spiral Pitch Standard")]
        public string SpiralPitchStandard { get; set; } = string.Empty;

        [Display(Name = "Spiral Pitch Actual")]
        public decimal? SpiralPitchActual { get; set; }

        // Visual Check
        [Display(Name = "Visual Check Standard")]
        public string VisualCheckStandard { get; set; } = "No Burry, No sire, No Bubble, No Scratch";

        [Display(Name = "Visual Check OK")]
        public bool VisualCheckOK { get; set; }

        [Display(Name = "Visual Check Notes")]
        public string? VisualCheckNotes { get; set; }

        // Part Number & VIN Code
        [Display(Name = "Part Number")]
        public string? PartNumber { get; set; }

        [Display(Name = "VIN Code")]
        public string? VinCode { get; set; }

        // Quantity Tracking
        [Display(Name = "Qty Std Panung")]
        public int? QtyStdPanung { get; set; }

        [Display(Name = "Qty Act Panung")]
        public int? QtyActPanung { get; set; }

        [Display(Name = "Qty Target")]
        public int? QtyTarget { get; set; }

        [Display(Name = "Qty OK")]
        public int? QtyOK { get; set; }

        [Display(Name = "Qty NG Dimensi")]
        public int? QtyNGDimensi { get; set; }

        [Display(Name = "Qty NG Visual")]
        public int? QtyNGVisual { get; set; }

        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [Display(Name = "Dicatat Oleh")]
        public string RecordedBy { get; set; } = string.Empty;
    }
}
