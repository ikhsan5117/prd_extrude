using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddWasteStringFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DAC_Akhir",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DAC_Awal",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DAI_Akhir",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DAI_Awal",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DRAC_Akhir",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DRAC_Awal",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DRAI_Akhir",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DRAI_Awal",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DAC_Akhir",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "DAC_Awal",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "DAI_Akhir",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "DAI_Awal",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "DRAC_Akhir",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "DRAC_Awal",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "DRAI_Akhir",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "DRAI_Awal",
                table: "ProductionReports");
        }
    }
}
