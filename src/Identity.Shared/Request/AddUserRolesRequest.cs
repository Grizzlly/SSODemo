using Company.Services.Identity.Shared.ViewModels;
using System.Collections.Generic;

namespace Company.Services.Identity.Shared.Request;

public class AddUserRolesRequest
{
    public string? UserName { get; set; }

    public List<UserRoleViewModel> RolesToAdd { get; set; } = new();

    public AddUserRolesRequest()
    {

    }

    public AddUserRolesRequest(string userName, IEnumerable<UserRoleViewModel> rolesToAdd)
    {
        UserName = userName;
        RolesToAdd.AddRange(rolesToAdd);
    }

}
