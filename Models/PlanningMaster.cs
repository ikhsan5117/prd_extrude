using System.ComponentModel.DataAnnotations;

namespace VelastoProductionSystem.Models
{
    public class PlanningMaster
    {
        public int Id { get; set; }
        
        [Display(Name = "Machine")]
        public string? MachineName { get; set; }
        
        [Display(Name = "Tanggal & Shift")]
        public string? DateShiftString { get; set; } 
        
        [Display(Name = "Part Name 1 (Activity)")]
        public string? PartName1 { get; set; } 
        
        [Display(Name = "Part Name 2")]
        public string? PartName2 { get; set; }
        
        [Display(Name = "Compound")]
        public string? Compound { get; set; }

        // New fields to match DL0
        public string? CompoundInner { get; set; }
        public string? CompoundMiddle { get; set; }
        public string? CompoundOuter { get; set; }
        public string? CompoundCombo { get; set; }
        public string? Length { get; set; }
        
        [Display(Name = "Kode")]
        public string? Kode { get; set; }

        public string? CtAwal { get; set; }
        public string? CtMinus20 { get; set; }
        public string? NeedKgInner { get; set; }
        public string? NeedKgMiddle { get; set; }
        public string? NeedKgOuter { get; set; }
        
        [Display(Name = "Target (Pcs)")]
        public int? PlanTargetPcs { get; set; }
        
        [Display(Name = "Durasi (Menit)")]
        public string? Menit { get; set; }
        
        [Display(Name = "Waktu Mulai")]
        public string? WaktuMulai { get; set; } 
        
        [Display(Name = "Waktu Selesai")]
        public string? WaktuSelesai { get; set; } 
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
