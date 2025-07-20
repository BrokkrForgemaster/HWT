using HWT.Application.Interfaces;
using HWT.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HWT.Application;

/// <summary>
/// This class provides methods to register application services
/// with the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Application layer services
        services.AddScoped<IUpdateService, UpdateService>();
        
        // Add other application services here as needed
        // services.AddScoped<IUserService, UserService>();
        // services.AddScoped<IFleetService, FleetService>();
        
        return services;
    }
}