namespace VelastoProductionSystem.Models
{
    public class DimensionReportAppData
    {
        public int ReportId { get; set; }
        public string? DocumentNumber { get; set; }
        public string? HoseType { get; set; }
        public string? DimensionDisplay { get; set; }
        public string? CustomerName { get; set; }
        public string? Yarn { get; set; }
        public string? VinCode { get; set; }
        public string? ActualLength { get; set; }
        public int QtyTarget { get; set; }
        public int QtyOk { get; set; }
        public int NgDimension { get; set; }
        public int NgVisual { get; set; }
        public string? Remark { get; set; }
        public string? ByPass { get; set; }
        
        // Dimension readings array
        public List<DimensionReadingData> DimensionReadings { get; set; } = new List<DimensionReadingData>();
    }

    public class DimensionReadingData
    {
        public string PointName { get; set; } = "";
        public string TimeSection { get; set; } = "";
        public string Initial { get; set; } = "";
        public string Reading1 { get; set; } = "";
        public string Reading2 { get; set; } = "";
        public string Reading3 { get; set; } = "";
        public string Reading4 { get; set; } = "";
        public string Reading5 { get; set; } = "";
    }

    public class ExtruderProducePayload
    {
        public int Id { get; set; }
        public string? HoseType { get; set; }
        public string? Dimension { get; set; }
        public string? Yarn { get; set; }
        public string? MaterialInner { get; set; }
        public string? MaterialInnerLotNo { get; set; }
        public decimal? MaterialInnerSG { get; set; }
        public string? MaterialMiddle { get; set; }
        public string? MaterialMiddleLotNo { get; set; }
        public decimal? MaterialMiddleSG { get; set; }
        public string? MaterialOuter { get; set; }
        public string? MaterialOuterLotNo { get; set; }
        public decimal? MaterialOuterSG { get; set; }
        public DateTime? DandoriStart { get; set; }
        public DateTime? DandoriEnd { get; set; }
        public DateTime? ProductionStart { get; set; }
        public bool SPVCheck { get; set; }
    }
}
