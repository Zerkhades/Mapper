using Mapper.Application.Features.GeoMarks.Commands.CameraMarkCommands;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Mapper.Tests.GeoMarks.Commands
{
    public class UpdateCameraMarkCommandHandlerTests : TestCommandBase
    {
        private readonly Mock<ICacheService> _mockCache;
        private readonly Mock<IMapRealtimeNotifier> _mockNotifier;

        public UpdateCameraMarkCommandHandlerTests() : base(new GeoMarksContextFactory())
        {
            _mockCache = new Mock<ICacheService>();
            _mockNotifier = new Mock<IMapRealtimeNotifier>();
        }

        [Fact]
        public async Task UpdateCameraMarkCommand_Success()
        {
            // Arrange
            var handler = new UpdateCameraMarkHandler(Context, _mockCache.Object, _mockNotifier.Object);
            var markId = GeoMarksContextFactory.CameraMarkId;
            var mapId = GeoMarksContextFactory.GeoMapId;

            // Act
            await handler.Handle(
                new UpdateCameraMarkCommand(
                    GeoMapId: mapId,
                    MarkId: markId,
                    X: 0.6,
                    Y: 0.7,
                    Title: "Updated Camera",
                    Description: "Updated camera description",
                    CameraName: "CAM-999",
                    StreamUrl: "rtsp://updated.com/stream"
                ),
                CancellationToken.None);

            // Assert
            var updatedMark = await Context.GeoMarks.OfType<CameraMark>()
                .SingleOrDefaultAsync(m => m.Id == markId);

            Assert.NotNull(updatedMark);
            Assert.Equal(0.6, updatedMark.X);
            Assert.Equal(0.7, updatedMark.Y);
            Assert.Equal("Updated Camera", updatedMark.Title);
            Assert.Equal("Updated camera description", updatedMark.Description);
            Assert.Equal("CAM-999", updatedMark.CameraName);
            Assert.Equal("rtsp://updated.com/stream", updatedMark.StreamUrl);

            _mockCache.Verify(x => x.RemoveAsync($"geomap:{mapId}", It.IsAny<CancellationToken>()), Times.Once);
            _mockNotifier.Verify(x => x.MarkUpdated(mapId, It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCameraMarkCommand_WithNullValues_Success()
        {
            // Arrange
            var handler = new UpdateCameraMarkHandler(Context, _mockCache.Object, _mockNotifier.Object);
            var markId = GeoMarksContextFactory.CameraMarkId;
            var mapId = GeoMarksContextFactory.GeoMapId;

            // Act
            await handler.Handle(
                new UpdateCameraMarkCommand(
                    GeoMapId: mapId,
                    MarkId: markId,
                    X: 0.5,
                    Y: 0.5,
                    Title: "Camera Without Details",
                    Description: null,
                    CameraName: null,
                    StreamUrl: null
                ),
                CancellationToken.None);

            // Assert
            var updatedMark = await Context.GeoMarks.OfType<CameraMark>()
                .SingleOrDefaultAsync(m => m.Id == markId);

            Assert.NotNull(updatedMark);
            Assert.Equal("Camera Without Details", updatedMark.Title);
            Assert.Null(updatedMark.Description);
            Assert.Null(updatedMark.CameraName);
            Assert.Null(updatedMark.StreamUrl);
        }

        [Fact]
        public async Task UpdateCameraMarkCommand_FailOnWrongMarkId()
        {
            // Arrange
            var handler = new UpdateCameraMarkHandler(Context, _mockCache.Object, _mockNotifier.Object);
            var wrongMarkId = Guid.NewGuid();
            var mapId = GeoMarksContextFactory.GeoMapId;

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new UpdateCameraMarkCommand(mapId, wrongMarkId, 0.5, 0.5, "Title", null, "CAM-001", null),
                    CancellationToken.None)
            );
        }

        [Fact]
        public async Task UpdateCameraMarkCommand_FailOnWrongMapId()
        {
            // Arrange
            var handler = new UpdateCameraMarkHandler(Context, _mockCache.Object, _mockNotifier.Object);
            var markId = GeoMarksContextFactory.CameraMarkId;
            var wrongMapId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new UpdateCameraMarkCommand(wrongMapId, markId, 0.5, 0.5, "Title", null, "CAM-001", null),
                    CancellationToken.None)
            );
        }
    }
}
