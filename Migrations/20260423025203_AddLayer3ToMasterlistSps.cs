using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddLayer3ToMasterlistSps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cylinder1_3",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cylinder2_3",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cylinder3_1",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cylinder3_2",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cylinder3_3",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feed3",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeadTemp3",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pressure3",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScrewSpeed3",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScrewTemp3",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cylinder1_3",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "Cylinder2_3",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "Cylinder3_1",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "Cylinder3_2",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "Cylinder3_3",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "Feed3",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "HeadTemp3",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "Pressure3",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "ScrewSpeed3",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "ScrewTemp3",
                table: "MasterlistSpsDoubleLayers");
        }
    }
}
