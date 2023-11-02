using Company.Services.Identity.Shared.Request;
using Company.Services.Identity.Shared.ViewModels;
using Company.WebApps.Identity.BlazorClient.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Company.WebApps.Identity.BlazorClient.Pages.Application;

/// <summary>
/// Component for displaying applications
/// </summary>
public partial class ApplicationList : ComponentBase
{
    [Inject]
    public IApplicationService Service { get; set; } = default!;

    [Inject]
    public IDialogService DialogService { get; set; } = default!;

    [Inject]
    public ISnackbar SnackBar { get; set; } = default!;

    [Inject]
    public NavigationManager Navigator { get; set; } = default!;

    private readonly int[] pageSizeOptions = { 10, 20, 30, 40, 50 };
    private MudTable<ApplicationViewModel>? applicationsTable;
    private GetApplicationsRequest applicationsRequest = new GetApplicationsRequest();
    private bool resetCurrentPage = false;

    /// <summary>
    /// Get roles from api endpoint for the current page of the data table
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    private async Task<TableData<ApplicationViewModel>> GetApplicationsDataAsync(TableState state)
    {
        try
        {
            applicationsRequest.CurrentPage = resetCurrentPage ? 1 : state.Page + 1;
            resetCurrentPage = false;
            applicationsRequest.PageSize = state.PageSize;
            var sessionPage = await Service.GetApplicationsAsync(applicationsRequest);

            return new TableData<ApplicationViewModel>
            {
                Items = sessionPage.Items,
                TotalItems = sessionPage.ItemsCount
            };
        }
        catch (Exception ex)
        {
            SnackBar.Add($"Error while retrieving applications.{ex.Message}", Severity.Error);
        }
        return new TableData<ApplicationViewModel>
        {
            Items = Enumerable.Empty<ApplicationViewModel>(),
            TotalItems = 0
        };
    }

    /// <summary>
    /// Refresh data for the search query
    /// </summary>
    /// <param name="text"></param>
    private void OnSearch(string text)
    {
        applicationsRequest.ApplicationFilter = string.Empty;
        if (!string.IsNullOrEmpty(text))
        {
            applicationsRequest.ApplicationFilter = text;
        }
        resetCurrentPage = true;
        applicationsTable?.ReloadServerData();
    }

    /// <summary>
    /// Navigate to add new application page
    /// </summary>
    void AddNewApplication()
    {
        Navigator.NavigateTo($"applications/new");
    }

    /// <summary>
    /// Navigate to edit application page
    /// </summary>
    /// <param name="application"></param>
    void EditApplication(ApplicationViewModel application)
    {
        Navigator.NavigateTo($"applications/edit/{application.ClientId}");
    }

    /// <summary>
    /// Delete the application
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    async Task DeleteApplicationAsync(ApplicationViewModel application)
    {
        bool? dialogResult = await DialogService.ShowMessageBox("Warning", "Delete can't be undone !!",
            yesText: "Delete!", cancelText: "Cancel", options: new DialogOptions() { FullWidth = true });
        if (dialogResult.GetValueOrDefault())
        {
            var result = await Service.DeleteApplicationDescriptorAsync(application);
            if (result.IsSuccess)
            {
                SnackBar.Add("Deleted successfully.", Severity.Success);
                await applicationsTable.ReloadServerData();
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
