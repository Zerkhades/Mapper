

using Mapper.Persistence;
using Mapper.Tests.Common.ContextFactories;

namespace Mapper.Tests.Common
{
    public abstract class TestCommandBase : IDisposable
    {
        protected readonly MapperDbContext Context;

        public TestCommandBase()
        {
            Context = GeoMapsContextFactory.Create();
        }

        public void Dispose()
        {
            GeoMapsContextFactory.Destroy(Context);
        }
    }
}
