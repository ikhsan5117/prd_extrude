
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

var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

using var context = new ApplicationDbContext(optionsBuilder.Options);

var machines = context.Machines.ToList();
Console.WriteLine("ID | Code | Name | Status");
Console.WriteLine("-------------------------");
foreach (var m in machines)
{
    Console.WriteLine($"{m.Id} | {m.MachineCode} | {m.MachineName} | {m.Status}");
}
