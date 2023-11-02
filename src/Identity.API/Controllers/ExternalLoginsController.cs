using Company.Services.Identity.API.Models;
using Company.Services.Identity.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Company.Services.Identity.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public sealed class ExternalLoginsController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly IUserStore<ApplicationUser> userStore;
    private readonly ILogger<ExternalLoginsController> logger;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="signInManager"></param>       
    public ExternalLoginsController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, IUserStore<ApplicationUser> userStore,
        ILogger<ExternalLoginsController> logger)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.userStore = userStore;
        this.logger = logger;
    }

    /// <summary>
    /// Get the external logins for user account
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetExternalLogins()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(new NotFoundResponse("User could not be loaded."));
        }
        var currentLogins = await userManager.GetLoginsAsync(user);
        return Ok(currentLogins);
    }

    /// <summary>
    /// Remove external login from user account
    /// </summary>
    /// <param name="loginProvider"></param>
    /// <param name="providerKey"></param>
    /// <returns></returns>
    [HttpDelete("{loginProvider}/{providerKey}")]
    public async Task<IActionResult> RemoveExternalLogin(string loginProvider, string providerKey)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound(new NotFoundResponse("User could not be loaded."));
        }

        var currentLogins = await userManager.GetLoginsAsync(user);
        string passwordHash = string.Empty;
        if (userStore is IUserPasswordStore<ApplicationUser> userPasswordStore)
        {
            passwordHash = await userPasswordStore.GetPasswordHashAsync(user, HttpContext.RequestAborted) ?? passwordHash;
        }

        string hasPassword = string.IsNullOrEmpty(passwordHash) ? "no password" : "local password";
        logger.LogInformation("User has {LoginCount} external logins and {HasPassword}", currentLogins.Count, hasPassword);

        if (!string.IsNullOrEmpty(passwordHash) || currentLogins.Count > 1)
        {
            var result = await userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded)
            {
                logger.LogError("Error while removing external login provider {LoginProvider} for user {UserName}." +
                    " {Errors}", loginProvider, await userManager.GetUserNameAsync(user), string.Join(';', result.Errors));
                return Problem($"Error while removing external login provider : {loginProvider}");
            }
            logger.LogInformation("External login {LoginProvider} removed successfully for user {UserName}",
                loginProvider, await userManager.GetUserNameAsync(user));
            await signInManager.RefreshSignInAsync(user);
            return Ok();
        }

        return Problem($"Error while removing external login provider : {loginProvider}");
    }
}
