using System.ComponentModel.DataAnnotations;

namespace Company.Services.Identity.Shared.Models;

public class ChangeEmailModel
{
    [Required]
    [EmailAddress]
    public string? NewEmail { get; set; }
}
