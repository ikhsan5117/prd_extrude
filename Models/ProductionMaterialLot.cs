using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VelastoProductionSystem.Models
{
    public class ProductionMaterialLot
    {
        public int Id { get; set; }
        
        public int ProductionReportId { get; set; }
        
        [ForeignKey("ProductionReportId")]
        public ProductionReport? ProductionReport { get; set; }

        // INNER, MIDDLE, OUTER, YARN
        [Display(Name = "Lapisan / Tipe")]
        public string? LayerType { get; set; } 
        
        [Display(Name = "Material Standard")]
        public string? MaterialName { get; set; } 
        
        [Display(Name = "Material Actual")]
        public string? MaterialActual { get; set; } 
        
        [Display(Name = "No. Lot")]
        public string? LotNumber { get; set; } 
        
        [Display(Name = "SG (Specific Gravity)")]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal? SGValue { get; set; } 
    }
}
