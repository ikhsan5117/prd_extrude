using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddStandaloneDimensionReportsV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DimensionReadings");

            migrationBuilder.CreateTable(
                name: "DimensionReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Shift = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoseType = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    ScaleValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    R1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    R2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    R3 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    R4 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    R5 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_DimensionMeasurements_DimensionReportId",
                table: "DimensionMeasurements",
                column: "DimensionReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DimensionMeasurements");

            migrationBuilder.DropTable(
                name: "DimensionReports");

            migrationBuilder.CreateTable(
                name: "DimensionReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionReportId = table.Column<int>(type: "int", nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InnerDiameter1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerDiameter2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerDiameter3 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerDiameter4 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerDiameter5 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerDiameterStandard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InnerThickness1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerThickness2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerThickness3 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerThickness4 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerThickness5 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InnerThicknessStandard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IntervalMinutes = table.Column<int>(type: "int", nullable: false),
                    PartNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QtyActPanung = table.Column<int>(type: "int", nullable: true),
                    QtyNGDimensi = table.Column<int>(type: "int", nullable: true),
                    QtyNGVisual = table.Column<int>(type: "int", nullable: true),
                    QtyOK = table.Column<int>(type: "int", nullable: true),
                    QtyStdPanung = table.Column<int>(type: "int", nullable: true),
                    QtyTarget = table.Column<int>(type: "int", nullable: true),
                    ReadingTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecordedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpiralPitchActual = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SpiralPitchStandard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalThickness1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalThickness2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalThickness3 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalThickness4 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalThickness5 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalThicknessStandard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VinCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisualCheckNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisualCheckOK = table.Column<bool>(type: "bit", nullable: false),
                    VisualCheckStandard = table.Column<string>(type: "nvarchar(max)", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_DimensionReadings_ProductionReportId",
                table: "DimensionReadings",
                column: "ProductionReportId");
        }
    }
}
