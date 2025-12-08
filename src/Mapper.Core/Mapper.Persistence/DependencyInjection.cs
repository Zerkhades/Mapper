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
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? configuration["ConnectionStrings__DefaultConnection"]
                ?? configuration["DbConnection"];

            services.AddDbContext<MapperDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsql =>
                {
                    npgsql.EnableRetryOnFailure(5);
                });
            });
            services.AddScoped<IMapperDbContext>(provider =>
                provider.GetRequiredService<MapperDbContext>());
            return services;
        }
    }
}
