using Company.Services.Identity.API.Config;
using Company.Services.Identity.API.Data;
using Company.Services.Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Microsoft.AspNetCore.HttpOverrides;
using Company.Services.Identity.API.Extensions;
using System.Security.Cryptography.X509Certificates;
using Company.Services.Identity.Core.Plugins;
using MudBlazor.Services;
using Company.Services.Identity.Shared.Plugins;
using Company.Services.Identity.API;
using OpenIddict.Validation.AspNetCore;
using System.Reflection;
using AutoMapper;
using OpenIddict.Server.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var pluginsOptions = new PluginOptions();
builder.Configuration.GetSection(PluginOptions.Plugins).Bind(pluginsOptions);

//builder.AddServiceDefaults();
var mapperConfiguration = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<AutoMapProfile>();
});

#if DEBUG
mapperConfiguration.AssertConfigurationIsValid();
#endif

var mapper = mapperConfiguration.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.ConfigureCors();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(options =>
{
    options.Conventions.Add(new IdentityPageModelConvention<ApplicationUser, Guid>());
});
builder.Services.AddServerSideBlazor();

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

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
//});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityDB"));
    options.UseOpenIddict<Guid>();
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
    options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
    options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
    //options.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;

    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
})
.AddSignInManager()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddUserStore<ApplicationUserStore>()
.AddRoleStore<ApplicationRoleStore>()
.AddUserManager<UserManager<ApplicationUser>>()
//.AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Identity/Account/Login";
});

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<ApplicationDbContext>()
            .ReplaceDefaultEntities<Guid>();
    })
    .AddServer(options =>
    {
        // Enable the required endpoints
        options.SetAuthorizationEndpointUris("connect/authorize")
        .SetLogoutEndpointUris("connect/logout")
        .SetTokenEndpointUris("connect/token")
        .SetUserinfoEndpointUris("connect/userinfo")
        .SetIntrospectionEndpointUris("connect/introspect")
        .SetDeviceEndpointUris("connect/device")
        .SetVerificationEndpointUris("connect/verify");

        // When integrating with third-party APIs/resource servers this is desired.
        options.DisableAccessTokenEncryption();

        // Supported flows are:
        //      - Authorization code flow
        //      - Client credentials flow
        //      - Device code flow
        //      - Implicit flow
        //      - Password flow
        //      - Refresh token flow
        options.AllowAuthorizationCodeFlow()
        .AllowClientCredentialsFlow()
        .AllowDeviceCodeFlow()
        .AllowImplicitFlow()
        .AllowPasswordFlow()
        .AllowRefreshTokenFlow();

        // Custom auth flows are also supported
        options.AllowCustomFlow("custom_flow_name");

        // Using reference tokens means the actual access and refresh tokens
        // are stored in the database and different tokens, referencing the actual
        // tokens (in the db), are used in request headers. The actual tokens are not
        // made public.
        //options.UseReferenceAccessTokens();
        //options.UseReferenceRefreshTokens();

        // Register your scopes - Scopes are a list of identifiers used to specify
        // what access privileges are requested.
        //options.RegisterScopes(Config.GetScopes().ToArray());
        options.DisableScopeValidation();

        // Set the lifetime of your tokens
        options.SetAccessTokenLifetime(TimeSpan.FromMinutes(30));
        options.SetRefreshTokenLifetime(TimeSpan.FromDays(7));

        // https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html
        // OpenIdDict uses two types of credentials to secure the token it issues.
        // 1.Encryption credentials are used to ensure the content of tokens cannot be read by malicious parties
        if (!string.IsNullOrEmpty(builder.Configuration["Identity:Certificates:EncryptionCertificatePath"]))
        {
            var encryptionKeyBytes = File.ReadAllBytes(builder.Configuration["Identity:Certificates:EncryptionCertificatePath"]!);
            X509Certificate2 encryptionKey = new(encryptionKeyBytes, builder.Configuration["Identity:EncryptionCertificateKey"],
                 X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.EphemeralKeySet);
            options.AddEncryptionCertificate(encryptionKey);
        }
        else
        {
            options.AddDevelopmentEncryptionCertificate();
        }

        // 2.Signing credentials are used to protect against tampering
        if (!string.IsNullOrEmpty(builder.Configuration["Identity:Certificates:SigningCertificatePath"]))
        {

            var signingKeyBytes = File.ReadAllBytes(builder.Configuration["Identity:Certificates:SigningCertificatePath"]!);
            X509Certificate2 signingKey = new(signingKeyBytes, builder.Configuration["Identity:SigningCertificateKey"],
                 X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.EphemeralKeySet);
            options.AddSigningCertificate(signingKey);
        }
        else
        {
            options.AddDevelopmentSigningCertificate();
        }

        // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
        var optBuilder = options.UseAspNetCore()
        .EnableAuthorizationEndpointPassthrough()
        .EnableLogoutEndpointPassthrough()
        .EnableTokenEndpointPassthrough()
        .EnableUserinfoEndpointPassthrough()
        .EnableStatusCodePagesIntegration();

        if (builder.Configuration["Development"] == "true")
        {
            optBuilder.DisableTransportSecurityRequirement();
        }
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

var authenticationBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    //options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

foreach (var externalProvider in pluginsOptions["OAuthProvider"])
{
    builder.Services.AddPlugin<IExternalAuthProvider>(externalProvider, (provider, serviceCollection) =>
    {
        provider.AddProvider(builder.Configuration, authenticationBuilder);
    });
}

builder.Services.ConfigureAuthorizationPolicy();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        optionsBuilder =>
        {
            optionsBuilder.WithOrigins("http://127.0.0.1", "http://localhost", "http://host.docker.internal");

            var allowedOrigins = builder.Configuration["AllowedOrigins"];
            foreach (var item in allowedOrigins?.Split(';') ?? Enumerable.Empty<string>())
            {
                optionsBuilder.WithOrigins(item);
            }

            optionsBuilder.AllowAnyHeader();
            optionsBuilder.AllowAnyMethod();
            optionsBuilder.AllowCredentials();
        });
});

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.UseForwardedHeaders();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error-local-development");
    //app.UseWebAssemblyDebugging();
    //app.UseSwagger();
    //app.UseSwaggerUI(options =>
    //{
    //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Grizzlly Identity");
    //    options.RoutePrefix = string.Empty;
    //});
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapRazorPages();
    endpoints.MapDefaultControllerRoute();
    endpoints.MapFallbackToFile("index.html");
});
#pragma warning restore ASP0014 // Suggest using top level route registrations

app.Run();