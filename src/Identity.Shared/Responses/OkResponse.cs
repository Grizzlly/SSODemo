namespace Company.Services.Identity.Shared.Responses;

public class OkResponse : ResponseBase
{
    public object? Result { get; set; }

    public OkResponse() : base()
    {
    }

    public OkResponse(object result) : base(200, string.Empty)
    {
        Result = result;
    }      
}
