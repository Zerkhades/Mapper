using Mapper.Application.Features.Employees.Commands.UpdateEmployee;
using Mapper.Application.Common.Exceptions;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Tests.Employees.Commands
{
    public class UpdateEmployeeCommandHandlerTests : TestCommandBase
    {
        public UpdateEmployeeCommandHandlerTests() : base(new EmployeesContextFactory())
        {
        }

        [Fact]
        public async Task UpdateEmployeeCommand_Success()
        {
            // Arrange
            var handler = new UpdateEmployeeHandler(Context);
            var employeeId = EmployeesContextFactory.EmployeeIdForUpdate;

            // Act
            await handler.Handle(
                new UpdateEmployeeCommand(
                    Id: employeeId,
                    FirstName: "UpdatedFirstName",
                    Surname: "UpdatedSurname",
                    Patronymic: "UpdatedPatronymic",
                    Phone: "+9999999999",
                    Email: "updated@test.com",
                    Cabinet: "999",
                    Comment: "Updated comment",
                    GeoMarkId: EmployeesContextFactory.WorkplaceMarkId
                ),
                CancellationToken.None);

            // Assert
            var updatedEmployee = await Context.Employees
                .SingleOrDefaultAsync(e => e.Id == employeeId);

            Assert.NotNull(updatedEmployee);
            Assert.Equal("UpdatedFirstName", updatedEmployee.FirstName);
            Assert.Equal("UpdatedSurname", updatedEmployee.Surname);
            Assert.Equal("UpdatedPatronymic", updatedEmployee.Patronymic);
            Assert.Equal("+9999999999", updatedEmployee.Phone);
            Assert.Equal("updated@test.com", updatedEmployee.Email);
            Assert.Equal("999", updatedEmployee.Cabinet);
            Assert.Equal("Updated comment", updatedEmployee.Comment);
        }

        [Fact]
        public async Task UpdateEmployeeCommand_ChangeWorkplace_Success()
        {
            // Arrange
            var handler = new UpdateEmployeeHandler(Context);
            var employeeId = EmployeesContextFactory.EmployeeIdForUpdate;
            var newWorkplaceId = EmployeesContextFactory.AnotherWorkplaceMarkId;

            // Act
            await handler.Handle(
                new UpdateEmployeeCommand(
                    Id: employeeId,
                    FirstName: "John",
                    Surname: "Doe",
                    Patronymic: null,
                    Phone: null,
                    Email: null,
                    Cabinet: null,
                    Comment: null,
                    GeoMarkId: newWorkplaceId
                ),
                CancellationToken.None);

            // Assert
            var updatedEmployee = await Context.Employees
                .SingleOrDefaultAsync(e => e.Id == employeeId);

            Assert.NotNull(updatedEmployee);
            Assert.Equal(newWorkplaceId, updatedEmployee.GeoMarkId);
        }

        [Fact]
        public async Task UpdateEmployeeCommand_FailOnWrongEmployeeId()
        {
            // Arrange
            var handler = new UpdateEmployeeHandler(Context);
            var wrongId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new UpdateEmployeeCommand(wrongId, "Test", "User", null, null, null, null, null, EmployeesContextFactory.WorkplaceMarkId),
                    CancellationToken.None)
            );
        }

        [Fact]
        public async Task UpdateEmployeeCommand_FailOnWrongWorkplaceId()
        {
            // Arrange
            var handler = new UpdateEmployeeHandler(Context);
            var employeeId = EmployeesContextFactory.EmployeeIdForUpdate;
            var wrongWorkplaceId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new UpdateEmployeeCommand(employeeId, "Test", "User", null, null, null, null, null, wrongWorkplaceId),
                    CancellationToken.None)
            );
        }
    }
}
