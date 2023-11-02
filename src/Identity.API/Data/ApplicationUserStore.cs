using Company.Services.Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Company.Services.Identity.API.Data;

public class ApplicationUserStore :
    UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid,
        IdentityUserClaim, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityUserToken<Guid>, IdentityRoleClaim>
{
    public ApplicationUserStore(ApplicationDbContext context, IdentityErrorDescriber? describer = null) : base(context, describer)
    {
    }
    public override Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        return base.GetClaimsAsync(user, cancellationToken);
    }
    protected override IdentityUserClaim CreateUserClaim(ApplicationUser user, Claim claim)
    {
        var userClaim = base.CreateUserClaim(user, claim);
        userClaim.InitializeFromClaim(claim);
        return userClaim;
    }
}
