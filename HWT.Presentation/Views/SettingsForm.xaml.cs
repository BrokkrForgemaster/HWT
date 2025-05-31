using System;
using System.Windows;
using System.Windows.Controls;
using HWT.Domain.Entities;
using HWT.Application.Interfaces;
using HWT.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HWT.Presentation.Views
{
    public partial class SettingsForm : UserControl
    {
        private readonly ISettingsService      _settingsService;
        private readonly IThemeManager         _themeManager;
        private readonly INavigationService    _nav;
        private readonly ILogger<SettingsForm> _logger;
        private AppSettings                    _current;

        public SettingsForm(
            IOptions<AppSettings> settings,
            IThemeManager      themeManager,
            INavigationService navigationService,
            ILogger<SettingsForm> logger,
            ISettingsService settingsService)
        {
            InitializeComponent();
            _settingsService = settingsService;
            _themeManager      = themeManager;
            _nav = navigationService;
            _logger            = logger;
            
            _current = settings.Value;

            // Populate UI...
            CmbThemes.ItemsSource  = _themeManager.AvailableThemes;
            CmbThemes.SelectedItem = _current.Theme ?? _themeManager.CurrentTheme;
            
            TxtPlayerName.Text           = _current.PlayerName;
            TxtDiscordToken.Text         = _current.DiscordToken;
            TxtGoogleSheetsKey.Text      = _current.GoogleSheetsId;
            TxtKillTrackerSheetsId.Text  = _current.KillSheetKey;
            TxtApiKey.Text               = _current.StarCitizenApiKey;
            TrdUrl.Text                  = _current.TradingApiUrl;
            TrdApiKey.Text               = _current.TradingApiKey;
            TxtGameLog.Text              = _current.GameLogFilePath;
        }

        private void cmbThemes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbThemes.SelectedItem is string theme)
                _themeManager.ApplyTheme(theme);
        }
        
        private async Task OnThemeSelected(string newThemeKey)
        {
            _themeManager.ApplyTheme(newThemeKey);
            
            var settings = _settingsService.GetSettings();
            settings.Theme = newThemeKey;
            await _settingsService.SaveAsync(settings);
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInformation("Settings saved by user.");

            // Update model
            _current.Theme              = CmbThemes.SelectedItem as string;
            _current.DiscordToken       = TxtDiscordToken.Text;
            _current.GoogleSheetsId     = TxtGoogleSheetsKey.Text;
            _current.KillSheetKey       = TxtKillTrackerSheetsId.Text;
            _current.StarCitizenApiKey  = TxtApiKey.Text;
            _current.TradingApiUrl      = TrdUrl.Text;
            _current.TradingApiKey      = TrdApiKey.Text;
            _current.GameLogFilePath    = TxtGameLog.Text;

            // Persist
            await _settingsService.SaveAsync(_current);

            MessageBox.Show("Settings saved successfully.", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);

            // Navigate back to Dashboard
            _nav.Navigate("Dashboard");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInformation("Settings cancelled by user.");

            // Reload from disk
            _current = _settingsService.GetSettings();

            // Reset UI
            CmbThemes.SelectedItem         = _current.Theme;
            TxtDiscordToken.Text           = _current.DiscordToken       ?? string.Empty;
            TxtGoogleSheetsKey.Text        = _current.GoogleSheetsId    ?? string.Empty;
            TxtKillTrackerSheetsId.Text    = _current.KillSheetKey      ?? string.Empty;
            TxtApiKey.Text                 = _current.StarCitizenApiKey ?? string.Empty;
            TrdUrl.Text                    = _current.TradingApiUrl     ?? string.Empty;
            TrdApiKey.Text                 = _current.TradingApiKey     ?? string.Empty;
            TxtGameLog.Text                = _current.GameLogFilePath   ?? string.Empty;

            // Re-apply theme
            if (_current.Theme is not null)
                _themeManager.ApplyTheme(_current.Theme);

            MessageBox.Show("Settings reset to last saved state.", "Cancelled",
                            MessageBoxButton.OK, MessageBoxImage.Information);

            // Navigate back to Dashboard
            _nav.Navigate("Dashboard");
        }
    }
}
