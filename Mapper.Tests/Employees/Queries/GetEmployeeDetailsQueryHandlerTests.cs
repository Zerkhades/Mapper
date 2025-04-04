using AutoMapper;
using Mapper.Application.CommandsAndQueries.Employee.Queries.GetEmployeeDetails;
using Mapper.Application.CommandsAndQueries.GeoMap.Queries.GetGeoMapDetails;
using Mapper.Application.Common.Exceptions;
using Mapper.Persistence;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Mapper.Tests.Common.QueryTestFixtures;

namespace Mapper.Tests.Employees.Queries
{
    [Collection("EmployeesQueryCollection")]
    public class GetEmployeeDetailsQueryHandlerTests
    {
        private readonly IContextFactory Context;
        private readonly IMapper Mapper;

        public GetEmployeeDetailsQueryHandlerTests(EmployeesQueryTestFixture fixture)
        {
            Context = fixture.ContextFactory;
            Mapper = fixture.Mapper;
        }

        [Fact]
        public async Task GetEmployeeDetailsQueryHandler_Success()
        {
            // Arrange
            using var context = Context.Create();
            var handler = new GetEmployeeDetailsQueryHandler(context, Mapper);
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
            Assert.Equal("John Doe", result.FullName);
            Assert.Equal("john.doe@example.com", result.Email);
            Assert.False(result.IsArchived);
            Assert.NotNull(result.GeoMark);
            //Assert.NotNull(result.EmployeePhoto);
        }

        [Fact]
        public async Task GetEmployeeDetailsQueryHandler_FailOnNotFound()
        {
            // Arrange
            using var context = Context.Create();
            var handler = new GetEmployeeDetailsQueryHandler(context, Mapper);
            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await handler.Handle(
                    new GetEmployeeDetailsQuery()
                    {
                        Id = nonExistentId
                    },
                    CancellationToken.None);
            });
        }
    }
}
