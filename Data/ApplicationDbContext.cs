using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Models;

namespace VelastoProductionSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Database Tables (sesuai dengan prd_extrude_hose)
        public DbSet<ProductionReport> ProductionReports { get; set; }
        public DbSet<ProductionReading> ProductionReadings { get; set; }
        public DbSet<SpsNoDoc> SpsNoDocs { get; set; }
        public DbSet<SpsItemList> SpsItemLists { get; set; }
        public DbSet<DimensionReport> DimensionReports { get; set; }
        public DbSet<DimensionMeasurement> DimensionMeasurements { get; set; }
        public DbSet<DimensionSummary> DimensionSummaries { get; set; }
        public DbSet<PlanningMaster> PlanningMasters { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<PartMaster> PartMasters { get; set; }
        public DbSet<ShiftMaster> ShiftMasters { get; set; }
        public DbSet<ProductionMaterialLot> ProductionMaterialLots { get; set; }
        public DbSet<SensorIngestLog> SensorIngestLogs { get; set; }
        public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
        public DbSet<ApprovalRequestLog> ApprovalRequestLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships

            modelBuilder.Entity<SpsItemList>()
                .HasOne(item => item.SpsNoDoc)
                .WithMany(doc => doc.ItemLists)
                .HasForeignKey(item => item.DocumentNumber)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductionReading>()
                .HasOne(p => p.ProductionReport)
                .WithMany(r => r.ProductionReadings)
                .HasForeignKey(p => p.ProductionReportId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DimensionMeasurement>()
                .HasOne(d => d.DimensionReport)
                .WithMany(r => r.Measurements)
                .HasForeignKey(d => d.DimensionReportId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DimensionSummary>()
                .HasOne(s => s.DimensionReport)
                .WithMany(r => r.Summaries)
                .HasForeignKey(s => s.DimensionReportId)
                .OnDelete(DeleteBehavior.Cascade);

            // Decimal precision for ProductionReading properties

            modelBuilder.Entity<ProductionReading>().Property(p => p.ScrewSpeedInner).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionReading>().Property(p => p.PressureInner).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionReading>().Property(p => p.ScrewSpeedOuter).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionReading>().Property(p => p.PressureOuter).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionReading>().Property(p => p.SpiralSpeed).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionReading>().Property(p => p.SpiralPitchSetting).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionReading>().Property(p => p.SpiralPitchDisplay).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionReading>().Property(p => p.PresetTemp).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionReading>().Property(p => p.ControlValue).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionReading>().Property(p => p.HoseSpeed).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionReading>().Property(p => p.TakeupConveyorSpeed).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionReading>().Property(p => p.CoolConveyorSpeed).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionReading>().Property(p => p.UnsmoothSurface).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionReading>().Property(p => p.ChillerWaterTemp).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionReading>().Property(p => p.CaterpillarGap).HasPrecision(18, 4);

            // Configure indexes for better performance
            modelBuilder.Entity<ProductionReport>()
                .HasIndex(p => p.ProductionDate);
            modelBuilder.Entity<ProductionReport>()
                .HasIndex(p => p.DocumentNumber);

            // SensorIngestLogs indexes
            modelBuilder.Entity<SensorIngestLog>()
                .HasIndex(s => s.IdempotencyKey)
                .IsUnique()
                .HasFilter("[IdempotencyKey] IS NOT NULL");
            modelBuilder.Entity<SensorIngestLog>()
                .HasIndex(s => new { s.MachineCode, s.SensorTimestamp });
            modelBuilder.Entity<SensorIngestLog>()
                .HasIndex(s => s.DeviceId);

            modelBuilder.Entity<ApprovalRequest>()
                .HasMany(r => r.Logs)
                .WithOne(l => l.ApprovalRequest)
                .HasForeignKey(l => l.ApprovalRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApprovalRequest>()
                .HasIndex(r => new { r.Status, r.CreatedAt });

            modelBuilder.Entity<ApprovalRequest>()
                .HasIndex(r => new { r.RequesterUserName, r.Status });

            modelBuilder.Entity<ApprovalRequest>()
                .HasIndex(r => new { r.ActionType, r.TargetKey, r.RequesterUserName, r.Status });

            modelBuilder.Entity<ApprovalRequest>()
                .HasIndex(r => r.RequestCode)
                .IsUnique();

            // Seed initial data (optional)
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // You can add seed data here if needed
            // Example:
            // modelBuilder.Entity<PackingStandard>().HasData(
            //     new PackingStandard { Id = 1, NACode = "NA-1420", ... }
            // );
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
            configurationBuilder.Properties<decimal>().HavePrecision(18, 4);
        }
    }
}
