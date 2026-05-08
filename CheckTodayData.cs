using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Utilities
{
    /// <summary>
    /// Quick data check script - utility class for debugging production data
    /// </summary>
    public static class CheckTodayData
    {
        public static void Run()
        {
            // Quick data check script
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=.;Database=VelastoDb;Integrated Security=true;TrustServerCertificate=True;");

            using (var context = new ApplicationDbContext(optionsBuilder.Options))
            {
                var today = DateTime.Today;
                var endOfDay = today.AddDays(1).AddTicks(-1);
                
                // Cek ProductionReport hari ini
                var reportsToday = context.ProductionReports
                    .Where(r => r.CreatedDate >= today && r.CreatedDate <= endOfDay)
                    .OrderByDescending(r => r.CreatedDate)
                    .ToList();
                
                Console.WriteLine("=== PRODUCTION REPORTS TODAY ===");
                Console.WriteLine($"Total reports: {reportsToday.Count}");
                foreach (var report in reportsToday.Take(5))
                {
                    Console.WriteLine($"\n  DocumentNumber: {report.DocumentNumber}");
                    Console.WriteLine($"  CreatedDate: {report.CreatedDate:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine($"  HoseType: {report.HoseType}");
                    Console.WriteLine($"  Shift: {report.Shift}");
                    Console.WriteLine($"  Status: {report.Status}");
                    
                    // Cek ProductionReadings untuk report ini
                    var readings = context.ProductionReadings
                        .Where(pr => pr.ProductionReportId == report.Id)
                        .OrderBy(pr => pr.ReadingTime)
                        .ToList();
                    
                    Console.WriteLine($"  Readings Count: {readings.Count}");
                    if (readings.Count > 0)
                    {
                        Console.WriteLine($"    First Reading: {readings.First().ReadingTime:HH:mm:ss}");
                        Console.WriteLine($"    Last Reading: {readings.Last().ReadingTime:HH:mm:ss}");
                        Console.WriteLine($"    HeadTempInner sample: {readings.First().HeadTempInner}°C");
                    }
                }
                
                Console.WriteLine("\n\n=== LATEST PRODUCTION READING ===");
                var latestReading = context.ProductionReadings
                    .Where(r => r.ProductionReport!.CreatedDate >= today)
                    .OrderByDescending(r => r.ReadingTime)
                    .FirstOrDefault();
                
                if (latestReading != null)
                {
                    Console.WriteLine($"ReadingTime: {latestReading.ReadingTime:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine($"Report: {latestReading.ProductionReport?.DocumentNumber}");
                    Console.WriteLine($"Temperatures (Inner): {latestReading.HeadTempInner}, {latestReading.Cylinder1TempInner}, {latestReading.Cylinder2TempInner}, {latestReading.Cylinder3TempInner}, {latestReading.ScrewTempInner}");
                    Console.WriteLine($"Speeds (Inner): {latestReading.ScrewSpeedInner} rpm, Pressure: {latestReading.PressureInner} MPa");
                }
            }
        }
    }
}
