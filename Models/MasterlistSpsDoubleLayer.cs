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

        [Display(Name = "Yarn")]
        public string? Yarn { get; set; }

        [Display(Name = "Use limits of the material | #Inner")]
        public string? UseLimitsInner { get; set; }

        [Display(Name = "Use limits of the material | #Outer")]
        public string? UseLimitsOuter { get; set; }

        [Display(Name = "Nipple | #material")]
        public string? Nipple { get; set; }

        [Display(Name = "Tube Die | #material")]
        public string? TubeDie { get; set; }

        [Display(Name = "Cover Die | #material")]
        public string? CoverDie { get; set; }

        [Display(Name = "Mesh screen | #material 1")]
        public string? MeshScreen1 { get; set; }

        [Display(Name = "Mesh screen | #material 2")]
        public string? MeshScreen2 { get; set; }

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

        [Display(Name = "Tebal Outer")]
        public string? TebalOuter { get; set; }

        [Display(Name = "Tebal Total")]
        public string? TebalTotal { get; set; }

        [Display(Name = "Selisih tebal")]
        public string? SelisihTebal { get; set; }

        [Display(Name = "ITEM (Part List)")]
        public string? ItemList { get; set; }

        [Display(Name = "MC")]
        public string? MachineCode { get; set; }
    }
}
