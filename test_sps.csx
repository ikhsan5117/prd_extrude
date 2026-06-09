using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;

var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionsBuilder.UseSqlServer("Server=10.14.149.34,1433;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;");
using var _context = new ApplicationDbContext(optionsBuilder.Options);

var report = _context.ProductionReports.Find(1433);
if (report == null) {
    Console.WriteLine("Report 1433 not found!");
    return;
}

Console.WriteLine($"Report: ItemCode={report.ItemCode}, DocumentNumber={report.DocumentNumber}");

var sps = _context.SpsNoDocs
    .OrderByDescending(s => s.DocumentNumber)
    .FirstOrDefault(s => s.DocumentNumber == report.ItemCode || s.DocumentNumber == report.DocumentNumber);

if (sps == null) {
    Console.WriteLine("sps is NULL!");
} else {
    Console.WriteLine($"Found SPS: {sps.DocumentNumber}");
    Console.WriteLine($"OdSensor_Asli: {sps.OdSensor_Asli}");
}
