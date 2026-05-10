using Amazon.S3;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Hangfire;
using Hangfire.PostgreSql;
using Mapper.Application;
using Mapper.Application.Common.Mappings;
using Mapper.Application.Interfaces;
using Mapper.Infrastructure.BackgroundJobs;
using Mapper.Infrastructure.Caching;
using Mapper.Infrastructure.Cameras;
using Mapper.Infrastructure.Realtime;
using Mapper.Infrastructure.Storage.S3;
using Mapper.Persistence;
using Mapper.WebApi.Extensions;
using Mapper.WebApi.HealthChecks;
using Mapper.WebApi.Middleware;
using Mapper.WebApi.Options;
using Mapper.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Mapper.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication();
            services.AddPersistence(Configuration);
            services.AddControllers();
            services.AddProblemDetails();

            services.AddOptions<CorsPolicyOptions>()
                .Bind(Configuration.GetSection(CorsPolicyOptions.SectionName))
                .Validate(options =>
                    Environment.IsDevelopment()
                    || options.AllowedOrigins.Any(origin => !string.IsNullOrWhiteSpace(origin)),
                    "Cors:AllowedOrigins must contain at least one origin outside Development.")
                .ValidateOnStart();

            services.AddOptions<StartupTasksOptions>()
                .Bind(Configuration.GetSection(StartupTasksOptions.SectionName))
                .Validate(options => options.MigrateAttempts > 0, "StartupTasks:MigrateAttempts must be greater than zero.")
                .ValidateOnStart();

            services.Configure<OtelOptions>(Configuration.GetSection(OtelOptions.SectionName));
            services.Configure<S3Options>(Configuration.GetSection(S3Options.SectionName));

            var ffmpegPath = Configuration["Camera:FfmpegPath"];
            if (!string.IsNullOrWhiteSpace(ffmpegPath))
                System.Environment.SetEnvironmentVariable("FFMPEG_PATH", ffmpegPath);

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    var cors = Configuration.GetSection(CorsPolicyOptions.SectionName).Get<CorsPolicyOptions>() ?? new CorsPolicyOptions();

                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();

                    if (Environment.IsDevelopment() && cors.AllowAnyOriginInDevelopment)
                    {
                        policy.AllowAnyOrigin();
                    }
                    else
                    {
                        policy.WithOrigins([.. cors.AllowedOrigins.Where(origin => !string.IsNullOrWhiteSpace(origin))]);
                    }
                });
            });

            // JWT Authentication against Keycloak.
            var authority = Configuration["Jwt:Authority"] ?? "http://localhost:5002/auth/realms/mapper";
            var metadataAddress = Configuration["Jwt:MetadataAddress"];
            var audience = Configuration["Jwt:Audience"] ?? "api";

            services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;
                options.RequireHttpsMetadata = false;
                if (!string.IsNullOrWhiteSpace(metadataAddress))
                {
                    options.MetadataAddress = metadataAddress;
                }
                options.TokenValidationParameters.ValidateIssuer = true;
                options.TokenValidationParameters.ValidateAudience = true;
                options.TokenValidationParameters.NameClaimType = "preferred_username";
                options.TokenValidationParameters.RoleClaimType = "roles";

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/map"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization();

            services.AddHealthChecks()
                .AddCheck<MapperDbContextHealthCheck>("postgres", tags: ["ready"])
                .AddCheck<RedisHealthCheck>("redis", tags: ["ready"])
                .AddCheck<S3HealthCheck>("s3", tags: ["ready"]);

            services.AddApiVersioning()
                .AddMvc()
                .AddApiExplorer(opt =>
                {
                    opt.GroupNameFormat = "'v'VVV";
                    opt.SubstituteApiVersionInUrl = true;
                });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen();

            services.AddOpenTelemetry()
                    .WithTracing(t =>
                    {
                        t.AddAspNetCoreInstrumentation()
                         .AddHttpClientInstrumentation()
                         .AddEntityFrameworkCoreInstrumentation();

                        var otelEndpoint = Configuration["Otel:Endpoint"];
                        if (Uri.TryCreate(otelEndpoint, UriKind.Absolute, out var endpoint))
                        {
                            t.AddOtlpExporter(o => o.Endpoint = endpoint);
                        }
                    })
                    .WithMetrics(m =>
                    {
                        m.AddAspNetCoreInstrumentation()
                         .AddHttpClientInstrumentation()
                         .AddRuntimeInstrumentation()
                         .AddMeter(BackgroundJobMetrics.MeterName)
                         .AddPrometheusExporter();
                    });
            services.AddSignalR();
            services.AddSingleton<IConnectionMultiplexer>(
                _ => ConnectionMultiplexer.Connect(Configuration["Redis:ConnectionString"]!));
            services.AddSingleton<ICacheService, RedisCacheService>();
            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<IAmazonS3>(_ =>
            {
                var cfg = Configuration;
                var s3cfg = new AmazonS3Config { ServiceURL = cfg["S3:ServiceUrl"], ForcePathStyle = true };
                return new AmazonS3Client(cfg["S3:AccessKey"], cfg["S3:SecretKey"], s3cfg);
            });
            services.AddSingleton<IMapImageStorage, S3MapImageStorage>();
            services.AddSingleton<IS3ObjectStorage, S3ObjectStorage>();
            services.AddSingleton<IMapRealtimeNotifier, MapRealtimeNotifier>();

            services.AddSingleton<ICameraAdapter, SimpleCameraAdapter>();

            var hangfireConnection = Configuration.GetConnectionString("DefaultConnection")
                ?? Configuration["ConnectionStrings__DefaultConnection"];
            if (string.IsNullOrWhiteSpace(hangfireConnection))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
            }

            services.AddHangfire(c =>
            {
                c.UsePostgreSqlStorage(
                    options => options.UseNpgsqlConnection(hangfireConnection),
                    new PostgreSqlStorageOptions { PrepareSchemaIfNecessary = true });
            });
            services.AddHangfireServer();
            services.AddOptions<ArchiveRetentionCleanupJobOptions>()
                .Bind(Configuration.GetSection(ArchiveRetentionCleanupJobOptions.SectionName))
                .Validate(options => !string.IsNullOrWhiteSpace(options.Cron), "Retention:ArchiveCleanup:Cron must be configured.")
                .Validate(options => options.MotionVideoRetentionDays > 0, "Retention:ArchiveCleanup:MotionVideoRetentionDays must be greater than zero.")
                .Validate(options => options.NoMotionVideoRetentionDays > 0, "Retention:ArchiveCleanup:NoMotionVideoRetentionDays must be greater than zero.")
                .Validate(options => options.ArchivedVideoRetentionDays > 0, "Retention:ArchiveCleanup:ArchivedVideoRetentionDays must be greater than zero.")
                .Validate(options => options.Take > 0, "Retention:ArchiveCleanup:Take must be greater than zero.")
                .ValidateOnStart();
            services.AddTransient<PollCameraStatusAndLogHistoryJob>();
            services.AddTransient<DetectCameraMotionJob>();
            services.AddTransient<RecordCameraVideoJob>();
            services.AddTransient<FetchCameraSnapshotsJob>();
            services.AddTransient<CleanupArchiveRetentionJob>();
            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSerilogRequestLogging();
            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    config.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    config.RoutePrefix = string.Empty;
                }
                config.OAuthClientId(Configuration["SwaggerOAuth:ClientId"] ?? "mapper.swagger");
                config.OAuthUsePkce();
                config.OAuthScopes("api", "openid", "profile");
            });
            app.UseCustomExceptionHandler();
            app.UseRouting();
            //app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = [new HangfireDashboardAuthorizationFilter()]
            });
            app.ScheduleCameraJobs(Configuration);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MapHub>("/hubs/map");
                endpoints.MapPrometheusScrapingEndpoint("/metrics");
                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    Predicate = _ => false,
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
                    }
                });
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("ready"),
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
                    }
                });
            });
        }
    }
}
