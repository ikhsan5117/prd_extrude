using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace VelastoProductionSystem.Scratch
{
    public class CheckDb
    {
        public static void Main()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(config.GetConnectionString("DefaultConnection"))
                .Options;

            var elwpOptions = new DbContextOptionsBuilder<ElwpDbContext>()
                .UseSqlite(config.GetConnectionString("ElwpConnection"))
                .Options;

            using var context = new ApplicationDbContext(options);
            using var elwpContext = new ElwpDbContext(elwpOptions);

            Console.WriteLine("--- ELWP Plannings ---");
            var plannings = elwpContext.ElwpPlannings.ToList();
            foreach (var p in plannings)
            {
                Console.WriteLine($"ID: {p.Id}, Date: {p.TanggalPlanning:yyyy-MM-dd}, Shift: {p.Shift}, Item: {p.KodeItem}");
            }

            Console.WriteLine("\n--- Masterlist Standards ---");
            var masters = context.MasterlistSpsDoubleLayers.Where(m => m.ItemList.StartsWith("SPS-FH") || m.ItemList.StartsWith("BH-2026")).ToList();
            foreach (var m in masters)
            {
                Console.WriteLine($"Item: {m.ItemList}, Hose: {m.HoseType}");
            }
        }
    }
}
