using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
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
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "Mapper")
                .WriteTo.Console()
                .WriteTo.File(
                    path: "logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
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

                    var bucket = configuration["S3:Bucket"];
                    if (!string.IsNullOrWhiteSpace(bucket))
                    {
                        var s3 = sp.GetRequiredService<IAmazonS3>();
                        var exists = AmazonS3Util.DoesS3BucketExistV2Async(s3, bucket).GetAwaiter().GetResult();
                        if (!exists)
                        {
                            s3.PutBucketAsync(new PutBucketRequest { BucketName = bucket }).GetAwaiter().GetResult();
                            Log.Information("Created S3 bucket {Bucket}", bucket);
                        }
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
