using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class RenameSpsIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionReports_StandardParameterSettings_StandardParameterSettingId",
                table: "ProductionReports");

            migrationBuilder.RenameColumn(
                name: "StandardParameterSettingId",
                table: "ProductionReports",
                newName: "SpsId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductionReports_StandardParameterSettingId",
                table: "ProductionReports",
                newName: "IX_ProductionReports_SpsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionReports_StandardParameterSettings_SpsId",
                table: "ProductionReports",
                column: "SpsId",
                principalTable: "StandardParameterSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionReports_StandardParameterSettings_SpsId",
                table: "ProductionReports");

            migrationBuilder.RenameColumn(
                name: "SpsId",
                table: "ProductionReports",
                newName: "StandardParameterSettingId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductionReports_SpsId",
                table: "ProductionReports",
                newName: "IX_ProductionReports_StandardParameterSettingId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionReports_StandardParameterSettings_StandardParameterSettingId",
                table: "ProductionReports",
                column: "StandardParameterSettingId",
                principalTable: "StandardParameterSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
