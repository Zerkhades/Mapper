using AutoMapper;
using Mapper.Application.CommandsAndQueries.GeoMark.Queries.GetGeoMarkList;
using Mapper.Persistence;
using Mapper.Tests.Common;
using Mapper.Tests.Common.QueryTestFixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task GetGeoMarksListQueryHandler_Success()
        {
            // Arrange
            using var context = ContextFactory.Create();
            var handler = new GetGeoMarkListQueryHandler(context, Mapper);

            // Act
            var result = await handler.Handle(
                new GetGeoMarkListQuery(),
                CancellationToken.None);

            // Assert
            Assert.NotEmpty(result.GeoMarks);
        }

        [Fact]
        public async Task GetGeoMarksListQueryHandler_EmptyList()
        {
            // Arrange
            using var context = ContextFactory.Create();
            var handler = new GetGeoMarkListQueryHandler(context, Mapper);

            // Удаляем все записи из контекста для проверки пустого списка
            context.GeoMarks.RemoveRange(context.GeoMarks);
            await context.SaveChangesAsync();

            // Act
            var result = await handler.Handle(
                new GetGeoMarkListQuery(),
                CancellationToken.None);

            // Assert
            Assert.Empty(result.GeoMarks);
        }
    }
}
