using Mapper.Tests.Common.ContextFactories;

namespace Mapper.Tests.Common.QueryTestFixtures
{
    public class GeoMarksQueryTestFixture : QueryTestFixture<GeoMarksContextFactory> { }

    [CollectionDefinition("GeoMarksQueryCollection")]
    public class GeoMarksQueryCollection : ICollectionFixture<GeoMarksQueryTestFixture> { }
}
