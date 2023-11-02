using Company.Services.Identity.Shared.ViewModels;
using System.Collections.Generic;

namespace Company.Services.Identity.Shared.Request;

public class RemoveUserRolesRequest
{
    public string? UserName { get; set; }

    public List<UserRoleViewModel> RolesToRemove { get; set; } = new();

    public RemoveUserRolesRequest()
    {

    }
    public RemoveUserRolesRequest(string userName, IEnumerable<UserRoleViewModel> rolesToRemove)
    {
        UserName = userName;
        RolesToRemove.AddRange(rolesToRemove);
    }

}
