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

        [Display(Name = "Shift")]
        public string? Shift { get; set; }

        [Display(Name = "Customer")]
        public string? CustomerName { get; set; }

        [Display(Name = "Tipe Hose")]
        public string? HoseType { get; set; } 

        [Display(Name = "Dimensi")]
        public string? Dimension { get; set; } 

        [Display(Name = "Parameter Setting ID")]
        public int? StandardParameterSettingId { get; set; }
        public StandardParameterSetting? StandardParameterSetting { get; set; }

        // IMAGE 1: NOW I'M PRODUCE (Keeping legacy names to avoid breakage)
        [Display(Name = "Material Inner")]
        public string? InnerMaterial { get; set; } 

        [Display(Name = "No. Lot Inner")]
        public string? InnerMaterialLotNo { get; set; } 

        [Display(Name = "SG Inner")]
        public decimal? InnerMaterialSG { get; set; } 

        [Display(Name = "Material Outer")]
        public string? OuterMaterial { get; set; } 

        [Display(Name = "No. Lot Outer")]
        public string? OuterMaterialLotNo { get; set; } 

        [Display(Name = "SG Outer")]
        public decimal? OuterMaterialSG { get; set; } 

        [Display(Name = "Yarn")]
        public string? Yarn { get; set; }

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
        public string? InnerMaterialActual { get; set; } 
        public string? OuterMaterialActual { get; set; } 
        public string? YarnActual { get; set; } 

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

        // WASTE TRACKING (IMAGE 5)
        public decimal? WasteWeightAwal { get; set; }
        public decimal? WasteWeightAkhir { get; set; }
        public decimal? WasteInnerAwal { get; set; }
        public decimal? WasteInnerAkhir { get; set; }
        public decimal? WasteCoverAwal { get; set; }
        public decimal? WasteCoverAkhir { get; set; }

        // Status
        [Display(Name = "Status")]
        public string Status { get; set; } = "NOW PRODUCING"; 

        [Display(Name = "Dibuat Oleh")]
        public string? CreatedBy { get; set; } 

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
