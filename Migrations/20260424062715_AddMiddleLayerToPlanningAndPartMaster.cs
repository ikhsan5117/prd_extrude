using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMiddleLayerToPlanningAndPartMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompoundMiddle",
                table: "PlanningMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NeedKgMiddle",
                table: "PlanningMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompoundMiddle",
                table: "PartMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NeedKgMiddle",
                table: "PartMasters",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompoundMiddle",
                table: "PlanningMasters");

            migrationBuilder.DropColumn(
                name: "NeedKgMiddle",
                table: "PlanningMasters");

            migrationBuilder.DropColumn(
                name: "CompoundMiddle",
                table: "PartMasters");

            migrationBuilder.DropColumn(
                name: "NeedKgMiddle",
                table: "PartMasters");
        }
    }
}
