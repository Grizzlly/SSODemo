﻿using AutoMapper;
using Company.Services.Identity.API.Extensions;
using Company.Services.Identity.API.Models;
using Company.Services.Identity.Shared;
using Company.Services.Identity.Shared.Request;
using Company.Services.Identity.Shared.Responses;
using Company.Services.Identity.Shared.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Company.Services.Identity.API.Controllers;

/// <summary>
/// Api endpoint for managing asp.net identity users
/// It must be inherited in DbStore plugin which should provide the desired TUser
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly IMapper mapper;
    private readonly UserManager<ApplicationUser> userManager;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="mapper">Implementation of <see cref="IMapper"/> for mapping models</param>
    /// <param name="userManager">Asp.Net Identity <see cref="UserManager{TUser}"/></param>
    public UsersController(IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        this.mapper = mapper;
        this.userManager = userManager;
    }

    /// <summary>
    /// Get the available users as a paging list i.e. n items at a time.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = Policies.CanManageUsers)]
    public PagedList<UserDetailsViewModel> GetAll([FromQuery] GetUsersRequest request)
    {
        int count = 0;
        IEnumerable<ApplicationUser> applicationUsers;
        if (!string.IsNullOrEmpty(request.UsersFilter))
        {
            count = userManager.Users.Where(u => u.UserName!.Contains(request.UsersFilter)
            || u.Email!.Contains(request.UsersFilter)).Skip(request.Skip).Take(request.Take).Count();
            applicationUsers = userManager.Users.Where(u => u.UserName!.Contains(request.UsersFilter)
            || u.Email!.Contains(request.UsersFilter)).Skip(request.Skip).Take(request.Take).OrderBy(u => u.UserName);
        }
        else
        {
            count = userManager.Users.Count();
            applicationUsers = userManager.Users.Skip(request.Skip).Take(request.Take).OrderBy(u => u.UserName);
        }

        var userDetails = mapper.Map<IEnumerable<UserDetailsViewModel>>(applicationUsers);
        return new PagedList<UserDetailsViewModel>()
        {
            Items = new List<UserDetailsViewModel>(userDetails),
            ItemsCount = count,
            CurrentPage = request.CurrentPage,
            PageCount = request.PageSize
        };
    }

    /// <summary>
    /// Get the details of a user given user name
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>       
    [HttpGet("name/{userName}")]
    public async Task<ActionResult<UserDetailsViewModel>> GetUserByName(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user != null)
        {
            var userDetails = mapper.Map<UserDetailsViewModel>(user);
            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                userDetails.UserRoles.Add(new UserRoleViewModel(userRole));
            }
            var userClaims = await userManager.GetClaimsAsync(user);
            foreach (var claim in userClaims)
            {
                userDetails.UserClaims.Add(ClaimViewModel.FromClaim(claim));
            }
            return userDetails;
        }
        return NotFound(new NotFoundResponse($"User with name : {userName} doesn't exist."));
    }


    [HttpGet("id/{userId}")]
    public async Task<ActionResult<UserDetailsViewModel>> GetUserById(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var userDetails = mapper.Map<UserDetailsViewModel>(user);
            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                userDetails.UserRoles.Add(new UserRoleViewModel(userRole));
            }
            var userClaims = await userManager.GetClaimsAsync(user);
            foreach (var claim in userClaims)
            {
                userDetails.UserClaims.Add(ClaimViewModel.FromClaim(claim));
            }
            return userDetails;
        }
        return NotFound(new NotFoundResponse($"User with Id : {userId} doesn't exist"));
    }

    [HttpPost("lock")]
    [Authorize(Policy = Policies.CanManageUsers)]
    public async Task<IActionResult> LockUserAsync([FromBody] string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await userManager.SetLockoutEnabledAsync(user, true);
            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddDays(90));
            await userManager.UpdateSecurityStampAsync(user);
            return Ok();
        }
        return NotFound(new NotFoundResponse($"User with Id : {userId} doesn't exist"));
    }

    [HttpPost("unlock")]
    [Authorize(Policy = Policies.CanManageUsers)]
    public async Task<IActionResult> UnlockUserAsync([FromBody] string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var result = await userManager.SetLockoutEndDateAsync(user, null);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(new BadRequestResponse(result.GetErrors()));
        }
        return NotFound(new NotFoundResponse($"User with Id : {userId} doesn't exist"));
    }

    /// <summary>
    /// Update users details like
    /// </summary>
    /// <param name="userDetails"></param>
    /// <returns></returns>
    [HttpPost("{userId}")]
    [Authorize(Policy = Policies.CanManageUsers)]
    public async Task<IActionResult> Post(string userId, UserDetailsViewModel userDetails)
    {
        if (!string.IsNullOrEmpty(userId) && ModelState.IsValid)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.UserName = userDetails.Email; // done on purpose. we use email for both username and email
                user.Email = userDetails.Email;
                user.EmailConfirmed = true;
                user.LockoutEnabled = userDetails.LockoutEnabled;
                if (!string.IsNullOrEmpty(userDetails.PhoneNumber))
                {
                    user.PhoneNumber = userDetails.PhoneNumber;
                }
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok();
                }
                return BadRequest(new BadRequestResponse(result.GetErrors()));
            }
            return NotFound(new NotFoundResponse("User doesn't exist."));
        }
        return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));

    }

    [HttpDelete("{userName}")]
    [Authorize(Policy = Policies.CanManageUsers)]
    public async Task<IActionResult> Delete(string userName)
    {
        if (!string.IsNullOrEmpty(userName))
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                await userManager.DeleteAsync(user);
                return Ok();
            }
            return NotFound(new NotFoundResponse("User doesn't exist."));
        }
        return BadRequest(new BadRequestResponse(new[] { "UserName is required" }));
    }

    [HttpPost("add/claim")]
    [Authorize(Policy = Policies.CanManageUsers)]
    public async Task<IActionResult> AddUserClaim([FromBody] AddClaimRequest request)
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByNameAsync(request.Owner!);
            if (user != null)
            {
                var claims = await userManager.GetClaimsAsync(user);
                foreach (var claim in claims)
                {
                    if (claims.Any(a => a.Type.Equals(request.ClaimToAdd!.Type) && a.Value.Equals(request.ClaimToAdd.Value)))
                    {
                        return BadRequest(new BadRequestResponse(new[] { "Claim already exists for role" }));
                    }
                }
                Claim? claimToAdd = request.ClaimToAdd?.ToClaim();
                if (claimToAdd is not null)
                {
                    await userManager.AddClaimAsync(user, claimToAdd);
                }
                return Ok();
            }
            return NotFound(new NotFoundResponse($"User : {request.Owner} not found."));
        }
        return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
    }

    [HttpPost("update/claim")]
    [Authorize(Policy = Policies.CanManageUsers)]
    public async Task<IActionResult> UpdateUserClaim([FromBody] UpdateClaimRequest request)
    {

        if (ModelState.IsValid)
        {
            var user = await userManager.FindByNameAsync(request.Owner!);
            if (user != null)
            {
                var claims = await userManager.GetClaimsAsync(user);
                var claimToRemove = claims.FirstOrDefault(c => c.Type.Equals(request.Original?.Type)
                    && c.Value.Equals(request.Original.Value));
                if (claimToRemove != null)
                {
                    await userManager.RemoveClaimAsync(user, claimToRemove);
                    Claim? claim = request.Modified?.ToClaim();
                    if (claim is not null)
                    {
                        await userManager.AddClaimAsync(user, claim);
                    }
                }
                return Ok();
            }
            return NotFound(new NotFoundResponse($"User : {request.Owner} not found."));

        }
        return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
    }

    [HttpPost("delete/claim")]
    [Authorize(Policy = Policies.CanManageUsers)]
    public async Task<IActionResult> DeleteUserClaim([FromBody] RemoveClaimRequest request)
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByNameAsync(request.Owner!);
            if (user != null)
            {
                var claims = await userManager.GetClaimsAsync(user);
                if (claims != null)
                {
                    var claimToRemove = claims.FirstOrDefault(a => a.Type.Equals(request.ClaimToRemove?.Type) && a.Value.Equals(request.ClaimToRemove?.Value));
                    if (claimToRemove != null)
                    {
                        await userManager.RemoveClaimAsync(user, claimToRemove);
                        return Ok();
                    }
                }
                return NotFound(new NotFoundResponse($"Claim doesn't exist on role."));
            }
            return NotFound(new NotFoundResponse($"Role : {request.Owner} not found."));
        }
        return BadRequest(new BadRequestResponse(ModelState.GetValidationErrors()));
    }


}
