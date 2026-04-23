using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMiddleLayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MiddleTube",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UseLimitsMiddle",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MiddleTube",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "UseLimitsMiddle",
                table: "MasterlistSpsDoubleLayers");
        }
    }
}
