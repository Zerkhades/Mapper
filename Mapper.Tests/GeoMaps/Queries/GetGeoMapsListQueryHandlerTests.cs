using AutoMapper;
using Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapList;
using Mapper.Persistence;
using Mapper.Tests.Common;
using Mapper.Tests.Common.QueryTestFixtures;
using Microsoft.EntityFrameworkCore.Internal;


namespace Mapper.Tests.GeoMaps.Queries
{
    [Collection("GeoMapsQueryCollection")]
    public class GetGeoMapsListQueryHandlerTests
    {
        private readonly IContextFactory ContextFactory;
        private readonly IMapper Mapper;

        public GetGeoMapsListQueryHandlerTests(GeoMapsQueryTestFixture fixture)
        {
            ContextFactory = fixture.ContextFactory;
            Mapper = fixture.Mapper;
        }

        [Fact]
        public async Task GetGeoMapsListQueryHandler_Success()
        {
            // Arrange
            using var context = ContextFactory.Create();
            var handler = new GetGeoMapListQueryHandler(context, Mapper);

            // Act
            var result = await handler.Handle(
                new GetGeoMapListQuery(),
                CancellationToken.None);

            // Assert
            Assert.NotEmpty(result.GeoMaps);
        }

        [Fact]
        public async Task GetGeoMapsListQueryHandler_EmptyList()
        {
            // Arrange
            using var context = ContextFactory.Create();
            var handler = new GetGeoMapListQueryHandler(context, Mapper);

            // Удаляем все записи из контекста для проверки пустого списка
            context.GeoMaps.RemoveRange(context.GeoMaps);
            await context.SaveChangesAsync();

            // Act
            var result = await handler.Handle(
                new GetGeoMapListQuery(),
                CancellationToken.None);

            // Assert
            Assert.Empty(result.GeoMaps);
        }
    }
}