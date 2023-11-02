namespace Company.Services.Identity.Shared.Responses;

public class NotFoundResponse : ResponseBase
{
    public NotFoundResponse() : base()
    {
    }

    public NotFoundResponse(string message) : base(404, message)
    {           
    }
}
