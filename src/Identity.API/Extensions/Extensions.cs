using Company.Services.Identity.Shared;
using Company.Services.Identity.Shared.Plugins;

namespace Company.Services.Identity.API.Extensions;

public static class Extensions
{
    public static WebApplicationBuilder ConfigureCors(this WebApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    var allowedOrigins = applicationBuilder.Configuration["AllowedOrigins"];
                    foreach (var item in allowedOrigins?.Split(';') ?? Enumerable.Empty<string>())
                    {
                        builder.WithOrigins(item);
                    }
                    //This is required for pre-flight request for CORS
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.AllowCredentials();
                });
        });

        return applicationBuilder;
    }

    /// <summary>
    /// Configure the authorization policies for access control.
    /// </summary>
    /// <param name="services"></param>
    public static IServiceCollection ConfigureAuthorizationPolicy(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
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

        return services;
    }

    /// <summary>
    /// Load a service plugin from plugins directory and invoke the ConfigureService() method on plugin.
    /// This will allow plugin to register the required services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="pluginName"></param>
    /// <param name="sharedTypes"></param>
    /// <returns></returns>
    public static IServiceCollection AddPlugin<T>(this IServiceCollection services, Plugin plugin,
        Action<T, IServiceCollection> configure) where T : class
    {
        string pluginsDirectory = Path.Combine(AppContext.BaseDirectory, plugin.Path!, plugin.Name!);
        if (Directory.Exists(pluginsDirectory))
        {
            var pluginFile = Directory.GetFiles(pluginsDirectory, "*.dll").Where(f => Path.GetFileNameWithoutExtension(f).Equals(plugin.Name)).Single();
            var pluginLoadContext = new PluginLoadContext(pluginFile);
            foreach (var type in pluginLoadContext.LoadFromAssemblyName(new System.Reflection.AssemblyName(Path.GetFileNameWithoutExtension(pluginFile))).GetTypes()
                .Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract))
            {
                var servicePlugin = (T)Activator.CreateInstance(type)!;
                configure(servicePlugin, services);
            }
        }
        return services;
    }
}
