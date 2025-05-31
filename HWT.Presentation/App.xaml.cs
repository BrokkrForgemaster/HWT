using System.Windows;
using HWT.Application.Interfaces;
using HWT.Presentation.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HWT.Presentation
{
    public partial class App : System.Windows.Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = AppHost.Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            // start the generic host
            await _host.StartAsync();

            // create a scope to resolve scoped/singleton services
            using (var scope = _host.Services.CreateScope())
            {
                var settingsService = scope.ServiceProvider.GetRequiredService<ISettingsService>();
                var themeManager    = scope.ServiceProvider.GetRequiredService<IThemeManager>();

                // get the user's saved theme key
                var userThemeKey = settingsService.GetSettings().ThemeKey;
                
                // apply it (you could fallback to a default if null/empty)
                if (!string.IsNullOrWhiteSpace(userThemeKey))
                    themeManager.ApplyTheme(userThemeKey);
            }

            // now resolve and show the MainWindow
            var main = _host.Services.GetRequiredService<MainWindow>();
            main.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();

            base.OnExit(e);
        }
    }
}