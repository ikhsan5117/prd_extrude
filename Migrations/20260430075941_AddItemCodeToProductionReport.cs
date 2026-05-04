using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddItemCodeToProductionReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemCode",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ItemCode",
                table: "DimensionReports",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemCode",
                table: "ProductionReports");

            migrationBuilder.AlterColumn<string>(
                name: "ItemCode",
                table: "DimensionReports",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
