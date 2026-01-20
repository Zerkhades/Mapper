using AutoMapper;
using Mapper.Application.Features.Employees.Queries.GetEmployeeDetails;
using Mapper.Application.Common.Exceptions;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Mapper.Tests.Common.QueryTestFixtures;

namespace Mapper.Tests.Employees.Queries
{
    [Collection("EmployeesQueryCollection")]
    public class GetEmployeeDetailsQueryHandlerTests
    {
        private readonly IContextFactory ContextFactory;
        private readonly IMapper Mapper;

        public GetEmployeeDetailsQueryHandlerTests(EmployeesQueryTestFixture fixture)
        {
            ContextFactory = fixture.ContextFactory;
            Mapper = fixture.Mapper;
        }

        [Fact]
        public async Task GetEmployeeDetailsQuery_Success()
        {
            // Arrange
            using var context = ContextFactory.Create();
            var handler = new GetEmployeeDetailsHandler(context, Mapper);
            var employeeId = EmployeesContextFactory.EmployeeIdForUpdate;

            // Act
            var result = await handler.Handle(
                new GetEmployeeDetailsQuery(employeeId),
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(employeeId, result.Id);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.Surname);
            Assert.Equal("Smith", result.Patronymic);
            Assert.Equal("+1234567890", result.Phone);
            Assert.Equal("john.doe@test.com", result.Email);
            Assert.Equal("101", result.Cabinet);
            Assert.Equal("Test employee for update", result.Comment);
            Assert.NotNull(result.FullName);
            Assert.Contains("John", result.FullName);
            Assert.Contains("Doe", result.FullName);
        }

        [Fact]
        public async Task GetEmployeeDetailsQuery_FailOnNotFound()
        {
            // Arrange
            using var context = ContextFactory.Create();
            var handler = new GetEmployeeDetailsHandler(context, Mapper);
            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new GetEmployeeDetailsQuery(nonExistentId),
                    CancellationToken.None)
            );
        }

        [Fact]
        public async Task GetEmployeeDetailsQuery_FailOnArchivedEmployee()
        {
            // Arrange
            using var context = ContextFactory.Create();
            var handler = new GetEmployeeDetailsHandler(context, Mapper);
            var employeeId = EmployeesContextFactory.EmployeeIdForDelete;

            var employee = await context.Employees.FindAsync(employeeId);
            employee!.IsArchived = true;
            await context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await handler.Handle(
                    new GetEmployeeDetailsQuery(employeeId),
                    CancellationToken.None)
            );
        }
    }
}
