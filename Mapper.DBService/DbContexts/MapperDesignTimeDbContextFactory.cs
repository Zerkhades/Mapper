using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Mapper.DBService.DbContexts
{
    class MapperDesignTimeDbContextFactory : IDesignTimeDbContextFactory<MapperDbContext>
    {
        public MapperDbContext CreateDbContext(string[] args)
        {
            DbContextOptions options = new DbContextOptionsBuilder().UseSqlServer("Data Source=DESKTOP-RFEI2A0\\SQLEXPRESS;Initial Catalog=MapDB;TrustServerCertificate=True;Integrated Security=True;").Options;

            return new MapperDbContext(options);
        }
    }
}
