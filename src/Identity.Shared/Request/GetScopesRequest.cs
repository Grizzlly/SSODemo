using System.Runtime.Serialization;

namespace Company.Services.Identity.Shared.Request;

[DataContract]
public class GetScopesRequest : PagedDataRequest
{
    public string? ScopesFilter { get; set; }
}
