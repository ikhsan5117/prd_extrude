using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddSpsImportTrialRows : Migration
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

            migrationBuilder.AddColumn<DateTime>(
                name: "DandoriEndEndTime",
                table: "ProductionReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DandoriEndStartTime",
                table: "ProductionReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemCode",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpsId",
                table: "DimensionReports",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SpsImportTrialRows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SourceFileName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SourceSheet = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DetectedFormat = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ExcelId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Machine = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DocumentNumber = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Customer = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    HoseType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Dimensi = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SourceRowIndex = table.Column<int>(type: "int", nullable: false),
                    ExistsInProduction = table.Column<bool>(type: "bit", nullable: false),
                    ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImportedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpsImportTrialRows", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DimensionReports_SpsId",
                table: "DimensionReports",
                column: "SpsId");

            migrationBuilder.CreateIndex(
                name: "IX_SpsImportTrialRows_BatchId",
                table: "SpsImportTrialRows",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_SpsImportTrialRows_ExcelId_ItemCode",
                table: "SpsImportTrialRows",
                columns: new[] { "ExcelId", "ItemCode" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DimensionReports_StandardParameterSettings_SpsId",
                table: "DimensionReports",
                column: "SpsId",
                principalTable: "StandardParameterSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

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
                name: "FK_DimensionReports_StandardParameterSettings_SpsId",
                table: "DimensionReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductionReports_StandardParameterSettings_SpsId",
                table: "ProductionReports");

            migrationBuilder.DropTable(
                name: "SpsImportTrialRows");

            migrationBuilder.DropIndex(
                name: "IX_DimensionReports_SpsId",
                table: "DimensionReports");

            migrationBuilder.DropColumn(
                name: "DandoriEndEndTime",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "DandoriEndStartTime",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "ItemCode",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "SpsId",
                table: "DimensionReports");

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
