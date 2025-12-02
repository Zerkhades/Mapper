using Mapper.Application.CommandsAndQueries.GeoMark.Commands.DeleteGeoMarkCommand;
using Mapper.Application.Common.Exceptions;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;

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
            var handler = new DeleteGeoMarkCommandHandler(Context);

            // Act
            await handler.Handle(new DeleteGeoMarkCommand()
            {
                Id = GeoMarksContextFactory.GeoMarkIdForDelete,
                // UserId = GeoMarksContextFactory.UserAId
            }, CancellationToken.None);

            // Assert
            Assert.Null(Context.GeoMarks.SingleOrDefault(geoMark =>
                geoMark.Id == GeoMarksContextFactory.GeoMarkIdForDelete));
        }

        [Fact]
        public async Task DeleteGeoMarkCommandHandler_FailOnWrongId()
        {
            // Arrange
            var handler = new DeleteGeoMarkCommandHandler(Context);

            // Act
            // Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new DeleteGeoMarkCommand()
                    {
                        Id = Guid.NewGuid(),
                    },
                    CancellationToken.None));
        }

        //[Fact]
        //public async Task DeleteGeoMarkCommandHandler_FailOnWrongUserId()
        //{
        //    // Arrange
        //    var deleteHandler = new DeleteGeoMarkCommandHandler(Context);
        //    var createHandler = new CreateGeoMarkCommandHandler(Context);
        //    var markId = await createHandler.Handle(
        //        new CreateGeoMarkCommand
        //        {
        //            MarkName = "MarkTitle",
        //            //UserId = GeoMarksContextFactory.UserAId
        //        }, CancellationToken.None);

        //    // Act
        //    // Assert
        //    await Assert.ThrowsAsync<NotFoundException>(async () =>
        //        await deleteHandler.Handle(
        //            new DeleteGeoMarkCommand()
        //            {
        //                Id = markId,
        //                UserId = GeoMarksContextFactory.UserBId
        //            }, CancellationToken.None));
        //}
    }
}
