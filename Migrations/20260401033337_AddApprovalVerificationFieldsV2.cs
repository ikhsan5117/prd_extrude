using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalVerificationFieldsV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedBySignature",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "ProductionReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckedBySignature",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckedDate",
                table: "ProductionReports",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedBySignature",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "CheckedBySignature",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "CheckedDate",
                table: "ProductionReports");
        }
    }
}
