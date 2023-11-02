using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Company.Services.Identity.API.Extensions;

public static class ModelStateExtensions
{
    public static IEnumerable<string> GetValidationErrors(this ModelStateDictionary modelState)
    {
        if (!modelState.IsValid)
        {
            return modelState.SelectMany(x => x.Value?.Errors ?? new ModelErrorCollection()).Select(x => x.ErrorMessage);
        }
        return Enumerable.Empty<string>();
    }
}
