using Company.Services.Identity.Shared.ViewModels;
using Company.WebApps.Identity.BlazorClient.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace Company.WebApps.Identity.BlazorClient.Pages.Authenticator;

/// <summary>
/// Component for setting up the authenticator for 2FA
/// </summary>
public partial class EnableAuthenticator : ComponentBase, IAsyncDisposable
{

    [Inject]
    public IAuthenticatorService Service { get; set; } = default!;

    [Inject]
    public IJSRuntime JS { get; set; } = default!;

    [Inject]
    public NavigationManager Navigator { get; set; } = default!;

    [Inject]
    public ISnackbar SnackBar { get; set; } = default!;

    [Inject]
    public IDialogService DialogService { get; set; } = default!;

    EnableAuthenticatorViewModel model = new();
    IJSObjectReference? module;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var result = await Service.GetAuthenticatorSetupConfigurationAsync();
            model.SharedKey = result.SharedKey;
            model.AuthenticatorUri = result.AuthenticatorUri;
        }
        catch (Exception ex)
        {
            SnackBar.Add(ex.Message, Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
        }
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            module = await JS.InvokeAsync<IJSObjectReference>("import", "./Pages/Authenticator/EnableAuthenticator.razor.js");
        }
        await GenerateQRCodeAsync();

    }

    /// <summary>
    /// Enable the authenticator once user has completed the required setup steps
    /// </summary>
    /// <returns></returns>
    async Task EnableAuthenticatorAsync()
    {
        var result = await Service.EnableAuthenticatorAsync(model.Code);
        if (!result.IsSuccess)
        {
            SnackBar.Add(result.ToString(), Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
            return;
        }
        await DialogService.ShowMessageBox("Success", "2FA is enabled and your account is more secure now. ");
        Navigator.NavigateTo("account/authenticator/manage");
    }

    public async Task GenerateQRCodeAsync()
    {
        if (module is not null)
        {
            await module.InvokeVoidAsync("generateQrCode");
            return;
        }
        await Task.CompletedTask;
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (module is not null)
        {
            await module.DisposeAsync();
        }
    }
}
