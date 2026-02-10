using Microsoft.EntityFrameworkCore;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using Mapper.Persistence.EntityTypeConfigurations;

namespace Mapper.Persistence
{
    public class MapperDbContext : DbContext, IMapperDbContext
    {
        public DbSet<GeoMap> GeoMaps { get; set; }
        public DbSet<GeoMark> GeoMarks { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<CameraVideoArchive> CameraVideoArchives { get; set; }
        public DbSet<CameraMotionAlert> CameraMotionAlerts { get; set; }
        public DbSet<CameraStatusHistory> CameraStatusHistories { get; set; }

        public MapperDbContext(DbContextOptions<MapperDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GeoMap>(b =>
            {
                b.ToTable("geo_maps");
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).IsRequired().HasMaxLength(200);
                b.Property(x => x.ImagePath).IsRequired().HasMaxLength(500);
                b.Property(x => x.IsDeleted).IsRequired();
                b.Property(x => x.DeletedAt);
                b.HasQueryFilter(x => !x.IsDeleted);
                b.HasMany(x => x.Marks).WithOne().HasForeignKey("GeoMapId").OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<GeoMark>(b =>
            {
                b.ToTable("geo_marks");
                b.HasKey(x => x.Id);

                b.Property(x => x.Title).IsRequired().HasMaxLength(200);
                b.Property(x => x.X).IsRequired();
                b.Property(x => x.Y).IsRequired();

                b.Property(x => x.IsDeleted).IsRequired();
                b.Property(x => x.DeletedAt);
                b.HasQueryFilter(x => !x.IsDeleted);

                b.HasDiscriminator(x => x.Type)
                    .HasValue<TransitionMark>(GeoMarkType.Transition)
                    .HasValue<WorkplaceMark>(GeoMarkType.Workplace)
                    .HasValue<CameraMark>(GeoMarkType.Camera);

                b.HasIndex("GeoMapId");
            });

            modelBuilder.Entity<WorkplaceMark>(b =>
            {
                b.HasMany(x => x.Employees)
                    .WithOne(x => x.GeoMark)
                    .HasForeignKey(x => x.GeoMarkId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CameraVideoArchive>(b =>
            {
                b.ToTable("camera_video_archives");
                b.HasKey(x => x.Id);
                b.Property(x => x.CameraMarkId).IsRequired();
                b.Property(x => x.VideoPath).IsRequired().HasMaxLength(1000);
                b.Property(x => x.ThumbnailPath).HasMaxLength(1000);
                b.Property(x => x.RecordedAt).IsRequired();
                b.Property(x => x.Duration).IsRequired();
                b.Property(x => x.FileSizeBytes).IsRequired();
                b.Property(x => x.HasMotionDetected).IsRequired();
                b.Property(x => x.Resolution).IsRequired().HasMaxLength(50);
                b.Property(x => x.FramesPerSecond).IsRequired();
                b.Property(x => x.IsArchived).IsRequired();
                b.Property(x => x.CreatedAt).IsRequired();
                b.HasIndex(x => x.CameraMarkId);
                b.HasIndex(x => x.RecordedAt);
            });

            modelBuilder.Entity<CameraMotionAlert>(b =>
            {
                b.ToTable("camera_motion_alerts");
                b.HasKey(x => x.Id);
                b.Property(x => x.CameraMarkId).IsRequired();
                b.Property(x => x.DetectedAt).IsRequired();
                b.Property(x => x.ConfirmedAt);
                b.Property(x => x.Severity).IsRequired();
                b.Property(x => x.MotionPercentage).IsRequired();
                b.Property(x => x.SnapshotPath).HasMaxLength(1000);
                b.Property(x => x.IsResolved).IsRequired();
                b.Property(x => x.ResolutionNotes).HasMaxLength(1000);
                b.Property(x => x.RelatedVideoArchiveId);
                b.HasIndex(x => x.CameraMarkId);
                b.HasIndex(x => x.DetectedAt);
                b.HasIndex(x => x.IsResolved);
            });

            modelBuilder.Entity<CameraStatusHistory>(b =>
            {
                b.ToTable("camera_status_histories");
                b.HasKey(x => x.Id);
                b.Property(x => x.CameraMarkId).IsRequired();
                b.Property(x => x.IsOnline).IsRequired();
                b.Property(x => x.Reason).IsRequired();
                b.Property(x => x.ChangedAt).IsRequired();
                b.Property(x => x.DurationSinceLastChange);
                b.Property(x => x.Details).HasMaxLength(1000);
                b.Property(x => x.ResponseTimeMs);
                b.HasIndex(x => x.CameraMarkId);
                b.HasIndex(x => x.ChangedAt);
            });
        }
    }
}
