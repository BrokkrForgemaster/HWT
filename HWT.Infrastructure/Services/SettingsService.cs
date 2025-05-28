using System.Text.Json;
using System.Text.Json.Nodes;
using HWT.Application.Interfaces;
using HWT.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HWT.Infrastructure.Services;

/// <summary name="SettingsService">
/// This service provides access to application settings and allows
/// saving changes to the settings file.
/// </summary>
public class SettingsService : ISettingsService, IDisposable
{
    #region Fields

    private readonly IOptionsMonitor<AppSettings> _opts;
    private readonly IConfigurationRoot _configRoot;
    private readonly ILogger<SettingsService> _logger;
    private readonly IDisposable _subscription;

    private static readonly string JsonPath =
        Path.Combine(AppContext.BaseDirectory, "appsettings.json");

    #endregion

    #region Constructor

    public SettingsService(
        IOptionsMonitor<AppSettings> opts,
        IConfiguration configuration,
        ILogger<SettingsService> logger)
    {
        _opts = opts;
        _logger = logger;
        _configRoot = (IConfigurationRoot)configuration;
        _subscription = _opts.OnChange(s =>
            _logger.LogInformation("AppSettings reloaded: {@Settings}", s)
        );
    }

    #endregion

    #region Methods

    /// <summary name="GetSettings">
    /// Retrieves the current application settings from the configuration.
    /// </summary>
    public AppSettings GetSettings() =>
        _opts.CurrentValue;

    /// <summary name="SaveAsync">
    /// Saves the provided application settings to the appsettings.json file.
    /// This method reads the existing file, updates the AppSettings section,
    /// and writes it back to the same file.
    /// <param name="newSettings">The new settings to save.</param>
    /// </summary>
    public async Task SaveAsync(AppSettings newSettings)
    {
        _logger.LogInformation("Saving AppSettings to {Path}", JsonPath);

        try
        {
            var text = await File.ReadAllTextAsync(JsonPath);
            var root = JsonNode.Parse(text)?.AsObject();

            if (root == null)
            {
                _logger.LogError("Failed to parse JSON from {Path}", JsonPath);
                throw new JsonException("Failed to parse JSON from the settings file.");
            }

            root["AppSettings"] = JsonSerializer.SerializeToNode(
                newSettings,
                new JsonSerializerOptions { WriteIndented = true }
            );

            var output = root.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(JsonPath, output);

            _logger.LogInformation("AppSettings successfully saved to {Path}", JsonPath);
            _configRoot.Reload();
            _logger.LogInformation("Configuration reloaded after saving AppSettings");
        }
        catch (JsonException jex)
        {
            _logger.LogError(
                jex,
                "JSON parsing or serialization error when saving AppSettings to {Path}",
                JsonPath
            );
            throw;
        }
        catch (IOException ioex)
        {
            _logger.LogError(
                ioex,
                "I/O error when saving AppSettings to {Path}",
                JsonPath
            );
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error when saving AppSettings to {Path}",
                JsonPath
            );
            throw;
        }
    }


    /// <summary name="Dispose">
    /// Disposes the SettingsService, releasing any resources it holds,
    /// including the subscription to changes in AppSettings.
    /// </summary>
    public void Dispose() => _subscription.Dispose();

    #endregion
}