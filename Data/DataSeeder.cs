using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Data
{
    public static class DataSeeder
    {
        public static void SeedData(ApplicationDbContext context, ElwpDbContext? elwpContext = null)
        {
        // 1. SEED ELWP PRODUCTION DATABASE (elwpContext) - Read-only sync
        // Skip seeding for ELWP - use existing production data
        // if (elwpContext != null)
        // {
        //     try
        //     {
        //         if (!elwpContext.ElwpMachines.Any())
        //         {
        //             elwpContext.ElwpMachines.AddRange(new List<ElwpMachine>
        //             {
        //                 new ElwpMachine { Id = 1, KodeMesin = "EXT-01", NamaMesin = "Extruder 01" },
        //                 new ElwpMachine { Id = 2, KodeMesin = "EXT-02", NamaMesin = "Extruder 02" },
        //                 new ElwpMachine { Id = 3, KodeMesin = "EXT-03", NamaMesin = "Extruder 03" }
        //             });
        //             elwpContext.SaveChanges();
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         // Silently ignore seeding errors for ElwpContext - it's a read-only connection to production
        //         System.Diagnostics.Debug.WriteLine($"ElwpContext seeding skipped: {ex.Message}");
        //     }
        // }
            // StandardParameterSettings seeding removed (table dropped)
            // MasterlistSpsDoubleLayers seeding removed (table dropped)

            /*
            // Seed Masterlist SPS (Fallback Master) - Ensuring ITEM-001/002/003 have standards
            if (!context.MasterlistSpsDoubleLayers.Any())
            {
                var masterList = new List<MasterlistSpsDoubleLayer>
                {
                    new MasterlistSpsDoubleLayer
                    {
                        ItemList = "ITEM-001", HoseType = "Fuel Hose 8mm", Dimensi = "8.0 x 12.0", Customer = "TOYOTA", 
                        MachineCode = "EXT-01", InnerTube = "NBR", OuterCover = "CR",
                        HeadTemp1 = "180", HeadTemp2 = "175", Cylinder1_1 = "175", Cylinder1_2 = "170",
                        ScrewSpeed1 = "45", ScrewSpeed2 = "42", Pressure1 = "12", Pressure2 = "10"
                    },
                    new MasterlistSpsDoubleLayer
                    {
                        ItemList = "ITEM-002", HoseType = "Oil Hose 10mm", Dimensi = "10.0 x 15.0", Customer = "HONDA", 
                        MachineCode = "EXT-02", InnerTube = "ACM", OuterCover = "AEM",
                        HeadTemp1 = "190", HeadTemp2 = "185", Cylinder1_1 = "185", Cylinder1_2 = "180",
                        ScrewSpeed1 = "38", ScrewSpeed2 = "35", Pressure1 = "14", Pressure2 = "12"
                    },
                    new MasterlistSpsDoubleLayer
                    {
                        ItemList = "ITEM-003", HoseType = "Brake Hose 6mm", Dimensi = "6.0 x 11.0", Customer = "SUZUKI", 
                        MachineCode = "EXT-03", InnerTube = "EPDM", OuterCover = "EPDM",
                        HeadTemp1 = "170", HeadTemp2 = "165", Cylinder1_1 = "165", Cylinder1_2 = "160",
                    }
                };
                context.MasterlistSpsDoubleLayers.AddRange(masterList);
                context.SaveChanges();
            }

            // ALWAYS ENSURE TEST STANDARDS FOR 2026 ITEMS EXIST
            if (!context.MasterlistSpsDoubleLayers.Any(m => m.ItemList == "SPS-FH-2026"))
            {
                context.MasterlistSpsDoubleLayers.Add(new MasterlistSpsDoubleLayer
                {
                    ItemList = "SPS-FH-2026", HoseType = "High-Pressure Fuel Hose", Dimensi = "8.2", Customer = "TOYOTA MOTORS", 
                    MachineCode = "EXT-01", InnerTube = "NBR/PVC", OuterCover = "CSM",
                    TebalInner = "1.5", TebalTotal = "3.2", ToleranceInner = "0.2", ToleranceOuter = "0.3", ToleranceSpiralPitch = "5",
                    HeadTemp1 = "185", HeadTemp2 = "180", DocumentNumber = "DOC-HPFH-01", RevisionNumber = "2"
                });
            }
            if (!context.MasterlistSpsDoubleLayers.Any(m => m.ItemList == "BH-2026-X1"))
            {
                context.MasterlistSpsDoubleLayers.Add(new MasterlistSpsDoubleLayer
                {
                    ItemList = "BH-2026-X1", HoseType = "Premium Brake Hose V1", Dimensi = "6.5", Customer = "HONDA CARS", 
                    MachineCode = "EXT-02", InnerTube = "EPDM-HQ", OuterCover = "EPDM-HQ",
                    TebalInner = "2.0", TebalTotal = "4.5", ToleranceInner = "0.15", ToleranceOuter = "0.25", ToleranceSpiralPitch = "3",
                    HeadTemp1 = "175", HeadTemp2 = "170", DocumentNumber = "DOC-BHX1-02", RevisionNumber = "1"
                });
            }
            context.SaveChanges();
            */

            // PackingStandards seeding removed (table dropped)

            // Seed Planning Master (Local Active Planning)
            if (!context.PlanningMasters.Any())
            {
                var todayStr = DateTime.Now.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID")).ToUpper();
                var planningList = new List<PlanningMaster>
                {
                    new PlanningMaster
                    {
                        MachineName = "EXT-01", DateShiftString = $"SENIN, {todayStr} SHIFT 1",
                        PartName1 = "ITEM-001", PartName2 = "FUEL HOSE 8MM", Kode = "FH8-TYT",
                        PlanTargetPcs = 1000, WaktuMulai = "08:00", WaktuSelesai = "16:00",
                        CompoundInner = "NBR-01", CompoundOuter = "CR-01"
                    },
                    new PlanningMaster
                    {
                        MachineName = "EXT-01", DateShiftString = $"SENIN, {todayStr} SHIFT 1",
                        PartName1 = "DIE-TEST-001", PartName2 = "TEST DIES HOSE", Kode = "DIE-TEST",
                        PlanTargetPcs = 100, WaktuMulai = "08:00", WaktuSelesai = "16:00",
                        CompoundInner = "TEST-I", CompoundOuter = "TEST-O"
                    }
                };
                context.PlanningMasters.AddRange(planningList);
                context.SaveChanges();
            }

            // ALWAYS ENSURE TEST PLANNING MASTER FOR 20 APRIL 2026 EXISTS (Fallback)
            if (!context.PlanningMasters.Any(p => p.PartName1 == "SPS-FH-2026"))
            {
                context.PlanningMasters.Add(new PlanningMaster
                {
                    MachineName = "EXT-01", 
                    DateShiftString = "SENIN, 20 APRIL 2026 SHIFT 2",
                    PartName1 = "SPS-FH-2026", 
                    PartName2 = "High-Pressure Fuel Hose", 
                    Kode = "HPFH-2026",
                    PlanTargetPcs = 500, WaktuMulai = "16:00", WaktuSelesai = "00:00"
                });
            }
            if (!context.PlanningMasters.Any(p => p.PartName1 == "BH-2026-X1"))
            {
                context.PlanningMasters.Add(new PlanningMaster
                {
                    MachineName = "EXT-02", 
                    DateShiftString = "SENIN, 20 APRIL 2026 SHIFT 2",
                    PartName1 = "BH-2026-X1", 
                    PartName2 = "Premium Brake Hose V1", 
                    Kode = "BH-X1",
                    PlanTargetPcs = 1200, WaktuMulai = "16:00", WaktuSelesai = "00:00"
                });
            }
            context.SaveChanges();

            // NowProducings seeding removed (table dropped)
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

            // LotTags seeding removed (table dropped)

            // Seed Dimension Reports & Measurements for Chart Analysis
            if (!context.DimensionReports.Any(d => d.DocumentNumber == "DIM-NA2060-REF"))
            {
                var dimReport = new DimensionReport
                {
                    DocumentNumber = "DIM-NA2060-REF",
                    RevisionNumber = 1,
                    ProductionDate = DateTime.Now.AddDays(-1),
                    Shift = "Shift 1",
                    ItemCode = "ITEM-001",
                    CustomerName = "TOYOTA",
                    HoseType = "Fuel Hose 8mm",
                    MachineName = "EXT-01",
                    DimensionDisplay = "8.0 x 12.0",
                    Yarn = "Polyester 1500D",
                    VinCode = "VIN-FH8",
                    StandardLength = "1000mm",
                    ActualLength = "1000mm",
                    QtyTarget = 500,
                    QtyOk = 485,
                    NgDimension = 10,
                    NgVisual = 5,
                    Remark = "Sample dimension report for chart analysis",
                    CreatedBy = "System Seed",
                    CreatedDate = DateTime.Now.AddDays(-1)
                };

                context.DimensionReports.Add(dimReport);
                context.SaveChanges();

                // Add DimensionMeasurements with varying data for chart
                var measurements = new List<DimensionMeasurement>();
                decimal[] innerThicknessValues = { 1.45m, 1.48m, 1.50m, 1.52m, 1.49m, 1.47m, 1.51m, 1.48m, 1.46m, 1.49m };
                decimal[] innerDiameterValues = { 8.05m, 8.08m, 8.10m, 8.12m, 8.09m, 8.07m, 8.11m, 8.08m, 8.06m, 8.09m };

                for (int i = 0; i < 10; i++)
                {
                    var recordTime = DateTime.Now.AddHours(-10 + i);
                    
                    // Inner Thickness measurements
                    measurements.Add(new DimensionMeasurement
                    {
                        DimensionReportId = dimReport.Id,
                        TimeSection = recordTime.ToString("HH:mm"),
                        PointName = "Inner Tube Thickness",
                        Frequency = "30m Sekali",
                        StandardDimension = "1.50",
                        Initial = "0",
                        ScaleValue = 0.01m,
                        R1 = innerThicknessValues[i],
                        R2 = innerThicknessValues[i] + 0.01m,
                        R3 = innerThicknessValues[i] - 0.01m,
                        Status = (i % 10) < 9 ? "OK" : "OK",
                        RecordedTime = recordTime
                    });

                    // Inner Diameter measurements
                    measurements.Add(new DimensionMeasurement
                    {
                        DimensionReportId = dimReport.Id,
                        TimeSection = recordTime.ToString("HH:mm"),
                        PointName = "Inner Tube Diameter",
                        Frequency = "30m Sekali",
                        StandardDimension = "8.10",
                        Initial = "0",
                        ScaleValue = 0.01m,
                        R1 = innerDiameterValues[i],
                        R2 = innerDiameterValues[i] + 0.02m,
                        R3 = innerDiameterValues[i] - 0.02m,
                        Status = "OK",
                        RecordedTime = recordTime
                    });

                    // Outer Cover Thickness (Total Thickness)
                    measurements.Add(new DimensionMeasurement
                    {
                        DimensionReportId = dimReport.Id,
                        TimeSection = recordTime.ToString("HH:mm"),
                        PointName = "Outer Cover Total Thickness",
                        Frequency = "30m Sekali",
                        StandardDimension = "3.20",
                        Initial = "0",
                        ScaleValue = 0.01m,
                        R1 = 3.18m + (i * 0.01m),
                        R2 = 3.20m + (i * 0.01m),
                        R3 = 3.22m + (i * 0.01m),
                        Status = (i % 10) < 8 ? "OK" : "NG",
                        RecordedTime = recordTime
                    });

                    // Spiral Pitch measurements
                    measurements.Add(new DimensionMeasurement
                    {
                        DimensionReportId = dimReport.Id,
                        TimeSection = recordTime.ToString("HH:mm"),
                        PointName = "Spiral Pitch Distance",
                        Frequency = "30m Sekali",
                        StandardDimension = "25.0",
                        Initial = "0",
                        ScaleValue = 0.1m,
                        R1 = 24.8m + (i * 0.05m),
                        R2 = 25.0m + (i * 0.05m),
                        R3 = 25.2m + (i * 0.05m),
                        Status = "OK",
                        RecordedTime = recordTime
                    });
                }

                context.DimensionMeasurements.AddRange(measurements);
                context.SaveChanges();
            }

            // Seed ProductionReports with ProductionReadings for ChartAnalysis
            if (!context.ProductionReports.Any(p => p.DocumentNumber == "VI-SOP-PROD-133"))
            {
                var prodReport = new ProductionReport
                {
                    DocumentNumber = "VI-SOP-PROD-133",
                    RevisionNumber = 0,
                    ProductionDate = DateTime.Now.AddDays(-1),
                    Shift = "Shift 1",
                    CustomerName = "TOYOTA AUTOMOTIVE",
                    HoseType = "Radiator Hose",
                    MachineName = "EXT-01",
                    Dimension = "28mm x 35mm",
                    InnerMaterial = "NBR Grade A",
                    InnerMaterialLotNo = "NBR-20260428-001",
                    InnerMaterialSG = 1.18m,
                    OuterMaterial = "EPDM Premium",
                    OuterMaterialLotNo = "EPDM-20260428-001",
                    OuterMaterialSG = 1.22m,
                    Yarn = "Polyester 2000D",
                    Status = "Completed",
                    VinCode = "VIN-RAD-28",
                    CreatedBy = "Operator A",
                    CreatedDate = DateTime.Now.AddDays(-1)
                };

                context.ProductionReports.Add(prodReport);
                context.SaveChanges();

                // Add ProductionReadings (hourly for 10 hours) - MUST be after SaveChanges
                var readings = new List<ProductionReading>();
                for (int i = 0; i < 10; i++)
                {
                    var readingTime = DateTime.Now.AddHours(-10 + i);
                    readings.Add(new ProductionReading
                    {
                        ProductionReportId = prodReport.Id,
                        ReadingTime = readingTime,
                        IntervalMinutes = 60,
                        HeadTempInner = 180 + (i % 3) - 1,
                        Cylinder1TempInner = 175 + (i % 2),
                        Cylinder2TempInner = 172 + (i % 2),
                        Cylinder3TempInner = 170 + (i % 2),
                        ScrewTempInner = 165 + (i % 3),
                        HeadTempOuter = 175 + (i % 2),
                        Cylinder1TempOuter = 170 + (i % 2),
                        Cylinder2TempOuter = 168 + (i % 2),
                        Cylinder3TempOuter = 165 + (i % 2),
                        ScrewTempOuter = 160 + (i % 3),
                        ScrewSpeedInner = 45.5m + (i * 0.5m),
                        ScrewSpeedOuter = 42.0m + (i * 0.3m),
                        FeedRollRatioInner = 85 + (i % 5),
                        FeedRollRatioOuter = 88 + (i % 5),
                        PressureInner = 12.5m + (i * 0.1m),
                        PressureOuter = 11.8m + (i * 0.1m),
                        HoseSpeed = 18.5m + (i * 0.2m),
                        SpiralSpeed = 150 + (i % 10),
                        SpiralPitchSetting = 25.0m,
                        SpiralPitchDisplay = 25.1m + (i * 0.05m),
                        InnerDiameter = 28.2m + (i * 0.05m),
                        TotalThicknessX = 3.5m + (i * 0.02m),
                        TotalThicknessY = 3.48m + (i * 0.02m)
                    });
                }
                context.ProductionReadings.AddRange(readings);
                context.SaveChanges();
            }

            // Add one more sample report
            if (!context.ProductionReports.Any(p => p.DocumentNumber == "PR-HOSE-FH8-001"))
            {
                var prodReport2 = new ProductionReport
                {
                    DocumentNumber = "PR-HOSE-FH8-001",
                    RevisionNumber = 0,
                    ProductionDate = DateTime.Now.AddDays(-2),
                    Shift = "Shift 2",
                    CustomerName = "HONDA MOTORS",
                    HoseType = "Fuel Hose 8mm",
                    MachineName = "EXT-02",
                    Dimension = "8mm x 12mm",
                    InnerMaterial = "NBR Grade B",
                    InnerMaterialLotNo = "NBR-20260427-002",
                    InnerMaterialSG = 1.19m,
                    OuterMaterial = "CR Premium",
                    OuterMaterialLotNo = "CR-20260427-001",
                    OuterMaterialSG = 1.20m,
                    Yarn = "Polyester 1500D",
                    Status = "Completed",
                    VinCode = "VIN-FH8-001",
                    CreatedBy = "Operator B",
                    CreatedDate = DateTime.Now.AddDays(-2)
                };

                context.ProductionReports.Add(prodReport2);
                context.SaveChanges();

                // Add readings for second report - MUST be after SaveChanges
                var readings2 = new List<ProductionReading>();
                for (int i = 0; i < 8; i++)
                {
                    var readingTime = DateTime.Now.AddDays(-2).AddHours(-8 + i);
                    readings2.Add(new ProductionReading
                    {
                        ProductionReportId = prodReport2.Id,
                        ReadingTime = readingTime,
                        IntervalMinutes = 60,
                        HeadTempInner = 185 + (i % 4) - 1,
                        Cylinder1TempInner = 180 + (i % 2),
                        Cylinder2TempInner = 177 + (i % 2),
                        Cylinder3TempInner = 175 + (i % 2),
                        ScrewTempInner = 170 + (i % 3),
                        HeadTempOuter = 180 + (i % 2),
                        Cylinder1TempOuter = 175 + (i % 2),
                        Cylinder2TempOuter = 173 + (i % 2),
                        Cylinder3TempOuter = 170 + (i % 2),
                        ScrewTempOuter = 165 + (i % 3),
                        ScrewSpeedInner = 42.0m + (i * 0.4m),
                        ScrewSpeedOuter = 40.0m + (i * 0.2m),
                        FeedRollRatioInner = 90 + (i % 5),
                        FeedRollRatioOuter = 92 + (i % 5),
                        PressureInner = 10.5m + (i * 0.1m),
                        PressureOuter = 10.0m + (i * 0.1m),
                        HoseSpeed = 16.5m + (i * 0.15m),
                        SpiralSpeed = 145 + (i % 8),
                        SpiralPitchSetting = 22.0m,
                        SpiralPitchDisplay = 22.05m + (i * 0.03m),
                        InnerDiameter = 8.1m + (i * 0.02m),
                        TotalThicknessX = 2.5m + (i * 0.01m),
                        TotalThicknessY = 2.48m + (i * 0.01m)
                    });
                }
                context.ProductionReadings.AddRange(readings2);
                context.SaveChanges();
            }
        }
    }
}
