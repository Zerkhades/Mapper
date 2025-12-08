using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Mapper.IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Read Postgres connection string
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                                   ?? builder.Configuration["ConnectionStrings__DefaultConnection"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured. Set it in appsettings.json or via ConnectionStrings__DefaultConnection environment variable.");
            }

            var migrationsAssembly = typeof(Program).Assembly.GetName().Name;

            // Configure IdentityServer with EF stores backed by PostgreSQL
            builder.Services
                .AddIdentityServer(opt =>
                {
                    opt.Authentication.CookieSameSiteMode = SameSiteMode.Lax;
                    opt.Authentication.CheckSessionCookieSameSiteMode = SameSiteMode.Lax;

                    opt.KeyManagement.RotationInterval = TimeSpan.FromDays(30);
                    opt.KeyManagement.PropagationTime = TimeSpan.FromDays(2);
                    opt.KeyManagement.RetentionDuration = TimeSpan.FromDays(7);
                    opt.KeyManagement.DeleteRetiredKeys = false;
                })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(connectionString, sql =>
                        sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(connectionString, sql =>
                        sql.MigrationsAssembly(migrationsAssembly));

                    // enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                })
                .AddInMemoryIdentityResources(IdentityConfig.IdentityResources)
                .AddInMemoryApiScopes(IdentityConfig.ApiScopes)
                //.AddInMemoryApiResources(IdentityConfig.ApiResources)
                .AddInMemoryClients(IdentityConfig.Clients)
                .AddDeveloperSigningCredential();

            var app = builder.Build();

            // Apply EF migrations / create schema and seed configuration at startup (dev/demo)
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                void MigrateOrCreate<TContext>() where TContext : DbContext
                {
                    var ctx = services.GetRequiredService<TContext>();
                    // If there are no migrations, create schema; otherwise apply migrations
                    var hasMigrations = ctx.Database.GetMigrations().Any();
                    if (hasMigrations)
                        ctx.Database.Migrate();
                    else
                        ctx.Database.EnsureCreated();
                }

                MigrateOrCreate<ConfigurationDbContext>();
                MigrateOrCreate<PersistedGrantDbContext>();

                // Seed data if DB is empty
                var configDb = services.GetRequiredService<ConfigurationDbContext>();

                if (!configDb.Clients.Any())
                {
                    foreach (var client in IdentityConfig.Clients)
                    {
                        configDb.Clients.Add(client.ToEntity());
                    }
                    configDb.SaveChanges();
                }

                if (!configDb.IdentityResources.Any())
                {
                    foreach (var resource in IdentityConfig.IdentityResources)
                    {
                        configDb.IdentityResources.Add(resource.ToEntity());
                    }
                    configDb.SaveChanges();
                }

                if (!configDb.ApiScopes.Any())
                {
                    foreach (var scopeEntity in IdentityConfig.ApiScopes)
                    {
                        configDb.ApiScopes.Add(scopeEntity.ToEntity());
                    }
                    configDb.SaveChanges();
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // IdentityServer middleware
            app.UseIdentityServer();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
