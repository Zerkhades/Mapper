using Mapper.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapper.Application.CommandsAndQueries.GeoMap.Commands.DeleteGeoMapCommand;
using Mapper.Tests.Common;

namespace Mapper.Tests.GeoMaps.Commands
{
    public class DeleteGeoMapCommandHandlerTests : TestCommandBase
    {
        [Fact]
        public async Task DeleteNoteCommandHandler_Success()
        {
            // Arrange
            var handler = new DeleteGeoMapCommandHandler(Context);

            // Act
            await handler.Handle(new DeleteGeoMapCommand()
            {
                Id = GeoMapsContextFactory.GeoMapIdForDelete,
                // UserId = GeoMapsContextFactory.UserAId
            }, CancellationToken.None);

            // Assert
            Assert.NotNull(Context.GeoMaps.SingleOrDefault(note =>
                note.Id == GeoMapsContextFactory.GeoMapIdForDelete));
        }

        [Fact]
        public async Task DeleteNoteCommandHandler_FailOnWrongId()
        {
            // Arrange
            var handler = new DeleteGeoMapCommandHandler(Context);

            // Act
            // Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new DeleteGeoMapCommand()
                    {
                        Id = Guid.NewGuid(),
                    },
                    CancellationToken.None));
        }

        //[Fact]
        //public async Task DeleteNoteCommandHandler_FailOnWrongUserId()
        //{
        //    // Arrange
        //    var deleteHandler = new DeleteGeoMapCommandHandler(Context);
        //    var createHandler = new CreateGeoMapCommandHandler(Context);
        //    var noteId = await createHandler.Handle(
        //        new CreateGeoMapCommand
        //        {
        //            MapName = "NoteTitle",
        //            //UserId = GeoMapsContextFactory.UserAId
        //        }, CancellationToken.None);

        //    // Act
        //    // Assert
        //    await Assert.ThrowsAsync<NotFoundException>(async () =>
        //        await deleteHandler.Handle(
        //            new DeleteGeoMapCommand()
        //            {
        //                Id = noteId,
        //                UserId = GeoMapsContextFactory.UserBId
        //            }, CancellationToken.None));
        //}
    }
}
