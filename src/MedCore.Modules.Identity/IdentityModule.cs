namespace MedCore.Modules.Identity;

using MedCore.Common.Modules;
using MedCore.Modules.Identity.Configuration;
using MedCore.Modules.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public sealed class IdentityModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityModule(configuration);
        return services;
    }

    public WebApplication MapEndpoints(WebApplication app)
    {
        return app;
    }
    
    public async Task RunStartupTasksAsync(WebApplication app)
    {
        await IdentityRoleSeeder.SeedAsync(app.Services);
        await AdminUserSeeder.SeedAsync(app.Services);
    }
}