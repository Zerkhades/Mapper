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
                    .WithOne()
                    .HasForeignKey(x => x.GeoMarkId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }

}
