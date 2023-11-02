using Company.Services.Identity.API.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Company.Services.Identity.API.Areas.Identity.Pages.Account;

[AllowAnonymous]
[IdentityDefaultUI(typeof(LoginWithRecoveryCodeModel<,>))]
public abstract class LoginWithRecoveryCodeModel : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public class InputModel
    {
        [BindProperty]
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Recovery Code")]
        public string RecoveryCode { get; set; }
    }

    public virtual Task<IActionResult> OnGetAsync(string returnUrl = null) => throw new NotImplementedException();

    public virtual Task<IActionResult> OnPostAsync(string returnUrl = null) => throw new NotImplementedException();
}

public class LoginWithRecoveryCodeModel<TUser, TKey> : LoginWithRecoveryCodeModel 
    where TUser : IdentityUser<TKey>, new()
    where TKey : IEquatable<TKey>
{
    private readonly SignInManager<TUser> signInManager;
    private readonly UserManager<TUser> userManager;
    private readonly ILogger<LoginWithRecoveryCodeModel> logger;

    public LoginWithRecoveryCodeModel(SignInManager<TUser> signInManager,
        UserManager<TUser> userManager, ILogger<LoginWithRecoveryCodeModel> logger)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.logger = logger;
    }

    public override async Task<IActionResult> OnGetAsync(string returnUrl = null)
    {
        // Ensure the user has gone through the username & password screen first
        var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            throw new InvalidOperationException($"Unable to load two-factor authentication user.");
        }

        ReturnUrl = returnUrl;

        return Page();
    }

    public override async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            throw new InvalidOperationException($"Unable to load two-factor authentication user.");
        }

        var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

        var result = await signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        var userId = await userManager.GetUserIdAsync(user);

        if (result.Succeeded)
        {
            logger.LogInformation("User logged in with a recovery code.");
            return LocalRedirect(returnUrl ?? Url.Content("~/"));
        }
        if (result.IsLockedOut)
        {
            logger.LogWarning("User account locked out.");
            return RedirectToPage("./Lockout");
        }
        else
        {
            logger.LogWarning("Invalid recovery code entered.");
            ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
            return Page();
        }
    }
}
