using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Mapper.Persistence;
using Mapper.WebApi.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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

            RunStartupTasks(host);

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

        private static void RunStartupTasks(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var sp = scope.ServiceProvider;
            var options = sp.GetRequiredService<IOptions<StartupTasksOptions>>().Value;

            if (!options.ApplyMigrations && !options.SeedDatabase && !options.EnsureS3Bucket)
            {
                Log.Information("Startup infrastructure tasks are disabled");
                return;
            }

            try
            {
                var db = sp.GetRequiredService<MapperDbContext>();

                if (options.ApplyMigrations)
                {
                    for (var i = 1; i <= options.MigrateAttempts; i++)
                    {
                        try
                        {
                            db.Database.Migrate();
                            break;
                        }
                        catch when (i < options.MigrateAttempts)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(2));
                        }
                    }
                }

                if (options.SeedDatabase)
                {
                    DbInitializer.Initialize(db);
                }

                if (options.EnsureS3Bucket)
                {
                    EnsureS3Bucket(sp);
                }

                Log.Information("Startup infrastructure tasks completed");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Startup infrastructure tasks failed");
                throw; // лучше падать, чем работать без схемы
            }
        }

        private static void EnsureS3Bucket(IServiceProvider sp)
        {
            var s3Options = sp.GetRequiredService<IOptions<S3Options>>().Value;
            var bucket = s3Options.Bucket;
            if (string.IsNullOrWhiteSpace(bucket))
            {
                Log.Warning("S3 bucket provisioning skipped because S3:Bucket is not configured");
                return;
            }

            var s3 = sp.GetRequiredService<IAmazonS3>();
            var exists = AmazonS3Util.DoesS3BucketExistV2Async(s3, bucket).GetAwaiter().GetResult();
            if (exists)
            {
                return;
            }

            s3.PutBucketAsync(new PutBucketRequest { BucketName = bucket }).GetAwaiter().GetResult();
            Log.Information("Created S3 bucket {Bucket}", bucket);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((ctx, lc) =>
                {
                    lc.ReadFrom.Configuration(ctx.Configuration)
                      .Enrich.FromLogContext();

                    var seqServerUrl = ctx.Configuration["Seq:ServerUrl"];
                    if (!string.IsNullOrWhiteSpace(seqServerUrl))
                    {
                        lc.WriteTo.Seq(seqServerUrl);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
