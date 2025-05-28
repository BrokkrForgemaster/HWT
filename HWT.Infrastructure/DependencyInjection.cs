using HWT.Application.Interfaces;
using HWT.Domain.Entities;
using HWT.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HWT.Infrastructure;

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