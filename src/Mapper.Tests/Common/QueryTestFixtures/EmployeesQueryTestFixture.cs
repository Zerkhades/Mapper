using Mapper.Tests.Common.ContextFactories;

namespace Mapper.Tests.Common.QueryTestFixtures
{
    public class EmployeesQueryTestFixture : QueryTestFixture<EmployeesContextFactory> { }

    [CollectionDefinition("EmployeesQueryCollection")]
    public class EmployeesQueryCollection : ICollectionFixture<EmployeesQueryTestFixture> { }
}
