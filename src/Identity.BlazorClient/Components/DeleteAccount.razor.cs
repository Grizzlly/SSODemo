using Company.Services.Identity.Shared.Models;
using Company.WebApps.Identity.BlazorClient.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MudBlazor;
using System.Threading.Tasks;

namespace Company.WebApps.Identity.BlazorClient.Components;

/// <summary>
/// Component to facilitate user account deletion
/// </summary>
public partial class DeleteAccount : ComponentBase
{
    [Inject]
    public IAccountService AccountService { get; set; } = default!;

    [Inject]
    public ISnackbar SnackBar { get; set; } = default!;

    [Inject]
    public SignOutSessionStateManager SignOutManager { get; set; } = default!;

    [Inject]
    public NavigationManager Navigator { get; set; } = default!;

    DeleteAccountModel model = new();

    /// <summary>
    /// Permantently delete user account
    /// </summary>
    /// <returns></returns>
    async Task DeleteAccountAsync()
    {
        var result = await AccountService.DeleteAccountAsync(model);
        if (result.IsSuccess)
        {
            await SignOutManager.SetSignOutState();
            Navigator.NavigateTo("authentication/register");
            return;
        }
        SnackBar.Add(result.ToString(), Severity.Error, config =>
        {
            config.ShowCloseIcon = true;
            config.RequireInteraction = true;
        });
    }
}
