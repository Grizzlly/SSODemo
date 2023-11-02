﻿using System.ComponentModel.DataAnnotations;

namespace Company.Services.Identity.Shared.ViewModels;

public class EnableAuthenticatorViewModel
{
    [Required(ErrorMessage = "Code is required to enable 2FA")]
    public string? Code { get; set; }

    [Required(ErrorMessage = "Key is required")]
    public string? SharedKey { get; set; }

    [Required(ErrorMessage = "Authenticator Uri is required")]
    public string? AuthenticatorUri { get; set; }
}
