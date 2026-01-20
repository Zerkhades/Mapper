using Mapper.Application.Features.GeoMarks.Commands.GeoMarkCommands;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Mapper.Tests.GeoMarks.Commands
{
    public class CreateGeoMarkCommandHandlerTests : TestCommandBase
    {
        private readonly Mock<IMapRealtimeNotifier> _mockNotifier;

        public CreateGeoMarkCommandHandlerTests() : base(new GeoMarksContextFactory())
        {
            _mockNotifier = new Mock<IMapRealtimeNotifier>();
        }

        [Fact]
        public async Task AddGeoMarkCommand_TransitionMark_Success()
        {
            // Arrange
            var handler = new AddGeoMarkHandler(Context, _mockNotifier.Object);
            var targetMapId = GeoMarksContextFactory.TargetGeoMapId;

            // Act
            var markId = await handler.Handle(
                new AddGeoMarkCommand(
                    GeoMapId: GeoMarksContextFactory.GeoMapId,
                    Type: GeoMarkType.Transition,
                    X: 0.6,
                    Y: 0.7,
                    Title: "New Transition",
                    Description: "New transition description",
                    TargetGeoMapId: targetMapId,
                    WorkplaceCode: null,
                    EmployeeIds: null,
                    CameraName: null,
                    StreamUrl: null
                ),
                CancellationToken.None);

            // Assert
            var mark = await Context.GeoMarks.OfType<TransitionMark>()
                .SingleOrDefaultAsync(m => m.Id == markId);
            
            Assert.NotNull(mark);
            Assert.Equal("New Transition", mark.Title);
            Assert.Equal(targetMapId, mark.TargetGeoMapId);
            Assert.Equal(0.6, mark.X);
            Assert.Equal(0.7, mark.Y);
        }

        [Fact]
        public async Task AddGeoMarkCommand_WorkplaceMark_Success()
        {
            // Arrange
            var handler = new AddGeoMarkHandler(Context, _mockNotifier.Object);

            // Act
            var markId = await handler.Handle(
                new AddGeoMarkCommand(
                    GeoMapId: GeoMarksContextFactory.GeoMapId,
                    Type: GeoMarkType.Workplace,
                    X: 0.2,
                    Y: 0.3,
                    Title: "New Workplace",
                    Description: "Workplace description",
                    TargetGeoMapId: null,
                    WorkplaceCode: "WP-002",
                    EmployeeIds: null,
                    CameraName: null,
                    StreamUrl: null
                ),
                CancellationToken.None);

            // Assert
            var mark = await Context.GeoMarks.OfType<WorkplaceMark>()
                .SingleOrDefaultAsync(m => m.Id == markId);
            
            Assert.NotNull(mark);
            Assert.Equal("New Workplace", mark.Title);
            Assert.Equal("WP-002", mark.WorkplaceCode);
        }

        [Fact]
        public async Task AddGeoMarkCommand_CameraMark_Success()
        {
            // Arrange
            var handler = new AddGeoMarkHandler(Context, _mockNotifier.Object);

            // Act
            var markId = await handler.Handle(
                new AddGeoMarkCommand(
                    GeoMapId: GeoMarksContextFactory.GeoMapId,
                    Type: GeoMarkType.Camera,
                    X: 0.8,
                    Y: 0.9,
                    Title: "New Camera",
                    Description: "Camera description",
                    TargetGeoMapId: null,
                    WorkplaceCode: null,
                    EmployeeIds: null,
                    CameraName: "CAM-002",
                    StreamUrl: "rtsp://camera2.com/stream"
                ),
                CancellationToken.None);

            // Assert
            var mark = await Context.GeoMarks.OfType<CameraMark>()
                .SingleOrDefaultAsync(m => m.Id == markId);
            
            Assert.NotNull(mark);
            Assert.Equal("New Camera", mark.Title);
            Assert.Equal("CAM-002", mark.CameraName);
            Assert.Equal("rtsp://camera2.com/stream", mark.StreamUrl);
        }

        [Fact]
        public async Task AddGeoMarkCommand_FailOnNonExistentMap()
        {
            // Arrange
            var handler = new AddGeoMarkHandler(Context, _mockNotifier.Object);
            var wrongMapId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new AddGeoMarkCommand(
                        GeoMapId: wrongMapId,
                        Type: GeoMarkType.Camera,
                        X: 0.5,
                        Y: 0.5,
                        Title: "Test",
                        Description: null,
                        TargetGeoMapId: null,
                        WorkplaceCode: null,
                        EmployeeIds: null,
                        CameraName: "CAM-001",
                        StreamUrl: null
                    ),
                    CancellationToken.None)
            );
        }
    }
}
