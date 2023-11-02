using Company.Services.Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Company.Services.Identity.API.Data;

public class ApplicationRoleStore 
    : RoleStore<ApplicationRole, ApplicationDbContext, Guid, IdentityUserRole<Guid>, IdentityRoleClaim>
{
    private DbSet<IdentityRoleClaim> RoleClaims { get { return Context.Set<IdentityRoleClaim>(); } }

    public ApplicationRoleStore(ApplicationDbContext context, IdentityErrorDescriber? describer = null) : base(context, describer)
    {
    }

    public async override Task<IList<Claim>> GetClaimsAsync(ApplicationRole role, CancellationToken cancellationToken = default)
    {
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        return await RoleClaims.Where(rc => rc.RoleId.Equals(role.Id)).Select(c => c.ToClaim()).ToListAsync(cancellationToken);
    }

    protected override IdentityRoleClaim CreateRoleClaim(ApplicationRole role, Claim claim)
    {
        var roleClaim = base.CreateRoleClaim(role, claim);
        roleClaim.InitializeFromClaim(claim);
        return roleClaim;
    }
}
