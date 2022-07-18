using Duende.IdentityServer.Models;

namespace AuthenticationService;

public static class IdentityResourcesProvider {

    public static IEnumerable<IdentityResource> GetAll =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

}
