using Company.Services.Identity.Shared.ViewModels;
using Company.WebApps.Identity.BlazorClient.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace Company.WebApps.Identity.BlazorClient.Pages.Roles;

/// <summary>
/// Add role view allows user to create a new Asp.Net Identity Role/>
/// </summary>
public partial class AddRole : ComponentBase
{
    [Inject]
    public IDialogService Dialog { get; set; } = default!;

    [Inject]
    public IUserRolesService UserRolesService { get; set; } = default!;

    [Inject]
    public ISnackbar SnackBar { get; set; } = default!;

    [Inject]
    public NavigationManager Navigator { get; set; } = default!;

    UserRoleViewModel model = new(string.Empty);

    /// <summary>
    /// Create a new role
    /// </summary>
    /// <returns></returns>
    async Task AddRoleAsync()
    {
        var result = await UserRolesService.CreateRoleAsync(model);
        if (result.IsSuccess)
        {
            SnackBar.Add("Added successfully.", Severity.Success);
            Navigator.NavigateTo($"roles/list");
            return;
        }
        SnackBar.Add(result.ToString(), Severity.Error);
    }
}
