using HWT.Application;
using HWT.Application.Interfaces;
using HWT.Infrastructure;
using HWT.Presentation.Services;
using HWT.Presentation.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace HWT.Presentation
{
    public static class AppHost
    {
        public static IHost Build() =>
            Host.CreateDefaultBuilder()
                .UseSerilog((ctx, cfg) =>
                    cfg.ReadFrom.Configuration(ctx.Configuration))
                
                .ConfigureAppConfiguration((ctx, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })

                .ConfigureServices((ctx, services) =>
                {
                    var configuration = ctx.Configuration;

                    // Add layered services
                    services
                        .AddApplication()
                        .AddInfrastructure(configuration);

                    // Presentation-specific services
                    services
                        .AddHttpClient()
                        .AddSingleton<INavigationService, NavigationService>()
                        .AddSingleton<IThemeManager>(sp =>
                            new ThemeManager(System.Windows.Application.Current))
                        .AddSingleton<MainWindow>()
                        .AddSingleton<DashboardView>()
                        .AddSingleton<KillTracker>()
                        .AddSingleton<LocopsView>()
                        .AddSingleton<SettingsForm>();
                })

                .Build();
    }
}