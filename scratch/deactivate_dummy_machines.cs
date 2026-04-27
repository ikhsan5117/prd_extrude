
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var optionsBuilder = new DbContextOptionsBuilder<ElwpDbContext>();
optionsBuilder.UseSqlServer(configuration.GetConnectionString("ElwpConnection"));

using var context = new ElwpDbContext(optionsBuilder.Options);

var machinesToDeactivate = context.ElwpMachines
    .Where(m => m.KodeMesin == "DL01" || m.KodeMesin == "DL02")
    .ToList();

foreach (var m in machinesToDeactivate)
{
    m.IsActive = false;
    Console.WriteLine($"Deactivating machine: {m.KodeMesin}");
}

context.SaveChanges();
Console.WriteLine("Cleanup completed.");
