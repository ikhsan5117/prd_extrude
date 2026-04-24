using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMeshDimFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MeshDim1",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeshDim2",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeshDim3",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeshDim1",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "MeshDim2",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "MeshDim3",
                table: "MasterlistSpsDoubleLayers");
        }
    }
}
