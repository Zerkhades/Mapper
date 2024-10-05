using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mapper.Domain;

namespace Mapper.Persistence.EntityTypeConfigurations
{
    public class MapperConfiguration : IEntityTypeConfiguration<GeoMap>
    {
        public void Configure(EntityTypeBuilder<GeoMap> builder)
        {
            builder.HasKey(geomap => geomap.Id);
            builder.HasIndex(geomap => geomap.Id).IsUnique();
        }
    }
}
