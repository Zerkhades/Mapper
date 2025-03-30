using Mapper.Application.CommandsAndQueries.Employee.Commands.UpdateEmployeeCommand;
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
        public async Task UpdateEmployeeCommandHandler_Success()
        {
            // Arrange
            var handler = new UpdateEmployeeCommandHandler(Context);
            var updatedFirstName = "Booba";
            var updatedSurname = "Job";

            // Act
            await handler.Handle(new UpdateEmployeeCommand
            {
                Id = EmployeesContextFactory.EmployeeIdForUpdate,
                FirstName = updatedFirstName,
                Surname = updatedSurname
            }, CancellationToken.None);

            // Assert
            Assert.NotNull(await Context.Employees.SingleOrDefaultAsync(employee =>
                employee.Id == EmployeesContextFactory.EmployeeIdForUpdate &&
                employee.FirstName == updatedFirstName &&
                employee.Surname == updatedSurname));
        }

        [Fact]
        public async Task UpdateEmployeeCommandHandler_FailOnWrongId()
        {
            // Arrange
            var handler = new UpdateEmployeeCommandHandler(Context);

            // Act
            // Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new UpdateEmployeeCommand
                    {
                        Id = Guid.NewGuid(),
                    },
                    CancellationToken.None));
        }
    }
}
