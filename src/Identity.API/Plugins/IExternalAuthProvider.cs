using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Company.Services.Identity.Core.Plugins;

/// <summary>
/// IExternalAuthProvider provides a contract to add external OAuth authentication providers like Google,
/// Microsoft, Github, etc. which can be used to sign in to the application
/// </summary>
public interface IExternalAuthProvider
{
    /// <summary>
    /// Add a new external authentication provider to the AuthenticationBuilder
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="authenticationBuilder"></param>
    /// <returns></returns>
    AuthenticationBuilder AddProvider(IConfiguration configuration, AuthenticationBuilder authenticationBuilder);
}
