using AutoMapper;
using Mapper.Application.CommandsAndQueries.Employee.Queries.GetEmployeeList;
using Mapper.Persistence;
using Mapper.Tests.Common.QueryTestFixtures;

namespace Mapper.Tests.Employees.Queries
{
    [Collection("EmployeesQueryCollection")]
    [TestCaseOrderer("FullNameOfOrderStrategyHere", "OrderStrategyAssemblyName")]
    public class GetEmployeeListQueryHandlerTests
    {
        private readonly MapperDbContext Context;
        private readonly IMapper Mapper;

        public GetEmployeeListQueryHandlerTests(EmployeesQueryTestFixture fixture)
        {
            Context = fixture.Context;
            Mapper = fixture.Mapper;
        }

        [Fact]

        public async Task GetEmployeeListQueryHandler_Success()
        {
            // Arrange
            var handler = new GetEmployeeListQueryHandler(Context, Mapper);

            // Act
            var result = await handler.Handle(
                new GetEmployeeListQuery(),
                CancellationToken.None);

            // Assert
            Assert.NotEmpty(result.Employees);
        }

        // Dunno why, but it keeps testing this first
        //[Fact]
        //public async Task GetEmployeeListQueryHandler_EmptyList()
        //{
        //    // Arrange
        //    var handler = new GetEmployeeListQueryHandler(Context, Mapper);

        //    // Удаляем все записи из контекста для проверки пустого списка
        //    Context.Employees.RemoveRange(Context.Employees);
        //    await Context.SaveChangesAsync();

        //    // Act
        //    var result = await handler.Handle(
        //        new GetEmployeeListQuery(),
        //        CancellationToken.None);

        //    // Assert
        //    Assert.Empty(result.Employees);
        //}
    }
}
