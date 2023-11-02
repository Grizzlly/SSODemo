using Company.WebApps.Identity.BlazorClient.Services;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Company.WebApps.Identity.BlazorClient.Pages.Authenticator;

/// <summary>
/// Self managment page for user authenticator for 2FA
/// </summary>
public partial class Authenticator : ComponentBase
{
    [Inject]
    public IAuthenticatorService Service { get; set; } = default!;

    [Inject]
    public NavigationManager Navigator { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (!await Service.CheckIsAuthenticatorEnabledAsync())
        {
            Navigator.NavigateTo("account/authenticator/enable");
        }

        await base.OnInitializedAsync();
    }
}
