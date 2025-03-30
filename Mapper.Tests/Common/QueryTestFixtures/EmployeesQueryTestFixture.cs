using Mapper.Tests.Common.ContextFactories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Tests.Common.QueryTestFixtures
{
    public class EmployeesQueryTestFixture : QueryTestFixture<EmployeesContextFactory> { }

    [CollectionDefinition("EmployeesQueryCollection")]
    public class EmployeesQueryCollection : ICollectionFixture<EmployeesQueryTestFixture> { }
}
