using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapList;
using Mapper.Persistence;
using Mapper.Tests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Mapper.Tests.GeoMaps.Queries
{
    [Collection("QueryCollection")]
    public class GetGeoMapsListQueryHandlerTests
    {
        private readonly MapperDbContext Context;
        private readonly IMapper Mapper;

        public GetGeoMapsListQueryHandlerTests(QueryTestFixture fixture)
        {
            Context = fixture.Context;
            Mapper = fixture.Mapper;
        }

        [Fact]
        public async Task GetGeoMapsListQueryHandler_Success()
        {
            // Arrange
            var handler = new GetGeoMapListQueryHandler(Context, Mapper);

            // Act
            var result = await handler.Handle(
                new GetGeoMapListQuery(),
                CancellationToken.None);

            // Assert
            // TODO: add istype check
            // Assert.IsType<List<GeoMapListVm>>(result);
            Assert.NotEmpty(result.GeoMaps);
        }

        [Fact]
        public async Task GetGeoMapsListQueryHandler_EmptyList()
        {
            // Arrange
            var handler = new GetGeoMapListQueryHandler(Context, Mapper);

            // Удаляем все записи из контекста для проверки пустого списка
            Context.GeoMaps.RemoveRange(Context.GeoMaps);
            await Context.SaveChangesAsync();

            // Act
            var result = await handler.Handle(
                new GetGeoMapListQuery(),
                CancellationToken.None);

            // Assert
            // TODO: add istype check
            //Assert.IsType<List<GeoMapListVm>>(result);
            Assert.Empty(result.GeoMaps);
        }
    }
}