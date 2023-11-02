using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Collections.Immutable;

namespace Company.Services.Identity.API.Models;

/// <summary>
/// IdentityRoleClaim with <see cref="Guid"/> as the Identifier type
/// </summary>
public class IdentityRoleClaim : IdentityRoleClaim<Guid>
{
    /// <summary>
    /// Additional properties associated with this claim
    /// </summary>
    public virtual string? Properties { get; set; }

    /// <summary>
    /// constructor
    /// </summary>
    public IdentityRoleClaim()
    {
    }

    /// <inheritdoc />  
    public override Claim ToClaim()
    {
        var claim = base.ToClaim();
        foreach (var keyValue in from property in Properties?.Trim(']', '[').Split(',', StringSplitOptions.None).Select(r => r.Trim('\"'))
                                 let keyValue = property.Split(':')
                                 select keyValue)
        {
            claim.Properties.Add(keyValue[0], keyValue[1]);
        }

        return claim;
    }

    /// <inheritdoc />  
    public override void InitializeFromClaim(Claim? claim)
    {
        base.InitializeFromClaim(claim);
        StringBuilder sb = new();
        foreach (var (key, value) in claim?.Properties ?? ImmutableDictionary<string, string>.Empty)
        {

            sb.Append('"');
            sb.Append(key);
            sb.Append(':');
            sb.Append(value);
            sb.Append('"');
            sb.Append(',');
        }
        Properties = $"[{sb.ToString().Trim(',')}]";
    }
}