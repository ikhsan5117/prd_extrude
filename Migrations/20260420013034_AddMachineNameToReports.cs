using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMachineNameToReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MachineName",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MachineName",
                table: "DimensionReports",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MachineName",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "MachineName",
                table: "DimensionReports");
        }
    }
}
