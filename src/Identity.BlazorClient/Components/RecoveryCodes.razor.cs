using Company.WebApps.Identity.BlazorClient.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Company.WebApps.Identity.BlazorClient.Components;

public partial class RecoveryCodes : ComponentBase
{

    List<string> recoveryCodes = new();

    int recoveryCodesCount = 0;

    [Inject]
    public IAuthenticatorService Service { get; set; } = default!;

    [Inject]
    public ISnackbar SnackBar { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            recoveryCodesCount = await Service.GetRecoveryCodesCountAsync();

        }
        catch (Exception ex)
        {
            SnackBar.Add(ex.Message, Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
        }
    }


    /// <summary>
    /// Generate new recovery codes 
    /// </summary>
    /// <returns></returns>
    async Task GenerateRecoveryCodesAsync()
    {
        try
        {
            var codes = await Service.GenerateRecoveryCodesAsync();
            recoveryCodes.Clear();
            recoveryCodes.AddRange(codes);
            recoveryCodesCount = recoveryCodes.Count;
        }
        catch (Exception ex)
        {
            SnackBar.Add(ex.Message, Severity.Error, config =>
            {
                config.ShowCloseIcon = true;
                config.RequireInteraction = true;
            });
        }
    }
}
