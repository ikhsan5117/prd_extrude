using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddInnerMidMatrix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InnerMidLCL",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InnerMidMax",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InnerMidMin",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InnerMidTarget",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InnerMidTol",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InnerMidUCL",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InnerMidLCL",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "InnerMidMax",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "InnerMidMin",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "InnerMidTarget",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "InnerMidTol",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "InnerMidUCL",
                table: "MasterlistSpsDoubleLayers");
        }
    }
}
