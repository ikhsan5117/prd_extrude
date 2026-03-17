using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddWasteAndPresetValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "WasteCoverAkhir",
                table: "ProductionReports",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WasteCoverAwal",
                table: "ProductionReports",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WasteInnerAkhir",
                table: "ProductionReports",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WasteInnerAwal",
                table: "ProductionReports",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WasteWeightAkhir",
                table: "ProductionReports",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WasteWeightAwal",
                table: "ProductionReports",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PresetValue",
                table: "ProductionReadings",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WasteCoverAkhir",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "WasteCoverAwal",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "WasteInnerAkhir",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "WasteInnerAwal",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "WasteWeightAkhir",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "WasteWeightAwal",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "PresetValue",
                table: "ProductionReadings");
        }
    }
}
