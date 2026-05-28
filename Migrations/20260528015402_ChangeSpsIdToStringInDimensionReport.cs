using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSpsIdToStringInDimensionReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop FK and index on DimensionReports.SpsId before altering column type
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_DimensionReports_StandardParameterSettings_SpsId' AND parent_object_id = OBJECT_ID('[dbo].[DimensionReports]'))
                    ALTER TABLE [dbo].[DimensionReports] DROP CONSTRAINT [FK_DimensionReports_StandardParameterSettings_SpsId];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DimensionReports_SpsId' AND object_id = OBJECT_ID('[dbo].[DimensionReports]'))
                    DROP INDEX [IX_DimensionReports_SpsId] ON [dbo].[DimensionReports];
            ");

            // Drop FK on ProductionReports.SpsId before altering column type
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ProductionReports_StandardParameterSettings_SpsId' AND parent_object_id = OBJECT_ID('[dbo].[ProductionReports]'))
                    ALTER TABLE [dbo].[ProductionReports] DROP CONSTRAINT [FK_ProductionReports_StandardParameterSettings_SpsId];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ProductionReports_SpsId' AND object_id = OBJECT_ID('[dbo].[ProductionReports]'))
                    DROP INDEX [IX_ProductionReports_SpsId] ON [dbo].[ProductionReports];
            ");

            // Conditional: alter column only if it's currently int (skip if already nvarchar)
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.columns c
                    JOIN sys.types t ON c.user_type_id = t.user_type_id
                    WHERE c.object_id = OBJECT_ID('[dbo].[ProductionReports]') AND c.name = 'SpsId' AND t.name = 'int'
                )
                    ALTER TABLE [dbo].[ProductionReports] ALTER COLUMN [SpsId] nvarchar(max) NULL;
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.columns c
                    JOIN sys.types t ON c.user_type_id = t.user_type_id
                    WHERE c.object_id = OBJECT_ID('[dbo].[DimensionReports]') AND c.name = 'SpsId' AND t.name = 'int'
                )
                    ALTER TABLE [dbo].[DimensionReports] ALTER COLUMN [SpsId] nvarchar(max) NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SpsId",
                table: "ProductionReports",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SpsId",
                table: "DimensionReports",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
