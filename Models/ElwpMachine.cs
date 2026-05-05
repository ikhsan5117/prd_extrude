using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VelastoProductionSystem.Models
{
    [Table("tb_elwp_produksi_mesins", Schema = "produksi")]
    public class ElwpMachine
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? KodeMesin { get; set; }

        [Required]
        [StringLength(200)]
        public string? NamaMesin { get; set; }

        public int PlantId { get; set; }
        public int AreaId { get; set; }
        public string? Keterangan { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public decimal? Kapasitas { get; set; }
        public int? RequiredManPower { get; set; }

        // Navigation property
        [ForeignKey("AreaId")]
        public virtual ElwpArea? Area { get; set; }
    }
}
