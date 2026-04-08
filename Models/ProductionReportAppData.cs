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
        public string? StandardLength { get; set; }
        public string? ActualLength { get; set; }
        public int? QtyTarget { get; set; }
        public int? QtyOk { get; set; }
        public int? NgDimension { get; set; }
        public int? NgVisual { get; set; }
        public string? Remark { get; set; }
        public string? ByPass { get; set; }
        public string? Shift { get; set; }
        public int RevisionNumber { get; set; }
        
        // Dimension readings array
        public List<DimensionReadingData> DimensionReadings { get; set; } = new List<DimensionReadingData>();
    }

    public class DimensionReadingData
    {
        public string? PointName { get; set; }
        public string? TimeSection { get; set; }
        public string? Frequency { get; set; }
        public string? StandardDimension { get; set; }
        public string? Initial { get; set; }
        public string? Reading1 { get; set; }
        public string? Reading2 { get; set; }
        public string? Reading3 { get; set; }
        public string? Reading4 { get; set; }
        public string? Reading5 { get; set; }
        public string? Status { get; set; }
        public decimal? ScaleValue { get; set; }
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

    public class ProductionReportSaveDto
    {
        public string? DocumentNumber { get; set; }
        public int RevisionNumber { get; set; }
        public string? ProductionDate { get; set; }
        public string? Shift { get; set; }
        public string? CustomerName { get; set; }
        public string? HoseType { get; set; }
        public string? InnerMaterial { get; set; }
        public string? InnerMaterialActual { get; set; }
        public string? InnerMaterialLotNo { get; set; }
        public string? OuterMaterial { get; set; }
        public string? OuterMaterialActual { get; set; }
        public string? OuterMaterialLotNo { get; set; }
        public string? Yarn { get; set; }
        public string? YarnActual { get; set; }
        public string? YarnLotNo { get; set; }
        public string? DAI_Awal { get; set; }
        public string? DAI_Akhir { get; set; }
        public string? DAC_Awal { get; set; }
        public string? DAC_Akhir { get; set; }
        public string? DRAI_Awal { get; set; }
        public string? DRAI_Akhir { get; set; }
        public string? DRAC_Awal { get; set; }
        public string? DRAC_Akhir { get; set; }
        public string? CreatedBy { get; set; }
        public List<ProductionReadingSaveDto> Readings { get; set; } = new List<ProductionReadingSaveDto>();
    }

    public class ProductionReadingSaveDto
    {
        public string? ReadingTime { get; set; }
        public string? RecordedBy { get; set; }
        public string? HeadTempInner { get; set; }
        public string? Cylinder1TempInner { get; set; }
        public string? Cylinder2TempInner { get; set; }
        public string? Cylinder3TempInner { get; set; }
        public string? ScrewTempInner { get; set; }
        public string? ScrewSpeedInner { get; set; }
        public string? FeedRollRatioInner { get; set; }
        public string? PressureInner { get; set; }
        public string? HeadTempOuter { get; set; }
        public string? Cylinder1TempOuter { get; set; }
        public string? Cylinder2TempOuter { get; set; }
        public string? Cylinder3TempOuter { get; set; }
        public string? ScrewTempOuter { get; set; }
        public string? ScrewSpeedOuter { get; set; }
        public string? FeedRollRatioOuter { get; set; }
        public string? PressureOuter { get; set; }
        public string? SpiralSpeed { get; set; }
        public string? SpiralPitchSetting { get; set; }
        public string? SpiralPitchDisplay { get; set; }
        public string? PresetValue { get; set; }
        public string? ControlValue { get; set; }
        public string? HoseSpeed { get; set; }
        public string? TakeupConveyorSpeed { get; set; }
        public string? CoolConveyorSpeed { get; set; }
        public string? ConveyorRatio { get; set; }
        public string? UnsmoothSurface { get; set; }
        public string? ChillerWaterTemp { get; set; }
        public string? CaterpillarGap { get; set; }
    }
}
