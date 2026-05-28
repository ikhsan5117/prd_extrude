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
            // Conditional: drop FK only if still exists (may have been dropped by RenameSpsIdColumn migration)
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.foreign_keys
                    WHERE name = 'FK_ProductionReports_StandardParameterSettings_StandardParameterSettingId'
                      AND parent_object_id = OBJECT_ID('[dbo].[ProductionReports]')
                )
                    ALTER TABLE [dbo].[ProductionReports] DROP CONSTRAINT [FK_ProductionReports_StandardParameterSettings_StandardParameterSettingId];
            ");

            // Conditional: rename column only if old name exists and new name does not exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[ProductionReports]') AND name = 'StandardParameterSettingId')
                   AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[ProductionReports]') AND name = 'SpsId')
                    EXEC sp_rename N'[dbo].[ProductionReports].[StandardParameterSettingId]', N'SpsId', N'COLUMN';
            ");

            // Conditional: rename index only if old name still exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ProductionReports_StandardParameterSettingId' AND object_id = OBJECT_ID('[dbo].[ProductionReports]'))
                    EXEC sp_rename N'[dbo].[ProductionReports].[IX_ProductionReports_StandardParameterSettingId]', N'IX_ProductionReports_SpsId', N'INDEX';
            ");

            // Conditional: add DandoriEndEndTime only if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[ProductionReports]') AND name = 'DandoriEndEndTime')
                    ALTER TABLE [dbo].[ProductionReports] ADD [DandoriEndEndTime] datetime2 NULL;
            ");

            // Conditional: add DandoriEndStartTime only if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[ProductionReports]') AND name = 'DandoriEndStartTime')
                    ALTER TABLE [dbo].[ProductionReports] ADD [DandoriEndStartTime] datetime2 NULL;
            ");

            // Conditional: add ItemCode to ProductionReports only if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[ProductionReports]') AND name = 'ItemCode')
                    ALTER TABLE [dbo].[ProductionReports] ADD [ItemCode] nvarchar(max) NULL;
            ");

            // Conditional: add SpsId to DimensionReports only if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[DimensionReports]') AND name = 'SpsId')
                    ALTER TABLE [dbo].[DimensionReports] ADD [SpsId] int NULL;
            ");

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

            // Conditional: create IX_DimensionReports_SpsId only if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DimensionReports_SpsId' AND object_id = OBJECT_ID('[dbo].[DimensionReports]'))
                    CREATE INDEX [IX_DimensionReports_SpsId] ON [dbo].[DimensionReports] ([SpsId]);
            ");

            migrationBuilder.CreateIndex(
                name: "IX_SpsImportTrialRows_BatchId",
                table: "SpsImportTrialRows",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_SpsImportTrialRows_ExcelId_ItemCode",
                table: "SpsImportTrialRows",
                columns: new[] { "ExcelId", "ItemCode" },
                unique: true);

            // Conditional: add FK_DimensionReports_StandardParameterSettings_SpsId only if not exists
            migrationBuilder.Sql(@"
                IF OBJECT_ID('[dbo].[StandardParameterSettings]') IS NOT NULL
                   AND OBJECT_ID('[dbo].[DimensionReports]') IS NOT NULL
                   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_DimensionReports_StandardParameterSettings_SpsId' AND parent_object_id = OBJECT_ID('[dbo].[DimensionReports]'))
                    ALTER TABLE [dbo].[DimensionReports] ADD CONSTRAINT [FK_DimensionReports_StandardParameterSettings_SpsId] FOREIGN KEY ([SpsId]) REFERENCES [dbo].[StandardParameterSettings] ([Id]) ON DELETE SET NULL;
            ");

            // Conditional: add FK_ProductionReports_StandardParameterSettings_SpsId only if not exists
            migrationBuilder.Sql(@"
                IF OBJECT_ID('[dbo].[StandardParameterSettings]') IS NOT NULL
                   AND OBJECT_ID('[dbo].[ProductionReports]') IS NOT NULL
                   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ProductionReports_StandardParameterSettings_SpsId' AND parent_object_id = OBJECT_ID('[dbo].[ProductionReports]'))
                    ALTER TABLE [dbo].[ProductionReports] ADD CONSTRAINT [FK_ProductionReports_StandardParameterSettings_SpsId] FOREIGN KEY ([SpsId]) REFERENCES [dbo].[StandardParameterSettings] ([Id]) ON DELETE SET NULL;
            ");
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
