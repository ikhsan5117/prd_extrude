using System.ComponentModel.DataAnnotations;

namespace VelastoProductionSystem.Models
{
    public class ProductionReport
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "No. Dokumen")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Display(Name = "No. Revisi")]
        public int RevisionNumber { get; set; }

        [Required]
        [Display(Name = "Tanggal Produksi")]
        public DateTime ProductionDate { get; set; }

        [Required]
        [Display(Name = "Shift")]
        public string Shift { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Customer")]
        public string CustomerName { get; set; } = string.Empty;

        [Display(Name = "Tipe Hose")]
        public string HoseType { get; set; } = string.Empty;

        [Display(Name = "Dimensi")]
        public string Dimension { get; set; } = string.Empty;

        [Display(Name = "Parameter Setting ID")]
        public int? StandardParameterSettingId { get; set; }
        public StandardParameterSetting? StandardParameterSetting { get; set; }

        // IMAGE 1: NOW I'M PRODUCE (Keeping legacy names to avoid breakage)
        [Display(Name = "Material Inner")]
        public string InnerMaterial { get; set; } = string.Empty; // Legacy field

        [Display(Name = "No. Lot Inner")]
        public string InnerMaterialLotNo { get; set; } = string.Empty;

        [Display(Name = "SG Inner")]
        public decimal InnerMaterialSG { get; set; } // Back to decimal

        [Display(Name = "Material Outer")]
        public string OuterMaterial { get; set; } = string.Empty;

        [Display(Name = "No. Lot Outer")]
        public string OuterMaterialLotNo { get; set; } = string.Empty;

        [Display(Name = "SG Outer")]
        public decimal OuterMaterialSG { get; set; } // Back to decimal

        [Display(Name = "Yarn")]
        public string Yarn { get; set; } = string.Empty;

        // Times for Image 1
        public DateTime? DandoriStartTime { get; set; }
        public DateTime? DandoriEndTime { get; set; }
        public DateTime? ProductionStartTime { get; set; }
        public DateTime? ProductionEndTime { get; set; }

        // Die Check Results (Needed for Edit view)
        public bool NippleDieOK { get; set; }
        public bool TubeDieOK { get; set; }
        public bool MiddleDieOK { get; set; }
        public bool CoverDieOK { get; set; }
        public bool SpacerDieOK { get; set; }
        public bool ToleranceDieOK { get; set; }

        // Fields used in Edit/Dashboard
        public string InnerMaterialActual { get; set; } = string.Empty;
        public string OuterMaterialActual { get; set; } = string.Empty;
        public string YarnActual { get; set; } = string.Empty;

        // PRODUCTION EVALUATION (IMAGE 2)
        [Display(Name = "VIN Kode")]
        public string? VinCode { get; set; }

        [Display(Name = "Std. Panjang (mm)")]
        public string? StandardLength { get; set; }

        [Display(Name = "Actual Panjang (mm)")]
        public string? ActualLength { get; set; }

        [Display(Name = "Qty Target")]
        public int QtyTarget { get; set; }

        [Display(Name = "Qty OK")]
        public int QtyOk { get; set; }

        [Display(Name = "NG Dimensi")]
        public int NgDimension { get; set; }

        [Display(Name = "NG Visual")]
        public int NgVisual { get; set; }

        [Display(Name = "Remark")]
        public string? Remark { get; set; }

        // Status
        [Display(Name = "Status")]
        public string Status { get; set; } = "NOW PRODUCING"; 

        [Display(Name = "Dibuat Oleh")]
        public string CreatedBy { get; set; } = string.Empty;

        [Display(Name = "Tanggal Dibuat")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Diperiksa Oleh")]
        public string? CheckedBy { get; set; }

        [Display(Name = "Disetujui Oleh")]
        public string? ApprovedBy { get; set; }

        // Navigation Properties
        public ICollection<ProductionReading> ProductionReadings { get; set; } = new List<ProductionReading>();
    }
}
