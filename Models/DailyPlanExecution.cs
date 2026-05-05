using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VelastoProductionSystem.Models
{
    public class DailyPlanExecution
    {
        public int Id { get; set; }

        [Display(Name = "Machine")]
        public string? MachineName { get; set; } 
        
        [Display(Name = "Tanggal")]
        public DateTime ExecutionDate { get; set; }
        
        [Display(Name = "Shift")]
        public string? Shift { get; set; } 
        
        [Display(Name = "Group Name")]
        public string? GroupName { get; set; } 

        // Footer inputs
        [Display(Name = "PIC 1")]
        public string? Pic1 { get; set; }
        [Display(Name = "PIC 2")]
        public string? Pic2 { get; set; }
        
        [Display(Name = "Line Stop Note")]
        public string? LineStopNote { get; set; }
        
        [Display(Name = "Line Stop (Menit)")]
        public int? LineStopMinutes { get; set; }
        
        [Display(Name = "Start Mesin")]
        public string? StartMesin { get; set; } 
        
        [Display(Name = "Finish Mesin")]
        public string? FinishMesin { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public List<DailyPlanActivity> Activities { get; set; } = new List<DailyPlanActivity>();
    }

    public class DailyPlanActivity
    {
        public int Id { get; set; }
        public int DailyPlanExecutionId { get; set; }
        
        [ForeignKey("DailyPlanExecutionId")]
        public DailyPlanExecution? DailyPlanExecution { get; set; }

        public string? PartName1 { get; set; } // Aktivitas (Dandori/Part Model)
        public string? PartName2 { get; set; } // VIN Code (NA1610 dll)
        
        public int? PlanQty { get; set; }
        public int? PlanDurationMinutes { get; set; }
        public string? PlanStart { get; set; }
        public string? PlanEnd { get; set; }

        public int? ActualQty { get; set; }
        public int? ActualDurationMinutes { get; set; }
        public string? ActualStart { get; set; }
        public string? ActualEnd { get; set; }
        
        public string? Remarks { get; set; }
        public string? StopReason { get; set; }
        public int OrderIndex { get; set; }
    }
}
