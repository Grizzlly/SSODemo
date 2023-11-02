using Company.Services.Identity.Shared.Models;
using Company.WebApps.Identity.BlazorClient.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace Company.WebApps.Identity.BlazorClient.Pages.Account;

/// <summary>
/// Component to resend email verification link
/// </summary>
public partial class ResendEmailConfirmation : ComponentBase
{
    [Inject]
    public ISnackbar SnackBar { get; set; }

    [Inject]
    public IAccountService AccountService { get; set; }

    ResendEmailConfirmationModel model = new ();

    /// <summary>
    /// Send password reset link
    /// </summary>
    /// <returns></returns>
    async Task ResendEmailConfirmationLinkAsync()
    {
        var result = await AccountService.ResendEmailConfirmationAsync(model);
        if (result.IsSuccess)
        {
            SnackBar.Add("Please check your mail for email confirmation link.", Severity.Success);
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
