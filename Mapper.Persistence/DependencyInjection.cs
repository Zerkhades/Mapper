using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Mapper.Application.Interfaces;

namespace Mapper.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection
            services, IConfiguration configuration)
        {
            var connectionString = configuration["DbConnection"];
            services.AddDbContext<MapperDbContext>(options =>
            {
                // Sqlite server
                // options.UseSqlite(connectionString);

                // MsSql server
                options.UseSqlServer(connectionString);

                // Inmemory database purposes is only for testing
                //options.UseInMemoryDatabase(connectionString);
            });
            services.AddScoped<IMapperDbContext>(provider =>
                provider.GetService<MapperDbContext>());
            return services;
        }
    }
}
