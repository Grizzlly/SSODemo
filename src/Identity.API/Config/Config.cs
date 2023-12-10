using OpenIddict.Abstractions;

namespace Company.Services.Identity.API.Config
{
    public class Config
    {
        public static IEnumerable<string> GetScopes()
        {
            yield return OpenIddictConstants.Permissions.Scopes.Email;
            yield return OpenIddictConstants.Permissions.Scopes.Profile;
            yield return OpenIddictConstants.Permissions.Scopes.Roles;
            yield return "api";
            yield return "scopes";
        }
    }
}
