using Mapper.Application.Features.Employees.Commands.CreateEmployee;
using Mapper.Application.Common.Exceptions;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Tests.Employees.Commands
{
    public class CreateEmployeeCommandHandlerTests : TestCommandBase
    {
        public CreateEmployeeCommandHandlerTests() : base(new EmployeesContextFactory())
        {
        }

        [Fact]
        public async Task CreateEmployeeCommand_Success()
        {
            // Arrange
            var handler = new CreateEmployeeHandler(Context);

            // Act
            var employeeId = await handler.Handle(
                new CreateEmployeeCommand(
                    FirstName: "Alice",
                    Surname: "Johnson",
                    Patronymic: "Marie",
                    Phone: "+1111111111",
                    Email: "alice.johnson@test.com",
                    Cabinet: "202",
                    Comment: "New employee",
                    GeoMarkId: EmployeesContextFactory.WorkplaceMarkId
                ),
                CancellationToken.None);

            // Assert
            var createdEmployee = await Context.Employees
                .SingleOrDefaultAsync(e => e.Id == employeeId);

            Assert.NotNull(createdEmployee);
            Assert.Equal("Alice", createdEmployee.FirstName);
            Assert.Equal("Johnson", createdEmployee.Surname);
            Assert.Equal("Marie", createdEmployee.Patronymic);
            Assert.Equal("+1111111111", createdEmployee.Phone);
            Assert.Equal("alice.johnson@test.com", createdEmployee.Email);
            Assert.Equal("202", createdEmployee.Cabinet);
            Assert.Equal("New employee", createdEmployee.Comment);
            Assert.Equal(EmployeesContextFactory.WorkplaceMarkId, createdEmployee.GeoMarkId);
            Assert.False(createdEmployee.IsArchived);
        }

        [Fact]
        public async Task CreateEmployeeCommand_WithMinimalData_Success()
        {
            // Arrange
            var handler = new CreateEmployeeHandler(Context);

            // Act
            var employeeId = await handler.Handle(
                new CreateEmployeeCommand(
                    FirstName: "Bob",
                    Surname: "Brown",
                    Patronymic: null,
                    Phone: null,
                    Email: null,
                    Cabinet: null,
                    Comment: null,
                    GeoMarkId: EmployeesContextFactory.WorkplaceMarkId
                ),
                CancellationToken.None);

            // Assert
            var createdEmployee = await Context.Employees
                .SingleOrDefaultAsync(e => e.Id == employeeId);

            Assert.NotNull(createdEmployee);
            Assert.Equal("Bob", createdEmployee.FirstName);
            Assert.Equal("Brown", createdEmployee.Surname);
            Assert.Null(createdEmployee.Patronymic);
        }

        [Fact]
        public async Task CreateEmployeeCommand_FailOnNonExistentWorkplace()
        {
            // Arrange
            var handler = new CreateEmployeeHandler(Context);
            var wrongWorkplaceId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new CreateEmployeeCommand("Test", "User", null, null, null, null, null, wrongWorkplaceId),
                    CancellationToken.None)
            );
        }
    }
}
