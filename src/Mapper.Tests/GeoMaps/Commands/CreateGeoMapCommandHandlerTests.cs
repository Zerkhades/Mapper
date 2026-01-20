using Mapper.Application.Features.GeoMaps.Commands.CreateGeoMap;
using Mapper.Application.Interfaces;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Mapper.Tests.GeoMaps.Commands
{
    public class CreateGeoMapCommandHandlerTests : TestCommandBase
    {
        private readonly Mock<IMapImageStorage> _mockStorage;

        public CreateGeoMapCommandHandlerTests() : base(new GeoMapsContextFactory())
        {
            _mockStorage = new Mock<IMapImageStorage>();
            _mockStorage.Setup(x => x.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("/maps/test.jpg");
        }

        [Fact]
        public async Task CreateGeoMapCommandHandler_Success()
        {
            // Arrange
            var handler = new CreateGeoMapHandler(Context, _mockStorage.Object);
            var mapName = "Test GeoMap";
            var mapDescription = "Test Description";
            var imageStream = new MemoryStream([1, 2, 3, 4, 5]);

            // Act
            var mapId = await handler.Handle(
                new CreateGeoMapCommand(
                    Name: mapName,
                    Description: mapDescription,
                    ImageStream: imageStream,
                    FileName: "test.jpg",
                    ContentType: "image/jpeg",
                    ImageWidth: 1920,
                    ImageHeight: 1080
                ),
                CancellationToken.None);

            // Assert
            var createdMap = await Context.GeoMaps.SingleOrDefaultAsync(m =>
                m.Id == mapId &&
                m.Name == mapName &&
                m.Description == mapDescription);

            Assert.NotNull(createdMap);
            Assert.Equal(1920, createdMap.ImageWidth);
            Assert.Equal(1080, createdMap.ImageHeight);
            _mockStorage.Verify(x => x.SaveAsync(imageStream, "test.jpg", "image/jpeg", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateGeoMapCommandHandler_WithoutDescription_Success()
        {
            // Arrange
            var handler = new CreateGeoMapHandler(Context, _mockStorage.Object);
            var mapName = "Test GeoMap Without Description";
            var imageStream = new MemoryStream([1, 2, 3]);

            // Act
            var mapId = await handler.Handle(
                new CreateGeoMapCommand(
                    Name: mapName,
                    Description: null,
                    ImageStream: imageStream,
                    FileName: "test2.png",
                    ContentType: "image/png",
                    ImageWidth: 800,
                    ImageHeight: 600
                ),
                CancellationToken.None);

            // Assert
            var createdMap = await Context.GeoMaps.SingleOrDefaultAsync(m => m.Id == mapId);
            Assert.NotNull(createdMap);
            Assert.Null(createdMap.Description);
        }
    }
}
