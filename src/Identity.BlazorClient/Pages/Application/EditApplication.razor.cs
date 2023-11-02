using Company.Services.Identity.Shared.ViewModels;
using Company.WebApps.Identity.BlazorClient.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace Company.WebApps.Identity.BlazorClient.Pages.Application;

/// <summary>
/// Component to edit application details
/// </summary>
public partial class EditApplication : ComponentBase
{
    [Inject]
    public IDialogService Dialog { get; set; } = default!;

    [Inject]
    public ISnackbar SnackBar { get; set; } = default!;

    [Inject]
    public IApplicationService Service { get; set; } = default!;

    [Inject]
    public NavigationManager Navigator { get; set; } = default!;

    [Parameter]
    public string? ClientId { get; set; }

    ApplicationViewModel? application;

    bool hasErrors = false;

    /// <summary>
    /// Fetch application details when clientId is set
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        application = await GetApplicationDetailsAsync(ClientId);
    }

    /// <summary>
    /// Update application details
    /// </summary>
    /// <returns></returns>
    async Task UpdateApplicationDetailsAsync()
    {
        var result = await Service.UpdateApplicationDescriptorAsync(application);
        if (result.IsSuccess)
        {
            SnackBar.Add("Updated successfully.", Severity.Success);
            application = await GetApplicationDetailsAsync(ClientId);
            return;
        }
        SnackBar.Add(result.ToString(), Severity.Error);
    }

    /// <summary>
    /// Retrieve application details given it's client id
    /// </summary>
    /// <param name="applicationClientId"></param>
    /// <returns></returns>
    private async Task<ApplicationViewModel?> GetApplicationDetailsAsync(string? applicationClientId)
    {
        if (!string.IsNullOrEmpty(applicationClientId))
        {
            try
            {
                return await Service.GetByClientIdAsync(applicationClientId);
            }
            catch (Exception ex)
            {
                hasErrors = true;
                SnackBar.Add($"Failed to retrieve application data. {ex.Message}", Severity.Error);
            }
        }
        else
        {
            hasErrors = true;
            SnackBar.Add("No clientId specified.", Severity.Error);
        }
        return null;
    }
}
