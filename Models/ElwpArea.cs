using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VelastoProductionSystem.Models
{
    public class ElwpArea
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int PlantId { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? NoUrut { get; set; }
    }
}
