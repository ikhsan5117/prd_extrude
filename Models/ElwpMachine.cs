using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VelastoProductionSystem.Models
{
    [Table("tb_elwp_produksi_mesins", Schema = "produksi")]
    public class ElwpMachine
    {
        [Key]
        public int Id { get; set; }
        public string? KodeMesin { get; set; }
        public string? NamaMesin { get; set; }
    }
}
