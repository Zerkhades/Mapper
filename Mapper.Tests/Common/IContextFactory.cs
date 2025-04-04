using Mapper.Persistence;

namespace Mapper.Tests.Common
{
    public interface IContextFactory
    {
        MapperDbContext Create();
        void Destroy(MapperDbContext context);
    }
}
