using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VelastoProductionSystem.Models
{
    /// <summary>
    /// Read-only mapping ke tabel tb_elwp_produksi_plannings di database ELWP_PRD.
    /// Schema: produksi
    /// </summary>
    [Table("tb_elwp_produksi_plannings", Schema = "produksi")]
    public class ElwpPlanning
    {
        [Key]
        public int Id { get; set; }

        public int? PlantId { get; set; }
        public int? AreaId { get; set; }

        /// <summary>
        /// ID Mesin di sistem ELWP (46 = DL01, 47 = DL02, dst)
        /// </summary>
        public int? MesinId { get; set; }

        public DateTime? TanggalPlanning { get; set; }

        /// <summary>
        /// Part Number / SAP Number
        /// </summary>
        public string? PnSap { get; set; }

        /// <summary>
        /// Kode item (NA1640, NA1610, dst)
        /// </summary>
        public string? KodeItem { get; set; }

        /// <summary>
        /// Nama part HOSE FUEL TANK TO FILLER dll
        /// </summary>
        public string? PartName { get; set; }

        public int? QtyPlanning { get; set; }

        /// <summary>
        /// Shift (Shift 1, Shift 2, Shift 3)
        /// </summary>
        public string? Shift { get; set; }

        public decimal? LoadingTimeHours { get; set; }
    }
}
