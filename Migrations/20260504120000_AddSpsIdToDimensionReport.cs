using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddSpsIdToDimensionReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpsId",
                table: "DimensionReports",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DimensionReports_SpsId",
                table: "DimensionReports",
                column: "SpsId");

            migrationBuilder.AddForeignKey(
                name: "FK_DimensionReports_StandardParameterSettings_SpsId",
                table: "DimensionReports",
                column: "SpsId",
                principalTable: "StandardParameterSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
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
                name: "SpsId",
                table: "DimensionReports");
        }
    }
}
