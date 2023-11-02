using System.ComponentModel.DataAnnotations;

namespace Company.Services.Identity.Shared.Models;

public class ResendEmailConfirmationModel
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}
