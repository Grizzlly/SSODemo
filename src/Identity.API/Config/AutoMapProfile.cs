﻿using AutoMapper;
using Company.Services.Identity.Shared.ViewModels;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace Company.Services.Identity.API.Config;

/// <summary>
/// AutoMap profile for mapping required for MongoDb models and pixel identity view models
/// </summary>
public class AutoMapProfile : Profile
{
    public AutoMapProfile()
    {
        CreateMap<ApplicationViewModel, OpenIddictApplicationDescriptor>()
           .ForMember(d => d.DisplayNames, opt => opt.Ignore())
           .ForMember(a => a.Properties, opt => opt.Ignore())
           .ForMember(d => d.RedirectUris, opt => opt.MapFrom(s => s.RedirectUris))
           .ForMember(d => d.PostLogoutRedirectUris, opt => opt.MapFrom(s => s.PostLogoutRedirectUris));

        CreateMap<OpenIddictEntityFrameworkCoreApplication<Guid>, ApplicationViewModel>()
        .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.ToString()))
        .ForMember(d => d.IsConfidentialClient, opt => opt.Ignore())
        .ForMember(d => d.RedirectUris, opt => opt.MapFrom(s => s.RedirectUris.Trim(']', '[').Split(',', StringSplitOptions.None).Select(u => new Uri(u.Trim('\"'), UriKind.RelativeOrAbsolute))))
        .ForMember(d => d.PostLogoutRedirectUris, opt => opt.MapFrom(s => s.PostLogoutRedirectUris.Trim(']', '[').Split(',', StringSplitOptions.None).Select(u => new Uri(u.Trim('\"'), UriKind.RelativeOrAbsolute))))
        .ForMember(d => d.Permissions, opt => opt.MapFrom(s => s.Permissions.Trim(']', '[').Split(',', StringSplitOptions.None).Select(p => p.Trim('\"'))))
        .ForMember(d => d.Requirements, opt => opt.MapFrom(s => s.Requirements.Trim(']', '[').Split(',', StringSplitOptions.None).Select(r => r.Trim('\"'))));

        CreateMap<IdentityUser<Guid>, UserDetailsViewModel>()
         .ForMember(d => d.UserRoles, opt => opt.Ignore())
         .ForMember(d => d.UserClaims, opt => opt.Ignore());

        CreateMap<IdentityRole<Guid>, UserRoleViewModel>()
        .ForMember(d => d.RoleId, opt => opt.MapFrom(s => s.Id))
        .ForMember(d => d.RoleName, opt => opt.MapFrom(s => s.Name))
        .ForMember(d => d.Claims, opt => opt.Ignore());

        CreateMap<ScopeViewModel, OpenIddictScopeDescriptor>()
        .ForMember(d => d.DisplayNames, opt => opt.Ignore())
        .ForMember(d => d.Properties, opt => opt.Ignore())
        .ForMember(d => d.Descriptions, opt => opt.Ignore());

        CreateMap<OpenIddictEntityFrameworkCoreScope<Guid>, ScopeViewModel>()
        .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.ToString()))
        .ForMember(d => d.Resources, opt => opt.MapFrom(s => s.Resources.Trim(']', '[').Split(',', StringSplitOptions.None).Select(r => r.Trim('\"'))));
    }
}
