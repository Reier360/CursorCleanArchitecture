using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace TestApi.Identity;

public static class Config
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
            new ApiScope("api", "Test API")
        };

    public static IEnumerable<Client> Clients =>
        new[]
        {
            new Client
            {
                ClientId = "test-client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes =
                {
                    "api",
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile
                }
            }
        };
}

