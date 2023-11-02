using Company.Services.Identity.Shared.ViewModels;
using Company.WebApps.Identity.BlazorClient.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace Company.WebApps.Identity.BlazorClient.Pages.Scopes;

/// <summary>
/// Edit scope view allows authorized user to edit an existing <see cref="OpenIddict.Abstractions.OpenIddictScopeDescriptor"/>
/// </summary>
public partial class EditScope : ComponentBase
{
    [Inject]
    public IDialogService Dialog { get; set; } = default!;

    [Inject]
    public IScopeService Service { get; set; } = default!;

    [Inject]
    public ISnackbar SnackBar { get; set; } = default!;

    [Parameter]
    public string Id { get; set; }

    ScopeViewModel scope;

    bool hasErrors = false;

    /// <summary>
    /// Retrieve the Scope details when Id parameter is set
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        scope = await GetScopeDetailsAsync(Id);
    }

    /// <summary>
    /// Update the details of scope 
    /// </summary>
    /// <returns></returns>
    async Task UpdateScopeAsync()
    {
        var result = await Service.UpdateScopeAsync(scope);
        if (result.IsSuccess)
        {
            SnackBar.Add("Updated successfully.", Severity.Success);
            scope = await GetScopeDetailsAsync(Id);
            return;
        }
        SnackBar.Add(result.ToString(), Severity.Error, config =>
        {
            config.ShowCloseIcon = true;
            config.RequireInteraction = true;
        });
    }

    /// <summary>
    /// Retrieve scope details given scope id
    /// </summary>
    /// <param name="scopeId"></param>
    /// <returns></returns>
    private async Task<ScopeViewModel> GetScopeDetailsAsync(string scopeId)
    {
        if (!string.IsNullOrEmpty(scopeId))
        {
            try
            {
                return await Service.GetByIdAsync(scopeId);
            }
            catch (Exception ex)
            {
                hasErrors = true;
                SnackBar.Add($"Failed to retrieve scope data. {ex.Message}", Severity.Error);
            }
        }
        else
        {
            hasErrors = true;
            SnackBar.Add("No scopeId specified.", Severity.Error);
        }
        return null;
    }
}
