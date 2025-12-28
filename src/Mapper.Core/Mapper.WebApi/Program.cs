using Mapper.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;

namespace Mapper.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .WriteTo.File("MapperWebAppLog-.txt", rollingInterval:
                    RollingInterval.Day)
                .CreateLogger();
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var sp = scope.ServiceProvider;
                try
                {
                    var configuration = sp.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
                    var db = sp.GetRequiredService<MapperDbContext>();

                    var attempts = configuration.GetValue<int?>("Database:MigrateAttempts") ?? 10;
                    for (var i = 1; i <= attempts; i++)
                    {
                        try
                        {
                            db.Database.Migrate();
                            break;
                        }
                        catch when (i < attempts)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(2));
                        }
                    }

                    var seedOnStart = configuration.GetValue<bool>("Database:SeedOnStart");
                    if (seedOnStart)
                    {
                        DbInitializer.Initialize(db);
                    }

                    Log.Information("Database migration/seed completed");
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Database migration/seed failed");
                    throw; // лучше падать, чем работать без схемы
                }
            }
            var lifetime = host.Services.GetRequiredService<Microsoft.Extensions.Hosting.IHostApplicationLifetime>();
            lifetime.ApplicationStarted.Register(() => Log.Information("Host started"));
            lifetime.ApplicationStopping.Register(() => Log.Warning("Host stopping"));
            lifetime.ApplicationStopped.Register(() => Log.Warning("Host stopped"));

            try
            {
                Log.Information("Starting host");
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((ctx, lc) =>
                {
                    lc.ReadFrom.Configuration(ctx.Configuration)
                      .Enrich.FromLogContext()
                      .WriteTo.Seq(ctx.Configuration["Seq:ServerUrl"]!);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
