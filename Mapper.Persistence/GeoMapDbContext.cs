using Microsoft.EntityFrameworkCore;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using Mapper.Persistence.EntityTypeConfigurations;

namespace Mapper.Persistence
{
    public class GeoMapDbContext : DbContext, IMapperDbContext
    {
        public DbSet<GeoMap> Notes { get; set; }

        public GeoMapDbContext(DbContextOptions<GeoMapDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new GeoMapConfiguration());
            base.OnModelCreating(builder);
        }
    }
}
