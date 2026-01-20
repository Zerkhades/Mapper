using Mapper.Application.Features.GeoMarks.Commands.WorkplaceMarkCommands;
using Mapper.Application.Common.Exceptions;
using Mapper.Application.Interfaces;
using Mapper.Domain;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Mapper.Tests.GeoMarks.Commands
{
    public class UpdateWorkplaceMarkCommandHandlerTests : TestCommandBase
    {
        private readonly Mock<ICacheService> _mockCache;
        private readonly Mock<IMapRealtimeNotifier> _mockNotifier;

        public UpdateWorkplaceMarkCommandHandlerTests() : base(new GeoMarksContextFactory())
        {
            _mockCache = new Mock<ICacheService>();
            _mockNotifier = new Mock<IMapRealtimeNotifier>();
        }

        [Fact]
        public async Task UpdateWorkplaceMarkCommand_Success()
        {
            // Arrange
            var handler = new UpdateWorkplaceMarkHandler(Context, _mockCache.Object, _mockNotifier.Object);
            var markId = GeoMarksContextFactory.WorkplaceMarkId;
            var mapId = GeoMarksContextFactory.GeoMapId;

            // Act
            await handler.Handle(
                new UpdateWorkplaceMarkCommand(
                    GeoMapId: mapId,
                    MarkId: markId,
                    X: 0.4,
                    Y: 0.5,
                    Title: "Updated Workplace",
                    Description: "Updated workplace description",
                    WorkplaceCode: "WP-999",
                    EmployeeIds: null
                ),
                CancellationToken.None);

            // Assert
            var updatedMark = await Context.GeoMarks.OfType<WorkplaceMark>()
                .SingleOrDefaultAsync(m => m.Id == markId);

            Assert.NotNull(updatedMark);
            Assert.Equal(0.4, updatedMark.X);
            Assert.Equal(0.5, updatedMark.Y);
            Assert.Equal("Updated Workplace", updatedMark.Title);
            Assert.Equal("Updated workplace description", updatedMark.Description);
            Assert.Equal("WP-999", updatedMark.WorkplaceCode);

            _mockCache.Verify(x => x.RemoveAsync($"geomap:{mapId}", It.IsAny<CancellationToken>()), Times.Once);
            _mockNotifier.Verify(x => x.MarkUpdated(mapId, It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateWorkplaceMarkCommand_WithEmployees_Success()
        {
            // Arrange
            var handler = new UpdateWorkplaceMarkHandler(Context, _mockCache.Object, _mockNotifier.Object);
            var markId = GeoMarksContextFactory.WorkplaceMarkId;
            var mapId = GeoMarksContextFactory.GeoMapId;

            // Создаем тестовых сотрудников
            var employee1 = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                Surname = "Doe",
                Email = "john@test.com",
                GeoMarkId = markId
            };
            var employee2 = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                Surname = "Smith",
                Email = "jane@test.com",
                GeoMarkId = markId
            };

            Context.Employees.AddRange(employee1, employee2);
            await Context.SaveChangesAsync();

            // Act
            await handler.Handle(
                new UpdateWorkplaceMarkCommand(
                    GeoMapId: mapId,
                    MarkId: markId,
                    X: 0.4,
                    Y: 0.5,
                    Title: "Workplace with Employees",
                    Description: null,
                    WorkplaceCode: "WP-888",
                    EmployeeIds: new[] { employee1.Id, employee2.Id }
                ),
                CancellationToken.None);

            // Assert
            var updatedMark = await Context.GeoMarks.OfType<WorkplaceMark>()
                .Include(m => m.Employees)
                .SingleOrDefaultAsync(m => m.Id == markId);

            Assert.NotNull(updatedMark);
            Assert.Equal(2, updatedMark.Employees.Count);
            Assert.Contains(updatedMark.Employees, e => e.Id == employee1.Id);
            Assert.Contains(updatedMark.Employees, e => e.Id == employee2.Id);
        }

        [Fact]
        public async Task UpdateWorkplaceMarkCommand_FailOnWrongMarkId()
        {
            // Arrange
            var handler = new UpdateWorkplaceMarkHandler(Context, _mockCache.Object, _mockNotifier.Object);
            var wrongMarkId = Guid.NewGuid();
            var mapId = GeoMarksContextFactory.GeoMapId;

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new UpdateWorkplaceMarkCommand(mapId, wrongMarkId, 0.5, 0.5, "Title", null, "WP-001", null),
                    CancellationToken.None)
            );
        }
    }
}
