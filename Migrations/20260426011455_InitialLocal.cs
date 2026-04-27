using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialLocal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyPlanExecutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Shift = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pic1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pic2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LineStopNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LineStopMinutes = table.Column<int>(type: "int", nullable: true),
                    StartMesin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinishMesin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyPlanExecutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DimensionReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Shift = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoseType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DimensionDisplay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Yarn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VinCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StandardLength = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualLength = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QtyTarget = table.Column<int>(type: "int", nullable: false),
                    QtyOk = table.Column<int>(type: "int", nullable: false),
                    NgDimension = table.Column<int>(type: "int", nullable: false),
                    NgVisual = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ByPass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CheckedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimensionReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Line = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MachineType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YearMade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CapacityPerHour = table.Column<int>(type: "int", nullable: true),
                    LastMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MasterlistSpsDoubleLayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExcelId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    No = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Machine = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevisionNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevisionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Formulasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoseType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dimensi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Material = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerTube = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OuterCover = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiddleTube = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UseLimitsInner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UseLimitsOuter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UseLimitsMiddle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nipple = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TubeDie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoverDie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiddleDie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpacerDie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ADistance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Yarn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TensionYarnInner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TensionYarnOuter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeshDim1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeshScreen1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeshDim2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeshScreen2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeshDim3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeshScreen3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadTemp1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadTemp2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder1_1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder1_2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder2_1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder2_2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder3_1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder3_2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder3_3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadTemp3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder1_3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder2_3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrewTemp3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Feed1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Feed2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrewTemp1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrewTemp2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrewSpeed1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrewSpeed2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pressure1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pressure2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Feed3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrewSpeed3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pressure3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmMeter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OdSensor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarkingSort = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextMarkingMaterial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarkingColour = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChillerWaterTemp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CuttingSpeed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TakeUpConveyorSpeed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToleranceInner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToleranceOuter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TebalInner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TebalInnerMiddle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TebalOuter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TebalTotal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelisihTebal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemList = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToleranceSpiralPitch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MachineCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PitchYarn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FeedRollRatio1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FeedRollRatio2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FeedRollRatio3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmMeter2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmMeter3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PresetValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControlValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpiralPitchSetting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpiralPitchDisplay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpiralSpeed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoseSpeed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnsmoothSurface = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DancerPosition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CaterpillarGap = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoolConveyorSpeed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoolConveyorSpeed2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConveyorRatio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerTarget = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerTol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerLCL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerUCL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMidTarget = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMidTol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMidLCL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMidMin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMidUCL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMidMax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThickTarget = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThickTol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThickLCL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThickMin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThickUCL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThickMax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalTarget = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalTol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalLCL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalMin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalUCL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalMax = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterlistSpsDoubleLayers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NowProducings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Class = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dimension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Yarn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaterialInner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaterialInnerLotNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaterialInnerSG = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MaterialMiddle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialMiddleLotNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialMiddleSG = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    MaterialOuter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaterialOuterLotNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaterialOuterSG = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    DandoriStartProdTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DandoriEndProdTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProductionStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProductionEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CHS2LTargetWaktu = table.Column<int>(type: "int", nullable: true),
                    CHS2LTargetDH = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CHS2LTargetMaterial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CHS2LTargetBenang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CHS3LTargetWaktu = table.Column<int>(type: "int", nullable: true),
                    CHS3LTargetDH = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CHS3LTargetMaterial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CHS3LTargetBenang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SPVCheck = table.Column<bool>(type: "bit", nullable: false),
                    SPVCheckBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SPVCheckTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NowProducings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PackingStandards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NACode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaterialName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VinCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dandori = table.Column<int>(type: "int", nullable: false),
                    DH = table.Column<int>(type: "int", nullable: false),
                    StdQty = table.Column<int>(type: "int", nullable: false),
                    ActualQty = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackingStandards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Length = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Diameter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompoundInner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompoundOuter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompoundMiddle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompoundCombo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NeedKgInner = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    NeedKgOuter = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    NeedKgMiddle = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    SecPerPcs = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    CtMinus20 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    CtAwal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    Senin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Selasa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rabu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kamis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Jumat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sabtu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Minggu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanningMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateShiftString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartName1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartName2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Compound = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompoundInner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompoundMiddle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompoundOuter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompoundCombo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Length = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CtAwal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CtMinus20 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NeedKgInner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NeedKgMiddle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NeedKgOuter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanTargetPcs = table.Column<int>(type: "int", nullable: true),
                    Menit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WaktuMulai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WaktuSelesai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanningMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShiftMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StandardParameterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RevisionNumber = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LayerType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoseType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Diameter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMaterial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OuterMaterial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YarnType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerDie = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    OuterDie = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    TubeDie = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MiddleDie = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CoverDie = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    SpacerDie = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ToleranceDie = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Tol_TubeDie = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Tol_MiddleDie = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Tol_OuterDie = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Tol_CoverDie = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Tol_SpiralPitch = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MeshScreen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MeshScreen2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeshScreen3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadTemp = table.Column<int>(type: "int", nullable: false),
                    Cylinder1Temp = table.Column<int>(type: "int", nullable: false),
                    Cylinder2Temp = table.Column<int>(type: "int", nullable: false),
                    Cylinder3Temp = table.Column<int>(type: "int", nullable: false),
                    ScrewTemp = table.Column<int>(type: "int", nullable: false),
                    ScrewSpeed = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    FeedRollRatio = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Pressure = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    AmMeter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadTemp2 = table.Column<int>(type: "int", nullable: true),
                    Cylinder1_2 = table.Column<int>(type: "int", nullable: true),
                    Cylinder2_2 = table.Column<int>(type: "int", nullable: true),
                    Cylinder3_2 = table.Column<int>(type: "int", nullable: true),
                    ScrewTemp2 = table.Column<int>(type: "int", nullable: true),
                    ScrewSpeed2 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    FeedRollRatio2 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    Pressure2 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    AmMeter2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadTemp3 = table.Column<int>(type: "int", nullable: true),
                    Cylinder1_3 = table.Column<int>(type: "int", nullable: true),
                    Cylinder2_3 = table.Column<int>(type: "int", nullable: true),
                    Cylinder3_3 = table.Column<int>(type: "int", nullable: true),
                    ScrewTemp3 = table.Column<int>(type: "int", nullable: true),
                    ScrewSpeed3 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    FeedRollRatio3 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    Pressure3 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    AmMeter3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AirPressureA = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PresetValve = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    SpiralSpeed = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    SpiralPitch = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    SpiralSpeedDisplay = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    SpiralPitchDisplay = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PresetTemp = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ControlValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoseSpeed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TakeupConveyorSpeed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoolConveyorSpeed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConveyorRatio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnsmoothSurface = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ChillerWaterTemp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaterpillarGap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarkingMaterialColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarkingMaterialInner = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MarkingMaterialOuter = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ItemList = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MachineCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardParameterSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyPlanActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DailyPlanExecutionId = table.Column<int>(type: "int", nullable: false),
                    PartName1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartName2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanQty = table.Column<int>(type: "int", nullable: true),
                    PlanDurationMinutes = table.Column<int>(type: "int", nullable: true),
                    PlanStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualQty = table.Column<int>(type: "int", nullable: true),
                    ActualDurationMinutes = table.Column<int>(type: "int", nullable: true),
                    ActualStart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualEnd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StopReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyPlanActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyPlanActivities_DailyPlanExecutions_DailyPlanExecutionId",
                        column: x => x.DailyPlanExecutionId,
                        principalTable: "DailyPlanExecutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DimensionMeasurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DimensionReportId = table.Column<int>(type: "int", nullable: false),
                    TimeSection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PointName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StandardDimension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Initial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScaleValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    R1 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    R2 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    R3 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    R4 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    R5 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimensionMeasurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DimensionMeasurements_DimensionReports_DimensionReportId",
                        column: x => x.DimensionReportId,
                        principalTable: "DimensionReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Shift = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoseType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dimension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StandardParameterSettingId = table.Column<int>(type: "int", nullable: true),
                    InnerMaterial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMaterialLotNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMaterialSG = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    OuterMaterial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OuterMaterialLotNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OuterMaterialSG = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    Yarn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YarnLotNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DandoriStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DandoriEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProductionStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProductionEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NippleDieOK = table.Column<bool>(type: "bit", nullable: false),
                    TubeDieOK = table.Column<bool>(type: "bit", nullable: false),
                    MiddleDieOK = table.Column<bool>(type: "bit", nullable: false),
                    CoverDieOK = table.Column<bool>(type: "bit", nullable: false),
                    SpacerDieOK = table.Column<bool>(type: "bit", nullable: false),
                    ToleranceDieOK = table.Column<bool>(type: "bit", nullable: false),
                    InnerMaterialActual = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OuterMaterialActual = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YarnActual = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VinCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StandardLength = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualLength = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QtyTarget = table.Column<int>(type: "int", nullable: false),
                    QtyOk = table.Column<int>(type: "int", nullable: false),
                    NgDimension = table.Column<int>(type: "int", nullable: false),
                    NgVisual = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WasteWeightAwal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    WasteWeightAkhir = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    WasteInnerAwal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    WasteInnerAkhir = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    WasteCoverAwal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    WasteCoverAkhir = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    DAI_Awal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DAI_Akhir = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DAC_Awal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DAC_Akhir = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DRAI_Awal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DRAI_Akhir = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DRAC_Awal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DRAC_Akhir = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeshInner10Before = table.Column<bool>(type: "bit", nullable: false),
                    MeshInner40Before = table.Column<bool>(type: "bit", nullable: false),
                    MeshOuter10Before = table.Column<bool>(type: "bit", nullable: false),
                    MeshOuter40Before = table.Column<bool>(type: "bit", nullable: false),
                    MeshInnerCheck = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeshOuterCheck = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmbossMarkContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmbossMarkDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QcCond = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QcSurf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QcRes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitHeadTempInner = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitCylinder1TempInner = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitCylinder2TempInner = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitCylinder3TempInner = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitScrewTempInner = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitScrewSpeedInner = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitFeedRollRatioInner = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitPressureInner = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitHeadTempOuter = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitCylinder1TempOuter = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitCylinder2TempOuter = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitCylinder3TempOuter = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitScrewTempOuter = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitScrewSpeedOuter = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitFeedRollRatioOuter = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitPressureOuter = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    CheckedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBySignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckedBySignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitSpiralSpeed = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitSpiralPitchSetting = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitSpiralPitchDisplay = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitPresetValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitControlValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitHoseSpeed = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitTakeupConveyorSpeed = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitCoolConveyorSpeed = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitConveyorRatio = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitUnsmoothSurface = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitChillerWaterTemp = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InitCaterpillarGap = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    NippleDieInitial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NippleDieFinal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TubeDieInitial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TubeDieFinal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiddleDieInitial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiddleDieFinal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoverDieInitial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoverDieFinal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpacerDieInitial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpacerDieFinal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToleranceInitial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToleranceFinal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionReports_StandardParameterSettings_StandardParameterSettingId",
                        column: x => x.StandardParameterSettingId,
                        principalTable: "StandardParameterSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LotTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LotTagNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Plant = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PartDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetQty = table.Column<int>(type: "int", nullable: false),
                    ActualQty = table.Column<int>(type: "int", nullable: true),
                    LotPackaging = table.Column<int>(type: "int", nullable: false),
                    CompoundCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BomText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompoundQty = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    NoLot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DaftarKomponen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubconCheck = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MesinCheck = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TanggalCheck = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QtyOK = table.Column<int>(type: "int", nullable: true),
                    QtyNG = table.Column<int>(type: "int", nullable: true),
                    NGReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrintedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastPrintedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PrintCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductionReportId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LotTags_ProductionReports_ProductionReportId",
                        column: x => x.ProductionReportId,
                        principalTable: "ProductionReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProductionMaterialLots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionReportId = table.Column<int>(type: "int", nullable: false),
                    LayerType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialActual = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LotNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SGValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionMaterialLots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionMaterialLots_ProductionReports_ProductionReportId",
                        column: x => x.ProductionReportId,
                        principalTable: "ProductionReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionReportId = table.Column<int>(type: "int", nullable: false),
                    ReadingTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IntervalMinutes = table.Column<int>(type: "int", nullable: false),
                    HeadTempInner = table.Column<int>(type: "int", nullable: true),
                    Cylinder1TempInner = table.Column<int>(type: "int", nullable: true),
                    Cylinder2TempInner = table.Column<int>(type: "int", nullable: true),
                    Cylinder3TempInner = table.Column<int>(type: "int", nullable: true),
                    ScrewTempInner = table.Column<int>(type: "int", nullable: true),
                    ScrewSpeedInner = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    FeedRollRatioInner = table.Column<int>(type: "int", nullable: true),
                    PressureInner = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    HeadTempOuter = table.Column<int>(type: "int", nullable: true),
                    Cylinder1TempOuter = table.Column<int>(type: "int", nullable: true),
                    Cylinder2TempOuter = table.Column<int>(type: "int", nullable: true),
                    Cylinder3TempOuter = table.Column<int>(type: "int", nullable: true),
                    ScrewTempOuter = table.Column<int>(type: "int", nullable: true),
                    ScrewSpeedOuter = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    FeedRollRatioOuter = table.Column<int>(type: "int", nullable: true),
                    PressureOuter = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    SpiralSpeed = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    SpiralPitchSetting = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    SpiralPitchDisplay = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    PresetTemp = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    PresetValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ControlValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    HoseSpeed = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    TakeupConveyorSpeed = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    CoolConveyorSpeed = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ConveyorRatio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnsmoothSurface = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ChillerWaterTemp = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    CaterpillarGap = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InnerDiameter = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InnerThicknessX = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    InnerThicknessY = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    TotalThicknessX = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    TotalThicknessY = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    SpiralPitch = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    VisualCheck = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionReadings_ProductionReports_ProductionReportId",
                        column: x => x.ProductionReportId,
                        principalTable: "ProductionReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorIngestLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MachineCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductionReportId = table.Column<int>(type: "int", nullable: true),
                    SensorTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MetricType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MetricValue = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 4, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Quality = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IngestedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorIngestLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorIngestLogs_ProductionReports_ProductionReportId",
                        column: x => x.ProductionReportId,
                        principalTable: "ProductionReports",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlanActivities_DailyPlanExecutionId",
                table: "DailyPlanActivities",
                column: "DailyPlanExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_DimensionMeasurements_DimensionReportId",
                table: "DimensionMeasurements",
                column: "DimensionReportId");

            migrationBuilder.CreateIndex(
                name: "IX_LotTags_LotTagNumber",
                table: "LotTags",
                column: "LotTagNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LotTags_PartNumber",
                table: "LotTags",
                column: "PartNumber");

            migrationBuilder.CreateIndex(
                name: "IX_LotTags_ProductionReportId",
                table: "LotTags",
                column: "ProductionReportId");

            migrationBuilder.CreateIndex(
                name: "IX_PackingStandards_NACode",
                table: "PackingStandards",
                column: "NACode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductionMaterialLots_ProductionReportId",
                table: "ProductionMaterialLots",
                column: "ProductionReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionReadings_ProductionReportId",
                table: "ProductionReadings",
                column: "ProductionReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionReports_DocumentNumber",
                table: "ProductionReports",
                column: "DocumentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionReports_ProductionDate",
                table: "ProductionReports",
                column: "ProductionDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionReports_StandardParameterSettingId",
                table: "ProductionReports",
                column: "StandardParameterSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorIngestLogs_DeviceId",
                table: "SensorIngestLogs",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorIngestLogs_IdempotencyKey",
                table: "SensorIngestLogs",
                column: "IdempotencyKey",
                unique: true,
                filter: "[IdempotencyKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SensorIngestLogs_MachineCode_SensorTimestamp",
                table: "SensorIngestLogs",
                columns: new[] { "MachineCode", "SensorTimestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_SensorIngestLogs_ProductionReportId",
                table: "SensorIngestLogs",
                column: "ProductionReportId");

            migrationBuilder.CreateIndex(
                name: "IX_StandardParameterSettings_CustomerName",
                table: "StandardParameterSettings",
                column: "CustomerName");

            migrationBuilder.CreateIndex(
                name: "IX_StandardParameterSettings_DocumentNumber",
                table: "StandardParameterSettings",
                column: "DocumentNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyPlanActivities");

            migrationBuilder.DropTable(
                name: "DimensionMeasurements");

            migrationBuilder.DropTable(
                name: "LotTags");

            migrationBuilder.DropTable(
                name: "Machines");

            migrationBuilder.DropTable(
                name: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropTable(
                name: "NowProducings");

            migrationBuilder.DropTable(
                name: "PackingStandards");

            migrationBuilder.DropTable(
                name: "PartMasters");

            migrationBuilder.DropTable(
                name: "PlanningMasters");

            migrationBuilder.DropTable(
                name: "ProductionMaterialLots");

            migrationBuilder.DropTable(
                name: "ProductionReadings");

            migrationBuilder.DropTable(
                name: "SensorIngestLogs");

            migrationBuilder.DropTable(
                name: "ShiftMasters");

            migrationBuilder.DropTable(
                name: "DailyPlanExecutions");

            migrationBuilder.DropTable(
                name: "DimensionReports");

            migrationBuilder.DropTable(
                name: "ProductionReports");

            migrationBuilder.DropTable(
                name: "StandardParameterSettings");
        }
    }
}
