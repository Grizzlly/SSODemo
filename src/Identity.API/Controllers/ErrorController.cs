using Company.Services.Identity.Core.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Company.Services.Identity.API.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public sealed class ErrorController : Controller
{
    /// <summary>
    /// Error handler endpoint for dev environment which includes the stack trace
    /// </summary>
    /// <param name="webHostEnvironment"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [Route("/error-local-development")]
    public IActionResult ErrorLocalDevelopment([FromServices] IWebHostEnvironment webHostEnvironment)
    {
        if (webHostEnvironment.EnvironmentName != "Development")
        {
            throw new InvalidOperationException(
                "This shouldn't be invoked in non-development environments.");
        }

        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

        //Api endpoints are prefixed with /api/end-point.
        //We want to return a json response in this case.
        if (context?.Path.Contains("api") is not null or false)
        {
            return Problem(detail: context.Error.StackTrace, title: context.Error.Message);
        }

        //For razor pages or mvc pages, we want to return a view with error details
        return View("Error", new ErrorViewModel()
        {
            RequestId = HttpContext.TraceIdentifier,
            Message = context?.Error.Message,
            StackTrace = context?.Error.StackTrace
        });

    }

    /// <summary>
    /// Error handler path for production environment which doesn't include the stack trace
    /// </summary>
    /// <param name="webHostEnvironment"></param>
    /// <returns></returns>
    [Route("/error")]
    public IActionResult Error([FromServices] IWebHostEnvironment webHostEnvironment)
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

        //Api endpoints are prefixed with /api/end-point.
        //We want to return a json response in this case.
        if (context?.Path.Contains("api") is not null or false)
        {
            return Problem(title: context.Error.Message);
        }

        //For razor pages or mvc pages, we want to return a view with error details
        return View("Error", new ErrorViewModel()
        {
            RequestId = HttpContext.TraceIdentifier,
            Message = context?.Error.Message
        });
    }
}
