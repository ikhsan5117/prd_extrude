using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanningMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanningMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateShiftString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartName1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartName2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Compound = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanTargetPcs = table.Column<int>(type: "int", nullable: true),
                    Menit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WaktuMulai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WaktuSelesai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanningMasters", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanningMasters");
        }
    }
}
