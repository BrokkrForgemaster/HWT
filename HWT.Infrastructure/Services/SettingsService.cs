using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using HWT.Application.Interfaces;
using HWT.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HWT.Infrastructure.Services;

/// <summary name="SettingsService"/>
/// This service provides per-user application settings stored in AppData
/// and handles loading and saving of those settings.
/// </summary>
public class SettingsService : ISettingsService, IDisposable
{
    #region Fields

    private readonly ILogger<SettingsService> _logger;
    private readonly string _userConfigPath;
    private readonly string _defaultConfigPath;
    private readonly FileSystemWatcher _watcher;

    #endregion

    #region Constructor

    /// <summary name="SettingsService"/>
    /// This constructor initializes the settings service,
    /// sets up the file paths for user and default settings,
    /// and ensures the user settings file exists.
    /// It also sets up a file watcher to monitor changes to the user settings file.
    /// </summary>
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

        _watcher = new FileSystemWatcher(userFolder, "settings.json")
        {
            NotifyFilter = NotifyFilters.LastWrite
        };
        _watcher.Changed += OnUserConfigChanged;
        _watcher.EnableRaisingEvents = true;
    }

    #endregion

    #region Methods

    /// <summary name="EnsureUserConfigExists"/>
    /// This method checks if the user settings file exists,
    /// and if not, copies the default settings file to the user settings path.
    /// It logs an error if the copy operation fails or if the file does not exist.
    /// </summary>
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

    /// <summary name="OnUserConfigChanged"/>
    /// This method is called when the user settings file changes.
    /// It logs the change event with the path of the modified file.
    /// </summary>
    private void OnUserConfigChanged(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("User settings changed on disk: {Path}", e.FullPath);
    }

    /// <summary name="GetSettings"/>
    /// This method reads the user settings from the JSON file,
    /// parses it, and returns an instance of AppSettings.
    /// If the file is missing or parsing fails,
    /// it returns a new instance of AppSettings with default values.
    /// </summary>
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

    /// <summary name="SaveAsync"/>
    /// This method saves the provided AppSettings to the user settings file.
    /// It reads the existing settings, updates the AppSettings section,
    /// and writes the updated JSON back to the file.
    /// If an error occurs during this process, it logs the error and rethrows the exception.
    /// </summary>
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

    /// <summary name="Dispose"/>
    /// This method disposes of the file watcher to release resources.
    /// </summary>
    public void Dispose()
    {
        _watcher.Dispose();
    }

    #endregion
}