
namespace Mapper.Persistence
{
    public class DbInitializer
    {
        public static void Initialize(MapperDbContext context)
        {
            context.Database.EnsureCreated();
        }
    }

}
