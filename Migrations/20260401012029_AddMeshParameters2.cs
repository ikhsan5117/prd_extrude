using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMeshParameters2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverDieFinal",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverDieInitial",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmbossMarkContent",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmbossMarkDate",
                table: "ProductionReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitCaterpillarGap",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitChillerWaterTemp",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitControlValue",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitConveyorRatio",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitCoolConveyorSpeed",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitCylinder1TempInner",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitCylinder1TempOuter",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitCylinder2TempInner",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitCylinder2TempOuter",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitCylinder3TempInner",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitCylinder3TempOuter",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitFeedRollRatioInner",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitFeedRollRatioOuter",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitHeadTempInner",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitHeadTempOuter",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitHoseSpeed",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitPresetValue",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitPressureInner",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitPressureOuter",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitScrewSpeedInner",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitScrewSpeedOuter",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitScrewTempInner",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitScrewTempOuter",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitSpiralPitchDisplay",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitSpiralPitchSetting",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitSpiralSpeed",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitTakeupConveyorSpeed",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InitUnsmoothSurface",
                table: "ProductionReports",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MeshInner10Before",
                table: "ProductionReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MeshInner40Before",
                table: "ProductionReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MeshInnerCheck",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MeshOuter10Before",
                table: "ProductionReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MeshOuter40Before",
                table: "ProductionReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MeshOuterCheck",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleDieFinal",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleDieInitial",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NippleDieFinal",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NippleDieInitial",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QcCond",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QcRes",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QcSurf",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpacerDieFinal",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpacerDieInitial",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToleranceFinal",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToleranceInitial",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TubeDieFinal",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TubeDieInitial",
                table: "ProductionReports",
                type: "nvarchar(max)",
                nullable: true);

/*
            migrationBuilder.AddColumn<string>(
                name: "CompoundCombo",
                table: "PlanningMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompoundInner",
                table: "PlanningMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompoundOuter",
                table: "PlanningMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CtAwal",
                table: "PlanningMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CtMinus20",
                table: "PlanningMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Length",
                table: "PlanningMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NeedKgInner",
                table: "PlanningMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NeedKgOuter",
                table: "PlanningMasters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductionReportId",
                table: "DimensionReports",
                type: "int",

            */
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyPlanActivities");

            migrationBuilder.DropTable(
                name: "Machines");

            migrationBuilder.DropTable(
                name: "PartMasters");

            migrationBuilder.DropTable(
                name: "ShiftMasters");

            migrationBuilder.DropTable(
                name: "DailyPlanExecutions");

            migrationBuilder.DropColumn(
                name: "CoverDieFinal",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "CoverDieInitial",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "EmbossMarkContent",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "EmbossMarkDate",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitCaterpillarGap",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitChillerWaterTemp",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitControlValue",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitConveyorRatio",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitCoolConveyorSpeed",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitCylinder1TempInner",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitCylinder1TempOuter",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitCylinder2TempInner",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitCylinder2TempOuter",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitCylinder3TempInner",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitCylinder3TempOuter",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitFeedRollRatioInner",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitFeedRollRatioOuter",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitHeadTempInner",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitHeadTempOuter",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitHoseSpeed",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitPresetValue",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitPressureInner",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitPressureOuter",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitScrewSpeedInner",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitScrewSpeedOuter",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitScrewTempInner",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitScrewTempOuter",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitSpiralPitchDisplay",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitSpiralPitchSetting",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitSpiralSpeed",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitTakeupConveyorSpeed",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "InitUnsmoothSurface",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "MeshInner10Before",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "MeshInner40Before",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "MeshInnerCheck",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "MeshOuter10Before",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "MeshOuter40Before",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "MeshOuterCheck",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "MiddleDieFinal",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "MiddleDieInitial",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "NippleDieFinal",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "NippleDieInitial",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "QcCond",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "QcRes",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "QcSurf",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "SpacerDieFinal",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "SpacerDieInitial",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "ToleranceFinal",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "ToleranceInitial",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "TubeDieFinal",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "TubeDieInitial",
                table: "ProductionReports");

            migrationBuilder.DropColumn(
                name: "CompoundCombo",
                table: "PlanningMasters");

            migrationBuilder.DropColumn(
                name: "CompoundInner",
                table: "PlanningMasters");

            migrationBuilder.DropColumn(
                name: "CompoundOuter",
                table: "PlanningMasters");

            migrationBuilder.DropColumn(
                name: "CtAwal",
                table: "PlanningMasters");

            migrationBuilder.DropColumn(
                name: "CtMinus20",
                table: "PlanningMasters");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "PlanningMasters");

            migrationBuilder.DropColumn(
                name: "NeedKgInner",
                table: "PlanningMasters");

            migrationBuilder.DropColumn(
                name: "NeedKgOuter",
                table: "PlanningMasters");

            migrationBuilder.DropColumn(
                name: "ProductionReportId",
                table: "DimensionReports");
        }
    }
}
