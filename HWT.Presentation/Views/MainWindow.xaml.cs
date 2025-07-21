using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using HWT.Application.Interfaces;
using HWT.Domain.Entities;
using HWT.Presentation.Controls;
using Microsoft.Extensions.Configuration;

namespace HWT.Presentation.Views
{
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public MainWindow(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            NavigateToDashboard();
        }

        private void Navigate_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                switch (tag)
                {
                    case "Dashboard":
                        NavigateToDashboard();
                        break;
                    case "Kill Tracker":
                        NavigateToKillTracker();
                        break;
                    case "Industry Tracker":
                        NavigateToIndustryTracker();
                        break;
                    case "Settings":
                        NavigateToSettings();
                        break;
                }
            }
        }

        private void NavigateToDashboard()
        {
            var scraper = _serviceProvider.GetRequiredService<IWebScraperService>();
            var logger = _serviceProvider.GetRequiredService<ILogger<DashboardView>>();
            var newsFeedLogger = _serviceProvider.GetRequiredService<ILogger<NewsFeedItemControl>>();

            var dashboardView = new DashboardView(scraper, logger, newsFeedLogger);
            ContentFrame.Navigate(dashboardView);
        }

        private void NavigateToKillTracker()
        {
            var killEvents = _serviceProvider.GetRequiredService<IKillEventService>();
            var logService = _serviceProvider.GetRequiredService<IGameLogService>();
            var settings = _serviceProvider.GetRequiredService<ISettingsService>();
            var logger = _serviceProvider.GetRequiredService<ILogger<KillTracker>>();

            var killTrackerView = new KillTracker(killEvents, logService, settings, logger);
            ContentFrame.Navigate(killTrackerView);
        }

        private void NavigateToIndustryTracker()
        {
            var uexService = _serviceProvider.GetRequiredService<IUexCorpService>();
            var logger = _serviceProvider.GetRequiredService<ILogger<LocopsView>>();
            var httpClient = _serviceProvider.GetRequiredService<HttpClient>();
            var configuration = _serviceProvider.GetRequiredService<IConfiguration>();

            var industryView = new LocopsView(uexService, logger, httpClient, configuration);
            ContentFrame.Navigate(industryView);
        }

        private void NavigateToSettings()
        {
            var settingsOptions = _serviceProvider.GetRequiredService<IOptions<AppSettings>>();
            var themeManager = _serviceProvider.GetRequiredService<IThemeManager>();
            var navigationService = _serviceProvider.GetRequiredService<INavigationService>();
            var logger = _serviceProvider.GetRequiredService<ILogger<SettingsForm>>();
            var settingsService = _serviceProvider.GetRequiredService<ISettingsService>();

            var settingsView = new SettingsForm(settingsOptions, themeManager, navigationService, logger, settingsService);
            ContentFrame.Navigate(settingsView);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateButton.IsEnabled = false;
                UpdateProgressBar.Visibility = Visibility.Visible;
                UpdateStatusText.Visibility = Visibility.Visible;
                UpdateStatusText.Text = "Updating...";

                // Simulate update process (replace with your actual update logic)
                for (int i = 0; i <= 100; i += 10)
                {
                    UpdateProgressBar.Value = i;
                    await Task.Delay(200);
                }

                UpdateStatusText.Text = "Update completed!";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Update failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateStatusText.Text = "Update failed.";
            }
            finally
            {
                UpdateProgressBar.Visibility = Visibility.Collapsed;
                UpdateButton.IsEnabled = true;
            }
        }
    }
}