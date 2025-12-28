using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.EnvironmentVariables; // Добавьте этот using
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Persistence
{
    public class MapperDbContextFactory : IDesignTimeDbContextFactory<MapperDbContext>
    {
        public MapperDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables() // Теперь метод доступен
                .Build();

            var cs = configuration.GetConnectionString("DefaultConnection")
                     ?? "Host=localhost;Database=MapperDB;Username=mapper_user;Password=mapper_pass";

            var options = new DbContextOptionsBuilder<MapperDbContext>()
                .UseNpgsql(cs, x => x.MigrationsAssembly(typeof(MapperDbContext).Assembly.FullName))
                .Options;

            return new MapperDbContext(options);
        }
    }
}
