using System.ComponentModel.DataAnnotations;

namespace VelastoProductionSystem.Models
{
    public class DimensionMeasurement
    {
        public int Id { get; set; }

        [Required]
        public int DimensionReportId { get; set; }
        public DimensionReport? DimensionReport { get; set; }

        [Required]
        [Display(Name = "Pukul / Jam")]
        public string TimeSection { get; set; } = string.Empty; // Example: "07:00"

        [Required]
        [Display(Name = "POINT")]
        public string PointName { get; set; } = string.Empty; // Example: "Inner Diameter"

        [Display(Name = "FREKUENSI")]
        public string? Frequency { get; set; } = "30m Sekali";

        [Display(Name = "STD DIMENSI")]
        public string? StandardDimension { get; set; }

        [Display(Name = "INITIAL / SCALE")]
        public string? Initial { get; set; }

        [Display(Name = "SCALE VALUE")]
        public decimal ScaleValue { get; set; }

        // Measurement Values 1-5
        [Display(Name = "1")]
        public decimal? R1 { get; set; }

        [Display(Name = "2")]
        public decimal? R2 { get; set; }

        [Display(Name = "3")]
        public decimal? R3 { get; set; }

        [Display(Name = "4")]
        public decimal? R4 { get; set; }

        [Display(Name = "5")]
        public decimal? R5 { get; set; }

        [Display(Name = "STATUS")]
        public string? Status { get; set; } // OK, NG

        public DateTime RecordedTime { get; set; } = DateTime.Now;
    }
}
