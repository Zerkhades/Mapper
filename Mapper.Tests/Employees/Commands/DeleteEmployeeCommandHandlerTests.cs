using Mapper.Application.CommandsAndQueries.Employee.Commands.DeleteEmployeeCommand;
using Mapper.Application.Common.Exceptions;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Tests.Employees.Commands
{
    public class DeleteEmployeeCommandHandlerTests : TestCommandBase
    {
        public DeleteEmployeeCommandHandlerTests() : base(new EmployeesContextFactory())
        {
        }

        [Fact]
        public async Task DeleteEmployeeCommandHandler_Success()
        {
            // Arrange
            var handler = new DeleteEmployeeCommandHandler(Context);
            var employeeId = EmployeesContextFactory.EmployeeIdForDelete;

            // Act
            await handler.Handle(
                new DeleteEmployeeCommand
                {
                    Id = employeeId
                },
                CancellationToken.None);

            // Assert
            Assert.Null(
                await Context.Employees.SingleOrDefaultAsync(employee =>
                    employee.Id == employeeId));
        }

        [Fact]
        public async Task DeleteEmployeeCommandHandler_FailOnWrongId()
        {
            // Arrange
            var handler = new DeleteEmployeeCommandHandler(Context);
            var wrongId = Guid.NewGuid();

            // Act
            // Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new DeleteEmployeeCommand
                    {
                        Id = wrongId
                    },
                    CancellationToken.None)
            );
        }
    }
}
