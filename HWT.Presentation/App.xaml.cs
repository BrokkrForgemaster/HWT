using System.Windows;
using HWT.Application.Interfaces;
using HWT.Presentation.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HWT.Presentation
{
    public partial class App : System.Windows.Application
    {
        private IHost _host;

        public App()
        {
            _host = AppHost.Build();
        }
        
        
        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();
            // using (var scope = _host.Services.CreateScope()){
            //     var settingsService = scope.ServiceProvider.GetRequiredService<ISettingsService>();
            //     var themeManager = scope.ServiceProvider.GetRequiredService<IThemeManager>();
            //     var usertheme = (await )
            // }
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