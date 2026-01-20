using Mapper.Application.Features.GeoMaps.Commands.DeleteGeoMap;
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
            var handler = new DeleteGeoMapHandler(Context);
            var geoMapId = GeoMapsContextFactory.GeoMapIdForDelete;

            // Act
            await handler.Handle(
                new DeleteGeoMapCommand(geoMapId),
                CancellationToken.None);

            // Assert
            var deletedMap = await Context.GeoMaps
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(m => m.Id == geoMapId);
            
            Assert.NotNull(deletedMap);
            Assert.True(deletedMap.IsDeleted);
            Assert.NotNull(deletedMap.DeletedAt);
        }

        [Fact]
        public async Task DeleteGeoMapCommandHandler_FailOnWrongId()
        {
            // Arrange
            var handler = new DeleteGeoMapHandler(Context);
            var wrongId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new DeleteGeoMapCommand(wrongId),
                    CancellationToken.None)
            );
        }
    }
}