using Mapper.Application.CommandsAndQueries.Employee.Commands.CreateEmployeeCommand;
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
                    IsArchived = false,
                    GeoMarkId = EmployeesContextFactory.GeoMapIdForCreate
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
            var geoMark = new Domain.GeoMark
            {
                MarkName = "Test Mark",
                Color = "Red",
                Emoji = "😊",
                XPos = 10.0,
                YPos = 20.0,
            };

            // Act
            // Assert
            await Assert.ThrowsAsync<AlreadyExistsException>(async () =>
                await handler.Handle(
                    new CreateEmployeeCommand
                    {
                        Id = EmployeesContextFactory.EmployeeIdForCreate,
                        FirstName = employeeName,
                        Surname = employeeSurname,
                        IsArchived = false,
                        GeoMarkId = EmployeesContextFactory.GeoMarkIdForCreate
                    },
                    CancellationToken.None)
            );
        }
    }
}
