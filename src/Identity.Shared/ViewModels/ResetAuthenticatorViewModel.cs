using System.ComponentModel.DataAnnotations;

namespace Company.Services.Identity.Shared.ViewModels;

public class ResetAuthenticatorViewModel
{
    [Required(ErrorMessage = "Code is required to reset Authenticator")]
    public string? Code { get; set; }
}
