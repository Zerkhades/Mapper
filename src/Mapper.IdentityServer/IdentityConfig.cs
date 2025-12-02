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
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { "api" }
            },
            // interactive client for swagger/ui testing
            new Client
            {
                ClientId = "mapper.swagger",
                ClientName = "Mapper Swagger UI",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,
                RedirectUris = { "https://localhost:7193/swagger/oauth2-redirect.html", "http://localhost:5161/swagger/oauth2-redirect.html" },
                AllowedCorsOrigins = { "https://localhost:7193", "http://localhost:5161" },
                PostLogoutRedirectUris = { "https://localhost:7193/swagger", "http://localhost:5161/swagger" },
                AllowedScopes = { "openid", "profile", "api" },
                AllowAccessTokensViaBrowser = true
            }
        };
}
