using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PatientManagementSystem.Common.Modules;

public static class ModuleExtensions
{
    private static readonly List<IModule> Modules = [];

    public static IServiceCollection RegisterModules(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies)
    {
        Modules.Clear();

        var moduleTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IModule).IsAssignableFrom(type)
                           && type is { IsAbstract: false, IsInterface: false });

        foreach (var moduleType in moduleTypes)
        {
            var module = (IModule)Activator.CreateInstance(moduleType)!;
            module.RegisterModule(services, configuration);

            services.AddControllers()
                .AddApplicationPart(moduleType.Assembly);

            Modules.Add(module);
        }
        
        return services;
    }

    public static WebApplication MapModuleEndpoints(this WebApplication app)
    {
        return Modules.Aggregate(app, (current, module) => module.MapEndpoints(current));
    }
}