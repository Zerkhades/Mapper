using AutoMapper;
using Mapper.Application.CommandsAndQueries.Employee.Queries.GetEmployeeList;
using Mapper.Persistence;
using Mapper.Tests.Common;
using Mapper.Tests.Common.QueryTestFixtures;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Tests.Employees.Queries
{
    [Collection("EmployeesQueryCollection")]
    public class GetEmployeeListQueryHandlerTests
    {
        private readonly IContextFactory _contextFactory;
        private readonly IMapper _mapper;

        public GetEmployeeListQueryHandlerTests(EmployeesQueryTestFixture fixture)
        {
            _contextFactory = fixture.ContextFactory;
            _mapper = fixture.Mapper;
        }

        [Fact]
        public async Task GetEmployeeListQueryHandler_Success()
        {
            // Arrange
            using var context = _contextFactory.Create();
            var handler = new GetEmployeeListQueryHandler(context, _mapper);
            
            // Act
            var result = await handler.Handle(
                new GetEmployeeListQuery(),
                CancellationToken.None);

            // Assert
            Assert.NotEmpty(result.Employees);
        }

        [Fact]
        public async Task GetEmployeeListQueryHandler_EmptyList()
        {
            // Arrange
            using var context = _contextFactory.Create();
            var handler = new GetEmployeeListQueryHandler(context, _mapper);

            // Удаляем все записи из контекста для проверки пустого списка
            context.Employees.RemoveRange(context.Employees);
            await context.SaveChangesAsync();

            // Act
            var result = await handler.Handle(new GetEmployeeListQuery(), CancellationToken.None);

            // Assert
            Assert.Empty(result.Employees);
        }
    }
}
