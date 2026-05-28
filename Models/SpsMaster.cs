using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VelastoProductionSystem.Models
{
    public class SpsMaster
    {
        [Display(Name = "No")]
        public string? No { get; set; }

        [Display(Name = "Machine")]
        public string? Machine { get; set; }

        [Key]
        [Display(Name = "No. Document")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Display(Name = "No. Rev.")]
        public string? RevisionNumber { get; set; }

        [Display(Name = "Customer")]
        public string? Customer { get; set; }

        [Display(Name = "Revision Date")]
        public string? RevisionDate { get; set; }

        [Display(Name = "Formulasi")]
        public string? Formulasi { get; set; }

        [Display(Name = "Hose Type")]
        public string? HoseType { get; set; }

        [Display(Name = "Dimensi")]
        public string? Dimensi { get; set; }

        [Display(Name = "Material TSM")]
        public string? Material { get; set; }

        [Display(Name = "Inner | #tube")]
        public string? InnerTube { get; set; }

        [Display(Name = "Outer | #cover")]
        public string? OuterCover { get; set; }

        [Display(Name = "Middle | #tube2")]
        public string? MiddleTube { get; set; }

        [Display(Name = "Use limits of the material | #Inner")]
        public string? UseLimitsInner { get; set; }

        [Display(Name = "Use limits of the material | #Outer")]
        public string? UseLimitsOuter { get; set; }

        [Display(Name = "Use limits of the material | #Middle")]
        public string? UseLimitsMiddle { get; set; }

        [Display(Name = "Nipple | #material")]
        public string? Nipple { get; set; }

        [Display(Name = "Tube Die | #material")]
        public string? TubeDie { get; set; }

        [Display(Name = "Cover Die | #material")]
        public string? CoverDie { get; set; }

        [Display(Name = "Middle Die | #material")]
        public string? MiddleDie { get; set; }

        [Display(Name = "Spacer | #material")]
        public string? SpacerDie { get; set; }

        [Display(Name = "A distance | #material")]
        public string? ADistance { get; set; }

        [Display(Name = "Yarn")]
        public string? Yarn { get; set; }

        [Display(Name = "Tension Yarn | #Inner")]
        public string? TensionYarnInner { get; set; }

        [Display(Name = "Tension Yarn | #Outer")]
        public string? TensionYarnOuter { get; set; }

        [Display(Name = "Mesh Dim. | #material 1")]
        public string? MeshDim1 { get; set; }

        [Display(Name = "Mesh Screen | #material 1")]
        public string? MeshScreen1 { get; set; }

        [Display(Name = "Mesh Dim. | #material 2")]
        public string? MeshDim2 { get; set; }

        [Display(Name = "Mesh Screen | #material 2")]
        public string? MeshScreen2 { get; set; }

        [Display(Name = "Mesh Dim. | #material 3")]
        public string? MeshDim3 { get; set; }

        [Display(Name = "Mesh Screen | #material 3")]
        public string? MeshScreen3 { get; set; }

        [Display(Name = "Head Temp. | #material 1")]
        public string? HeadTemp1 { get; set; }

        [Display(Name = "Head Temp. | #material 2")]
        public string? HeadTemp2 { get; set; }

        [Display(Name = "Cylinder 1 | #material 1")]
        public string? Cylinder1_1 { get; set; }

        [Display(Name = "Cylinder 1 | #material 2")]
        public string? Cylinder1_2 { get; set; }

        [Display(Name = "Cylinder 2 | #material 1")]
        public string? Cylinder2_1 { get; set; }

        [Display(Name = "Cylinder 2 | #material 2")]
        public string? Cylinder2_2 { get; set; }

        [Display(Name = "Cylinder 3 | #material 1")]
        public string? Cylinder3_1 { get; set; }

        [Display(Name = "Cylinder 3 | #material 2")]
        public string? Cylinder3_2 { get; set; }

        [Display(Name = "Cylinder 3 | #material 3")]
        public string? Cylinder3_3 { get; set; }

        [Display(Name = "Head Temp. | #material 3")]
        public string? HeadTemp3 { get; set; }

        [Display(Name = "Cylinder 1 | #material 3")]
        public string? Cylinder1_3 { get; set; }

        [Display(Name = "Cylinder 2 | #material 3")]
        public string? Cylinder2_3 { get; set; }

        [Display(Name = "Screw Temp. | #material 3")]
        public string? ScrewTemp3 { get; set; }

        [Display(Name = "FEED | #material 1")]
        public string? Feed1 { get; set; }

        [Display(Name = "FEED | #material 2")]
        public string? Feed2 { get; set; }

        [Display(Name = "Screw Temp. | #material 1")]
        public string? ScrewTemp1 { get; set; }

        [Display(Name = "Screw Temp. | #material 2")]
        public string? ScrewTemp2 { get; set; }

        [Display(Name = "Screw Speed | #material 1")]
        public string? ScrewSpeed1 { get; set; }

        [Display(Name = "Screw Speed | #material 2")]
        public string? ScrewSpeed2 { get; set; }

        [Display(Name = "Pressure | #material 1")]
        public string? Pressure1 { get; set; }

        [Display(Name = "Pressure | #material 2")]
        public string? Pressure2 { get; set; }

        [Display(Name = "FEED | #material 3")]
        public string? Feed3 { get; set; }

        [Display(Name = "Screw Speed | #material 3")]
        public string? ScrewSpeed3 { get; set; }

        [Display(Name = "Pressure | #material 3")]
        public string? Pressure3 { get; set; }

        [Display(Name = "Am meter")]
        public string? AmMeter { get; set; }

        [Display(Name = "OD sensor")]
        public string? OdSensor { get; set; }

        [Display(Name = "Marking Sort")]
        public string? MarkingSort { get; set; }

        [Display(Name = "Text marking mt'l")]
        public string? TextMarkingMaterial { get; set; }

        [Display(Name = "Marking Colour")]
        public string? MarkingColour { get; set; }

        [Display(Name = "Chiller water temp.")]
        public string? ChillerWaterTemp { get; set; }

        [Display(Name = "Cutting speed")]
        public string? CuttingSpeed { get; set; }

        [Display(Name = "Take up conveyor speed")]
        public string? TakeUpConveyorSpeed { get; set; }

        [Display(Name = "Min | Asli | Max Inner")]
        public string? ToleranceInner { get; set; }

        [Display(Name = "Min | Asli | Max Outer")]
        public string? ToleranceOuter { get; set; }

        [Display(Name = "Tebal Inner")]
        public string? TebalInner { get; set; }

        [Display(Name = "Tebal Inner + Middle")]
        public string? TebalInnerMiddle { get; set; }

        [Display(Name = "Tebal Outer")]
        public string? TebalOuter { get; set; }

        [Display(Name = "Tebal Total")]
        public string? TebalTotal { get; set; }

        [Display(Name = "Selisih tebal")]
        public string? SelisihTebal { get; set; }

        [Display(Name = "ITEM (Part List)")]
        public string? ItemList { get; set; }

        [Display(Name = "Spiral Pitch")]
        public string? ToleranceSpiralPitch { get; set; }

        [Display(Name = "MC")]
        public string? MachineCode { get; set; }

        // --- NEW FIELDS FROM CHS 2 LAYER REFINEMENT ---
        
        [Display(Name = "Pitch Yarn")]
        public string? PitchYarn { get; set; }

        [Display(Name = "Feed Roll Ratio 1")]
        public string? FeedRollRatio1 { get; set; }

        [Display(Name = "Feed Roll Ratio 2")]
        public string? FeedRollRatio2 { get; set; }

        [Display(Name = "Feed Roll Ratio 3")]
        public string? FeedRollRatio3 { get; set; }

        [Display(Name = "Current Value")]
        public string? CurrentValue { get; set; }

        [Display(Name = "Am Meter 2")]
        public string? AmMeter2 { get; set; }

        [Display(Name = "Am Meter 3")]
        public string? AmMeter3 { get; set; }

        [Display(Name = "Preset Value")]
        public string? PresetValue { get; set; }

        [Display(Name = "Control Value")]
        public string? ControlValue { get; set; }

        [Display(Name = "Spiral Pitch Setting")]
        public string? SpiralPitchSetting { get; set; }

        [Display(Name = "Spiral Pitch Display")]
        public string? SpiralPitchDisplay { get; set; }

        [Display(Name = "Spiral Speed")]
        public string? SpiralSpeed { get; set; }

        [Display(Name = "Hose Speed")]
        public string? HoseSpeed { get; set; }

        [Display(Name = "Unsmooth Surface")]
        public string? UnsmoothSurface { get; set; }

        [Display(Name = "Dancer Position")]
        public string? DancerPosition { get; set; }

        [Display(Name = "Caterpillar Gap")]
        public string? CaterpillarGap { get; set; }

        [Display(Name = "Cool Conveyor 1 Speed")]
        public string? CoolConveyorSpeed { get; set; }

        [Display(Name = "Cool Conveyor 2 Speed")]
        public string? CoolConveyorSpeed2 { get; set; }

        [Display(Name = "Conveyor Ratio")]
        public string? ConveyorRatio { get; set; }

        // --- QUALITY MATRIX (INNER) ---
        public string? InnerTarget { get; set; }
        public string? InnerTol { get; set; }
        public string? InnerLCL { get; set; }
        public string? InnerMin { get; set; }
        public string? InnerUCL { get; set; }
        public string? InnerMax { get; set; }

        // Final Quality Matrix (Inner + Middle)
        public string? InnerMidTarget { get; set; }
        public string? InnerMidTol { get; set; }
        public string? InnerMidLCL { get; set; }
        public string? InnerMidMin { get; set; }
        public string? InnerMidUCL { get; set; }
        public string? InnerMidMax { get; set; }

        // --- QUALITY MATRIX (THICKNESS) ---
        public string? ThickTarget { get; set; }
        public string? ThickTol { get; set; }
        public string? ThickLCL { get; set; }
        public string? ThickMin { get; set; }
        public string? ThickUCL { get; set; }
        public string? ThickMax { get; set; }

        // --- QUALITY MATRIX (TOTAL) ---
        public string? TotalTarget { get; set; }
        public string? TotalTol { get; set; }
        public string? TotalLCL { get; set; }
        public string? TotalMin { get; set; }
        public string? TotalUCL { get; set; }
        public string? TotalMax { get; set; }

        // === TOLERANCE SPLIT FIELDS (MIN | ASLI | MAX) ===
        
        // DIE/MATERIAL PARAMETERS
        [Display(Name = "Nipple Min")]
        public decimal? Nipple_Min { get; set; }
        [Display(Name = "Nipple Asli")]
        public decimal? Nipple_Asli { get; set; }
        [Display(Name = "Nipple Max")]
        public decimal? Nipple_Max { get; set; }

        [Display(Name = "Tube Die Min")]
        public decimal? TubeDie_Min { get; set; }
        [Display(Name = "Tube Die Asli")]
        public decimal? TubeDie_Asli { get; set; }
        [Display(Name = "Tube Die Max")]
        public decimal? TubeDie_Max { get; set; }

        [Display(Name = "Cover Die Min")]
        public decimal? CoverDie_Min { get; set; }
        [Display(Name = "Cover Die Asli")]
        public decimal? CoverDie_Asli { get; set; }
        [Display(Name = "Cover Die Max")]
        public decimal? CoverDie_Max { get; set; }

        [Display(Name = "Middle Die Min")]
        public decimal? MiddleDie_Min { get; set; }
        [Display(Name = "Middle Die Asli")]
        public decimal? MiddleDie_Asli { get; set; }
        [Display(Name = "Middle Die Max")]
        public decimal? MiddleDie_Max { get; set; }

        [Display(Name = "Spacer Die Min")]
        public decimal? SpacerDie_Min { get; set; }
        [Display(Name = "Spacer Die Asli")]
        public decimal? SpacerDie_Asli { get; set; }
        [Display(Name = "Spacer Die Max")]
        public decimal? SpacerDie_Max { get; set; }

        [Display(Name = "A Distance Min")]
        public decimal? ADistance_Min { get; set; }
        [Display(Name = "A Distance Asli")]
        public decimal? ADistance_Asli { get; set; }
        [Display(Name = "A Distance Max")]
        public decimal? ADistance_Max { get; set; }

        [Display(Name = "Mesh Dim 1 Min")]
        public decimal? MeshDim1_Min { get; set; }
        [Display(Name = "Mesh Dim 1 Asli")]
        public decimal? MeshDim1_Asli { get; set; }
        [Display(Name = "Mesh Dim 1 Max")]
        public decimal? MeshDim1_Max { get; set; }

        [Display(Name = "Mesh Dim 2 Min")]
        public decimal? MeshDim2_Min { get; set; }
        [Display(Name = "Mesh Dim 2 Asli")]
        public decimal? MeshDim2_Asli { get; set; }
        [Display(Name = "Mesh Dim 2 Max")]
        public decimal? MeshDim2_Max { get; set; }

        [Display(Name = "Mesh Dim 3 Min")]
        public decimal? MeshDim3_Min { get; set; }
        [Display(Name = "Mesh Dim 3 Asli")]
        public decimal? MeshDim3_Asli { get; set; }
        [Display(Name = "Mesh Dim 3 Max")]
        public decimal? MeshDim3_Max { get; set; }

        // TEMPERATURE PARAMETERS
        [Display(Name = "Head Temp 1 Min")]
        public decimal? HeadTemp1_Min { get; set; }
        [Display(Name = "Head Temp 1 Asli")]
        public decimal? HeadTemp1_Asli { get; set; }
        [Display(Name = "Head Temp 1 Max")]
        public decimal? HeadTemp1_Max { get; set; }

        [Display(Name = "Head Temp 2 Min")]
        public decimal? HeadTemp2_Min { get; set; }
        [Display(Name = "Head Temp 2 Asli")]
        public decimal? HeadTemp2_Asli { get; set; }
        [Display(Name = "Head Temp 2 Max")]
        public decimal? HeadTemp2_Max { get; set; }

        [Display(Name = "Head Temp 3 Min")]
        public decimal? HeadTemp3_Min { get; set; }
        [Display(Name = "Head Temp 3 Asli")]
        public decimal? HeadTemp3_Asli { get; set; }
        [Display(Name = "Head Temp 3 Max")]
        public decimal? HeadTemp3_Max { get; set; }

        [Display(Name = "Cylinder 1-1 Min")]
        public decimal? Cylinder1_1_Min { get; set; }
        [Display(Name = "Cylinder 1-1 Asli")]
        public decimal? Cylinder1_1_Asli { get; set; }
        [Display(Name = "Cylinder 1-1 Max")]
        public decimal? Cylinder1_1_Max { get; set; }

        [Display(Name = "Cylinder 1-2 Min")]
        public decimal? Cylinder1_2_Min { get; set; }
        [Display(Name = "Cylinder 1-2 Asli")]
        public decimal? Cylinder1_2_Asli { get; set; }
        [Display(Name = "Cylinder 1-2 Max")]
        public decimal? Cylinder1_2_Max { get; set; }

        [Display(Name = "Cylinder 1-3 Min")]
        public decimal? Cylinder1_3_Min { get; set; }
        [Display(Name = "Cylinder 1-3 Asli")]
        public decimal? Cylinder1_3_Asli { get; set; }
        [Display(Name = "Cylinder 1-3 Max")]
        public decimal? Cylinder1_3_Max { get; set; }

        [Display(Name = "Cylinder 2-1 Min")]
        public decimal? Cylinder2_1_Min { get; set; }
        [Display(Name = "Cylinder 2-1 Asli")]
        public decimal? Cylinder2_1_Asli { get; set; }
        [Display(Name = "Cylinder 2-1 Max")]
        public decimal? Cylinder2_1_Max { get; set; }

        [Display(Name = "Cylinder 2-2 Min")]
        public decimal? Cylinder2_2_Min { get; set; }
        [Display(Name = "Cylinder 2-2 Asli")]
        public decimal? Cylinder2_2_Asli { get; set; }
        [Display(Name = "Cylinder 2-2 Max")]
        public decimal? Cylinder2_2_Max { get; set; }

        [Display(Name = "Cylinder 2-3 Min")]
        public decimal? Cylinder2_3_Min { get; set; }
        [Display(Name = "Cylinder 2-3 Asli")]
        public decimal? Cylinder2_3_Asli { get; set; }
        [Display(Name = "Cylinder 2-3 Max")]
        public decimal? Cylinder2_3_Max { get; set; }

        [Display(Name = "Cylinder 3-1 Min")]
        public decimal? Cylinder3_1_Min { get; set; }
        [Display(Name = "Cylinder 3-1 Asli")]
        public decimal? Cylinder3_1_Asli { get; set; }
        [Display(Name = "Cylinder 3-1 Max")]
        public decimal? Cylinder3_1_Max { get; set; }

        [Display(Name = "Cylinder 3-2 Min")]
        public decimal? Cylinder3_2_Min { get; set; }
        [Display(Name = "Cylinder 3-2 Asli")]
        public decimal? Cylinder3_2_Asli { get; set; }
        [Display(Name = "Cylinder 3-2 Max")]
        public decimal? Cylinder3_2_Max { get; set; }

        [Display(Name = "Cylinder 3-3 Min")]
        public decimal? Cylinder3_3_Min { get; set; }
        [Display(Name = "Cylinder 3-3 Asli")]
        public decimal? Cylinder3_3_Asli { get; set; }
        [Display(Name = "Cylinder 3-3 Max")]
        public decimal? Cylinder3_3_Max { get; set; }

        [Display(Name = "Screw Temp 1 Min")]
        public decimal? ScrewTemp1_Min { get; set; }
        [Display(Name = "Screw Temp 1 Asli")]
        public decimal? ScrewTemp1_Asli { get; set; }
        [Display(Name = "Screw Temp 1 Max")]
        public decimal? ScrewTemp1_Max { get; set; }

        [Display(Name = "Screw Temp 2 Min")]
        public decimal? ScrewTemp2_Min { get; set; }
        [Display(Name = "Screw Temp 2 Asli")]
        public decimal? ScrewTemp2_Asli { get; set; }
        [Display(Name = "Screw Temp 2 Max")]
        public decimal? ScrewTemp2_Max { get; set; }

        [Display(Name = "Screw Temp 3 Min")]
        public decimal? ScrewTemp3_Min { get; set; }
        [Display(Name = "Screw Temp 3 Asli")]
        public decimal? ScrewTemp3_Asli { get; set; }
        [Display(Name = "Screw Temp 3 Max")]
        public decimal? ScrewTemp3_Max { get; set; }

        // SPEED/PRESSURE PARAMETERS
        [Display(Name = "Screw Speed 1 Min")]
        public decimal? ScrewSpeed1_Min { get; set; }
        [Display(Name = "Screw Speed 1 Asli")]
        public decimal? ScrewSpeed1_Asli { get; set; }
        [Display(Name = "Screw Speed 1 Max")]
        public decimal? ScrewSpeed1_Max { get; set; }

        [Display(Name = "Screw Speed 2 Min")]
        public decimal? ScrewSpeed2_Min { get; set; }
        [Display(Name = "Screw Speed 2 Asli")]
        public decimal? ScrewSpeed2_Asli { get; set; }
        [Display(Name = "Screw Speed 2 Max")]
        public decimal? ScrewSpeed2_Max { get; set; }

        [Display(Name = "Screw Speed 3 Min")]
        public decimal? ScrewSpeed3_Min { get; set; }
        [Display(Name = "Screw Speed 3 Asli")]
        public decimal? ScrewSpeed3_Asli { get; set; }
        [Display(Name = "Screw Speed 3 Max")]
        public decimal? ScrewSpeed3_Max { get; set; }

        [Display(Name = "Pressure 1 Min")]
        public decimal? Pressure1_Min { get; set; }
        [Display(Name = "Pressure 1 Asli")]
        public decimal? Pressure1_Asli { get; set; }
        [Display(Name = "Pressure 1 Max")]
        public decimal? Pressure1_Max { get; set; }

        [Display(Name = "Pressure 2 Min")]
        public decimal? Pressure2_Min { get; set; }
        [Display(Name = "Pressure 2 Asli")]
        public decimal? Pressure2_Asli { get; set; }
        [Display(Name = "Pressure 2 Max")]
        public decimal? Pressure2_Max { get; set; }

        [Display(Name = "Pressure 3 Min")]
        public decimal? Pressure3_Min { get; set; }
        [Display(Name = "Pressure 3 Asli")]
        public decimal? Pressure3_Asli { get; set; }
        [Display(Name = "Pressure 3 Max")]
        public decimal? Pressure3_Max { get; set; }

        [Display(Name = "Hose Speed Min")]
        public decimal? HoseSpeed_Min { get; set; }
        [Display(Name = "Hose Speed Asli")]
        public decimal? HoseSpeed_Asli { get; set; }
        [Display(Name = "Hose Speed Max")]
        public decimal? HoseSpeed_Max { get; set; }

        [Display(Name = "Take Up Conveyor Speed Min")]
        public decimal? TakeUpConveyorSpeed_Min { get; set; }
        [Display(Name = "Take Up Conveyor Speed Asli")]
        public decimal? TakeUpConveyorSpeed_Asli { get; set; }
        [Display(Name = "Take Up Conveyor Speed Max")]
        public decimal? TakeUpConveyorSpeed_Max { get; set; }

        [Display(Name = "Chiller Water Temp Min")]
        public decimal? ChillerWaterTemp_Min { get; set; }
        [Display(Name = "Chiller Water Temp Asli")]
        public decimal? ChillerWaterTemp_Asli { get; set; }
        [Display(Name = "Chiller Water Temp Max")]
        public decimal? ChillerWaterTemp_Max { get; set; }

        // FEED/RATIO PARAMETERS
        [Display(Name = "Feed 1 Min")]
        public decimal? Feed1_Min { get; set; }
        [Display(Name = "Feed 1 Asli")]
        public decimal? Feed1_Asli { get; set; }
        [Display(Name = "Feed 1 Max")]
        public decimal? Feed1_Max { get; set; }

        [Display(Name = "Feed 2 Min")]
        public decimal? Feed2_Min { get; set; }
        [Display(Name = "Feed 2 Asli")]
        public decimal? Feed2_Asli { get; set; }
        [Display(Name = "Feed 2 Max")]
        public decimal? Feed2_Max { get; set; }

        [Display(Name = "Feed 3 Min")]
        public decimal? Feed3_Min { get; set; }
        [Display(Name = "Feed 3 Asli")]
        public decimal? Feed3_Asli { get; set; }
        [Display(Name = "Feed 3 Max")]
        public decimal? Feed3_Max { get; set; }

        [Display(Name = "Feed Roll Ratio 1 Min")]
        public decimal? FeedRollRatio1_Min { get; set; }
        [Display(Name = "Feed Roll Ratio 1 Asli")]
        public decimal? FeedRollRatio1_Asli { get; set; }
        [Display(Name = "Feed Roll Ratio 1 Max")]
        public decimal? FeedRollRatio1_Max { get; set; }

        [Display(Name = "Feed Roll Ratio 2 Min")]
        public decimal? FeedRollRatio2_Min { get; set; }
        [Display(Name = "Feed Roll Ratio 2 Asli")]
        public decimal? FeedRollRatio2_Asli { get; set; }
        [Display(Name = "Feed Roll Ratio 2 Max")]
        public decimal? FeedRollRatio2_Max { get; set; }

        [Display(Name = "Feed Roll Ratio 3 Min")]
        public decimal? FeedRollRatio3_Min { get; set; }
        [Display(Name = "Feed Roll Ratio 3 Asli")]
        public decimal? FeedRollRatio3_Asli { get; set; }
        [Display(Name = "Feed Roll Ratio 3 Max")]
        public decimal? FeedRollRatio3_Max { get; set; }

        // GAP/POSITION PARAMETERS
        [Display(Name = "Caterpillar Gap Min")]
        public decimal? CaterpillarGap_Min { get; set; }
        [Display(Name = "Caterpillar Gap Asli")]
        public decimal? CaterpillarGap_Asli { get; set; }
        [Display(Name = "Caterpillar Gap Max")]
        public decimal? CaterpillarGap_Max { get; set; }

        [Display(Name = "Spiral Speed Min")]
        public decimal? SpiralSpeed_Min { get; set; }
        [Display(Name = "Spiral Speed Asli")]
        public decimal? SpiralSpeed_Asli { get; set; }
        [Display(Name = "Spiral Speed Max")]
        public decimal? SpiralSpeed_Max { get; set; }

        // THICKNESS PARAMETERS
        [Display(Name = "Tebal Inner Min")]
        public decimal? TebalInner_Min { get; set; }
        [Display(Name = "Tebal Inner Asli")]
        public decimal? TebalInner_Asli { get; set; }
        [Display(Name = "Tebal Inner Max")]
        public decimal? TebalInner_Max { get; set; }

        [Display(Name = "Tebal Outer Min")]
        public decimal? TebalOuter_Min { get; set; }
        [Display(Name = "Tebal Outer Asli")]
        public decimal? TebalOuter_Asli { get; set; }
        [Display(Name = "Tebal Outer Max")]
        public decimal? TebalOuter_Max { get; set; }

        [Display(Name = "Tebal Total Min")]
        public decimal? TebalTotal_Min { get; set; }
        [Display(Name = "Tebal Total Asli")]
        public decimal? TebalTotal_Asli { get; set; }
        [Display(Name = "Tebal Total Max")]
        public decimal? TebalTotal_Max { get; set; }

        [Display(Name = "Tebal Inner+Middle Min")]
        public decimal? TebalInnerMiddle_Min { get; set; }
        [Display(Name = "Tebal Inner+Middle Asli")]
        public decimal? TebalInnerMiddle_Asli { get; set; }
        [Display(Name = "Tebal Inner+Middle Max")]
        public decimal? TebalInnerMiddle_Max { get; set; }

        // ==================== NEW TOLERANCE FIELDS (13 fields × 3) ====================
        
        // Ampere Meter Parameters
        [Display(Name = "Am Meter 1 Min")]
        public decimal? AmMeter_Min { get; set; }
        [Display(Name = "Am Meter 1 Asli")]
        public decimal? AmMeter_Asli { get; set; }
        [Display(Name = "Am Meter 1 Max")]
        public decimal? AmMeter_Max { get; set; }

        [Display(Name = "Am Meter 2 Min")]
        public decimal? AmMeter2_Min { get; set; }
        [Display(Name = "Am Meter 2 Asli")]
        public decimal? AmMeter2_Asli { get; set; }
        [Display(Name = "Am Meter 2 Max")]
        public decimal? AmMeter2_Max { get; set; }

        [Display(Name = "Am Meter 3 Min")]
        public decimal? AmMeter3_Min { get; set; }
        [Display(Name = "Am Meter 3 Asli")]
        public decimal? AmMeter3_Asli { get; set; }
        [Display(Name = "Am Meter 3 Max")]
        public decimal? AmMeter3_Max { get; set; }

        // Control Parameters
        [Display(Name = "Preset Value Min")]
        public decimal? PresetValue_Min { get; set; }
        [Display(Name = "Preset Value Asli")]
        public decimal? PresetValue_Asli { get; set; }
        [Display(Name = "Preset Value Max")]
        public decimal? PresetValue_Max { get; set; }

        [Display(Name = "Control Value Min")]
        public decimal? ControlValue_Min { get; set; }
        [Display(Name = "Control Value Asli")]
        public decimal? ControlValue_Asli { get; set; }
        [Display(Name = "Control Value Max")]
        public decimal? ControlValue_Max { get; set; }

        // Spiral Pitch Parameters
        [Display(Name = "Spiral Pitch Setting Min")]
        public decimal? SpiralPitchSetting_Min { get; set; }
        [Display(Name = "Spiral Pitch Setting Asli")]
        public decimal? SpiralPitchSetting_Asli { get; set; }
        [Display(Name = "Spiral Pitch Setting Max")]
        public decimal? SpiralPitchSetting_Max { get; set; }

        [Display(Name = "Spiral Pitch Display Min")]
        public decimal? SpiralPitchDisplay_Min { get; set; }
        [Display(Name = "Spiral Pitch Display Asli")]
        public decimal? SpiralPitchDisplay_Asli { get; set; }
        [Display(Name = "Spiral Pitch Display Max")]
        public decimal? SpiralPitchDisplay_Max { get; set; }

        // Conveyor Speed Parameters
        [Display(Name = "Cool Conveyor Speed 1 Min")]
        public decimal? CoolConveyorSpeed_Min { get; set; }
        [Display(Name = "Cool Conveyor Speed 1 Asli")]
        public decimal? CoolConveyorSpeed_Asli { get; set; }
        [Display(Name = "Cool Conveyor Speed 1 Max")]
        public decimal? CoolConveyorSpeed_Max { get; set; }

        [Display(Name = "Cool Conveyor Speed 2 Min")]
        public decimal? CoolConveyorSpeed2_Min { get; set; }
        [Display(Name = "Cool Conveyor Speed 2 Asli")]
        public decimal? CoolConveyorSpeed2_Asli { get; set; }
        [Display(Name = "Cool Conveyor Speed 2 Max")]
        public decimal? CoolConveyorSpeed2_Max { get; set; }

        [Display(Name = "Conveyor Ratio Min")]
        public decimal? ConveyorRatio_Min { get; set; }
        [Display(Name = "Conveyor Ratio Asli")]
        public decimal? ConveyorRatio_Asli { get; set; }
        [Display(Name = "Conveyor Ratio Max")]
        public decimal? ConveyorRatio_Max { get; set; }

        // Tolerance Parameters
        [Display(Name = "Tolerance Inner Min")]
        public decimal? ToleranceInner_Min { get; set; }
        [Display(Name = "Tolerance Inner Asli")]
        public decimal? ToleranceInner_Asli { get; set; }
        [Display(Name = "Tolerance Inner Max")]
        public decimal? ToleranceInner_Max { get; set; }

        [Display(Name = "Tolerance Outer Min")]
        public decimal? ToleranceOuter_Min { get; set; }
        [Display(Name = "Tolerance Outer Asli")]
        public decimal? ToleranceOuter_Asli { get; set; }
        [Display(Name = "Tolerance Outer Max")]
        public decimal? ToleranceOuter_Max { get; set; }

        [Display(Name = "Tolerance Spiral Pitch Min")]
        public decimal? ToleranceSpiralPitch_Min { get; set; }
        [Display(Name = "Tolerance Spiral Pitch Asli")]
        public decimal? ToleranceSpiralPitch_Asli { get; set; }
        [Display(Name = "Tolerance Spiral Pitch Max")]
        public decimal? ToleranceSpiralPitch_Max { get; set; }

        [Display(Name = "Selisih Tebal Min")]
        public decimal? SelisihTebal_Min { get; set; }
        [Display(Name = "Selisih Tebal Asli")]
        public decimal? SelisihTebal_Asli { get; set; }
        [Display(Name = "Selisih Tebal Max")]
        public decimal? SelisihTebal_Max { get; set; }

        // NEW BATCH 3: Pitch Yarn & Dancer
        [Display(Name = "Pitch Yarn Min")]
        public decimal? PitchYarn_Min { get; set; }
        [Display(Name = "Pitch Yarn Asli")]
        public decimal? PitchYarn_Asli { get; set; }
        [Display(Name = "Pitch Yarn Max")]
        public decimal? PitchYarn_Max { get; set; }

        [Display(Name = "Dancer Position Min")]
        public decimal? DancerPosition_Min { get; set; }
        [Display(Name = "Dancer Position Asli")]
        public decimal? DancerPosition_Asli { get; set; }
        [Display(Name = "Dancer Position Max")]
        public decimal? DancerPosition_Max { get; set; }

        // NEW BATCH 4: OD Sensor & Cutting Speed
        [Display(Name = "OD Sensor Min")]
        public decimal? OdSensor_Min { get; set; }
        [Display(Name = "OD Sensor Asli")]
        public decimal? OdSensor_Asli { get; set; }
        [Display(Name = "OD Sensor Max")]
        public decimal? OdSensor_Max { get; set; }

        [Display(Name = "Cutting Speed Min")]
        public decimal? CuttingSpeed_Min { get; set; }
        [Display(Name = "Cutting Speed Asli")]
        public decimal? CuttingSpeed_Asli { get; set; }
        [Display(Name = "Cutting Speed Max")]
        public decimal? CuttingSpeed_Max { get; set; }
    }
}
