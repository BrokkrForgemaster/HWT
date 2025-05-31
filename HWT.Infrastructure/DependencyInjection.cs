using HWT.Domain.Entities;
using HWT.Application.Interfaces;
using HWT.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HWT.Infrastructure;

/// <summary name="DependencyInjection">
/// This class is responsible for registering the services in the dependency injection container.
/// </summary>
public static class DependencyInjection 
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services
            .AddSingleton<IGameLogService, GameLogService>()
            .AddSingleton<IGoogleSheetService, GoogleSheetService>()
            .AddSingleton<IKillEventService, KillEventService>()
            .AddSingleton<ISettingsService, SettingsService>()
            .AddSingleton<IWebScraperService, WebScraperService>();


        return services;
    }
    
}