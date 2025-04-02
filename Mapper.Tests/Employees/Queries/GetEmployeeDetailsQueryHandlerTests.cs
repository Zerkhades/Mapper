using AutoMapper;
using Mapper.Application.CommandsAndQueries.Employee.Queries.GetEmployeeDetails;
using Mapper.Persistence;
using Mapper.Tests.Common.ContextFactories;
using Mapper.Tests.Common.QueryTestFixtures;

namespace Mapper.Tests.Employees.Queries
{
    [Collection("EmployeesQueryCollection")]
    public class GetEmployeeDetailsQueryHandlerTests
    {
        private readonly MapperDbContext Context;
        private readonly IMapper Mapper;

        public GetEmployeeDetailsQueryHandlerTests(EmployeesQueryTestFixture fixture)
        {
            Context = fixture.Context;
            Mapper = fixture.Mapper;
        }

        [Fact]
        public async Task GetEmployeeDetailsQueryHandler_Success()
        {
            // Arrange
            var handler = new GetEmployeeDetailsQueryHandler(Context, Mapper);
            var id = EmployeesContextFactory.EmployeeIdForCreate;

            // Act
            var result = await handler.Handle(
                new GetEmployeeDetailsQuery()
                {
                    Id = id
                },
                CancellationToken.None);

            // Assert
            Assert.IsType<EmployeeDetailsVm>(result);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.Surname);
            // TODO: fix
            // There is always a blank space if patronymic is empty
            Assert.Equal("John Doe", result.FullName);
            Assert.Equal("john.doe@example.com", result.Email);
            Assert.False(result.IsArchived);
            Assert.NotNull(result.GeoMark);
            //Assert.NotNull(result.EmployeePhoto);
        }
    }
}
