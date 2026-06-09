using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;

var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=VelastoProductionSystem;Trusted_Connection=True;")
    .Options;

using var db = new ApplicationDbContext(options);
var sps = db.SpsNoDocs.FirstOrDefault(s => s.InnerTarget != null && s.DocumentNumber == "SPS-003"); // Or any just take first
if (sps == null) sps = db.SpsNoDocs.FirstOrDefault(s => s.InnerTarget != null);

Console.WriteLine($"Doc: {sps?.DocumentNumber}");
Console.WriteLine($"InnerTarget (DB): {sps?.InnerTarget}");
Console.WriteLine($"ThickTarget (DB): {sps?.ThickTarget}");
