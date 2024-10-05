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
                // Add mssql or/and postgres
                // options.UseSqlite(connectionString);
                options.UseSqlServer(connectionString);
            });
            services.AddScoped<IMapperDbContext>(provider =>
                provider.GetService<MapperDbContext>());
            return services;
        }
    }
}
