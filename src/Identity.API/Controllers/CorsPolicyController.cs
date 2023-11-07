﻿using Company.Services.Identity.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace Company.Services.Identity.API.Controllers;

/// <summary>
/// Manage allowed origins for default cors policy
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = Policies.CanManageApplications)]
public sealed class CorsPolicyController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly ICorsPolicyProvider corsPolicyProvider;
    private readonly IOpenIddictApplicationManager applicationManager;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="corsPolicyProvider"></param>
    /// <param name="applicationManager"></param>
    public CorsPolicyController(IConfiguration configuration, ICorsPolicyProvider corsPolicyProvider, IOpenIddictApplicationManager applicationManager)
    {
        this.configuration = configuration;
        this.corsPolicyProvider = corsPolicyProvider;
        this.applicationManager = applicationManager;
    }

    /// <summary>
    /// Get allowed origins list on default cors policy
    /// </summary>
    /// <returns></returns>
    [HttpGet("list/origins")]
    public async Task<IActionResult> GetAllowedOrigins()
    {
        var defaultCorsPolicy = await corsPolicyProvider.GetPolicyAsync(HttpContext, null);
        return Ok(defaultCorsPolicy?.Origins);
    }

    /// <summary>
    /// Add specified origins to the list of allowed origins on default cors policy.
    /// </summary>
    /// <param name="origins"></param>
    /// <returns></returns>
    //[HttpPost("allow/origins")]
    //public async Task<IActionResult> AddOrigins(IEnumerable<string> origins)
    //{
    //    var defaultCorsPolicy = await corsPolicyProvider.GetPolicyAsync(this.HttpContext, null);
    //    foreach (var uri in origins.Select(s => new Uri(s)))
    //    {
    //        string origin = $"{uri.Scheme}://{uri.Authority}";
    //        if (!defaultCorsPolicy.Origins.Contains(origin))
    //        {
    //            defaultCorsPolicy.Origins.Add(origin);
    //        }
    //    }
    //    return Ok();
    //}

    /// <summary>
    /// Remove specified origins from the list of allowed origins on default cors policy
    /// </summary>
    /// <param name="origins"></param>
    /// <returns></returns>
    //[HttpPost("remove/origins")]
    //public async Task<IActionResult> RemoveOrigins(IEnumerable<string> origins)
    //{
    //    var defaultCorsPolicy = await corsPolicyProvider.GetPolicyAsync(this.HttpContext, null);
    //    foreach (var uri in origins.Select(s => new Uri(s)))
    //    {
    //        string origin = $"{uri.Scheme}://{uri.Authority}";
    //        if (defaultCorsPolicy.Origins.Contains(origin))
    //        {
    //            defaultCorsPolicy.Origins.Remove(origin);
    //        }
    //    }
    //    return Ok();
    //}

    /// <summary>
    /// Refresh allowed origins from AllowedOrigins configuration and redirect uri's of configured applications
    /// </summary>
    /// <returns></returns>
    [HttpPost("/refresh/origins")]
    public async Task<IActionResult> RefreshOrigins()
    {
        var defaultCorsPolicy = await corsPolicyProvider.GetPolicyAsync(HttpContext, null);

        defaultCorsPolicy?.Origins.Clear();

        var allowedOrigins = configuration["AllowedOrigins"];
        foreach (var item in allowedOrigins?.Split(';') ?? Enumerable.Empty<string>())
        {
            defaultCorsPolicy?.Origins.Add(item);
        }

        await foreach (var app in ListApplicationsAsync())
        {
            var redirectUris = await applicationManager.GetRedirectUrisAsync(app);
            foreach (var uri in redirectUris.Select(s => new Uri(s)))
            {
                string origin = $"{uri.Scheme}://{uri.Authority}";
                if (defaultCorsPolicy?.Origins.Contains(origin) is not null or false)
                {
                    defaultCorsPolicy.Origins.Add(origin);
                }
            }
        }
        return Ok();
    }

    /// <summary>
    /// Get all the available applications
    /// </summary>
    /// <returns></returns>
    private IAsyncEnumerable<object> ListApplicationsAsync()
    {
        static IQueryable<OpenIddictEntityFrameworkCoreApplication> query(IQueryable<object> apps)
        {
            return apps.Where(app => app is OpenIddictEntityFrameworkCoreApplication).Select(s => (s as OpenIddictEntityFrameworkCoreApplication)!);
        }
        return applicationManager.ListAsync(query, CancellationToken.None);
    }

}
