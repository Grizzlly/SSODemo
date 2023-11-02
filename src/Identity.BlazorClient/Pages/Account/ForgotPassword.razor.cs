using Company.Services.Identity.Shared.Models;
using Company.WebApps.Identity.BlazorClient.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace Company.WebApps.Identity.BlazorClient.Pages.Account;

public partial class ForgotPassword : ComponentBase
{
    [Inject]
    public ISnackbar SnackBar { get; set; }

    [Inject]
    public IAccountService AccountService { get; set; }

    ForgotPasswordModel model = new();

    /// <summary>
    /// Send password reset link
    /// </summary>
    /// <returns></returns>
    async Task SendPasswordResetLinkAsync()
    {
        var result = await AccountService.SendPasswordResetLinkAsync(model);
        if (result.IsSuccess)
        {
            SnackBar.Add("Please check your mail for password reset link.", Severity.Success);
            model.Email = string.Empty;
            return;
        }
        SnackBar.Add(result.ToString(), Severity.Error, config =>
        {
            config.ShowCloseIcon = true;
            config.RequireInteraction = true;
        });
    }
}
