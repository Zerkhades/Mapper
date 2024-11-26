
using System.Diagnostics.CodeAnalysis;

namespace Mapper.Persistence
{
    public class DbInitializer
    {
        [SuppressMessage("ReSharper.DPA", "DPA0009: High execution time of DB command", MessageId = "time: 548ms")]
        public static void Initialize(MapperDbContext context)
        {
            context.Database.EnsureCreated();
        }
    }

}
