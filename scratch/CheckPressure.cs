using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Models;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TempScript
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<VelastoProductionSystem.Data.ApplicationDbContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            using var context = new VelastoProductionSystem.Data.ApplicationDbContext(optionsBuilder.Options);

            var items = context.MasterlistSpsDoubleLayers
                .Where(m => m.ExcelId.Contains("CHS 2 LAYER"))
                .Take(5)
                .ToList();

            foreach (var item in items)
            {
                Console.WriteLine($"ID: {item.ExcelId} | Item: {item.ItemList} | P1: {item.Pressure1} | P2: {item.Pressure2} | CV: {item.CurrentValue} | AM: {item.AmMeter}");
            }
        }
    }
}
