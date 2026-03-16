using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Data
{
    public static class DataSeeder
    {
        public static void SeedData(ApplicationDbContext context)
        {
            // Seed Standard Parameter Settings
            if (!context.StandardParameterSettings.Any())
            {
                var paramSettings = new List<StandardParameterSetting>
                {
                    new StandardParameterSetting
                    {
                        DocumentNumber = "VH-STD-001",
                        RevisionNumber = 2,
                        CustomerName = "Toyota Motor Corporation",
                        LayerType = "CHS 2 Layer",
                        EffectiveDate = new DateTime(2026, 1, 1),
                        HoseType = "Fuel Hose",
                        Diameter = "8mm",
                        ProductCode = "FH-8-2L",
                        InnerMaterial = "NBR",
                        OuterMaterial = "CR",
                        YarnType = "Polyester",
                        InnerDie = 8.5m,
                        OuterDie = 12.3m,
                        TubeDie = 10.2m,
                        MiddleDie = 9.8m,
                        CoverDie = 11.5m,
                        SpacerDie = 0.5m,
                        ToleranceDie = 0.2m,
                        MeshScreen = "60 Mesh",
                        HeadTemp = 180,
                        Cylinder1Temp = 175,
                        Cylinder2Temp = 170,
                        Cylinder3Temp = 165,
                        ScrewTemp = 160,
                        ScrewSpeed = 45.5m,
                        FeedRollRatio = 85.0m,
                        Pressure = 12.5m,
                        AirPressureA = 0.5m,
                        PresetValve = 3.2m,
                        SpiralSpeed = 12.5m,
                        SpiralPitch = 8.0m,
                        SpiralSpeedDisplay = 12.3m,
                        SpiralPitchDisplay = 7.9m,
                        PresetTemp = 25.0m,
                        ControlValue = "Ø 8.0±0.2mm",
                        HoseSpeed = "3.5-4.0",
                        TakeupConveyorSpeed = "3.8",
                        CoolConveyorSpeed = "4.0",
                        ConveyorRatio = "1:1.05",
                        UnsmoothSurface = 0.15m,
                        ChillerWaterTemp = "15-18°C"
                    }
                };
                context.StandardParameterSettings.AddRange(paramSettings);
                context.SaveChanges();
            }

            // Seed Packing Standards
            if (!context.PackingStandards.Any())
            {
                var packingStandards = new List<PackingStandard>
                {
                    new PackingStandard
                    {
                        NACode = "NA-001",
                        MaterialName = "Fuel Hose 8mm",
                        PartNumber = "90445-12073",
                        VinCode = "VIN-FH8",
                        Dandori = 150,
                        DH = 200,
                        StdQty = 100,
                        ActualQty = 100,
                        EffectiveDate = new DateTime(2026, 1, 1),
                        IsActive = true,
                        Remarks = "Standard quantity per box"
                    }
                };
                context.PackingStandards.AddRange(packingStandards);
                context.SaveChanges();
            }

            // Seed Now Producing
            if (!context.NowProducings.Any())
            {
                var nowProducing = new NowProducing
                {
                    ProductionDate = DateTime.Now.Date,
                    HoseType = "Fuel Hose",
                    Class = "A",
                    Dimension = "8mm x 2.5mm",
                    Yarn = "Polyester 1500D",
                    MaterialInner = "NBR Grade A",
                    MaterialInnerLotNo = "NBR20260310-001",
                    MaterialInnerSG = 1.18m,
                    MaterialOuter = "CR Grade Premium",
                    MaterialOuterLotNo = "CR20260310-001",
                    MaterialOuterSG = 1.22m,
                    DandoriStartProdTime = DateTime.Now.AddHours(-2),
                    DandoriEndProdTime = DateTime.Now.AddHours(-1),
                    ProductionStartTime = DateTime.Now.AddHours(-1),
                    Status = "In Progress",
                    CreatedBy = "Operator 1",
                    CreatedDate = DateTime.Now
                };
                context.NowProducings.Add(nowProducing);
                context.SaveChanges();
            }

            // Seed Production Reports
            if (!context.ProductionReports.Any())
            {
                var productionReport = new ProductionReport
                {
                    DocumentNumber = "PR-20260310-001",
                    RevisionNumber = 0,
                    ProductionDate = DateTime.Now.AddDays(-1),
                    Shift = "Shift 1",
                    CustomerName = "Toyota Motor Corporation",
                    HoseType = "Fuel Hose",
                    Dimension = "8mm x 2.5mm",
                    InnerMaterial = "NBR Grade A",
                    InnerMaterialLotNo = "NBR20260309-001",
                    InnerMaterialSG = 1.18m,
                    OuterMaterial = "CR Grade Premium",
                    OuterMaterialLotNo = "CR20260309-001",
                    OuterMaterialSG = 1.22m,
                    Yarn = "Polyester 1500D",
                    Status = "Completed",
                    CreatedBy = "Supervisor A",
                    CreatedDate = DateTime.Now.AddDays(-1)
                };
                context.ProductionReports.Add(productionReport);
                context.SaveChanges();
            }

            // Seed Lot Tags
            if (!context.LotTags.Any())
            {
                var lotTag = new LotTag
                {
                    LotTagNumber = "VH" + DateTime.Now.ToString("yyMMddHHmmss"),
                    NoLot = "LOT-001-20260309",
                    PartNumber = "90445-12073",
                    PartDescription = "HOSE, FUEL 8MM",
                    BomText = "8mm Fuel Hose - NBR/CR",
                    CompoundCode = "NBR-CR-001",
                    CompoundQty = 250,
                    DaftarKomponen = "Inner: NBR, Outer: CR, Yarn: Polyester",
                    LotPackaging = 100,
                    Plant = "2504 PT Velasto Mfg Fac 4 - Tango",
                    TargetQty = 500,
                    QtyOK = 485,
                    QtyNG = 15,
                    ActualQty = 485,
                    Status = "Completed",
                    Barcode = "90445-12073",
                    CreatedBy = "QC Team",
                    CreatedDate = DateTime.Now.AddDays(-1),
                    PrintedDate = DateTime.Now.AddDays(-1),
                    PrintCount = 1
                };
                context.LotTags.Add(lotTag);
                context.SaveChanges();
            }
        }
    }
}
