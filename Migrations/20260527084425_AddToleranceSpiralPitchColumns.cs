using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddToleranceSpiralPitchColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ToleranceSpiralPitch_Min",
                table: "SpsNoDocs",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ToleranceSpiralPitch_Asli",
                table: "SpsNoDocs",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ToleranceSpiralPitch_Max",
                table: "SpsNoDocs",
                type: "decimal(18,4)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ToleranceSpiralPitch_Min", table: "SpsNoDocs");
            migrationBuilder.DropColumn(name: "ToleranceSpiralPitch_Asli", table: "SpsNoDocs");
            migrationBuilder.DropColumn(name: "ToleranceSpiralPitch_Max", table: "SpsNoDocs");
        }
    }
}
