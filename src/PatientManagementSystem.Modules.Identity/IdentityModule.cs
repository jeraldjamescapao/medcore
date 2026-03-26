namespace PatientManagementSystem.Modules.Identity;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PatientManagementSystem.Common.Modules;
using PatientManagementSystem.Modules.Identity.Configuration;

public sealed class IdentityModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityModule(configuration);
        return services;
    }

    public WebApplication MapEndpoints(WebApplication app)
    {
        // If needed, map any minimal API endpoints.
        return app;
    }
}