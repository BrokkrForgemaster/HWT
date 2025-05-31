using System.Windows;
using HWT.Application.Interfaces;
using HWT.Domain.Entities;
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

        private void OnStartup(object sender, StartupEventArgs startupEventArgs)
        {
            var settingsSvc = _host.Services.GetRequiredService<ISettingsService>();
            var themeMgr = _host.Services.GetRequiredService<IThemeManager>();
            
            AppSettings userSettings = settingsSvc.GetSettings();
            
            string themeToApply = userSettings.Theme ?? "Dark";
            try
            {
                themeMgr.ApplyTheme(themeToApply);
            }
            catch (ArgumentException)
            {
                themeMgr.ApplyTheme("Dark");
            }

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();

            base.OnExit(e);
        }
    }
}