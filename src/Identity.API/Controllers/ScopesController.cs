using AutoMapper;
using Company.Services.Identity.API.Extensions;
using Company.Services.Identity.Shared;
using Company.Services.Identity.Shared.Request;
using Company.Services.Identity.Shared.Responses;
using Company.Services.Identity.Shared.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace Company.Services.Identity.API.Controllers;

/// <summary>
/// Api endpoint for managing scopes in OpenIddict
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = Policies.CanManageScopes)]
public sealed class ScopesController : ControllerBase
{
    private readonly IMapper mapper;
    private readonly IOpenIddictScopeManager scopeManager;
    private readonly IOpenIddictApplicationManager applicationManager;

    public ScopesController(IMapper mapper, IOpenIddictScopeManager scopeManager, IOpenIddictApplicationManager applicationManager)
    {
        this.mapper = mapper;
        this.scopeManager = scopeManager;
        this.applicationManager = applicationManager;
    }

    /// <summary>
    /// Get all the scpoes matching request criteria
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<PagedList<ScopeViewModel>> GetAll([FromQuery] GetScopesRequest request)
    {
        List<ScopeViewModel> scopeDescriptors = new();

        IQueryable<object> query(IQueryable<object> scopes) => scopes.Where(s => string.IsNullOrEmpty(request.ScopesFilter) || (s as OpenIddictEntityFrameworkCoreScope)!.DisplayName!.Contains(request.ScopesFilter)
            || (s as OpenIddictEntityFrameworkCoreScope)!.Name!.Contains(request.ScopesFilter))
            .Skip(request.Skip).Take(request.Take);

        long count = string.IsNullOrEmpty(request.ScopesFilter) ? await scopeManager.CountAsync() :
            await scopeManager.CountAsync(query, CancellationToken.None);
        await foreach (var scope in scopeManager.ListAsync(query, CancellationToken.None))
        {
            var scopeDescriptor = mapper.Map<ScopeViewModel>((OpenIddictEntityFrameworkCoreScope<Guid>)scope);
            scopeDescriptors.Add(scopeDescriptor);
        }

        return new PagedList<ScopeViewModel>()
        {
            Items = scopeDescriptors,
            ItemsCount = (int)count,
            CurrentPage = request.CurrentPage,
            PageCount = request.PageSize
        };
    }


    [HttpGet("id/{id}")]
    public async Task<ScopeViewModel> Get(string id)
    {
        var result = await scopeManager.FindByIdAsync(id, CancellationToken.None);
        var scope = mapper.Map<ScopeViewModel>(result!);
        return scope;
    }

    [HttpPost()]
    public async Task<IActionResult> Create([FromBody] ScopeViewModel scope)
    {
        if (ModelState.IsValid)
        {
            var scopeDescriptor = mapper.Map<OpenIddictScopeDescriptor>(scope);
            await scopeManager.CreateAsync(scopeDescriptor, CancellationToken.None);
            return Ok(new OkResponse(""));
        }
        return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
    }

    [HttpPut()]
    public async Task<IActionResult> Update([FromBody] ScopeViewModel scope)
    {
        if (ModelState.IsValid)
        {
            if (string.IsNullOrEmpty(scope.Id))
            {
                return BadRequest(new BadRequestResponse(new[] { "Missing Id on scope." }));
            }
            var existing = await scopeManager.FindByIdAsync(scope.Id);
            if (existing != null)
            {
                var openIdScopeDescriptor = mapper.Map<OpenIddictScopeDescriptor>(scope);
                await scopeManager.UpdateAsync(existing, openIdScopeDescriptor, CancellationToken.None);
                return Ok();
            }
            return NotFound(new NotFoundResponse($"Failed to find scope with Id : {scope.Id}"));
        }
        return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
    }

    /// <summary>
    /// Delete a scope with given Id if it is not in active use by any application
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{scopeId}")]
    public async Task<IActionResult> Delete(string scopeId)
    {
        var result = await scopeManager.FindByIdAsync(scopeId);
        ScopeViewModel? scope = mapper.Map<ScopeViewModel?>(result!);
        if (scope is not null)
        {
            IQueryable<OpenIddictEntityFrameworkCoreApplication> query(IQueryable<object> apps)
            {
                return apps.Where(app => (app is OpenIddictEntityFrameworkCoreApplication) && (app as OpenIddictEntityFrameworkCoreApplication)!
                 .Permissions!.Contains(scope.Name))
                 .Select(s => (s as OpenIddictEntityFrameworkCoreApplication)!);
            }

            long count = await applicationManager.CountAsync(query, CancellationToken.None);
            if (count > 0)
            {
                return BadRequest(new BadRequestResponse(new[] { $"Scope is in use by {count} applications." }));
            }
            await scopeManager.DeleteAsync(result!, CancellationToken.None);
            return Ok();
        }
        return NotFound(new NotFoundResponse($"Scope with id : {scopeId}  doesn't exist"));
    }
}
