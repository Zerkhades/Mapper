using Mapper.Application.CommandsAndQueries.GeoMap.Commands.UpdateGeoMapCommand;
using Mapper.Application.Common.Exceptions;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Tests.GeoMaps.Commands
{
    public class UpdateGeoMapCommandHandlerTests : TestCommandBase
    {
        public UpdateGeoMapCommandHandlerTests() : base(new GeoMapsContextFactory())
        {
        }

        [Fact]
        public async Task UpdateGeomapCommandHandler_Success()
        {
            // Arrange
            var handler = new UpdateGeoMapCommandHandler(Context);
            var updatedMapName = "new title";
            var updatedDescription = "new description";

            // Act
            await handler.Handle(new UpdateGeoMapCommand
            {
                Id = GeoMapsContextFactory.GeoMapIdForUpdate,
                //UserId = GeoMapsContextFactory.UserBId,
                MapName = updatedMapName,
                MapDescription = updatedDescription
            }, CancellationToken.None);

            // Assert
            Assert.NotNull(await Context.GeoMaps.SingleOrDefaultAsync(note =>
                note.Id == GeoMapsContextFactory.GeoMapIdForUpdate &&
                note.MapDescription == updatedDescription));
        }

        [Fact]
        public async Task UpdateGeomapCommandHandler_FailOnWrongId()
        {
            // Arrange
            var handler = new UpdateGeoMapCommandHandler(Context);

            // Act
            // Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new UpdateGeoMapCommand
                    {
                        Id = Guid.NewGuid(),
                        MapName = "Gruh",
                        MapDescription = "Bruh"
                        // UserId = NotesContextFactory.UserAId
                    },
                    CancellationToken.None));
        }

        //[Fact]
        //public async Task UpdateNoteCommandHandler_FailOnWrongUserId()
        //{
        //    // Arrange
        //    var handler = new UpdateNoteCommandHandler(Context);

        //    // Act
        //    // Assert
        //    await Assert.ThrowsAsync<NotFoundException>(async () =>
        //    {
        //        await handler.Handle(
        //            new UpdateNoteCommand
        //            {
        //                Id = NotesContextFactory.NoteIdForUpdate,
        //                UserId = NotesContextFactory.UserAId
        //            },
        //            CancellationToken.None);
        //    });
        //}
    }
}
