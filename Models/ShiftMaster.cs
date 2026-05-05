using System;

namespace VelastoProductionSystem.Models
{
    public class ShiftMaster
    {
        public int Id { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty; // e.g. "07:00"
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
