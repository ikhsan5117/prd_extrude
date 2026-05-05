namespace VelastoProductionSystem.Models
{
    // ─────────────────────────────────────────────────────────────────────────────
    //  REQUEST DTOs  (Payload yang dikirim sensor ke API)
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Payload tunggal dari sensor device. Kirim satu pengukuran per request.
    /// POST /api/sensor-readings/ingest
    /// Header: X-Api-Key: {key}
    /// </summary>
    public class SensorPayloadDto
    {
        /// <summary>ID unik device (e.g., "SENSOR-OD-EXT01", "LASER-EXT02")</summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>Kode mesin (e.g., "EXT-01", "EXT-02"). Digunakan untuk auto-link ke laporan aktif.</summary>
        public string? MachineCode { get; set; }

        /// <summary>ID ProductionReport spesifik. Jika null, sistem cari otomatis dari MachineCode.</summary>
        public int? ReportId { get; set; }

        /// <summary>
        /// Timestamp sensor. Jika null, server pakai waktu penerimaan.
        /// Format ISO 8601: "2026-04-22T10:30:15"
        /// </summary>
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// Tipe metrik. Nilai yang didukung:
        /// outer_diameter, inner_diameter, hose_speed,
        /// head_temp_inner, head_temp_outer, chiller_water_temp,
        /// screw_speed_inner, screw_speed_outer,
        /// pressure_inner, pressure_outer,
        /// spiral_pitch, caterpillar_gap
        /// </summary>
        public string MetricType { get; set; } = string.Empty;

        /// <summary>Nilai pengukuran numerik</summary>
        public decimal MetricValue { get; set; }

        /// <summary>Satuan (opsional): mm, °C, rpm, MPa, m/min</summary>
        public string? Unit { get; set; }

        /// <summary>Kualitas sinyal sensor (opsional): OK, WARN, ERROR</summary>
        public string? Quality { get; set; }
    }

    /// <summary>
    /// Payload batch: kirim banyak metrik dalam satu request untuk efisiensi.
    /// POST /api/sensor-readings/ingest/batch
    /// Header: X-Api-Key: {key}
    /// </summary>
    public class SensorBatchPayloadDto
    {
        public string DeviceId { get; set; } = string.Empty;
        public string? MachineCode { get; set; }
        public int? ReportId { get; set; }
        public List<SensorMetricItem> Metrics { get; set; } = new();
    }

    public class SensorMetricItem
    {
        public DateTime? Timestamp { get; set; }
        public string MetricType { get; set; } = string.Empty;
        public decimal MetricValue { get; set; }
        public string? Unit { get; set; }
        public string? Quality { get; set; }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    //  RESPONSE DTOs  (Data yang dikirim ke Dashboard / Client)
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>Live data reading untuk ditampilkan di dashboard real-time</summary>
    public class SensorLiveDataDto
    {
        public int Id { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public string MachineCode { get; set; } = string.Empty;
        public string MetricType { get; set; } = string.Empty;
        public decimal MetricValue { get; set; }
        public string? Unit { get; set; }
        public string Quality { get; set; } = "OK";
        public string Timestamp { get; set; } = string.Empty;
        public string TimestampFull { get; set; } = string.Empty;
        public string IngestedAt { get; set; } = string.Empty;
        public string Status { get; set; } = "OK";
    }

    /// <summary>Status per device sensor (online/offline, last value)</summary>
    public class DeviceStatusDto
    {
        public string DeviceId { get; set; } = string.Empty;
        public string MachineCode { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public string LastSeen { get; set; } = string.Empty;
        public int LastSeenSeconds { get; set; }
        public int TotalToday { get; set; }
        public string LastMetric { get; set; } = string.Empty;
        public decimal LastValue { get; set; }
        public string? LastUnit { get; set; }
    }

    /// <summary>Ringkasan statistik per tipe metrik dalam rentang waktu tertentu</summary>
    public class MetricSummaryDto
    {
        public string MetricType { get; set; } = string.Empty;
        public decimal Latest { get; set; }
        public string? Unit { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public decimal Avg { get; set; }
        public int Count { get; set; }
    }
}
