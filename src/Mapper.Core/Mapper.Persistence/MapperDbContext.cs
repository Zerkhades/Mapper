using Microsoft.EntityFrameworkCore;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using Mapper.Persistence.EntityTypeConfigurations;

namespace Mapper.Persistence
{
    public class MapperDbContext : DbContext
    {
        public DbSet<GeoMap> GeoMaps => Set<GeoMap>();
        public DbSet<GeoMark> GeoMarks => Set<GeoMark>();
        public DbSet<Employee> Employees => Set<Employee>();

        public MapperDbContext(DbContextOptions<MapperDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GeoMap>(b =>
            {
                b.ToTable("geo_maps");
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).IsRequired().HasMaxLength(200);
                b.Property(x => x.ImagePath).IsRequired().HasMaxLength(500);
                b.HasMany(x => x.Marks).WithOne().HasForeignKey("GeoMapId").OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<GeoMark>(b =>
            {
                b.ToTable("geo_marks");
                b.HasKey(x => x.Id);

                b.Property(x => x.Title).IsRequired().HasMaxLength(200);
                b.Property(x => x.X).IsRequired();
                b.Property(x => x.Y).IsRequired();

                b.HasDiscriminator(x => x.Type)
                    .HasValue<TransitionMark>(GeoMarkType.Transition)
                    .HasValue<WorkplaceMark>(GeoMarkType.Workplace)
                    .HasValue<CameraMark>(GeoMarkType.Camera);

                b.HasIndex("GeoMapId");
            });

            modelBuilder.Entity<WorkplaceEmployee>(b =>
            {
                b.ToTable("workplace_employees");
                b.HasKey(x => new { x.WorkplaceMarkId, x.EmployeeId });
            });
        }
    }

}
