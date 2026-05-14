using MedCorVis.Infrastructure.Caching;
using MedCorVis.Infrastructure.Email;
using MedCorVis.Infrastructure.Localization;

namespace MedCorVis.Infrastructure.Configuration;

using MedCorVis.Common.Caching;
using MedCorVis.Common.Services;
using MedCorVis.Common.Services.Email;
using Infrastructure.Caching;
using Infrastructure.Localization;
using Infrastructure.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddScoped<ICurrentCultureService, CurrentCultureService>();
        services.AddSingleton<IUserCultureCache, MemoryUserCultureCache>();
        
        services.AddOptions<EmailSettings>()
            .BindConfiguration(EmailSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IEmailService, EmailService>();
        
        return services;
    }
}