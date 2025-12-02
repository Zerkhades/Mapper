

using Mapper.Persistence;
using Mapper.Tests.Common.ContextFactories;

namespace Mapper.Tests.Common
{
    public abstract class TestCommandBase : IDisposable
    {
        protected readonly MapperDbContext Context;
        protected readonly IContextFactory ContextFactory;

        public TestCommandBase(IContextFactory contextFactory)
        {
            ContextFactory = contextFactory;
            Context = ContextFactory.Create();
        }

        public void Dispose()
        {
            ContextFactory.Destroy(Context);
        }
    }
}
