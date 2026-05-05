using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VelastoProductionSystem.Models
{
    public class MasterlistSpsDoubleLayer
    {
        public int Id { get; set; }

        [Display(Name = "ID Excel")]
        public string? ExcelId { get; set; }

        [Display(Name = "No")]
        public string? No { get; set; }

        [Display(Name = "Machine")]
        public string? Machine { get; set; }

        [Display(Name = "No. Document")]
        public string? DocumentNumber { get; set; }

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

        [Display(Name = "± Inner")]
        public string? ToleranceInner { get; set; }

        [Display(Name = "± Outer")]
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
    }
}
