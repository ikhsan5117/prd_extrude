using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddExtraDiesAndTension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ADistance",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleDie",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpacerDie",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TensionYarnInner",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TensionYarnOuter",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ADistance",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "MiddleDie",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "SpacerDie",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "TensionYarnInner",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "TensionYarnOuter",
                table: "MasterlistSpsDoubleLayers");

        }
    }
}
