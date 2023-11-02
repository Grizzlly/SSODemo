using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace Company.Services.Identity.API.Extensions;

public static class IdentityErrorExtensions
{
    public static IEnumerable<string> GetErrors(this IdentityResult identityResult)
    {
        return identityResult.Errors.Select(s => $"{s.Code}:{s.Description}").Distinct();
    }
}
