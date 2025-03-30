using Mapper.Application.CommandsAndQueries.GeoMap.Commands.DeleteGeoMapCommand;
using Mapper.Application.Common.Exceptions;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Tests.GeoMaps.Commands
{
    public class DeleteGeoMapCommandHandlerTests : TestCommandBase
    {
        public DeleteGeoMapCommandHandlerTests() : base(new GeoMapsContextFactory())
        {
        }

        [Fact]
        public async Task DeleteGeoMapCommandHandler_Success()
        {
            // Arrange
            var handler = new DeleteGeoMapCommandHandler(Context);
            var geomapId = GeoMapsContextFactory.GeoMapIdForDelete;

            // Act
            await handler.Handle(
                new DeleteGeoMapCommand
                {
                    Id = geomapId
                },
                CancellationToken.None);

            // Assert
            Assert.Null(
                await Context.GeoMaps.SingleOrDefaultAsync(note =>
                    note.Id == geomapId));
        }

        [Fact]
        public async Task DeleteGeoMapCommandHandler_FailOnWrongId()
        {
            // Arrange
            var handler = new DeleteGeoMapCommandHandler(Context);
            var wrongId = Guid.NewGuid();

            // Act
            // Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new DeleteGeoMapCommand
                    {
                        Id = wrongId
                    },
                    CancellationToken.None)
            );
        }
    }
}