using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Mapper.WebApi
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private const string SecuritySchemeName = "Bearer";

        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) =>
            _provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                var apiVersion = description.ApiVersion.ToString();
                options.SwaggerDoc(description.GroupName,
                    new OpenApiInfo
                    {
                        Version = apiVersion,
                        Title = $"Mapper API {apiVersion}",
                        Description =
                            "A simple example ASP NET Core Web API. Professional way",
                        TermsOfService =
                            new Uri("https://google.com"),
                        Contact = new OpenApiContact
                        {
                            Name = "Mapper",
                            Email = string.Empty,
                            Url =
                                new Uri("https://google.com")
                        },
                        License = new OpenApiLicense
                        {
                            Name = "Mapper",
                            Url =
                                new Uri("https://google.com")
                        }
                    });

                options.CustomOperationIds(apiDescription =>
                    apiDescription.TryGetMethodInfo(out MethodInfo methodInfo)
                        ? methodInfo.Name
                        : null);
            }

            options.AddSecurityDefinition(SecuritySchemeName,
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    Name = "Authorization",
                    Description = "Authorization token"
                });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference(SecuritySchemeName, document, null),
                    new List<string>()
                }
            });
        }
    }

}
