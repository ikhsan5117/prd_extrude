using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class SyncMasterlistSpsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AmMeter2",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CaterpillarGap",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ControlValue",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConveyorRatio",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoolConveyorSpeed",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentValue",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DancerPosition",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeedRollRatio1",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeedRollRatio2",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HoseSpeed",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InnerLCL",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InnerMax",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InnerMin",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InnerTarget",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InnerTol",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InnerUCL",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PitchYarn",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PresetValue",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpiralPitchDisplay",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpiralPitchSetting",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpiralSpeed",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThickLCL",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThickMax",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThickMin",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThickTarget",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThickTol",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThickUCL",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotalLCL",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotalMax",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotalMin",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotalTarget",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotalTol",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotalUCL",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnsmoothSurface",
                table: "MasterlistSpsDoubleLayers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmMeter2",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "CaterpillarGap",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "ControlValue",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "ConveyorRatio",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "CoolConveyorSpeed",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "CurrentValue",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "DancerPosition",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "FeedRollRatio1",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "FeedRollRatio2",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "HoseSpeed",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "InnerLCL",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "InnerMax",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "InnerMin",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "InnerTarget",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "InnerTol",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "InnerUCL",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "PitchYarn",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "PresetValue",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "SpiralPitchDisplay",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "SpiralPitchSetting",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "SpiralSpeed",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "ThickLCL",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "ThickMax",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "ThickMin",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "ThickTarget",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "ThickTol",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "ThickUCL",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "TotalLCL",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "TotalMax",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "TotalMin",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "TotalTarget",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "TotalTol",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "TotalUCL",
                table: "MasterlistSpsDoubleLayers");

            migrationBuilder.DropColumn(
                name: "UnsmoothSurface",
                table: "MasterlistSpsDoubleLayers");
        }
    }
}
