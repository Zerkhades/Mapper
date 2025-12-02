using AutoMapper;
using Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapDetails;
using Mapper.Application.Common.Exceptions;
using Mapper.Persistence;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Mapper.Tests.Common.QueryTestFixtures;

namespace Mapper.Tests.GeoMaps.Queries
{
    [Collection("GeoMapsQueryCollection")]
    public class GetGeoMapDetailsQueryHandlerTests
    {
        private readonly IContextFactory Context;
        private readonly IMapper Mapper;

        public GetGeoMapDetailsQueryHandlerTests(GeoMapsQueryTestFixture fixture)
        {
            Context = fixture.ContextFactory;
            Mapper = fixture.Mapper;
        }

        [Fact]
        public async Task GetGeoMapDetailsQueryHandler_Success()
        {
            // Arrange
            using var context = Context.Create();
            var handler = new GetGeoMapDetailsQueryHandler(context, Mapper);
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

        [Fact]
        public async Task GetGeoMapDetailsQueryHandler_FailOnNotFound()
        {
            // Arrange
            using var context = Context.Create();
            var handler = new GetGeoMapDetailsQueryHandler(context, Mapper);
            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await handler.Handle(
                    new GetGeoMapDetailsQuery()
                    {
                        Id = nonExistentId
                    },
                    CancellationToken.None);
            });
        }
    }
}
