using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using HWT.Application.Interfaces;
using HWT.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HWT.Infrastructure.Services
{
    /// <summary>
    /// This service provides per-user application settings stored in AppData
    /// and handles loading and saving of those settings.
    /// </summary>
    public class SettingsService : ISettingsService, IDisposable
    {
        private readonly ILogger<SettingsService> _logger;
        private readonly string _userConfigPath;
        private readonly string _defaultConfigPath;

        private readonly FileSystemWatcher _watcher;

        public SettingsService(ILogger<SettingsService> logger)
        {
            _logger = logger;

            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var userFolder = Path.Combine(appData, "HouseWolf");
            Directory.CreateDirectory(userFolder);

            _userConfigPath = Path.Combine(userFolder, "settings.json");

            var exeFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            _defaultConfigPath = Path.Combine(exeFolder, "appsettings.json");

            EnsureUserConfigExists();

            // Watch for external changes
            _watcher = new FileSystemWatcher(userFolder, "settings.json")
            {
                NotifyFilter = NotifyFilters.LastWrite
            };
            _watcher.Changed += OnUserConfigChanged;
            _watcher.EnableRaisingEvents = true;
        }

        private void EnsureUserConfigExists()
        {
            if (!File.Exists(_userConfigPath))
            {
                try
                {
                    File.Copy(_defaultConfigPath, _userConfigPath);
                    _logger.LogInformation("Copied default settings to {Path}", _userConfigPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to copy default settings");
                }
            }
        }

        private void OnUserConfigChanged(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("User settings changed on disk: {Path}", e.FullPath);
        }

        public AppSettings GetSettings()
        {
            try
            {
                var json = File.ReadAllText(_userConfigPath);
                var node = JsonNode.Parse(json)?["AppSettings"];
                if (node == null)
                {
                    _logger.LogWarning("AppSettings section missing in user config");
                    return new AppSettings();
                }
                return node.Deserialize<AppSettings>() ?? new AppSettings();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load user settings");
                return new AppSettings();
            }
        }

        public async Task SaveAsync(AppSettings newSettings)
        {
            try
            {
                _logger.LogInformation("Saving user settings to {Path}", _userConfigPath);
                var text = File.ReadAllText(_userConfigPath);
                var root = JsonNode.Parse(text)?.AsObject() ?? new JsonObject();

                root["AppSettings"] = JsonSerializer.SerializeToNode(
                    newSettings, new JsonSerializerOptions { WriteIndented = true }
                );

                var output = root.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_userConfigPath, output);
                _logger.LogInformation("User settings saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving user settings");
                throw;
            }
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }
    }
}