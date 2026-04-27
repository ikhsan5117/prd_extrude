using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDimensionSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DimensionSummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DimensionReportId = table.Column<int>(type: "int", nullable: false),
                    PartNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VinCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StandardLength = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualLength = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QtyTarget = table.Column<int>(type: "int", nullable: false),
                    QtyOk = table.Column<int>(type: "int", nullable: false),
                    NgDimension = table.Column<int>(type: "int", nullable: false),
                    NgVisual = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimensionSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DimensionSummaries_DimensionReports_DimensionReportId",
                        column: x => x.DimensionReportId,
                        principalTable: "DimensionReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DimensionSummaries_DimensionReportId",
                table: "DimensionSummaries",
                column: "DimensionReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DimensionSummaries");
        }
    }
}
