using Company.Services.Identity.Shared.Request;
using Company.Services.Identity.Shared.ViewModels;
using Company.WebApps.Identity.BlazorClient.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Linq;
using System.Threading.Tasks;

namespace Company.WebApps.Identity.BlazorClient.Pages.Roles;

/// <summary>
/// Component for displaying roles
/// </summary>
public partial class RoleList : ComponentBase
{
    [Inject]
    public IUserRolesService Service { get; set; } = default!;

    [Inject]
    public IDialogService DialogService { get; set; } = default!;

    [Inject]
    public ISnackbar SnackBar { get; set; } = default!;

    [Inject]
    public NavigationManager Navigator { get; set; } = default!;

    private readonly int[] pageSizeOptions = { 10, 20, 30, 40, 50 };
    private MudTable<UserRoleViewModel> rolesTable;
    private GetRolesRequest rolesRequest = new GetRolesRequest();
    private bool resetCurrentPage = false;

    /// <summary>
    /// Get roles from api endpoint for the current page of the data table
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    private async Task<TableData<UserRoleViewModel>> GetRolesDataAsync(TableState state)
    {
        try
        {
            rolesRequest.CurrentPage = resetCurrentPage ? 1 : state.Page + 1;
            resetCurrentPage = false;
            rolesRequest.PageSize = state.PageSize;
            var sessionPage = await Service.GetRolesAsync(rolesRequest);

            return new TableData<UserRoleViewModel>
            {
                Items = sessionPage.Items,
                TotalItems = sessionPage.ItemsCount
            };
        }
        catch (Exception ex)
        {
            SnackBar.Add($"Error while retrieving roles.{ex.Message}", Severity.Error);
        }
        return new TableData<UserRoleViewModel>
        {
            Items = Enumerable.Empty<UserRoleViewModel>(),
            TotalItems = 0
        };
    }

    /// <summary>
    /// Refresh data for the search query
    /// </summary>
    /// <param name="text"></param>
    private void OnSearch(string text)
    {
        rolesRequest.RoleFilter = string.Empty;
        if (!string.IsNullOrEmpty(text))
        {
            rolesRequest.RoleFilter = text;
        }
        resetCurrentPage = true;
        rolesTable.ReloadServerData();
    }

    /// <summary>
    /// Navigate to the add role page
    /// </summary>
    protected void NavigateToAddRolePage()
    {
        Navigator.NavigateTo("roles/new");
    }

    /// <summary>
    /// Navigate to the edit role page
    /// </summary>
    /// <param name="role"></param>
    protected void NavigateToEditRolePage(UserRoleViewModel role)
    {
        Navigator.NavigateTo($"roles/edit/{role.RoleId}");
    }


    /// <summary>
    /// Permanently delete user from system. 
    /// </summary>
    /// <param name="userDetails"></param>
    /// <returns></returns>
    async Task DeleteRoleAsync(UserRoleViewModel userRole)
    {
        bool? dialogResult = await DialogService.ShowMessageBox("Warning", "Delete can't be undone !!",
            yesText: "Delete!", cancelText: "Cancel", options: new DialogOptions() { FullWidth = true });
        if (dialogResult.GetValueOrDefault())
        {
            var result = await Service.DeleteRoleAsync(userRole.RoleName);
            if (result.IsSuccess)
            {
                SnackBar.Add("Deleted successfully.", Severity.Success);
                await rolesTable.ReloadServerData();
                return;
            }
            SnackBar.Add(result.ToString(), Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
        }
    }
}
