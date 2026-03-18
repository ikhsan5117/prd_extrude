namespace VelastoProductionSystem.Models
{
    public class DimensionReportAppData
    {
        public int ReportId { get; set; }
        public string? ActualLength { get; set; }
        public int QtyOk { get; set; }
        public int NgDimension { get; set; }
        public int NgVisual { get; set; }
        public string? Remark { get; set; }
        public string? VinCode { get; set; }
        
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
}
