using AutoMapper;
using Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapDetails;
using Mapper.Application.CommandsAndQueries.GeoMark.Queries.GetGeoMarkDetails;
using Mapper.Application.Common.Exceptions;
using Mapper.Persistence;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Mapper.Tests.Common.QueryTestFixtures;

namespace Mapper.Tests.GeoMarks.Queries
{
    [Collection("GeoMarksQueryCollection")]
    public class GetGeoMarkDetailsQueryHandlerTests
    {
        private readonly IContextFactory Context;
        private readonly IMapper Mapper;

        public GetGeoMarkDetailsQueryHandlerTests(GeoMarksQueryTestFixture fixture)
        {
            Context = fixture.ContextFactory;
            Mapper = fixture.Mapper;
        }

        [Fact]
        public async Task GetGeoMarkDetailsQueryHandler_Success()
        {
            // Arrange
            using var context = Context.Create();
            var handler = new GetGeoMarkDetailsQueryHandler(context, Mapper);
            var id = GeoMarksContextFactory.GeoMarkIdForCreate;

            // Act
            var result = await handler.Handle(
                new GetGeoMarkDetailsQuery()
                {
                    Id = id
                },
                CancellationToken.None);

            // Assert
            Assert.IsType<GeoMarkDetailsVm>(result);
            Assert.Equal("GeoMarkForCreate", result.MarkDescription);
            Assert.Equal("GeoMarkForCreate", result.MarkName);
            Assert.False(result.IsArchived);
            Assert.Equal("#FF0000", result.Color);
            Assert.Equal("\uD83D\uDE00", result.Emoji);
            Assert.Equal(10, result.Size);
            Assert.False(result.IsEmoji);
            Assert.True(result.IsEditable);
            //Assert.Equal(new DateTime(2023, 1, 1), result.CreationDate);
            //Assert.Equal(new DateTime(2023, 1, 2), result.EditDate);
            //Assert.Equal(Guid.Parse("00000000-0000-0000-0000-000000000001"), result.EditedBy);
            Assert.NotNull(result.Employees);
            Assert.NotNull(result.GeoPhotos);
        }

        [Fact]
        public async Task GetGeoMarkDetailsQueryHandler_FailOnNotFound()
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
