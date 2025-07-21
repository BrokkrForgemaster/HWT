using System.Net.Http;
using HWT.Application;
using HWT.Application.Interfaces;
using HWT.Infrastructure;
using HWT.Infrastructure.Services;
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
                    builder.AddUserSecrets<App>(optional: true);
                })
                .ConfigureServices((ctx, services) =>
                {
                    var configuration = ctx.Configuration;


                    // Add layered services
                    services.AddInfrastructure(configuration);
                    
                    var apiBaseUrl = ctx.Configuration["ApiBaseUrl"];
                    if (string.IsNullOrEmpty(apiBaseUrl))
                        throw new InvalidOperationException("ApiBaseUrl is not configured.");

                    services.AddHttpClient<IKillEventService, KillEventService>(client =>
                    {
                        client.BaseAddress = new Uri("https://" + apiBaseUrl);
                    });


                    // Presentation-specific services
                    services
                        .AddHttpClient()
                        .AddSingleton<INavigationService, NavigationService>()
                        .AddSingleton<IUpdateService, UpdateService>()
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