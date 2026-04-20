using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionMaterialLotsRelational : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductionMaterialLots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionReportId = table.Column<int>(type: "int", nullable: false),
                    LayerType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialActual = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LotNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SGValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionMaterialLots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionMaterialLots_ProductionReports_ProductionReportId",
                        column: x => x.ProductionReportId,
                        principalTable: "ProductionReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductionMaterialLots_ProductionReportId",
                table: "ProductionMaterialLots",
                column: "ProductionReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductionMaterialLots");
        }
    }
}
