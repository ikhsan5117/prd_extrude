using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;

namespace VelastoProductionSystem.Utilities
{
    /// <summary>
    /// Query script to verify Chart Analysis data exists in database
    /// Searches for: TA2700 - HOSE, WATER BY-PASS with operator Abdul Rohman
    /// </summary>
    public static class QueryChartData
    {
        public static void VerifyData()
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer("Server=10.14.149.34;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;MultipleActiveResultSets=True;");

                using (var context = new ApplicationDbContext(optionsBuilder.Options))
                {
                    // Search for the specific report shown in screenshot:
                    // TA2700 - HOSE, WATER BY-PASS with Abdul Rohman operator
                    var reports = context.ProductionReports
                        .Where(r => 
                            (r.ItemCode != null && r.ItemCode.Contains("TA2700")) &&
                            (r.HoseType != null && r.HoseType.Contains("WATER")) &&
                            (r.CreatedBy != null && r.CreatedBy.ToUpper().Contains("ABDUL"))
                        )
                        .OrderByDescending(r => r.CreatedDate)
                        .Take(10)
                        .ToList();

                    Console.WriteLine("=== CHART ANALYSIS DATA VERIFICATION ===\n");
                    Console.WriteLine($"Looking for: TA2700 - HOSE, WATER BY-PASS | Operator: Abdul Rohman\n");
                    Console.WriteLine($"Results found: {reports.Count}\n");

                    if (reports.Count == 0)
                    {
                        Console.WriteLine("❌ NO MATCHING RECORDS FOUND");
                        Console.WriteLine("\nTrying broader search...\n");

                        // Broader search
                        var allReports = context.ProductionReports
                            .Where(r => 
                                r.ItemCode != null && r.ItemCode.Contains("2700")
                            )
                            .OrderByDescending(r => r.CreatedDate)
                            .Take(20)
                            .ToList();

                        Console.WriteLine($"Found {allReports.Count} reports with '2700' in ItemCode:");
                        foreach (var report in allReports)
                        {
                            var readingCount = context.ProductionReadings
                                .Where(pr => pr.ProductionReportId == report.Id)
                                .Count();
                            
                            Console.WriteLine($"\n  Doc#: {report.DocumentNumber}");
                            Console.WriteLine($"  ItemCode: {report.ItemCode}");
                            Console.WriteLine($"  HoseType: {report.HoseType}");
                            Console.WriteLine($"  Machine: {report.MachineName}");
                            Console.WriteLine($"  Dimension: {report.Dimension}");
                            Console.WriteLine($"  Shift: {report.Shift}");
                            Console.WriteLine($"  CreatedBy: {report.CreatedBy}");
                            Console.WriteLine($"  CreatedDate: {report.CreatedDate:yyyy-MM-dd HH:mm:ss}");
                            Console.WriteLine($"  ProductionDate: {report.ProductionDate:yyyy-MM-dd}");
                            Console.WriteLine($"  Status: {report.Status}");
                            Console.WriteLine($"  Reading Points: {readingCount}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("✅ MATCHING RECORDS FOUND:\n");
                        foreach (var report in reports)
                        {
                            var readings = context.ProductionReadings
                                .Where(pr => pr.ProductionReportId == report.Id)
                                .OrderBy(pr => pr.ReadingTime)
                                .ToList();

                            Console.WriteLine($"📋 Document: {report.DocumentNumber}");
                            Console.WriteLine($"   ItemCode: {report.ItemCode}");
                            Console.WriteLine($"   HoseType: {report.HoseType}");
                            Console.WriteLine($"   Machine: {report.MachineName}");
                            Console.WriteLine($"   Dimension: {report.Dimension}");
                            Console.WriteLine($"   Shift: {report.Shift}");
                            Console.WriteLine($"   Operator: {report.CreatedBy}");
                            Console.WriteLine($"   Created: {report.CreatedDate:yyyy-MM-dd HH:mm:ss}");
                            Console.WriteLine($"   Production Date: {report.ProductionDate:yyyy-MM-dd}");
                            Console.WriteLine($"   Status: {report.Status}");
                            Console.WriteLine($"   Reading Points: {readings.Count}");
                            
                            if (readings.Count > 0)
                            {
                                Console.WriteLine($"   Time Range: {readings.First().ReadingTime:HH:mm:ss} - {readings.Last().ReadingTime:HH:mm:ss}");
                                Console.WriteLine($"\n   Sample Temperature Data (Inner Material):");
                                Console.WriteLine($"   First Reading - Head: {readings.First().HeadTempInner}°C, Cyl1: {readings.First().Cylinder1TempInner}°C, Cyl2: {readings.First().Cylinder2TempInner}°C");
                                Console.WriteLine($"   Last Reading  - Head: {readings.Last().HeadTempInner}°C, Cyl1: {readings.Last().Cylinder1TempInner}°C, Cyl2: {readings.Last().Cylinder2TempInner}°C");
                            }
                            Console.WriteLine();
                        }
                    }

                    // Also check for any Abdul Rohman records today
                    Console.WriteLine("\n=== ABDUL ROHMAN RECORDS TODAY (May 7, 2026) ===\n");
                    var today = new DateTime(2026, 5, 7);
                    var endOfDay = today.AddDays(1).AddTicks(-1);
                    
                    var abdulToday = context.ProductionReports
                        .Where(r => 
                            r.CreatedDate >= today && 
                            r.CreatedDate <= endOfDay &&
                            r.CreatedBy != null && 
                            r.CreatedBy.ToUpper().Contains("ABDUL")
                        )
                        .OrderBy(r => r.CreatedDate)
                        .ToList();

                    Console.WriteLine($"Records: {abdulToday.Count}\n");
                    foreach (var report in abdulToday)
                    {
                        Console.WriteLine($"  {report.CreatedDate:HH:mm:ss} | {report.DocumentNumber} | {report.ItemCode} | {report.HoseType}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
            }
        }
    }
}
