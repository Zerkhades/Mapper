using Mapper.Application.Features.Employees.Commands.DeleteEmployee;
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
        public async Task DeleteEmployeeCommand_Success()
        {
            // Arrange
            var handler = new DeleteEmployeeHandler(Context);
            var employeeId = EmployeesContextFactory.EmployeeIdForDelete;

            // Act
            await handler.Handle(
                new DeleteEmployeeCommand(employeeId),
                CancellationToken.None);

            // Assert
            var deletedEmployee = await Context.Employees
                .SingleOrDefaultAsync(e => e.Id == employeeId);

            Assert.NotNull(deletedEmployee);
            Assert.True(deletedEmployee.IsArchived);
        }

        [Fact]
        public async Task DeleteEmployeeCommand_FailOnWrongId()
        {
            // Arrange
            var handler = new DeleteEmployeeHandler(Context);
            var wrongId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new DeleteEmployeeCommand(wrongId),
                    CancellationToken.None)
            );
        }
    }
}
