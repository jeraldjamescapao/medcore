namespace MedCorVis.Common.Modules;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public interface IModule
{
    IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration);
    WebApplication MapEndpoints(WebApplication app);
    Task RunStartupTasksAsync(WebApplication app) => Task.CompletedTask;
}