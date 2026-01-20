using Mapper.Application.Features.GeoMaps.Commands.UpdateGeoMap;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Mapper.Tests.GeoMaps.Commands
{
    public class UpdateGeoMapCommandHandlerTests : TestCommandBase
    {
        private readonly Mock<ICacheService> _mockCache;

        public UpdateGeoMapCommandHandlerTests() : base(new GeoMapsContextFactory())
        {
            _mockCache = new Mock<ICacheService>();
        }

        [Fact]
        public async Task UpdateGeoMapCommandHandler_Success()
        {
            // Arrange
            var handler = new UpdateGeoMapHandler(Context, _mockCache.Object);
            var mapId = GeoMapsContextFactory.GeoMapIdForDelete;
            var newName = "Updated Map Name";
            var newDescription = "Updated Description";

            // Act
            await handler.Handle(
                new UpdateGeoMapCommand(mapId, newName, newDescription),
                CancellationToken.None);

            // Assert
            var updatedMap = await Context.GeoMaps.SingleOrDefaultAsync(m => m.Id == mapId);
            Assert.NotNull(updatedMap);
            Assert.Equal(newName, updatedMap.Name);
            Assert.Equal(newDescription, updatedMap.Description);

            _mockCache.Verify(x => x.RemoveAsync($"geomap:{mapId}", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateGeoMapCommandHandler_WithNullDescription_Success()
        {
            // Arrange
            var handler = new UpdateGeoMapHandler(Context, _mockCache.Object);
            var mapId = GeoMapsContextFactory.GeoMapIdForDelete;
            var newName = "Map Without Description";

            // Act
            await handler.Handle(
                new UpdateGeoMapCommand(mapId, newName, null),
                CancellationToken.None);

            // Assert
            var updatedMap = await Context.GeoMaps.SingleOrDefaultAsync(m => m.Id == mapId);
            Assert.NotNull(updatedMap);
            Assert.Equal(newName, updatedMap.Name);
            Assert.Null(updatedMap.Description);
        }

        [Fact]
        public async Task UpdateGeoMapCommandHandler_FailOnWrongId()
        {
            // Arrange
            var handler = new UpdateGeoMapHandler(Context, _mockCache.Object);
            var wrongId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new UpdateGeoMapCommand(wrongId, "New Name", "New Description"),
                    CancellationToken.None)
            );
        }
    }
}
