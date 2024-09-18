using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Mapper.DBService.DbContexts
{
    public class MapperDbContextFactory : IMapperDbContextFactory
    {
        private readonly string _connectionString;
        public MapperDbContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public MapperDbContext CreateDbContext()
        {
            DbContextOptions options = new DbContextOptionsBuilder().UseSqlServer(_connectionString).Options;
            return new MapperDbContext(options);
        }
    }
}
