using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Company.Services.Identity.API.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ResetPasswordConfirmationModel : PageModel
{        
    public void OnGet()
    {
    }
}
