using Company.Services.Identity.Shared;
using Company.Services.Identity.Shared.ViewModels;
using Company.WebApps.Identity.BlazorClient;
using Company.WebApps.Identity.BlazorClient.Security;
using Company.WebApps.Identity.BlazorClient.Services;
using Company.WebApps.Identity.BlazorClient.Validations;
using FluentValidation;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var baseAddr = new Uri(builder.Configuration["Identity:Url"]!);

builder.Services.AddScoped<CompanyAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient("Company.WebApps.Identity.BlazorClient", client => client.BaseAddress = baseAddr)
                .AddHttpMessageHandler<CompanyAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Company.WebApps.Identity.BlazorClient"));

builder.Services.AddHttpClient<IUserRolesService, UserRolesService>(
              client => client.BaseAddress = baseAddr)
              .AddHttpMessageHandler<CompanyAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IUsersService, UsersService>(
  client => client.BaseAddress = baseAddr)
  .AddHttpMessageHandler<CompanyAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IApplicationService, ApplicationService>(
client => client.BaseAddress = baseAddr)
.AddHttpMessageHandler<CompanyAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IScopeService, ScopeService>(
client => client.BaseAddress = baseAddr)
.AddHttpMessageHandler<CompanyAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IAccountService, AccountService>(
client => client.BaseAddress = baseAddr);

builder.Services.AddHttpClient<IExternalLoginsService, ExternalLoginsService>(
client => client.BaseAddress = baseAddr)
.AddHttpMessageHandler<CompanyAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IAuthenticatorService, AuthenticatorService>(
client => client.BaseAddress = baseAddr)
    .AddHttpMessageHandler<CompanyAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IRoleClaimsService, RoleClaimsService>(
client => client.BaseAddress = baseAddr)
 .AddHttpMessageHandler<CompanyAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IUserClaimsService, UserClaimsService>(
client => client.BaseAddress = baseAddr)
 .AddHttpMessageHandler<CompanyAddressAuthorizationMessageHandler>();

builder.Services.AddTransient<IValidator<ApplicationViewModel>, ApplicationDescriptionValidator>();
builder.Services.AddTransient<IValidator<ScopeViewModel>, ScopeValidator>();
builder.Services.AddTransient<IValidator<UserRoleViewModel>, UserRoleValidator>();

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.ClientId = "pixel-identity-ui";
    options.ProviderOptions.Authority = builder.Configuration["GrizzllyIdentityUI:Authority"];
    options.ProviderOptions.ResponseType = "code";

    // Note: response_mode=fragment is the best option for a SPA. Unfortunately, the Blazor WASM
    // authentication stack is impacted by a bug that prevents it from correctly extracting
    // authorization error responses (e.g error=access_denied responses) from the URL fragment.
    // For more information about this bug, visit https://github.com/dotnet/aspnetcore/issues/28344.
    //
    options.ProviderOptions.ResponseMode = "query";
    options.AuthenticationPaths.LogInPath = "./Identity/Account/Login";
    options.AuthenticationPaths.RegisterPath = "./Identity/Account/Register";

    options.UserOptions.RoleClaim = "role";

    options.ProviderOptions.DefaultScopes.Add("roles");
})
.AddAccountClaimsPrincipalFactory<IdentityClaimsPrincipalFactory<RemoteUserAccount>>();

builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy(Policies.CanManageApplications, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(Claims.ReadWriteClaim, "applications");
    });
    options.AddPolicy(Policies.CanManageScopes, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(Claims.ReadWriteClaim, "scopes");
    });
    options.AddPolicy(Policies.CanManageUsers, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(Claims.ReadWriteClaim, "users");
    });
    options.AddPolicy(Policies.CanManageRoles, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(Claims.ReadWriteClaim, "roles");
    });
});

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.TopRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
});

await builder.Build().RunAsync();
