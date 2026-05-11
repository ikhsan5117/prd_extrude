using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class CreateSpsMasterTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create SpsMaster table with same structure as MasterlistSpsDoubleLayer
            migrationBuilder.CreateTable(
                name: "SpsMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExcelId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    No = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Machine = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevisionNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevisionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Formulasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoseType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dimensi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Material = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerTube = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OuterCover = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiddleTube = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UseLimitsInner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UseLimitsOuter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UseLimitsMiddle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nipple = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TubeDie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoverDie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiddleDie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpacerDie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ADistance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Yarn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TensionYarnInner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TensionYarnOuter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeshDim1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeshScreen1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeshDim2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeshScreen2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeshDim3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeshScreen3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadTemp1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadTemp2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder1_1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder1_2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder2_1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder2_2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder3_1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder3_2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder3_3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadTemp3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder1_3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cylinder2_3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrewTemp3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Feed1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Feed2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrewTemp1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrewTemp2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrewSpeed1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrewSpeed2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pressure1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pressure2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Feed3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScrewSpeed3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pressure3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmMeter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OdSensor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarkingSort = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextMarkingMaterial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarkingColour = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChillerWaterTemp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CuttingSpeed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TakeUpConveyorSpeed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToleranceInner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToleranceOuter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TebalInner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TebalInnerMiddle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TebalOuter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TebalTotal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelisihTebal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemList = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToleranceSpiralPitch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MachineCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PitchYarn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FeedRollRatio1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FeedRollRatio2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FeedRollRatio3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmMeter2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmMeter3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PresetValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControlValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpiralPitchSetting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpiralPitchDisplay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpiralSpeed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoseSpeed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnsmoothSurface = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DancerPosition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CaterpillarGap = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoolConveyorSpeed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoolConveyorSpeed2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConveyorRatio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerTarget = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerTol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerLCL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerUCL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMidTarget = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMidTol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMidLCL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMidMin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMidUCL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InnerMidMax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThickTarget = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThickTol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThickLCL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThickMin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThickUCL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThickMax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalTarget = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalTol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalLCL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalMin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalUCL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalMax = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpsMasters", x => x.Id);
                });

            // Copy data from MasterlistSpsDoubleLayer to SpsMaster
            migrationBuilder.Sql(@"
                INSERT INTO SpsMasters 
                (ExcelId, No, Machine, DocumentNumber, RevisionNumber, Customer, RevisionDate, Formulasi, 
                 HoseType, Dimensi, Material, InnerTube, OuterCover, MiddleTube, UseLimitsInner, UseLimitsOuter, 
                 UseLimitsMiddle, Nipple, TubeDie, CoverDie, MiddleDie, SpacerDie, ADistance, Yarn, 
                 TensionYarnInner, TensionYarnOuter, MeshDim1, MeshScreen1, MeshDim2, MeshScreen2, MeshDim3, 
                 MeshScreen3, HeadTemp1, HeadTemp2, Cylinder1_1, Cylinder1_2, Cylinder2_1, Cylinder2_2, 
                 Cylinder3_1, Cylinder3_2, Cylinder3_3, HeadTemp3, Cylinder1_3, Cylinder2_3, ScrewTemp3, 
                 Feed1, Feed2, ScrewTemp1, ScrewTemp2, ScrewSpeed1, ScrewSpeed2, Pressure1, Pressure2, Feed3, 
                 ScrewSpeed3, Pressure3, AmMeter, OdSensor, MarkingSort, TextMarkingMaterial, MarkingColour, 
                 ChillerWaterTemp, CuttingSpeed, TakeUpConveyorSpeed, ToleranceInner, ToleranceOuter, 
                 TebalInner, TebalInnerMiddle, TebalOuter, TebalTotal, SelisihTebal, ItemList, 
                 ToleranceSpiralPitch, MachineCode, PitchYarn, FeedRollRatio1, FeedRollRatio2, FeedRollRatio3, 
                 CurrentValue, AmMeter2, AmMeter3, PresetValue, ControlValue, SpiralPitchSetting, 
                 SpiralPitchDisplay, SpiralSpeed, HoseSpeed, UnsmoothSurface, DancerPosition, CaterpillarGap, 
                 CoolConveyorSpeed, CoolConveyorSpeed2, ConveyorRatio, InnerTarget, InnerTol, InnerLCL, 
                 InnerMin, InnerUCL, InnerMax, InnerMidTarget, InnerMidTol, InnerMidLCL, InnerMidMin, 
                 InnerMidUCL, InnerMidMax, ThickTarget, ThickTol, ThickLCL, ThickMin, ThickUCL, ThickMax, 
                 TotalTarget, TotalTol, TotalLCL, TotalMin, TotalUCL, TotalMax)
                SELECT 
                ExcelId, No, Machine, DocumentNumber, RevisionNumber, Customer, RevisionDate, Formulasi, 
                HoseType, Dimensi, Material, InnerTube, OuterCover, MiddleTube, UseLimitsInner, UseLimitsOuter, 
                UseLimitsMiddle, Nipple, TubeDie, CoverDie, MiddleDie, SpacerDie, ADistance, Yarn, 
                TensionYarnInner, TensionYarnOuter, MeshDim1, MeshScreen1, MeshDim2, MeshScreen2, MeshDim3, 
                MeshScreen3, HeadTemp1, HeadTemp2, Cylinder1_1, Cylinder1_2, Cylinder2_1, Cylinder2_2, 
                Cylinder3_1, Cylinder3_2, Cylinder3_3, HeadTemp3, Cylinder1_3, Cylinder2_3, ScrewTemp3, 
                Feed1, Feed2, ScrewTemp1, ScrewTemp2, ScrewSpeed1, ScrewSpeed2, Pressure1, Pressure2, Feed3, 
                ScrewSpeed3, Pressure3, AmMeter, OdSensor, MarkingSort, TextMarkingMaterial, MarkingColour, 
                ChillerWaterTemp, CuttingSpeed, TakeUpConveyorSpeed, ToleranceInner, ToleranceOuter, 
                TebalInner, TebalInnerMiddle, TebalOuter, TebalTotal, SelisihTebal, ItemList, 
                ToleranceSpiralPitch, MachineCode, PitchYarn, FeedRollRatio1, FeedRollRatio2, FeedRollRatio3, 
                CurrentValue, AmMeter2, AmMeter3, PresetValue, ControlValue, SpiralPitchSetting, 
                SpiralPitchDisplay, SpiralSpeed, HoseSpeed, UnsmoothSurface, DancerPosition, CaterpillarGap, 
                CoolConveyorSpeed, CoolConveyorSpeed2, ConveyorRatio, InnerTarget, InnerTol, InnerLCL, 
                InnerMin, InnerUCL, InnerMax, InnerMidTarget, InnerMidTol, InnerMidLCL, InnerMidMin, 
                InnerMidUCL, InnerMidMax, ThickTarget, ThickTol, ThickLCL, ThickMin, ThickUCL, ThickMax, 
                TotalTarget, TotalTol, TotalLCL, TotalMin, TotalUCL, TotalMax
                FROM MasterlistSpsDoubleLayers;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpsMasters");
        }
    }
}

