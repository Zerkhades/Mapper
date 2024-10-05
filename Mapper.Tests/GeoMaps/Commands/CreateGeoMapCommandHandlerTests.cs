using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapper.Application.CommandsAndQueries.GeoMap.Commands.CreateGeoMapCommand;
using Mapper.Tests.Common;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Tests.GeoMaps.Commands
{
    public class CreateGeoMapCommandHandlerTests : TestCommandBase
    {
        [Fact]
        public async Task CreateNoteCommandHandler_Success()
        {
            // Arrange
            var handler = new CreateGeoMapCommandHandler(Context);
            var geomapName = "geomap name";
            var geomapDescription = "geomap description";

            // Act
            var geomapId = await handler.Handle(
                new CreateGeoMapCommand
                {
                    Id = GeoMapsContextFactory.GeoMapIdForCreate,
                    MapName = geomapName,
                    MapDescription = geomapDescription,
                    IsArchived = false
                },
                CancellationToken.None);

            // Assert
            Assert.NotNull(
                await Context.GeoMaps.SingleOrDefaultAsync(note =>
                    note.Id == geomapId &&
                    note.MapName == geomapName &&
                    note.MapDescription == geomapDescription));
        }
    }
}
