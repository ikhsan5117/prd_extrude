
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CleanupApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../"))
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ElwpDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("ElwpConnection"));

            using var context = new ElwpDbContext(optionsBuilder.Options);

            var machinesToDeactivate = context.ElwpMachines
                .Where(m => m.KodeMesin == "DL01" || m.KodeMesin == "DL02")
                .ToList();

            if (machinesToDeactivate.Any())
            {
                foreach (var m in machinesToDeactivate)
                {
                    m.IsActive = false;
                    Console.WriteLine($"Deactivating machine: {m.KodeMesin}");
                }
                context.SaveChanges();
                Console.WriteLine("Cleanup completed successfully.");
            }
            else
            {
                Console.WriteLine("No dummy machines (DL01/DL02) found to deactivate.");
            }
        }
    }
}
