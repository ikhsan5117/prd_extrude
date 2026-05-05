using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DbCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=10.14.149.34;Database=prd_extrude_hose;User ID=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True")
                .Options;

            using var context = new ApplicationDbContext(options);
            
            var code = "TA2990";
            
            Console.WriteLine($"Checking for: {code}");
            
            var master = context.MasterlistSpsDoubleLayers
                .FirstOrDefault(m => m.HoseType == code || m.ItemList == code || (m.HoseType != null && m.HoseType.Contains(code)));
            
            if (master != null) {
                Console.WriteLine($"Found in Masterlist: {master.HoseType} | {master.ItemList} | Doc: {master.DocumentNumber}");
            } else {
                Console.WriteLine("NOT Found in Masterlist");
            }
            
            var sps = context.StandardParameterSettings
                .FirstOrDefault(s => s.ProductCode == code || s.ItemList == code);
                
            if (sps != null) {
                Console.WriteLine($"Found in SPS: {sps.ProductCode} | {sps.ItemList} | Doc: {sps.DocumentNumber}");
            } else {
                Console.WriteLine("NOT Found in SPS");
            }

            // Check what's in Masterlist generally
            var any = context.MasterlistSpsDoubleLayers.Take(5).ToList();
            Console.WriteLine("\nSample Masterlist entries:");
            foreach(var a in any) Console.WriteLine($"- {a.HoseType} | {a.ItemList}");
        }
    }
}
