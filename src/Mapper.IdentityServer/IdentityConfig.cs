using Duende.IdentityServer.Models;
using Duende.IdentityServer;

namespace Mapper.IdentityServer;

public static class IdentityConfig
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new[]
        {
            new ApiScope("api", "Mapper API")
        };

    public static IEnumerable<Client> Clients =>
        new[]
        {
            // client credentials flow for service-to-service calls
            new Client
            {
                ClientId = "mapper.client",
                // enable interactive auth code flow for browser-based clients hitting /connect/authorize
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,
                RedirectUris =
                {
                    // local webapi swagger (kestrel/iis express)
                    "http://localhost:5001/swagger/oauth2-redirect.html",
                    "http://localhost:5001/oauth2-redirect.html",
                    // local dev https variant (when enabled)
                    "https://localhost:7193/swagger/oauth2-redirect.html",
                    // docker compose mapped ports
                    "http://localhost:8080/swagger/oauth2-redirect.html",
                    "https://localhost:8081/swagger/oauth2-redirect.html",
                    // when Swagger UI RoutePrefix is empty (UI at '/')
                    "http://localhost:8080/oauth2-redirect.html",
                    "https://localhost:8081/oauth2-redirect.html"
                },
                AllowedCorsOrigins =
                {
                    "http://localhost:5001",
                    "http://localhost:5002",
                    "http://localhost:8080",
                    "https://localhost:8081"
                },
                PostLogoutRedirectUris =
                {
                    "http://localhost:5001/swagger",
                    "http://localhost:5001/",

                    "https://localhost:7193/swagger",
                    // when Swagger UI RoutePrefix is empty, UI is at '/'
                    "http://localhost:8080/",
                    "https://localhost:8081/",
                    // fallback to '/swagger' when RoutePrefix is 'swagger'
                    "http://localhost:8080/swagger",
                    "https://localhost:8081/swagger"
                },
                AllowedScopes = { "openid", "profile", "api" },
                AllowAccessTokensViaBrowser = true
            },
            // interactive client for swagger/ui testing
            new Client
            {
                ClientId = "mapper.swagger",
                ClientName = "Mapper Swagger UI",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,
                RedirectUris =
                {
                    "http://localhost:5001/swagger/oauth2-redirect.html",
                    "http://localhost:5001/oauth2-redirect.html",
                    // local dev (IIS Express/Kestrel)
                    "https://localhost:7193/swagger/oauth2-redirect.html",
                    // docker compose mapped ports
                    "http://localhost:8080/swagger/oauth2-redirect.html",
                    "https://localhost:8081/swagger/oauth2-redirect.html",
                    // when Swagger UI RoutePrefix is empty (UI at '/')
                    "http://localhost:8080/oauth2-redirect.html",
                    "https://localhost:8081/oauth2-redirect.html"
                },
                AllowedCorsOrigins =
                {
                    "http://localhost:5001",
                    "http://localhost:5002",
                    "http://localhost:8080",
                    "https://localhost:8081"
                },
                PostLogoutRedirectUris =
                {
                    "http://localhost:5001/swagger",
                    "http://localhost:5001/",

                    "https://localhost:7193/swagger",
                    // when Swagger UI RoutePrefix is empty, UI is at '/'
                    "http://localhost:8080/",
                    "https://localhost:8081/",
                    // fallback to '/swagger' when RoutePrefix is 'swagger'
                    "http://localhost:8080/swagger",
                    "https://localhost:8081/swagger"
                },
                AllowedScopes = { "openid", "profile", "api" },
                AllowAccessTokensViaBrowser = true
            }
        };
}
