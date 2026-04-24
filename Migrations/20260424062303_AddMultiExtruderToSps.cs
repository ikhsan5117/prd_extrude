using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiExtruderToSps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AmMeter",
                table: "StandardParameterSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AmMeter2",
                table: "StandardParameterSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AmMeter3",
                table: "StandardParameterSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cylinder1_2",
                table: "StandardParameterSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cylinder1_3",
                table: "StandardParameterSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cylinder2_2",
                table: "StandardParameterSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cylinder2_3",
                table: "StandardParameterSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cylinder3_2",
                table: "StandardParameterSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cylinder3_3",
                table: "StandardParameterSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FeedRollRatio2",
                table: "StandardParameterSettings",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FeedRollRatio3",
                table: "StandardParameterSettings",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeadTemp2",
                table: "StandardParameterSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeadTemp3",
                table: "StandardParameterSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeshScreen2",
                table: "StandardParameterSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeshScreen3",
                table: "StandardParameterSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Pressure2",
                table: "StandardParameterSettings",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Pressure3",
                table: "StandardParameterSettings",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ScrewSpeed2",
                table: "StandardParameterSettings",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ScrewSpeed3",
                table: "StandardParameterSettings",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScrewTemp2",
                table: "StandardParameterSettings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScrewTemp3",
                table: "StandardParameterSettings",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmMeter",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "AmMeter2",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "AmMeter3",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "Cylinder1_2",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "Cylinder1_3",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "Cylinder2_2",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "Cylinder2_3",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "Cylinder3_2",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "Cylinder3_3",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "FeedRollRatio2",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "FeedRollRatio3",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "HeadTemp2",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "HeadTemp3",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "MeshScreen2",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "MeshScreen3",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "Pressure2",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "Pressure3",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "ScrewSpeed2",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "ScrewSpeed3",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "ScrewTemp2",
                table: "StandardParameterSettings");

            migrationBuilder.DropColumn(
                name: "ScrewTemp3",
                table: "StandardParameterSettings");
        }
    }
}
