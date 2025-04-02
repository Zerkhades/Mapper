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
        private readonly MapperDbContext Context;
        private readonly IMapper Mapper;

        public GetGeoMarksListQueryHandlerTests(GeoMarksQueryTestFixture fixture)
        {
            Context = fixture.Context;
            Mapper = fixture.Mapper;
        }

        [Fact]
        public async Task GetGeoMarksListQueryHandler_Success()
        {
            // Arrange
            var handler = new GetGeoMarkListQueryHandler(Context, Mapper);

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
            var handler = new GetGeoMarkListQueryHandler(Context, Mapper);

            // Удаляем все записи из контекста для проверки пустого списка
            Context.GeoMarks.RemoveRange(Context.GeoMarks);
            await Context.SaveChangesAsync();

            // Act
            var result = await handler.Handle(
                new GetGeoMarkListQuery(),
                CancellationToken.None);

            // Assert
            // TODO: add istype check
            // Assert.IsType<List<GeoMarkListVm>>(result);
            Assert.Empty(result.GeoMarks);
        }
    }
}
