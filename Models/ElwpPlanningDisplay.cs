namespace VelastoProductionSystem.Models
{
    public class ElwpPlanningDisplay
    {
        public int Id { get; set; }
        public DateTime? DateValue { get; set; }
        public string? DateString { get; set; }
        public string? MachineName { get; set; }
        public string? Shift { get; set; }
        public string? KodeItem { get; set; }
        public string? PartName { get; set; }
        public string? PnSap { get; set; }
        public int? QtyPlanning { get; set; }
    }
}
