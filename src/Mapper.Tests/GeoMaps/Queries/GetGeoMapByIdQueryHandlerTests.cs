using AutoMapper;
using Mapper.Application.Features.GeoMaps.Queries.GetGeoMapById;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using Mapper.Application.Features.DTOs;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Mapper.Tests.Common.QueryTestFixtures;
using Moq;

namespace Mapper.Tests.GeoMaps.Queries
{
    [Collection("GeoMapsQueryCollection")]
    public class GetGeoMapByIdQueryHandlerTests
    {
        private readonly IContextFactory ContextFactory;
        private readonly IMapper Mapper;
        private readonly Mock<ICacheService> _mockCache;

        public GetGeoMapByIdQueryHandlerTests(GeoMapsQueryTestFixture fixture)
        {
            ContextFactory = fixture.ContextFactory;
            Mapper = fixture.Mapper;
            _mockCache = new Mock<ICacheService>();
            _mockCache.Setup(x => x.GetAsync<GeoMapDetailsDto>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GeoMapDetailsDto?)null);
        }

        [Fact]
        public async Task GetGeoMapByIdQuery_Success()
        {
            // Arrange
            using var context = ContextFactory.Create();
            var handler = new GetGeoMapByIdHandler(context, Mapper, _mockCache.Object);
            var id = GeoMapsContextFactory.GeoMapIdForDelete;

            // Act
            var result = await handler.Handle(
                new GetGeoMapByIdQuery(id),
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("GeoMapForDelete", result.Name);
        }

        [Fact]
        public async Task GetGeoMapByIdQuery_FailOnNotFound()
        {
            // Arrange
            using var context = ContextFactory.Create();
            var handler = new GetGeoMapByIdHandler(context, Mapper, _mockCache.Object);
            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await handler.Handle(
                    new GetGeoMapByIdQuery(nonExistentId),
                    CancellationToken.None);
            });
        }
    }
}
