using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DbCheckApp
{
    public class ElwpPlanning {
        public int Id { get; set; }
        public string? KodeItem { get; set; }
        public string? PartName { get; set; }
        public int MesinId { get; set; }
        public DateTime? TanggalPlanning { get; set; }
    }

    public class MasterlistSpsDoubleLayer {
        public int Id { get; set; }
        public string? ItemList { get; set; }
        public string? HoseType { get; set; }
    }

    public class ElwpMachine {
        public int Id { get; set; }
        public string? KodeMesin { get; set; }
    }

    public class ElwpContext : DbContext {
        public DbSet<ElwpPlanning> ElwpPlannings { get; set; }
        public DbSet<ElwpMachine> ElwpMachines { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer("Server=10.14.149.34;Database=ELWP_PRD;User ID=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<ElwpPlanning>().ToTable("tb_elwp_produksi_plannings", "produksi");
            modelBuilder.Entity<ElwpMachine>().ToTable("tb_elwp_produksi_mesins", "produksi");
        }
    }

    public class SpsContext : DbContext {
        public DbSet<MasterlistSpsDoubleLayer> MasterlistSpsDoubleLayers { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer("Server=10.14.149.34;Database=prd_extrude_hose;User ID=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True");
        }
    }

    class Program {
        static void Main(string[] args) {
            try {
                using var elwp = new ElwpContext();
                using var sps = new SpsContext();
                
                var today = DateTime.Today;
                Console.WriteLine($"=== ANALISIS ITEM READY HARI INI ({today:dd MMM yyyy}) ===\n");

                var machines = elwp.ElwpMachines.Where(m => m.KodeMesin.StartsWith("EXT")).ToDictionary(m => m.Id, m => m.KodeMesin ?? "UNKWN");
                var plans = elwp.ElwpPlannings.Where(p => p.TanggalPlanning >= today).ToList();
                var masterList = sps.MasterlistSpsDoubleLayers.Select(x => x.ItemList).ToList();

                Console.WriteLine($"{"MESIN",-12} | {"KODE",-10} | {"ITEM MASTER (READY DI SPS)",-30}");
                Console.WriteLine(new string('-', 60));

                int count = 0;
                foreach(var p in plans) {
                    if (!machines.ContainsKey(p.MesinId)) continue;
                    
                    string shortCode = (p.KodeItem ?? "").Trim().ToUpper();
                    string mName = machines[p.MesinId];

                    var match = masterList.FirstOrDefault(m => (m ?? "").ToUpper().Contains(shortCode));
                    if (match != null) {
                        Console.WriteLine($"{mName,-12} | {shortCode,-10} | {match}");
                        count++;
                    }
                }

                if(count == 0) Console.WriteLine("(!) Tidak ada item yang match untuk hari ini.");
                else Console.WriteLine($"\nTotal: {count} item siap digunakan.");

            } catch (Exception ex) {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }
    }
}
