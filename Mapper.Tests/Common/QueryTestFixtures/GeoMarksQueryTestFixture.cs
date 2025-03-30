using Mapper.Tests.Common.ContextFactories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Tests.Common.QueryTestFixtures
{
    public class GeoMarksQueryTestFixture : QueryTestFixture<GeoMarksContextFactory> { }

    [CollectionDefinition("GeoMarksQueryCollection")]
    public class GeoMarksQueryCollection : ICollectionFixture<GeoMarksQueryTestFixture> { }
}
