using Mapper.Application.Common.Exceptions;
using Mapper.Application.CommandsAndQueries.GeoMap.Commands.DeleteGeoMapCommand;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;

namespace Mapper.Tests.GeoMaps.Commands
{
    public class ArchiveGeoMapCommandHandlerTests : TestCommandBase
    {
        public ArchiveGeoMapCommandHandlerTests() : base(new GeoMapsContextFactory())
        {
        }

        [Fact]
        public async Task ArchiveGeomapCommandHandler_Success()
        {
            // Arrange
            var handler = new ArchiveGeoMapCommandHandler(Context);

            // Act
            await handler.Handle(new ArchiveGeoMapCommand()
            {
                Id = GeoMapsContextFactory.GeoMapIdForDelete,
                // UserId = GeoMapsContextFactory.UserAId
            }, CancellationToken.None);

            // Assert
            Assert.NotNull(Context.GeoMaps.SingleOrDefault(geoMap =>
                geoMap.Id == GeoMapsContextFactory.GeoMapIdForDelete 
                && geoMap.IsArchived == true));
        }

        [Fact]
        public async Task ArchiveGeomapCommandHandler_FailOnWrongId()
        {
            // Arrange
            var handler = new ArchiveGeoMapCommandHandler(Context);

            // Act
            // Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new ArchiveGeoMapCommand()
                    {
                        Id = Guid.NewGuid(),
                    },
                    CancellationToken.None));
        }

        //[Fact]
        //public async Task ArchiveGeomapCommandHandler_FailOnWrongUserId()
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
