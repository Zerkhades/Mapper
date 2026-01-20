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
using Mapper.WebApi.Middleware;
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
using Microsoft.OpenApi.Models;
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

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication();
            services.AddPersistence(Configuration);
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowAnyOrigin();
                });
            });

            // JWT Authentication against IdentityServer
            var authority = Configuration["Jwt:Authority"] ?? "http://identityserver:5002";
            var audience = Configuration["Jwt:Audience"] ?? "api"; // scope-based

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
                options.TokenValidationParameters.ValidateIssuer = true;
                options.TokenValidationParameters.ValidateAudience = false;

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

            services.AddHealthChecks();

            services.AddApiVersioning()
                .AddMvc()
                .AddApiExplorer(opt => 
                {
                    opt.GroupNameFormat = "'v'VVV";
                    opt.SubstituteApiVersionInUrl = true;
                });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen(c =>
            {
                var swaggerAuthAuthority = Configuration["SwaggerOAuth:Authority"] ?? "http://localhost:5002"; // host URL for browser
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{swaggerAuthAuthority}/connect/authorize"),
                            TokenUrl = new Uri($"{swaggerAuthAuthority}/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "OpenID" },
                                { "profile", "User profile" },
                                { "api", "Mapper API" }
                            }
                        }
                    }
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                        },
                        new [] { "api", "openid", "profile" }
                    }
                });
            });

            services.AddOpenTelemetry()
                    .WithTracing(t =>
                    {
                        t.AddAspNetCoreInstrumentation()
                         .AddHttpClientInstrumentation()
                         .AddEntityFrameworkCoreInstrumentation()
                         .AddOtlpExporter(o => o.Endpoint = new Uri(Configuration["Otel:Endpoint"]!));
                     })
                    .WithMetrics(m =>
                    {
                        m.AddAspNetCoreInstrumentation()
                         .AddHttpClientInstrumentation()
                         .AddRuntimeInstrumentation()
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

            // ВРЕМЕННО: фейковая реализация адаптера камеры
            services.AddSingleton<ICameraAdapter, FakeCameraAdapter>();
            //services.AddSingleton<ICameraAdapter, SimpleCameraAdapter>();

            var hangfireConnection = Configuration.GetConnectionString("DefaultConnection")
                ?? Configuration["ConnectionStrings__DefaultConnection"];

            services.AddHangfire(c =>
            {
                c.UsePostgreSqlStorage(hangfireConnection,
                    new PostgreSqlStorageOptions { PrepareSchemaIfNecessary = true });
            });
            services.AddHangfireServer();
            services.AddTransient<PollCameraStatusesJob>();
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
                config.OAuthClientId("mapper.swagger");
                config.OAuthUsePkce();
                config.OAuthScopes("api", "openid", "profile");
            });
            app.UseCustomExceptionHandler();
            app.UseRouting();
            //app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard("/hangfire");
            try
            {
                RecurringJob.AddOrUpdate<PollCameraStatusesJob>(
                    "poll-cameras",
                    j => j.Execute(default),
                    "*/1 * * * *");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to schedule recurring job 'poll-cameras'. Check database connection settings.");
            }

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
                    Predicate = _ => true,
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
