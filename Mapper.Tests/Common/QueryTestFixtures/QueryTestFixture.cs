using AutoMapper;
using Mapper.Application.Common.Mappings;
using Mapper.Application.Interfaces;
using Mapper.Persistence;
using Mapper.Tests.Common.ContextFactories;

namespace Mapper.Tests.Common.QueryTestFixtures
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
