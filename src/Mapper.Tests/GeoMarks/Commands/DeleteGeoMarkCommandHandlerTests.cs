using Mapper.Application.Features.GeoMarks.Commands.DeleteGeoMark;
using Mapper.Application.Common.Exceptions;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Tests.GeoMarks.Commands
{
    public class DeleteGeoMarkCommandHandlerTests : TestCommandBase
    {
        public DeleteGeoMarkCommandHandlerTests() : base(new GeoMarksContextFactory())
        {
        }

        [Fact]
        public async Task DeleteGeoMarkCommandHandler_Success()
        {
            // Arrange
            var handler = new DeleteGeoMarkHandler(Context);
            var markId = GeoMarksContextFactory.TransitionMarkId;
            var mapId = GeoMarksContextFactory.GeoMapId;

            // Act
            await handler.Handle(
                new DeleteGeoMarkCommand(mapId, markId),
                CancellationToken.None);

            // Assert
            var deletedMark = await Context.GeoMarks
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(m => m.Id == markId);
            
            Assert.NotNull(deletedMark);
            Assert.True(deletedMark.IsDeleted);
            Assert.NotNull(deletedMark.DeletedAt);
        }

        [Fact]
        public async Task DeleteGeoMarkCommandHandler_FailOnWrongId()
        {
            // Arrange
            var handler = new DeleteGeoMarkHandler(Context);
            var wrongMarkId = Guid.NewGuid();
            var mapId = GeoMarksContextFactory.GeoMapId;

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new DeleteGeoMarkCommand(mapId, wrongMarkId),
                    CancellationToken.None));
        }

        [Fact]
        public async Task DeleteGeoMarkCommandHandler_FailOnWrongMapId()
        {
            // Arrange
            var handler = new DeleteGeoMarkHandler(Context);
            var markId = GeoMarksContextFactory.TransitionMarkId;
            var wrongMapId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new DeleteGeoMarkCommand(wrongMapId, markId),
                    CancellationToken.None));
        }
    }
}
