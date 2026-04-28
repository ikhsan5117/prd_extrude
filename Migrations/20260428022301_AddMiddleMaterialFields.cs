using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMiddleMaterialFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MiddleMaterial",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleMaterialActual",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleMaterialLotNo",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MiddleMaterialSG",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MiddleMaterial",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "MiddleMaterialActual",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "MiddleMaterialLotNo",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "MiddleMaterialSG",
                table: "ProductionReports");
        }
    }
}
