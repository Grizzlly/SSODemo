using System.Collections.Generic;
using System.Linq;

namespace Company.Services.Identity.Shared.Responses;

public class BadRequestResponse : ResponseBase
{
    public List<string> Errors { get; set; } = new List<string>();

    public BadRequestResponse() : base()
    {
    }

    public BadRequestResponse(IEnumerable<string>? errors) : base(400, string.Empty)
    {
        Errors.AddRange(errors ?? Enumerable.Empty<string>());
    }
}
