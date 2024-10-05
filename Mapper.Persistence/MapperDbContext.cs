using Microsoft.EntityFrameworkCore;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using Mapper.Persistence.EntityTypeConfigurations;

namespace Mapper.Persistence
{
    public class MapperDbContext : DbContext, IMapperDbContext
    {
        public MapperDbContext(DbContextOptions<MapperDbContext> options) : base(options) { }
        public DbSet<GeoMap> GeoMaps { get; set; }
        public DbSet<GeoMark> GeoMarks { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeePhoto> EmployeesPhotos { get; set; }


        // Might be useless
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new MapperConfiguration());
            base.OnModelCreating(builder);
        }
    }
}
