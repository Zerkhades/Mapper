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

            services.AddDbContext<MapperDbContext>(opt =>
            {
                opt.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    x => x.MigrationsAssembly(typeof(MapperDbContext).Assembly.FullName));
            });
            services.AddScoped<IMapperDbContext>(provider =>
                (IMapperDbContext)provider.GetRequiredService<MapperDbContext>());
            return services;
        }
    }
}
