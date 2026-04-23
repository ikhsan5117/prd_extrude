using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VelastoProductionSystem.Models
{
    /// <summary>
    /// Audit log setiap data yang diterima dari sensor device via Web API.
    /// Menyimpan raw data sebelum diproses/ditampilkan di dashboard.
    /// </summary>
    [Table("SensorIngestLogs")]
    public class SensorIngestLog
    {
        public int Id { get; set; }

        /// <summary>Identifikasi unik device sensor (e.g., "SENSOR-OD-EXT01")</summary>
        [Required]
        [MaxLength(100)]
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>Kode mesin yang dipantau (e.g., "EXT-01")</summary>
        [MaxLength(50)]
        public string MachineCode { get; set; } = string.Empty;

        /// <summary>Link ke ProductionReport aktif (opsional, diisi otomatis dari MachineCode)</summary>
        public int? ProductionReportId { get; set; }
        public ProductionReport? ProductionReport { get; set; }

        /// <summary>Timestamp dari sensor device saat data diambil</summary>
        public DateTime SensorTimestamp { get; set; }

        /// <summary>
        /// Tipe metrik yang dikirim. Contoh:
        /// outer_diameter, inner_diameter, hose_speed, head_temp_inner,
        /// head_temp_outer, chiller_water_temp, screw_speed_inner,
        /// screw_speed_outer, pressure_inner, pressure_outer,
        /// spiral_pitch, caterpillar_gap
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string MetricType { get; set; } = string.Empty;

        /// <summary>Nilai pengukuran dari sensor</summary>
        [Column(TypeName = "decimal(18,6)")]
        public decimal MetricValue { get; set; }

        /// <summary>Satuan nilai (e.g., "mm", "°C", "rpm", "MPa", "m/min")</summary>
        [MaxLength(20)]
        public string? Unit { get; set; }

        /// <summary>Kualitas signal sensor: OK, WARN, ERROR</summary>
        [MaxLength(20)]
        public string Quality { get; set; } = "OK";

        /// <summary>Status proses ingest: OK, DUPLICATE, IGNORED, ERROR</summary>
        [MaxLength(20)]
        public string Status { get; set; } = "OK";

        /// <summary>Pesan error jika Status != OK</summary>
        [MaxLength(500)]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Key idempotency untuk deduplikasi: DeviceId|yyyyMMddHHmmss|MetricType
        /// Memastikan data yang sama tidak tersimpan dua kali
        /// </summary>
        [MaxLength(200)]
        public string? IdempotencyKey { get; set; }

        /// <summary>Waktu data diterima oleh server</summary>
        public DateTime IngestedAt { get; set; } = DateTime.Now;
    }
}
