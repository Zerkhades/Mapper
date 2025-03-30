using Mapper.Application.CommandsAndQueries.Employee.Commands.CreateEmployeeCommand;
using Mapper.Application.Common.Exceptions;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Tests.Employees.Commands
{
    public class CreateEmployeeCommandHandlerTests : TestCommandBase
    {
        public CreateEmployeeCommandHandlerTests() : base(new EmployeesContextFactory())
        {
        }

        [Fact]
        public async Task CreateEmployeeCommandHandler_Success()
        {
            // Arrange
            var handler = new CreateEmployeeCommandHandler(Context);
            var employeeName = "John";
            var employeeSurname = "Doe";

            // Act
            var employeeId = await handler.Handle(
                new CreateEmployeeCommand
                {
                    Id = Guid.NewGuid(),
                    FirstName = employeeName,
                    Surname = employeeSurname,
                    IsArchived = false
                },
                CancellationToken.None);

            // Assert
            Assert.NotNull(
                await Context.Employees.SingleOrDefaultAsync(employee =>
                    employee.Id == employeeId &&
                    employee.FirstName == employeeName &&
                    employee.Surname == employeeSurname &&
                    employee.IsArchived == false));
        }

        [Fact]
        public async Task CreateEmployeeCommandHandler_FailOnSameId()
        {
            // Arrange
            var handler = new CreateEmployeeCommandHandler(Context);
            var employeeName = "John";
            var employeeSurname = "Doe";

            // Act
            // Assert
            await Assert.ThrowsAsync<AlreadyExistsException>(async () =>
                await handler.Handle(
                    new CreateEmployeeCommand
                    {
                        Id = EmployeesContextFactory.EmployeeIdForCreate,
                        FirstName = employeeName,
                        Surname = employeeSurname,
                        IsArchived = false
                    },
                    CancellationToken.None)
            );
        }
    }
}
