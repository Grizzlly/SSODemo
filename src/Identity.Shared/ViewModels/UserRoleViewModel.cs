using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Company.Services.Identity.Shared.ViewModels;

public class UserRoleViewModel
{
    [Required(AllowEmptyStrings = true)]
    public string? RoleId { get; set; }

    [Required]
    public string? RoleName { get; set; }

    [Required]
    public List<ClaimViewModel> Claims { get; set; } = new();

    public bool Exists => !string.IsNullOrEmpty(RoleId);

    public UserRoleViewModel()
    {

    }

    public UserRoleViewModel(string roleName)
    {
        RoleId = string.Empty;
        RoleName = roleName;
    }

    public UserRoleViewModel(string roleId, string roleName) : this(roleName)
    {
        RoleId = roleId;
    }
}
