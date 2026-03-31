using System;

namespace VelastoProductionSystem.Models
{
    public class ShiftMaster
    {
        public int Id { get; set; }
        public string ShiftName { get; set; }
        public string StartTime { get; set; } // e.g. "07:00"
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
