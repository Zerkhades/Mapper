using Mapper.Application.CommandsAndQueries.GeoMark.Commands.ArchiveGeoMarkCommand;
using Mapper.Application.Common.Exceptions;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;

namespace Mapper.Tests.GeoMarks.Commands
{
    public class ArchiveGeoMarkCommandHandlerTests : TestCommandBase
    {
        public ArchiveGeoMarkCommandHandlerTests() : base(new GeoMarksContextFactory())
        {
        }

        [Fact]
        public async Task ArchiveGeoMarkCommandHandler_Success()
        {
            // Arrange
            var handler = new ArchiveGeoMarkCommandHandler(Context);

            // Act
            await handler.Handle(new ArchiveGeoMarkCommand()
            {
                Id = GeoMarksContextFactory.GeoMarkIdForArchive,
            }, CancellationToken.None);

            // Assert
            Assert.NotNull(Context.GeoMarks.SingleOrDefault(geoMark =>
                geoMark.Id == GeoMarksContextFactory.GeoMarkIdForArchive
                && geoMark.IsArchived == true));
        }

        [Fact]
        public async Task ArchiveGeoMarkCommandHandler_FailOnWrongId()
        {
            // Arrange
            var handler = new ArchiveGeoMarkCommandHandler(Context);

            // Act
            // Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new ArchiveGeoMarkCommand()
                    {
                        Id = Guid.NewGuid(),
                    },
                    CancellationToken.None));
        }
    }
}
