using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDandoriEndTimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DandoriEndEndTime",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "DandoriEndStartTime",
                table: "ProductionReports");
        }
    }
}
