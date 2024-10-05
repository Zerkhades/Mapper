using AutoMapper;
using Mapper.Application.Common.Mappings;
using Mapper.Application.Interfaces;
using Mapper.Persistence;

namespace Mapper.Tests.Common
{
    public class QueryTestFixture : IDisposable
    {
        public MapperDbContext Context;
        public IMapper Mapper;

        public QueryTestFixture()
        {
            Context = GeoMapsContextFactory.Create();
            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AssemblyMappingProfile(
                    typeof(IMapperDbContext).Assembly));
            });
            Mapper = configurationProvider.CreateMapper();
        }

        public void Dispose()
        {
            GeoMapsContextFactory.Destroy(Context);
        }
    }

    [CollectionDefinition("QueryCollection")]
    public class QueryCollection : ICollectionFixture<QueryTestFixture> { }
}
