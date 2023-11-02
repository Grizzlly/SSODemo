using Microsoft.AspNetCore.Http;

namespace Company.Services.Identity.Shared.Responses;

public class ResponseBase
{       
    public int? Status { get; set; }

    public string? Message { get; set; }
  
    public ResponseBase()
    {
    }

    public ResponseBase(int status, string message)
    {
        Status = status;
        Message = message ?? GetDefaultMessageForStatusCode(status);
    }

    private static string GetDefaultMessageForStatusCode(int status)
    {
        return status switch
        {
            StatusCodes.Status400BadRequest => "Bad Request",
            StatusCodes.Status404NotFound => "Resource not found",
            StatusCodes.Status500InternalServerError => "An unhandled error occurred",
            _ => status.ToString(),
        };
    }
}
