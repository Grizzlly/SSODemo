﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Company.Services.Identity.Shared.ViewModels;

public class ApplicationViewModel
{
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the client identifier associated with the application.
    /// </summary>
    [Required]
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the application type associated with the application.
    /// </summary>
    [Required]
    public string? Type { get; set; }

    [IgnoreDataMember]
    public bool IsConfidentialClient => Type?.Equals(ClientTypes.Confidential) ?? false;

    /// <summary>
    /// Gets or sets the client secret associated with the application.
    /// Note: depending on the application manager used when creating it,
    /// this property may be hashed or encrypted for security reasons.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the consent type associated with the application.
    /// </summary>
    [Required]
    public string? ConsentType { get; set; }

    /// <summary>
    /// Gets or sets the display name associated with the application.
    /// </summary>
    [Required]
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets the permissions associated with the application.
    /// </summary>
    [Required]
    public List<string?> Permissions { get; set; } = new();

    /// <summary>
    /// Gets the callback URLs associated with the application.
    /// </summary>      
    public List<Uri> RedirectUris { get; set; } = new();


    /// <summary>
    /// Gets the logout callback URLs associated with the application.
    /// </summary>      
    public List<Uri> PostLogoutRedirectUris { get; set; } = new();

    /// <summary>
    /// Gets the requirements associated with the application.
    /// </summary>
    [Required]
    public List<string?> Requirements { get; set; } = new();

}
