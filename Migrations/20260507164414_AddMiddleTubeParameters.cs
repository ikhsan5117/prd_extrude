using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMiddleTubeParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "InitCylinder1TempMiddle",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitCylinder2TempMiddle",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitCylinder3TempMiddle",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitFeedRollRatioMiddle",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitHeadTempMiddle",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitPressureMiddle",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitScrewSpeedMiddle",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitScrewTempMiddle",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cylinder1TempMiddle",
                table: "ProductionReadings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cylinder2TempMiddle",
                table: "ProductionReadings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cylinder3TempMiddle",
                table: "ProductionReadings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FeedRollRatioMiddle",
                table: "ProductionReadings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeadTempMiddle",
                table: "ProductionReadings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PressureMiddle",
                table: "ProductionReadings",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ScrewSpeedMiddle",
                table: "ProductionReadings",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScrewTempMiddle",
                table: "ProductionReadings",
                type: "int",
                nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DimensionReports_StandardParameterSettings_SpsId",
                table: "DimensionReports");

            migrationBuilder.DropIndex(
                name: "IX_DimensionReports_SpsId",
                table: "DimensionReports");

            migrationBuilder.DropColumn(
                name: "InitCylinder1TempMiddle",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitCylinder2TempMiddle",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitCylinder3TempMiddle",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitFeedRollRatioMiddle",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitHeadTempMiddle",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitPressureMiddle",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitScrewSpeedMiddle",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitScrewTempMiddle",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "Cylinder1TempMiddle",
                table: "ProductionReadings");

            migrationBuilder.DropColumn(
                name: "Cylinder2TempMiddle",
                table: "ProductionReadings");

            migrationBuilder.DropColumn(
                name: "Cylinder3TempMiddle",
                table: "ProductionReadings");

            migrationBuilder.DropColumn(
                name: "FeedRollRatioMiddle",
                table: "ProductionReadings");

            migrationBuilder.DropColumn(
                name: "HeadTempMiddle",
                table: "ProductionReadings");

            migrationBuilder.DropColumn(
                name: "PressureMiddle",
                table: "ProductionReadings");

            migrationBuilder.DropColumn(
                name: "ScrewSpeedMiddle",
                table: "ProductionReadings");

            migrationBuilder.DropColumn(
                name: "ScrewTempMiddle",
                table: "ProductionReadings");

            migrationBuilder.DropColumn(
                name: "SpsId",
                table: "DimensionReports");
        }
    }
}
