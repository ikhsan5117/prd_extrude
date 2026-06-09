using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;

var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=VelastoProductionSystem;Trusted_Connection=True;");
using (var context = new ApplicationDbContext(optionsBuilder.Options))
{
    var sps = context.SpsNoDocs.FirstOrDefault(s => s.DocumentNumber == "VI-SOP-PROD-132");
    if (sps != null)
    {
        Console.WriteLine($"Dimensi: {sps.Dimensi}");
        Console.WriteLine($"InnerTarget: {sps.InnerTarget}");
        Console.WriteLine($"ThickTarget: {sps.ThickTarget}");
        Console.WriteLine($"TebalInner_Asli: {sps.TebalInner_Asli}");
        Console.WriteLine($"ToleranceInner_Asli: {sps.ToleranceInner_Asli}");
        Console.WriteLine($"TebalInner: {sps.TebalInner}");
        Console.WriteLine($"ToleranceInner: {sps.ToleranceInner}");
    }
}
