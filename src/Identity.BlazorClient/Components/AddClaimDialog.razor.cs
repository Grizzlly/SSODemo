using Company.Services.Identity.Shared.ViewModels;
using Company.WebApps.Identity.BlazorClient.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.WebApps.Identity.BlazorClient.Components;

public partial class AddClaimDialog : ComponentBase
{
    string error = null;
    ClaimViewModel model = new();

    [CascadingParameter]
    MudDialogInstance MudDialog { get; set; }

    [Parameter]
    public IClaimsService Service { get; set; } = default!;

    [Parameter]
    public string Owner { get; set; }

    [Parameter]
    public IEnumerable<ClaimViewModel> ExistingClaims { get; set; }

    /// <summary>
    /// Close the dialog with model as dialog result
    /// </summary>
    async Task AddNewClaimAsync()
    {
        if (!ExistingClaims.Any(u => u.Type.Equals(model.Type) && u.Value.Equals(model.Value)))
        {
            //Don't try to add claim to role if role is not yet created.
            if (!string.IsNullOrEmpty(Owner))
            {
                var result = await Service.AddClaimAsync(Owner, model);
                if (result.IsSuccess)
                {
                    MudDialog.Close(DialogResult.Ok<ClaimViewModel>(model));
                    return;
                }
                error = result.ToString();
                return;
            }

        }
        error = $"Claim with type {model.Type} already exists for role.";
    }

    /// <summary>
    /// Close the dialog without any result
    /// </summary>
    void Cancel() => MudDialog.Cancel();
}
