using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class SyncProductionReportAlur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DandoriEndTime",
                table: "ProductionReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DandoriStartTime",
                table: "ProductionReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProductionEndTime",
                table: "ProductionReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProductionStartTime",
                table: "ProductionReports",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DandoriEndTime",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "DandoriStartTime",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "ProductionEndTime",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "ProductionStartTime",
                table: "ProductionReports");
        }
    }
}
