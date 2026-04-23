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
            // Seed Standard Parameter Settings
            if (!context.StandardParameterSettings.Any())
            {
                var paramSettings = new List<StandardParameterSetting>
                {
                    new StandardParameterSetting
                    {
                        DocumentNumber = "VH-STD-TEST", ItemList = "DIE-TEST-001", HoseType = "TEST DIES HOSE",
                        CustomerName = "LAB TEST", ProductCode = "TEST-99", IsActive = true,
                        InnerDie = 10.0m, TubeDie = 12.0m, MiddleDie = 11.0m, 
                        CoverDie = 13.0m, SpacerDie = 0.5m, ToleranceDie = 2.0m,
                        HeadTemp = 180, Cylinder1Temp = 175, ScrewSpeed = 45, Pressure = 12
                    },
                    new StandardParameterSetting
                    {
                        DocumentNumber = "VH-FH8-001", ItemList = "ITEM-001", HoseType = "Fuel Hose 8mm",
                        CustomerName = "TOYOTA", ProductCode = "FH8-TYT", IsActive = true,
                        InnerDie = 8.0m, TubeDie = 10.5m, MiddleDie = 11.0m, 
                        CoverDie = 13.0m, SpacerDie = 0.4m, ToleranceDie = 1.5m,
                        HeadTemp = 175, Cylinder1Temp = 170, ScrewSpeed = 40, Pressure = 10
                    },
                    new StandardParameterSetting
                    {
                        DocumentNumber = "VH-OH10-001", ItemList = "ITEM-002", HoseType = "Oil Hose 10mm",
                        CustomerName = "HONDA", ProductCode = "OH10-HND", IsActive = true,
                        InnerDie = 10.0m, TubeDie = 12.5m, MiddleDie = 13.0m, 
                        CoverDie = 15.0m, SpacerDie = 0.6m, ToleranceDie = 1.8m,
                        HeadTemp = 185, Cylinder1Temp = 180, ScrewSpeed = 35, Pressure = 14
                    },
                    new StandardParameterSetting
                    {
                        DocumentNumber = "VH-BH6-001", ItemList = "ITEM-003", HoseType = "Brake Hose 6mm",
                        CustomerName = "SUZUKI", ProductCode = "BH6-SZK", IsActive = true,
                        InnerDie = 6.0m, TubeDie = 8.5m, MiddleDie = 9.0m, 
                        CoverDie = 11.0m, SpacerDie = 0.3m, ToleranceDie = 1.2m,
                        HeadTemp = 170, Cylinder1Temp = 165, ScrewSpeed = 50, Pressure = 12
                    }
                };
                context.StandardParameterSettings.AddRange(paramSettings);
                context.SaveChanges();
            }

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
