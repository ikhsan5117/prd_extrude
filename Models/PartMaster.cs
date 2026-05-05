using System.ComponentModel.DataAnnotations;

namespace VelastoProductionSystem.Models
{
    /// <summary>
    /// Data Master Part dari sheet DL0 — sumber utama untuk planning.
    /// </summary>
    public class PartMaster
    {
        public int Id { get; set; }

        [Display(Name = "Part Code")]
        public string? PartCode { get; set; }

        [Display(Name = "Part Number")]
        public string? PartNumber { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Length (mm)")]
        public string? Length { get; set; }

        [Display(Name = "Diameter")]
        public string? Diameter { get; set; }

        [Display(Name = "Compound Inner")]
        public string? CompoundInner { get; set; }

        [Display(Name = "Compound Outer")]
        public string? CompoundOuter { get; set; }
        
        [Display(Name = "Compound Middle")]
        public string? CompoundMiddle { get; set; }

        [Display(Name = "Compound Combo")]
        public string? CompoundCombo { get; set; }

        [Display(Name = "Need KG Inner (kg/pcs)")]
        public decimal? NeedKgInner { get; set; }

        [Display(Name = "Need KG Outer (kg/pcs)")]
        public decimal? NeedKgOuter { get; set; }
        
        [Display(Name = "Need KG Middle (kg/pcs)")]
        public decimal? NeedKgMiddle { get; set; }

        /// <summary>
        /// Cycle Time dasar dalam DETIK per pcs (sec/pcs dari DL0 kolom 11)
        /// </summary>
        [Display(Name = "Cycle Time (sec/pcs)")]
        public decimal? SecPerPcs { get; set; }

        /// <summary>
        /// Cycle Time dikurangi 20% (efisiensi)
        /// </summary>
        [Display(Name = "CT -20%")]
        public decimal? CtMinus20 { get; set; }

        /// <summary>
        /// ct awal — nilai standar yang dipakai untuk kalkulasi planning
        /// </summary>
        [Display(Name = "CT Awal")]
        public decimal? CtAwal { get; set; }

        // Jadwal hari produksi (nilai misal: 3D, 1D, OFF, 2D)
        [Display(Name = "Senin")] public string? Senin { get; set; }
        [Display(Name = "Selasa")] public string? Selasa { get; set; }
        [Display(Name = "Rabu")] public string? Rabu { get; set; }
        [Display(Name = "Kamis")] public string? Kamis { get; set; }
        [Display(Name = "Jumat")] public string? Jumat { get; set; }
        [Display(Name = "Sabtu")] public string? Sabtu { get; set; }
        [Display(Name = "Minggu")] public string? Minggu { get; set; }

        public DateTime ImportedAt { get; set; } = DateTime.Now;
    }
}
