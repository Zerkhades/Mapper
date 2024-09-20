using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mapper.Domain;

namespace Mapper.Persistence.EntityTypeConfigurations
{
    public class MapperConfiguration : IEntityTypeConfiguration<GeoMap>
    {
        public void Configure(EntityTypeBuilder<GeoMap> builder)
        {
            builder.HasKey(note => note.Id);
            builder.HasIndex(note => note.Id).IsUnique();
        }
    }
}
