using Mapper.Application.CommandsAndQueries.GeoMap.Commands.UpdateGeoMapCommand;
using Mapper.Application.CommandsAndQueries.GeoMark.Commands.UpdateGeoMarkCommand;
using Mapper.Application.Common.Exceptions;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Tests.GeoMarks.Commands
{
    public class UpdateGeoMarkCommandHandlerTests : TestCommandBase
    {
        public UpdateGeoMarkCommandHandlerTests() : base(new GeoMarksContextFactory())
        {
        }

        [Fact]
        public async Task UpdateGeoMarkCommandHandler_Success()
        {
            // Arrange
            var handler = new UpdateGeoMarkCommandHandler(Context);
            var updatedMarkName = "UpdatedGeoMark";

            // Act
            await handler.Handle(new UpdateGeoMarkCommand
            {
                Id = GeoMarksContextFactory.GeoMarkIdForUpdate,
                MarkName = updatedMarkName,
            }, CancellationToken.None);

            // Assert
            Assert.NotNull(await Context.GeoMarks.SingleOrDefaultAsync(geoMark =>
                geoMark.Id == GeoMarksContextFactory.GeoMarkIdForUpdate &&
                geoMark.MarkName == updatedMarkName));
        }

        [Fact]
        public async Task UpdateGeoMarkCommandHandler_FailOnWrongId()
        {
            // Arrange
            var handler = new UpdateGeoMarkCommandHandler(Context);
            var updatedMarkName = "UpdatedGeoMark";

            // Act
            // Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new UpdateGeoMarkCommand
                    {
                        Id = Guid.NewGuid(),
                        MarkName = updatedMarkName,
                    },
                    CancellationToken.None));
        }
    }
}
