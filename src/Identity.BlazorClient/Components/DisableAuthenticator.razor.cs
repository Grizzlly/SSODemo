using Company.Services.Identity.Shared.ViewModels;
using Company.WebApps.Identity.BlazorClient.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Company.WebApps.Identity.BlazorClient.Components;

/// <summary>
/// DisableAuthenticator component allows disabling the 2FA authentication for user account.
/// </summary>
public partial class DisableAuthenticator : ComponentBase
{
    DisableAuthenticatorViewModel model = new();

    [Inject]
    public IAuthenticatorService Service { get; set; } = default!;

    [Inject]
    public ISnackbar SnackBar { get; set; } = default!;

    [Inject]
    public IDialogService DialogService { get; set; } = default!;

    [Inject]
    public NavigationManager Navigator { get; set; } = default!;


    /// <summary>
    /// Disable 2FA for user account
    /// </summary>
    async void DisableAuthenticatorAsync()
    {
        var result = await Service.DisableAuthenticatorAsync(model.Code);
        if (!result.IsSuccess)
        {
            SnackBar.Add(result.ToString(), Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
            return;
        }

        await DialogService.ShowMessageBox("Success",
          "2FA is disabled now. You should enable 2FA for a better security of your account.");

        Navigator.NavigateTo("account/authenticator/enable");
    }
}
