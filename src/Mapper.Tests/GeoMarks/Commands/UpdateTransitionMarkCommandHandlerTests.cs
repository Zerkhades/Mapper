using Mapper.Application.Features.GeoMarks.Commands.TransitionMarkCommands;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Mapper.Tests.GeoMarks.Commands
{
    public class UpdateTransitionMarkCommandHandlerTests : TestCommandBase
    {
        private readonly Mock<ICacheService> _mockCache;
        private readonly Mock<IMapRealtimeNotifier> _mockNotifier;

        public UpdateTransitionMarkCommandHandlerTests() : base(new GeoMarksContextFactory())
        {
            _mockCache = new Mock<ICacheService>();
            _mockNotifier = new Mock<IMapRealtimeNotifier>();
        }

        [Fact]
        public async Task UpdateTransitionMarkCommand_Success()
        {
            // Arrange
            var handler = new UpdateTransitionMarkHandler(Context, _mockCache.Object, _mockNotifier.Object);
            var markId = GeoMarksContextFactory.TransitionMarkId;
            var mapId = GeoMarksContextFactory.GeoMapId;
            var targetMapId = GeoMarksContextFactory.TargetGeoMapId;

            // Act
            await handler.Handle(
                new UpdateTransitionMarkCommand(
                    GeoMapId: mapId,
                    MarkId: markId,
                    X: 0.8,
                    Y: 0.9,
                    Title: "Updated Transition",
                    Description: "Updated description",
                    TargetGeoMapId: targetMapId
                ),
                CancellationToken.None);

            // Assert
            var updatedMark = await Context.GeoMarks.OfType<TransitionMark>()
                .SingleOrDefaultAsync(m => m.Id == markId);

            Assert.NotNull(updatedMark);
            Assert.Equal(0.8, updatedMark.X);
            Assert.Equal(0.9, updatedMark.Y);
            Assert.Equal("Updated Transition", updatedMark.Title);
            Assert.Equal("Updated description", updatedMark.Description);
            Assert.Equal(targetMapId, updatedMark.TargetGeoMapId);

            _mockCache.Verify(x => x.RemoveAsync($"geomap:{mapId}", It.IsAny<CancellationToken>()), Times.Once);
            _mockNotifier.Verify(x => x.MarkUpdated(mapId, It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTransitionMarkCommand_FailOnWrongMarkId()
        {
            // Arrange
            var handler = new UpdateTransitionMarkHandler(Context, _mockCache.Object, _mockNotifier.Object);
            var wrongMarkId = Guid.NewGuid();
            var mapId = GeoMarksContextFactory.GeoMapId;
            var targetMapId = GeoMarksContextFactory.TargetGeoMapId;

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new UpdateTransitionMarkCommand(mapId, wrongMarkId, 0.5, 0.5, "Title", null, targetMapId),
                    CancellationToken.None)
            );
        }

        [Fact]
        public async Task UpdateTransitionMarkCommand_FailOnWrongMapId()
        {
            // Arrange
            var handler = new UpdateTransitionMarkHandler(Context, _mockCache.Object, _mockNotifier.Object);
            var markId = GeoMarksContextFactory.TransitionMarkId;
            var wrongMapId = Guid.NewGuid();
            var targetMapId = GeoMarksContextFactory.TargetGeoMapId;

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new UpdateTransitionMarkCommand(wrongMapId, markId, 0.5, 0.5, "Title", null, targetMapId),
                    CancellationToken.None)
            );
        }
    }
}
