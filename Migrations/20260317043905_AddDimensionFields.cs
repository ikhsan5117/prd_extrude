using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDimensionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "InnerDiameter",
                table: "ProductionReadings",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InnerThicknessX",
                table: "ProductionReadings",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InnerThicknessY",
                table: "ProductionReadings",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SpiralPitch",
                table: "ProductionReadings",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalThicknessX",
                table: "ProductionReadings",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalThicknessY",
                table: "ProductionReadings",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VisualCheck",
                table: "ProductionReadings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InnerDiameter",
                table: "ProductionReadings");

            migrationBuilder.DropColumn(
                name: "InnerThicknessX",
                table: "ProductionReadings");

            migrationBuilder.DropColumn(
                name: "InnerThicknessY",
                table: "ProductionReadings");

            migrationBuilder.DropColumn(
                name: "SpiralPitch",
                table: "ProductionReadings");

            migrationBuilder.DropColumn(
                name: "TotalThicknessX",
                table: "ProductionReadings");

            migrationBuilder.DropColumn(
                name: "TotalThicknessY",
                table: "ProductionReadings");

            migrationBuilder.DropColumn(
                name: "VisualCheck",
                table: "ProductionReadings");
        }
    }
}
