using Company.Services.Identity.Shared.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Company.Services.Identity.Shared.Models;

public class OperationResult
{
    public HttpStatusCode StatusCode { get; protected set; }

    public List<string> ErrorMessages { get; protected set; } = new List<string>();

    public bool IsSuccess { get; private set; } = true;

    protected OperationResult(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }

    protected OperationResult(HttpStatusCode statusCode, IEnumerable<string>? errorMessages) : this(statusCode)
    {
        ErrorMessages.AddRange(errorMessages ?? Enumerable.Empty<string>());
    }

    public static OperationResult Success(HttpStatusCode statusCode) => new(statusCode);

    public static OperationResult Failed(HttpStatusCode statusCode, string? errorMessage)
     => new(statusCode, errorMessage is not null ? new[] { errorMessage } : null) { IsSuccess = false };

    public static OperationResult Failed(HttpStatusCode statusCode, IEnumerable<string>? errorMessages)
       => new(statusCode, errorMessages) { IsSuccess = false };

    public static async Task<OperationResult> FromResponseAsync(HttpResponseMessage result)
    {
        try
        {
            result.EnsureSuccessStatusCode();
            return Success(result.StatusCode);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequestResponse = await result.Content.ReadFromJsonAsync<BadRequestResponse>();
            return Failed(result.StatusCode, badRequestResponse?.Errors);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            var notFoundResponse = await result.Content.ReadFromJsonAsync<NotFoundResponse>();
            return Failed(result.StatusCode, notFoundResponse?.Message);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.InternalServerError)
        {
            var problemResponse = await result.Content.ReadFromJsonAsync<ProblemResponse>();
            return Failed(result.StatusCode, problemResponse?.Title);
        }
    }

    public override string ToString()
    {
        if (IsSuccess)
        {
            return $"{StatusCode} success";
        }
        return $"{StatusCode} : {string.Join(';', ErrorMessages)}";
    }
}
