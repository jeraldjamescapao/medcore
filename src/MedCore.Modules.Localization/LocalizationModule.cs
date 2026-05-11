namespace MedCore.Modules.Localization;

using MedCore.Common.Localization;
using MedCore.Common.Modules;
using MedCore.Modules.Localization.Configuration;
using MedCore.Modules.Localization.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public sealed class LocalizationModule : IModule
{
    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddLocalizationModule(configuration);
        return services;
    }

    public WebApplication MapEndpoints(WebApplication app)
    {
        return app;
    }
    
    public async Task RunStartupTasksAsync(WebApplication app)
    {
        await TranslationSeeder.SeedAsync(app.Services);

        var cache = app.Services.GetRequiredService<ILocalizerCache>();
        await cache.LoadAsync();
    }
}