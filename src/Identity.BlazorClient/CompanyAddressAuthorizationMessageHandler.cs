using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Company.WebApps.Identity.BlazorClient;

/// <summary>
/// A <see cref="DelegatingHandler"/> that attaches access tokens to outgoing <see cref="HttpResponseMessage"/> instances.
/// Access tokens will only be added when the request URI is within the comapany's service URI.
/// </summary>
public class CompanyAddressAuthorizationMessageHandler : AuthorizationMessageHandler
{
    /// <inheritdoc/>
    public CompanyAddressAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigation) : base(provider, navigation)
    {
        ConfigureHandler(new[] { "https://grizzllycompany.xyz/" });
    }
}
