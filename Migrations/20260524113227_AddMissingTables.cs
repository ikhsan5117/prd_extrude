using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingTables : Migration
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
                    MachineName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Shift = table.Column<string>(type: "nvarchar(450)", nullable: true),
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
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NowProducings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoseType = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    PartNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LayerType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoseType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Diameter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlanActivities_DailyPlanExecutionId",
                table: "DailyPlanActivities",
                column: "DailyPlanExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlanExecutions_ExecutionDate",
                table: "DailyPlanExecutions",
                column: "ExecutionDate");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlanExecutions_ExecutionDate_MachineName_Shift",
                table: "DailyPlanExecutions",
                columns: new[] { "ExecutionDate", "MachineName", "Shift" });

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
                name: "IX_NowProducings_HoseType",
                table: "NowProducings",
                column: "HoseType");

            migrationBuilder.CreateIndex(
                name: "IX_NowProducings_ProductionDate",
                table: "NowProducings",
                column: "ProductionDate");

            migrationBuilder.CreateIndex(
                name: "IX_PackingStandards_IsActive",
                table: "PackingStandards",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PackingStandards_NACode",
                table: "PackingStandards",
                column: "NACode");

            migrationBuilder.CreateIndex(
                name: "IX_PackingStandards_PartNumber",
                table: "PackingStandards",
                column: "PartNumber");

            migrationBuilder.CreateIndex(
                name: "IX_StandardParameterSettings_DocumentNumber",
                table: "StandardParameterSettings",
                column: "DocumentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_StandardParameterSettings_EffectiveDate",
                table: "StandardParameterSettings",
                column: "EffectiveDate");

            migrationBuilder.CreateIndex(
                name: "IX_StandardParameterSettings_ProductCode",
                table: "StandardParameterSettings",
                column: "ProductCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyPlanActivities");

            migrationBuilder.DropTable(
                name: "LotTags");

            migrationBuilder.DropTable(
                name: "NowProducings");

            migrationBuilder.DropTable(
                name: "PackingStandards");

            migrationBuilder.DropTable(
                name: "StandardParameterSettings");

            migrationBuilder.DropTable(
                name: "DailyPlanExecutions");
        }
    }
}
