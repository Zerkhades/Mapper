using Mapper.Tests.Common.ContextFactories;

namespace Mapper.Tests.Common.QueryTestFixtures
{
    public class GeoMapsQueryTestFixture : QueryTestFixture<GeoMapsContextFactory> { }

    [CollectionDefinition("GeoMapsQueryCollection")]
    public class GeoMapsQueryCollection : ICollectionFixture<GeoMapsQueryTestFixture> { }
}
