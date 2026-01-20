using AutoMapper;
using Mapper.Application.Features.GeoMarks.Queries;
using Mapper.Domain;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Mapper.Tests.Common.QueryTestFixtures;

namespace Mapper.Tests.GeoMarks.Queries
{
    [Collection("GeoMarksQueryCollection")]
    public class GetGeoMarksListQueryHandlerTests
    {
        private readonly IContextFactory ContextFactory;
        private readonly IMapper Mapper;

        public GetGeoMarksListQueryHandlerTests(GeoMarksQueryTestFixture fixture)
        {
            ContextFactory = fixture.ContextFactory;
            Mapper = fixture.Mapper;
        }

        [Fact]
        public async Task GetGeoMarksQuery_Success()
        {
            // Arrange
            using var context = ContextFactory.Create();
            var handler = new GetGeoMarksHandler(context, Mapper);

            // Act
            var result = await handler.Handle(
                new GetGeoMarksQuery(GeoMarksContextFactory.GeoMapId),
                CancellationToken.None);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetGeoMarksQuery_FilterByType_Success()
        {
            // Arrange
            using var context = ContextFactory.Create();
            var handler = new GetGeoMarksHandler(context, Mapper);

            // Act
            var result = await handler.Handle(
                new GetGeoMarksQuery(GeoMarksContextFactory.GeoMapId, GeoMarkType.Camera),
                CancellationToken.None);

            // Assert
            Assert.Single(result);
            Assert.All(result, m => Assert.Equal(GeoMarkType.Camera, m.Type));
        }

        [Fact]
        public async Task GetGeoMarksQuery_EmptyList_AfterDeletion()
        {
            // Arrange
            using var context = ContextFactory.Create();
            var handler = new GetGeoMarksHandler(context, Mapper);

            context.GeoMarks.RemoveRange(context.GeoMarks);
            await context.SaveChangesAsync();

            // Act
            var result = await handler.Handle(
                new GetGeoMarksQuery(GeoMarksContextFactory.GeoMapId),
                CancellationToken.None);

            // Assert
            Assert.Empty(result);
        }
    }
}
