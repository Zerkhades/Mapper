using AutoMapper;
using Mapper.Application.Features.Employees.Queries.GetEmployeeList;
using Mapper.Tests.Common;
using Mapper.Tests.Common.ContextFactories;
using Mapper.Tests.Common.QueryTestFixtures;

namespace Mapper.Tests.Employees.Queries
{
    [Collection("EmployeesQueryCollection")]
    public class GetEmployeeListQueryHandlerTests
    {
        private readonly IContextFactory ContextFactory;
        private readonly IMapper Mapper;

        public GetEmployeeListQueryHandlerTests(EmployeesQueryTestFixture fixture)
        {
            ContextFactory = fixture.ContextFactory;
            Mapper = fixture.Mapper;
        }

        [Fact]
        public async Task GetEmployeeListQuery_Success()
        {
            // Arrange
            using var context = ContextFactory.Create();
            var handler = new GetEmployeeListHandler(context, Mapper);

            // Act
            var result = await handler.Handle(
                new GetEmployeeListQuery(),
                CancellationToken.None);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, e => Assert.NotNull(e.FullName));
        }

        [Fact]
        public async Task GetEmployeeListQuery_FilterByGeoMarkId_Success()
        {
            // Arrange
            using var context = ContextFactory.Create();
            var handler = new GetEmployeeListHandler(context, Mapper);
            var workplaceId = EmployeesContextFactory.WorkplaceMarkId;

            // Act
            var result = await handler.Handle(
                new GetEmployeeListQuery(workplaceId),
                CancellationToken.None);

            // Assert
            Assert.NotEmpty(result);
            Assert.All(result, e => Assert.Equal(workplaceId, e.GeoMarkId));
        }

        [Fact]
        public async Task GetEmployeeListQuery_OrderedBySurname_Success()
        {
            // Arrange
            using var context = ContextFactory.Create();
            var handler = new GetEmployeeListHandler(context, Mapper);

            // Act
            var result = await handler.Handle(
                new GetEmployeeListQuery(),
                CancellationToken.None);

            // Assert
            Assert.NotEmpty(result);
            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.True(
                    string.Compare(result[i].Surname, result[i + 1].Surname, StringComparison.Ordinal) <= 0,
                    "Employees should be ordered by surname");
            }
        }

        [Fact]
        public async Task GetEmployeeListQuery_EmptyList_AfterArchiving()
        {
            // Arrange
            using var context = ContextFactory.Create();
            var handler = new GetEmployeeListHandler(context, Mapper);

            foreach (var employee in context.Employees)
            {
                employee.IsArchived = true;
            }
            await context.SaveChangesAsync();

            // Act
            var result = await handler.Handle(
                new GetEmployeeListQuery(),
                CancellationToken.None);

            // Assert
            Assert.Empty(result);
        }
    }
}
