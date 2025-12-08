using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Mapper.Persistence
{
    public class DbInitializer
    {
        [SuppressMessage("ReSharper.DPA", "DPA0009: High execution time of DB command", MessageId = "time: 548ms")]
        public static void Initialize(MapperDbContext context)
        {
            try
            {
                // Apply pending migrations if any; otherwise no-op
                var pending = context.Database.GetPendingMigrations();
                if (pending.Any())
                {
                    context.Database.Migrate();
                }
            }
            catch
            {
                // Swallow exceptions to prevent app crash if DB is not reachable at startup
            }
        }
    }

}
