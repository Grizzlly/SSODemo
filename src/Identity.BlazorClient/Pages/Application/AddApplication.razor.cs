using Company.Services.Identity.Shared.ViewModels;
using Company.WebApps.Identity.BlazorClient.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace Company.WebApps.Identity.BlazorClient.Pages.Application;

/// <summary>
/// Component to add new application details
/// </summary>
public partial class AddApplication : ComponentBase
{
    [Inject]
    public IDialogService Dialog { get; set; } = default!;

    [Inject]
    public IApplicationService Service { get; set; } = default!;

    [Inject]
    public ISnackbar SnackBar { get; set; } = default!;

    [Inject]
    public IDialogService DialogService { get; set; } = default!;

    [Inject]
    public NavigationManager Navigator { get; set; } = default!;

    ApplicationViewModel application = new();

    Func<ApplicationPreset, string> displayStringConverter = ci => ci.ToDisplayString();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        application.ApplyPreset(ApplicationPreset.AuthorizationCodeFlow);
    }

    void ApplyPreset(ApplicationPreset preset)
    {
        application.ApplyPreset(preset);
    }

    /// <summary>
    /// Add new application details
    /// </summary>
    /// <returns></returns>
    async Task AddApplicationDetailsAsync()
    {
        if (application.IsConfidentialClient)
        {
            await DialogService.ShowMessageBox("Information", "Store client secret safely as it can't be viewed later.",
             "Ok", options: new DialogOptions() { FullWidth = true });
        }
        var result = await Service.AddApplicationDescriptorAsync(application);
        if (result.IsSuccess)
        {
            Navigator.NavigateTo($"applications/list");
            SnackBar.Add("Added successfully.", Severity.Success);
            return;
        }
        SnackBar.Add(result.ToString(), Severity.Error);
    }
}
