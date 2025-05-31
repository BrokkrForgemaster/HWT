using HWT.Application;
using HWT.Application.Interfaces;
using HWT.Domain.Entities;
using HWT.Infrastructure;
using HWT.Infrastructure.Services;
using HWT.Presentation.Services;
using HWT.Presentation.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Serilog;

namespace HWT.Presentation
{
    public static class AppHost
    {
        public static IHost Build() =>
            Host.CreateDefaultBuilder()
                // 1) Add Serilog, reading its settings from appsettings.json
                .UseSerilog((ctx, cfg) =>
                    cfg.ReadFrom.Configuration(ctx.Configuration))
                
                .ConfigureAppConfiguration((ctx, builder) =>
                {
                    // ensure appsettings.json is loaded
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })

                .ConfigureServices((ctx, services) =>
                {
                    var configuration = ctx.Configuration;

                    services
                        .AddInfrastructure(configuration)
                        .AddApplication()
                        .AddHttpClient()
                        .AddSingleton<INavigationService, NavigationService>()
                        .AddSingleton<ISettingsService, SettingsService>()
                        .AddSingleton<IThemeManager>(sp =>
                            new ThemeManager(System.Windows.Application.Current))
                        .AddSingleton<MainWindow>()
                        .AddSingleton<DashboardView>()
                        .AddSingleton<KillTracker>()
                        .AddSingleton<SettingsForm>();
                })

                .Build();
    }
}
