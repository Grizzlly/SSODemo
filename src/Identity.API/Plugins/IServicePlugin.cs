using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Company.Services.Identity.Core.Plugins;

/// <summary>
/// Contract to be implemented by a generic plugin
/// </summary>
public interface IServicePlugin
{
    void ConfigureService(IServiceCollection services, IConfiguration configuration);
}
