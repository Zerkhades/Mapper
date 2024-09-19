using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Persistence
{
    public class DbInitializer
    {
        public static void Initialize(GeoMapDbContext context)
        {
            context.Database.EnsureCreated();
        }
    }

}
