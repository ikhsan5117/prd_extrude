using System.ComponentModel.DataAnnotations;

namespace VelastoProductionSystem.Models
{
    public class StandardParameterSetting
    {
        public int Id { get; set; }

        [Display(Name = "No. Dokumen")]
        public string? DocumentNumber { get; set; } = string.Empty;

        [Display(Name = "No. Revisi")]
        public int RevisionNumber { get; set; }

        [Display(Name = "Customer")]
        public string? CustomerName { get; set; } = string.Empty;

        [Display(Name = "CHS 2 Layer")]
        public string? LayerType { get; set; } = "Default";

        [Display(Name = "Tanggal Efektif")]
        public DateTime EffectiveDate { get; set; }

        // Hose Specifications
        [Display(Name = "Tipe Hose")]
        public string? HoseType { get; set; } = string.Empty;

        [Display(Name = "Diameter")]
        public string? Diameter { get; set; } = string.Empty;

        [Display(Name = "Kode Produk")]
        public string? ProductCode { get; set; } = string.Empty;

        // Material Specifications
        [Display(Name = "Material Inner")]
        public string? InnerMaterial { get; set; } = string.Empty;

        [Display(Name = "Material Outer")]
        public string? OuterMaterial { get; set; } = string.Empty;

        [Display(Name = "Yarn Type")]
        public string? YarnType { get; set; } = string.Empty;

        // Die Dimensions
        [Display(Name = "Inner Die (mm)")]
        public decimal InnerDie { get; set; }

        [Display(Name = "Outer Die (mm)")]
        public decimal OuterDie { get; set; }

        [Display(Name = "Tube Die (mm)")]
        public decimal TubeDie { get; set; }

        [Display(Name = "Middle Die (mm)")]
        public decimal MiddleDie { get; set; }

        [Display(Name = "Cover Die (mm)")]
        public decimal CoverDie { get; set; }

        [Display(Name = "Spacer Die (mm)")]
        public decimal SpacerDie { get; set; }

        [Display(Name = "Tolerance Die (mm)")]
        public decimal ToleranceDie { get; set; }

        [Display(Name = "Tol Tube Die (mm)")]
        public decimal Tol_TubeDie { get; set; }

        [Display(Name = "Tol Middle Die (mm)")]
        public decimal Tol_MiddleDie { get; set; }

        [Display(Name = "Tol Outer Die (mm)")]
        public decimal Tol_OuterDie { get; set; }

        [Display(Name = "Tol Cover Die (mm)")]
        public decimal Tol_CoverDie { get; set; }

        [Display(Name = "Tol Spiral Pitch")]
        public decimal Tol_SpiralPitch { get; set; }

        // Mesh Screen
        [Display(Name = "Mesh Screen")]
        public string MeshScreen { get; set; } = string.Empty;

        // Temperature Settings
        [Display(Name = "Head Temp (°C)")]
        public int HeadTemp { get; set; }

        [Display(Name = "Cylinder 1 (°C)")]
        public int Cylinder1Temp { get; set; }

        [Display(Name = "Cylinder 2 (°C)")]
        public int Cylinder2Temp { get; set; }

        [Display(Name = "Cylinder 3 (°C)")]
        public int Cylinder3Temp { get; set; }

        [Display(Name = "Screw Temp (°C)")]
        public int ScrewTemp { get; set; }

        // Speed Settings
        [Display(Name = "Screw Speed (rpm)")]
        public decimal ScrewSpeed { get; set; }

        [Display(Name = "Feed Roll Ratio (%)")]
        public decimal FeedRollRatio { get; set; }

        // Pressure Settings
        [Display(Name = "Pressure (MPa)")]
        public decimal Pressure { get; set; }

        // Air Pressure
        [Display(Name = "Air Pressure (A)")]
        public decimal AirPressureA { get; set; }

        // Preset Values
        [Display(Name = "Preset Valve (mm)")]
        public decimal PresetValve { get; set; }

        // Spiral Settings
        [Display(Name = "Spiral Speed (mm)")]
        public decimal SpiralSpeed { get; set; }

        [Display(Name = "Spiral Pitch")]
        public decimal SpiralPitch { get; set; }

        // Display Settings
        [Display(Name = "Spiral Speed Display")]
        public decimal SpiralSpeedDisplay { get; set; }

        [Display(Name = "Spiral Pitch Display")]
        public decimal SpiralPitchDisplay { get; set; }

        // Control Values
        [Display(Name = "Preset Temp (°C)")]
        public decimal PresetTemp { get; set; }

        [Display(Name = "Control Value (Ø mm)")]
        public string ControlValue { get; set; } = string.Empty;

        [Display(Name = "Hose Speed (m/min)")]
        public string HoseSpeed { get; set; } = string.Empty;

        // Conveyor Settings
        [Display(Name = "Take-up Conveyor Speed (m/min)")]
        public string TakeupConveyorSpeed { get; set; } = string.Empty;

        [Display(Name = "Cool Conveyor Speed (m/min)")]
        public string CoolConveyorSpeed { get; set; } = string.Empty;

        [Display(Name = "Conveyor Ratio")]
        public string ConveyorRatio { get; set; } = string.Empty;

        // Un-smooth Surface
        [Display(Name = "Un-smooth Surface")]
        public decimal UnsmoothSurface { get; set; }

        // Chiller Settings
        [Display(Name = "Chiller Water Temp (°C)")]
        public string ChillerWaterTemp { get; set; } = string.Empty;

        // Caterpillar Gap
        [Display(Name = "Caterpillar Gap (mm)")]
        public string CaterpillarGap { get; set; } = string.Empty;

        // Marking Material
        [Display(Name = "Marking Material Color")]
        public string MarkingMaterialColor { get; set; } = string.Empty;

        [Display(Name = "Marking Material Inner (mm)")]
        public decimal MarkingMaterialInner { get; set; }

        [Display(Name = "Marking Material Outer (mm)")]
        public decimal MarkingMaterialOuter { get; set; }


        [Display(Name = "ITEM (Part List)")]
        public string? ItemList { get; set; }

        [Display(Name = "MC")]
        public string? MachineCode { get; set; }

        // Status
        [Display(Name = "Status")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Dibuat Oleh")]
        public string CreatedBy { get; set; } = string.Empty;

        [Display(Name = "Tanggal Dibuat")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Disetujui Oleh")]
        public string? ApprovedBy { get; set; }

        [Display(Name = "Tanggal Disetujui")]
        public DateTime? ApprovedDate { get; set; }
    }
}
