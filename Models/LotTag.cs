using System.ComponentModel.DataAnnotations;

namespace VelastoProductionSystem.Models
{
    public class LotTag
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Lot Tag Number")]
        public string LotTagNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Plant")]
        public string Plant { get; set; } = "2504 PT Velasto Mfg Fac 4 - Tango";

        [Required]
        [Display(Name = "Part Number")]
        public string PartNumber { get; set; } = string.Empty;

        [Display(Name = "Part Description")]
        public string PartDescription { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Target Qty (PCS)")]
        public int TargetQty { get; set; }

        [Display(Name = "Actual Qty (PCS)")]
        public int? ActualQty { get; set; }

        [Required]
        [Display(Name = "Lot Packaging (PCS)")]
        public int LotPackaging { get; set; }

        // Compound Information
        [Display(Name = "Compound Code")]
        public string? CompoundCode { get; set; }

        [Display(Name = "Bom Text")]
        public string? BomText { get; set; }

        [Display(Name = "Compound Qty (KG)")]
        public decimal? CompoundQty { get; set; }

        [Display(Name = "No. Lot")]
        public string? NoLot { get; set; }

        // Component List
        [Display(Name = "Daftar Komponen")]
        public string? DaftarKomponen { get; set; }

        // Quality Control
        [Display(Name = "Subcon Check")]
        public string? SubconCheck { get; set; }

        [Display(Name = "Mesin Check")]
        public string? MesinCheck { get; set; }

        [Display(Name = "Tanggal Check")]
        public DateTime? TanggalCheck { get; set; }

        [Display(Name = "Qty OK")]
        public int? QtyOK { get; set; }

        [Display(Name = "Qty NG")]
        public int? QtyNG { get; set; }

        [Display(Name = "NG Reason")]
        public string? NGReason { get; set; }

        // Barcode
        [Display(Name = "Barcode")]
        public string? Barcode { get; set; }

        // Print Information
        [Display(Name = "Printed Date")]
        public DateTime? PrintedDate { get; set; }

        [Display(Name = "Last Printed Date")]
        public DateTime? LastPrintedDate { get; set; }

        [Display(Name = "Print Count")]
        public int PrintCount { get; set; }

        // Status
        [Display(Name = "Status")]
        public string Status { get; set; } = "Created"; // Created, InProduction, Completed, Shipped

        [Display(Name = "Dibuat Oleh")]
        public string CreatedBy { get; set; } = string.Empty;

        [Display(Name = "Tanggal Dibuat")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Related Production Report
        [Display(Name = "Production Report ID")]
        public int? ProductionReportId { get; set; }
        public ProductionReport? ProductionReport { get; set; }
    }
}
