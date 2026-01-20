using AutoMapper;
using Mapper.Application.Common.Mappings;
using Mapper.Application.Interfaces;
using Mapper.Persistence;
using Mapper.Tests.Common.ContextFactories;

namespace Mapper.Tests.Common
{
    public abstract class QueryTestFixture<TContextFactory> : IDisposable
        where TContextFactory : IContextFactory, new()
    {
        public readonly MapperDbContext Context;
        public readonly IContextFactory ContextFactory;
        public readonly IMapper Mapper;

        public QueryTestFixture()
        {
            ContextFactory = new TContextFactory();
            Context = ContextFactory.Create();

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AssemblyMappingProfile(typeof(IMapperDbContext).Assembly));
                cfg.AddProfile<GeoMapProfile>();
            });
            Mapper = configurationProvider.CreateMapper();
        }

        public void Dispose()
        {
            ContextFactory.Destroy(Context);
        }
    }
}
