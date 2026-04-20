using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Data
{
    /// <summary>
    /// DbContext terpisah untuk membaca data dari database ELWP_PRD.
    /// Context ini read-only dan tidak punya migrasi.
    /// </summary>
    public class ElwpDbContext : DbContext
    {
        public ElwpDbContext(DbContextOptions<ElwpDbContext> options)
            : base(options)
        {
        }

        public DbSet<ElwpPlanning> ElwpPlannings { get; set; }
        public DbSet<ElwpMachine> ElwpMachines { get; set; }
        public DbSet<ElwpUser> ElwpUsers { get; set; }
        public DbSet<ElwpArea> ElwpAreas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ElwpPlanning>(entity =>
            {
                entity.ToTable("tb_elwp_produksi_plannings", "produksi");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TanggalPlanning).HasColumnName("TanggalPlanning");
                entity.Property(e => e.PnSap).HasColumnName("PnSap");
                entity.Property(e => e.KodeItem).HasColumnName("KodeItem");
                entity.Property(e => e.PartName).HasColumnName("PartName");
                entity.Property(e => e.Shift).HasColumnName("Shift");
                entity.Property(e => e.QtyPlanning).HasColumnName("QtyPlanning");
                entity.Property(e => e.MesinId).HasColumnName("MesinId");
                entity.Property(e => e.LoadingTimeHours).HasColumnName("LoadingTimeHours").HasPrecision(18, 4);
            });

            modelBuilder.Entity<ElwpMachine>(entity =>
            {
                entity.ToTable("tb_elwp_produksi_mesins", "produksi");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<ElwpUser>(entity =>
            {
                entity.ToTable("tb_elwp_produksi_users", "produksi");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<ElwpArea>(entity =>
            {
                entity.ToTable("tb_elwp_produksi_areas", "produksi");
                entity.HasKey(e => e.Id);
            });
        }
    }
}
