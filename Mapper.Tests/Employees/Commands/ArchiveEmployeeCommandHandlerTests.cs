using Mapper.Application.CommandsAndQueries.Employee.Commands.ArchiveEmployeeCommand;
using Mapper.Application.Common.Exceptions;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Tests.Employees.Commands
{
    public class ArchiveEmployeeCommandHandlerTests : TestCommandBase
    {
        [Fact]
        public async Task ArchiveEmployeeCommandHandler_Success()
        {
            // Arrange
            var handler = new ArchiveEmployeeCommandHandler(Context);

            // Act
            await handler.Handle(new ArchiveEmployeeCommand()
            {
                Id = EmployeesContextFactory.EmployeeIdForDelete,
            }, CancellationToken.None);

            // Assert
            Assert.NotNull(Context.Employees.SingleOrDefault(employee =>
                employee.Id == EmployeesContextFactory.EmployeeIdForDelete
                && employee.IsArchived == true));
        }

        [Fact]
        public async Task ArchiveEmployeeCommandHandler_FailOnWrongId()
        {
            // Arrange
            var handler = new ArchiveEmployeeCommandHandler(Context);

            // Act
            // Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new ArchiveEmployeeCommand()
                    {
                        Id = Guid.NewGuid(),
                    },
                    CancellationToken.None));
        }
    }
}
