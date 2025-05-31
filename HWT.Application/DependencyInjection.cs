using HWT.Application.Interfaces;
using HWT.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HWT.Application;

/// <summary name="DependencyInjection">
/// This class provides methods to register application services
/// with the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IUpdateService, UpdateService>();
        return services;
    }
}