using Mapper.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Tests.Common
{
    public interface IContextFactory
    {
        MapperDbContext Create();
        void Destroy(MapperDbContext context);
    }
}
