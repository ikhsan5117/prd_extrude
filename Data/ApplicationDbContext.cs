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

        public DbSet<StandardParameterSetting> StandardParameterSettings { get; set; }
        public DbSet<ProductionReport> ProductionReports { get; set; }
        public DbSet<ProductionReading> ProductionReadings { get; set; }
        public DbSet<DimensionReading> DimensionReadings { get; set; }
        public DbSet<NowProducing> NowProducings { get; set; }
        public DbSet<LotTag> LotTags { get; set; }
        public DbSet<PackingStandard> PackingStandards { get; set; }
        public DbSet<MasterlistSpsDoubleLayer> MasterlistSpsDoubleLayers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<ProductionReport>()
                .HasOne(p => p.StandardParameterSetting)
                .WithMany()
                .HasForeignKey(p => p.StandardParameterSettingId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ProductionReading>()
                .HasOne(p => p.ProductionReport)
                .WithMany(r => r.ProductionReadings)
                .HasForeignKey(p => p.ProductionReportId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DimensionReading>()
                .HasOne(d => d.ProductionReport)
                .WithMany()
                .HasForeignKey(d => d.ProductionReportId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LotTag>()
                .HasOne(l => l.ProductionReport)
                .WithMany()
                .HasForeignKey(l => l.ProductionReportId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure decimal precision for SQL Server compatibility
            var decimalProps = new[] { 
                "InnerDie", "OuterDie", "TubeDie", "MiddleDie", "CoverDie", "SpacerDie", 
                "ToleranceDie", "Tol_TubeDie", "Tol_MiddleDie", "Tol_OuterDie", "Tol_CoverDie", "Tol_SpiralPitch",
                "ScrewSpeed", "FeedRollRatio", "Pressure", "AirPressureA", "PresetValve", 
                "SpiralSpeed", "SpiralPitch", "SpiralSpeedDisplay", "SpiralPitchDisplay", "PresetTemp",
                "UnsmoothSurface", "MarkingMaterialInner", "MarkingMaterialOuter"
            };

            foreach (var prop in decimalProps)
            {
                modelBuilder.Entity<StandardParameterSetting>().Property(prop).HasPrecision(18, 4);
            }

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
            modelBuilder.Entity<StandardParameterSetting>()
                .HasIndex(s => s.DocumentNumber);
            modelBuilder.Entity<StandardParameterSetting>()
                .HasIndex(s => s.CustomerName);

            modelBuilder.Entity<ProductionReport>()
                .HasIndex(p => p.ProductionDate);
            modelBuilder.Entity<ProductionReport>()
                .HasIndex(p => p.DocumentNumber);

            modelBuilder.Entity<LotTag>()
                .HasIndex(l => l.LotTagNumber)
                .IsUnique();
            modelBuilder.Entity<LotTag>()
                .HasIndex(l => l.PartNumber);

            modelBuilder.Entity<PackingStandard>()
                .HasIndex(p => p.NACode)
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
    }
}
