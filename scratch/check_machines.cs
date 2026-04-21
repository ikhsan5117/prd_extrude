using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace DbCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=10.14.149.34;Database=prd_extrude_hose;User ID=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True")
                .Options;

            using var context = new ApplicationDbContext(options);
            
            Console.WriteLine("=== ANALISIS DATA STANDAR PER MESIN ===\n");
            
            // 1. Dari MasterlistSpsDoubleLayers
            var masterlistGroups = context.MasterlistSpsDoubleLayers
                .GroupBy(m => m.Machine ?? m.MachineCode ?? "Unknown")
                .Select(g => new { Machine = g.Key, Count = g.Count() })
                .ToList();
                
            Console.WriteLine("Table: MasterlistSpsDoubleLayers");
            foreach(var g in masterlistGroups) {
                Console.WriteLine($"- Mesin: {g.Machine,-25} | Standar: {g.Count} item");
            }
            
            // 2. Dari StandardParameterSettings
            var spsGroups = context.StandardParameterSettings
                .GroupBy(s => s.MachineCode ?? "Unknown")
                .Select(g => new { Machine = g.Key, Count = g.Count() })
                .ToList();
                
            Console.WriteLine("\nTable: StandardParameterSettings (SPS Parameter)");
            foreach(var g in spsGroups) {
                Console.WriteLine($"- Mesin: {g.Machine,-25} | Standar: {g.Count} item");
            }

            // Sample item with long name for confirmation
            var sample = context.MasterlistSpsDoubleLayers
                .Where(m => m.ItemList != null && m.ItemList.Contains("VHFUNC"))
                .Take(3)
                .ToList();
                
            if(sample.Any()){
                Console.WriteLine("\nSample Nama Lengkap di Masterlist:");
                foreach(var s in sample) Console.WriteLine($"- {s.ItemList}");
            }
        }
    }
}
