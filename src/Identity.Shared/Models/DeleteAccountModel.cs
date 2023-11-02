using System.ComponentModel.DataAnnotations;

namespace Company.Services.Identity.Shared.Models;

public  class DeleteAccountModel
{
    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}
