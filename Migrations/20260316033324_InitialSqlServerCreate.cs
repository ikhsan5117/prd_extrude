using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlServerCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    UseLimitsInner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UseLimitsOuter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nipple = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TubeDie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoverDie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeshScreen1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeshScreen2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadTemp1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadTemp2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder1_1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder1_2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder2_1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder2_2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Feed1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Feed2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrewTemp1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrewTemp2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrewSpeed1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrewSpeed2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pressure1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pressure2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    TebalOuter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TebalTotal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelisihTebal = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    MaterialInnerSG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaterialMiddle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialMiddleLotNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialMiddleSG = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaterialOuter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaterialOuterLotNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaterialOuterSG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    TubeDie = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MiddleDie = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CoverDie = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpacerDie = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ToleranceDie = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tol_TubeDie = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tol_MiddleDie = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tol_OuterDie = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tol_CoverDie = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tol_SpiralPitch = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MeshScreen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeadTemp = table.Column<int>(type: "int", nullable: false),
                    Cylinder1Temp = table.Column<int>(type: "int", nullable: false),
                    Cylinder2Temp = table.Column<int>(type: "int", nullable: false),
                    Cylinder3Temp = table.Column<int>(type: "int", nullable: false),
                    ScrewTemp = table.Column<int>(type: "int", nullable: false),
                    ScrewSpeed = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FeedRollRatio = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Pressure = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AirPressureA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PresetValve = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpiralSpeed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpiralPitch = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpiralSpeedDisplay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SpiralPitchDisplay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PresetTemp = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ControlValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoseSpeed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TakeupConveyorSpeed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoolConveyorSpeed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConveyorRatio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnsmoothSurface = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ChillerWaterTemp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaterpillarGap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarkingMaterialColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarkingMaterialInner = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MarkingMaterialOuter = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                name: "ProductionReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Shift = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dimension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StandardParameterSettingId = table.Column<int>(type: "int", nullable: true),
                    InnerMaterial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InnerMaterialLotNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InnerMaterialSG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OuterMaterial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OuterMaterialLotNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OuterMaterialSG = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Yarn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NippleDieOK = table.Column<bool>(type: "bit", nullable: false),
                    TubeDieOK = table.Column<bool>(type: "bit", nullable: false),
                    MiddleDieOK = table.Column<bool>(type: "bit", nullable: false),
                    CoverDieOK = table.Column<bool>(type: "bit", nullable: false),
                    SpacerDieOK = table.Column<bool>(type: "bit", nullable: false),
                    ToleranceDieOK = table.Column<bool>(type: "bit", nullable: false),
                    InnerMaterialActual = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OuterMaterialActual = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YarnActual = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VinCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StandardLength = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualLength = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QtyTarget = table.Column<int>(type: "int", nullable: false),
                    QtyOk = table.Column<int>(type: "int", nullable: false),
                    NgDimension = table.Column<int>(type: "int", nullable: false),
                    NgVisual = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                name: "DimensionReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionReportId = table.Column<int>(type: "int", nullable: false),
                    ReadingTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IntervalMinutes = table.Column<int>(type: "int", nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InnerDiameterStandard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InnerDiameter1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerDiameter2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerDiameter3 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerDiameter4 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerDiameter5 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerThicknessStandard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InnerThickness1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerThickness2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerThickness3 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerThickness4 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerThickness5 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalThicknessStandard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalThickness1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalThickness2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalThickness3 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalThickness4 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalThickness5 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SpiralPitchStandard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpiralPitchActual = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VisualCheckStandard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VisualCheckOK = table.Column<bool>(type: "bit", nullable: false),
                    VisualCheckNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VinCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QtyStdPanung = table.Column<int>(type: "int", nullable: true),
                    QtyActPanung = table.Column<int>(type: "int", nullable: true),
                    QtyTarget = table.Column<int>(type: "int", nullable: true),
                    QtyOK = table.Column<int>(type: "int", nullable: true),
                    QtyNGDimensi = table.Column<int>(type: "int", nullable: true),
                    QtyNGVisual = table.Column<int>(type: "int", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimensionReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DimensionReadings_ProductionReports_ProductionReportId",
                        column: x => x.ProductionReportId,
                        principalTable: "ProductionReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    CompoundQty = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
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
                    ScrewSpeedInner = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FeedRollRatioInner = table.Column<int>(type: "int", nullable: true),
                    PressureInner = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HeadTempOuter = table.Column<int>(type: "int", nullable: true),
                    Cylinder1TempOuter = table.Column<int>(type: "int", nullable: true),
                    Cylinder2TempOuter = table.Column<int>(type: "int", nullable: true),
                    Cylinder3TempOuter = table.Column<int>(type: "int", nullable: true),
                    ScrewTempOuter = table.Column<int>(type: "int", nullable: true),
                    ScrewSpeedOuter = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FeedRollRatioOuter = table.Column<int>(type: "int", nullable: true),
                    PressureOuter = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SpiralSpeed = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SpiralPitchSetting = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SpiralPitchDisplay = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PresetTemp = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ControlValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HoseSpeed = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TakeupConveyorSpeed = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CoolConveyorSpeed = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ConveyorRatio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnsmoothSurface = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ChillerWaterTemp = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CaterpillarGap = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_DimensionReadings_ProductionReportId",
                table: "DimensionReadings",
                column: "ProductionReportId");

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
                name: "DimensionReadings");

            migrationBuilder.DropTable(
                name: "LotTags");

            migrationBuilder.DropTable(
                name: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropTable(
                name: "NowProducings");

            migrationBuilder.DropTable(
                name: "PackingStandards");

            migrationBuilder.DropTable(
                name: "ProductionReadings");

            migrationBuilder.DropTable(
                name: "ProductionReports");

            migrationBuilder.DropTable(
                name: "StandardParameterSettings");
        }
    }
}
