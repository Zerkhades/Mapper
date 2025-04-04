using Mapper.Application.CommandsAndQueries.GeoMark.Commands.CreateGeoMarkCommand;
using Mapper.Application.Common.Exceptions;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Tests.GeoMarks.Commands
{
    public class CreateGeoMarkCommandHandlerTests : TestCommandBase
    {
        public CreateGeoMarkCommandHandlerTests() : base(new GeoMarksContextFactory())
        {
        }

        [Fact]
        public async Task CreateGeoMarkCommandHandler_Success()
        {
            // Arrange
            var handler = new CreateGeoMarkCommandHandler(Context);
            var geomarkName = "geomark name";
            var geomarkDescription = "geomark description";
            var geoMap = new Domain.GeoMap()
            {
                MapName = "mapname",
                MapDescription = "mapdesc"
            };

            // Act
            var geomarkId = await handler.Handle(
                new CreateGeoMarkCommand
                {
                    Id = Guid.NewGuid(),
                    MarkName = geomarkName,
                    MarkDescription = geomarkDescription,
                    IsArchived = false,
                    GeoMapId = GeoMarksContextFactory.GeoMarkIdForArchive
                },
                CancellationToken.None);

            // Assert
            Assert.NotNull(
                await Context.GeoMarks.SingleOrDefaultAsync(mark =>
                    mark.Id == geomarkId &&
                    mark.MarkName == geomarkName &&
                    mark.MarkDescription == geomarkDescription));
        }

        [Fact]
        public async Task CreateGeoMarkCommandHandler_FailOnSameId()
        {
            // Arrange
            var handler = new CreateGeoMarkCommandHandler(Context);
            var geomarkName = "geomark name";
            var geomarkDescription = "geomark description";
            var geoMap = new Domain.GeoMap()
            {
                MapName = "mapname",
                MapDescription = "mapdesc"
            };

            // Act
            // Assert
            await Assert.ThrowsAsync<AlreadyExistsException>(async () =>
                await handler.Handle(
                    new CreateGeoMarkCommand
                    {
                        Id = GeoMarksContextFactory.GeoMarkIdForCreate,
                        MarkName = geomarkName,
                        MarkDescription = geomarkDescription,
                        IsArchived = false,
                        GeoMapId = GeoMarksContextFactory.GeoMapIdForCreate
                    },
                    CancellationToken.None)
            );
        }
    }
}
