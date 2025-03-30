using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapDetails;
using Mapper.Persistence;
using Mapper.Tests.Common.ContextFactories;
using Mapper.Tests.Common.QueryTestFixtures;

namespace Mapper.Tests.GeoMaps.Queries
{
    [Collection("QueryCollection")]
    public class GetGeoMapDetailsQueryHandlerTests
    {
        private readonly MapperDbContext Context;
        private readonly IMapper Mapper;

        public GetGeoMapDetailsQueryHandlerTests(QueryTestFixture fixture)
        {
            Context = fixture.Context;
            Mapper = fixture.Mapper;
        }

        [Fact]
        public async Task GetGeoMapDetailsQueryHandler_Success()
        {
            // Arrange
            var handler = new GetGeoMapDetailsQueryHandler(Context, Mapper);
            var id = GeoMapsContextFactory.GeoMapIdForCreate;
            
            // Act
            var result = await handler.Handle(
                new GetGeoMapDetailsQuery()
                {
                    Id = id
                },
                CancellationToken.None);

            // Assert
            Assert.IsType<GeoMapDetailsVm>(result);
            Assert.Equal("GeoMapForCreate", result.MapDescription);
            Assert.Equal("GeoMapForCreate", result.MapName);
            Assert.False(result.IsArchived);
        }
    }
}
