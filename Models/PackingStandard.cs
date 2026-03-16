using System.ComponentModel.DataAnnotations;

namespace VelastoProductionSystem.Models
{
    public class PackingStandard
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "NA Code")]
        public string NACode { get; set; } = string.Empty;

        [Display(Name = "Nama Material")]
        public string MaterialName { get; set; } = string.Empty;

        [Display(Name = "Part Number")]
        public string PartNumber { get; set; } = string.Empty;

        [Display(Name = "Vin Code")]
        public string VinCode { get; set; } = string.Empty;

        [Display(Name = "Dandori (CHS)")]
        public int Dandori { get; set; }

        [Display(Name = "DH*")]
        public int DH { get; set; }

        [Display(Name = "Std. Qty")]
        public int StdQty { get; set; }

        [Display(Name = "Actual Qty")]
        public int? ActualQty { get; set; }

        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [Display(Name = "Tanggal Efektif")]
        public DateTime EffectiveDate { get; set; }

        [Display(Name = "Status Aktif")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Dibuat Oleh")]
        public string CreatedBy { get; set; } = string.Empty;

        [Display(Name = "Tanggal Dibuat")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
