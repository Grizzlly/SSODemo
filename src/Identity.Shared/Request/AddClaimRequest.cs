using Company.Services.Identity.Shared.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Company.Services.Identity.Shared.Request;

[DataContract]
public class AddClaimRequest
{
    /// <summary>
    /// User or Role to which claim should be added
    /// </summary>
    [Required]
    [DataMember(IsRequired = true)]
    public string? Owner { get; set; }

    /// <summary>
    /// Claim to add to the role
    /// </summary>
    [Required]
    [DataMember(IsRequired = true)]
    public ClaimViewModel? ClaimToAdd { get; set; }

    /// <summary>
    /// constructor
    /// </summary>
    public AddClaimRequest()
    {

    }

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="claimToAdd"></param>
    public AddClaimRequest(string owner, ClaimViewModel claimToAdd)
    {
        Owner = owner;
        ClaimToAdd = claimToAdd;
    }
}
