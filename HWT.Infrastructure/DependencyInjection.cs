using HWT.Infrastructure.Services;
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
        var settings = configuration.GetSection("AppSettings").Get<AppSettings>();
        var apiKey = settings?.TradingApiKey;
    
        services.AddHttpClient("UEXCorp", client =>
        {
            client.BaseAddress = new Uri("https://api.uexcorp.space/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            if (!string.IsNullOrEmpty(apiKey))
                client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        });
    
        services
            .AddScoped<IGameLogService, GameLogService>()        
            .AddScoped<IKillEventService, KillEventService>()    
            .AddScoped<IRefineryJobsService, RefineryJobsService>() 
            .AddScoped<ISettingsService, SettingsService>()     
            .AddScoped<IUexCorpService, UexCorpService>()        
            .AddScoped<IWebScraperService, WebScraperService>(); 

        return services;
    }
    
}