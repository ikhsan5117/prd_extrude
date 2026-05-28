using System.ComponentModel.DataAnnotations;

namespace VelastoProductionSystem.Models
{
    public class NowProducing
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tanggal Produksi")]
        public DateTime ProductionDate { get; set; }

        [Required]
        [Display(Name = "Tipe Hose")]
        public string HoseType { get; set; } = string.Empty;

        [Display(Name = "Class")]
        public string? Class { get; set; }

        [Required]
        [Display(Name = "Dimensi")]
        public string Dimension { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Yarn")]
        public string Yarn { get; set; } = string.Empty;

        // Material Inner
        [Display(Name = "Material Inner")]
        public string MaterialInner { get; set; } = string.Empty;

        [Display(Name = "No. Lot Inner")]
        public string MaterialInnerLotNo { get; set; } = string.Empty;

        [Display(Name = "SG Inner")]
        public decimal MaterialInnerSG { get; set; }

        // Material Middle (optional)
        [Display(Name = "Material Middle")]
        public string? MaterialMiddle { get; set; }

        [Display(Name = "No. Lot Middle")]
        public string? MaterialMiddleLotNo { get; set; }

        [Display(Name = "SG Middle")]
        public decimal? MaterialMiddleSG { get; set; }

        // Material Outer
        [Display(Name = "Material Outer")]
        public string MaterialOuter { get; set; } = string.Empty;

        [Display(Name = "No. Lot Outer")]
        public string MaterialOuterLotNo { get; set; } = string.Empty;

        [Display(Name = "SG Outer")]
        public decimal MaterialOuterSG { get; set; }

        // Dandori (Setup) Info
        [Display(Name = "Waktu Dandori - Start Prod")]
        public DateTime? DandoriStartProdTime { get; set; }

        [Display(Name = "Waktu Dandori - Akhir Prod")]
        public DateTime? DandoriEndProdTime { get; set; }

        // Production Time Tracking
        [Display(Name = "Waktu Produksi - Awal")]
        public DateTime? ProductionStartTime { get; set; }

        [Display(Name = "Waktu Produksi - Akhir")]
        public DateTime? ProductionEndTime { get; set; }

        // CHS 2 Layer Dandori Target
        [Display(Name = "CHS 2L Target Waktu (menit)")]
        public int? CHS2LTargetWaktu { get; set; }

        [Display(Name = "CHS 2L Target DH*")]
        public string? CHS2LTargetDH { get; set; }

        [Display(Name = "CHS 2L Target Material")]
        public string? CHS2LTargetMaterial { get; set; }

        [Display(Name = "CHS 2L Target BENANG")]
        public string? CHS2LTargetBenang { get; set; }

        // CHS 3 Layer Dandori Target
        [Display(Name = "CHS 3L Target Waktu (menit)")]
        public int? CHS3LTargetWaktu { get; set; }

        [Display(Name = "CHS 3L Target DH*")]
        public string? CHS3LTargetDH { get; set; }

        [Display(Name = "CHS 3L Target Material")]
        public string? CHS3LTargetMaterial { get; set; }

        [Display(Name = "CHS 3L Target BENANG")]
        public string? CHS3LTargetBenang { get; set; }

        // SPV Check
        [Display(Name = "SPV Check")]
        public bool SPVCheck { get; set; }

        [Display(Name = "SPV Check By")]
        public string? SPVCheckBy { get; set; }

        [Display(Name = "SPV Check Time")]
        public DateTime? SPVCheckTime { get; set; }

        // Status
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active"; // Active, Completed, Paused

        [Display(Name = "Dibuat Oleh")]
        public string CreatedBy { get; set; } = string.Empty;

        [Display(Name = "Tanggal Dibuat")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Catatan")]
        public string? Notes { get; set; }
    }
}
