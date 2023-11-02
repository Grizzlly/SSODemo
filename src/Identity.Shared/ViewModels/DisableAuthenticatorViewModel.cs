using System.ComponentModel.DataAnnotations;

namespace Company.Services.Identity.Shared.ViewModels;

public class DisableAuthenticatorViewModel
{
    [Required(ErrorMessage = "Code is required to disable 2FA")]
    public string? Code { get; set; }
}
