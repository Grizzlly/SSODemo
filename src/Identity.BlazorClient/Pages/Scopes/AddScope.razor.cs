using Company.Services.Identity.Shared.ViewModels;
using Company.WebApps.Identity.BlazorClient.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace Company.WebApps.Identity.BlazorClient.Pages.Scopes;

/// <summary>
/// Add Scope view allows user to  create a new <see cref="OpenIddict.Abstractions.OpenIddictScopeDescriptor"/>
/// </summary>
public partial class AddScope : ComponentBase
{
    [Inject]
    public IDialogService Dialog { get; set; } = default!;

    [Inject]
    public IScopeService Service { get; set; } = default!;

    [Inject]
    public ISnackbar SnackBar { get; set; } = default!;

    [Inject]
    public NavigationManager Navigator { get; set; } = default!;


    ScopeViewModel scope = new();

    /// <summary>
    /// Make a post request to service endpoint to add a new scope 
    /// </summary>
    /// <returns></returns>
    async Task AddScopeAsync()
    {
        var result = await Service.AddScopeAsync(scope);
        if (result.IsSuccess)
        {
            SnackBar.Add("Added successfully.", Severity.Success);
            Navigator.NavigateTo($"scopes/list");
            return;
        }
        SnackBar.Add(result.ToString(), Severity.Error);
    }
}
