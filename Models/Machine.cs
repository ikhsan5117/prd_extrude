using System.ComponentModel.DataAnnotations;

namespace VelastoProductionSystem.Models
{
    public class Machine
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Kode Mesin")]
        public string MachineCode { get; set; } = "";

        [Required]
        [Display(Name = "Nama Mesin")]
        public string MachineName { get; set; } = "";

        [Display(Name = "Line / Area")]
        public string? Line { get; set; }

        [Display(Name = "Tipe Mesin")]
        public string? MachineType { get; set; }

        [Display(Name = "Merk / Brand")]
        public string? Brand { get; set; }

        [Display(Name = "Tahun Produksi")]
        public string? YearMade { get; set; }

        [Display(Name = "Serial Number")]
        public string? SerialNumber { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = "AKTIF";   // AKTIF / MAINTENANCE / NON-AKTIF

        [Display(Name = "Kapasitas (pcs/jam)")]
        public int? CapacityPerHour { get; set; }

        [Display(Name = "Tanggal Terakhir PM")]
        public DateTime? LastMaintenanceDate { get; set; }

        [Display(Name = "Tanggal PM Berikutnya")]
        public DateTime? NextMaintenanceDate { get; set; }

        [Display(Name = "Catatan")]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
