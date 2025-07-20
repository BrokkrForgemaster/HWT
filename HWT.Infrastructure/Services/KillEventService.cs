using System.Text.Json;
using HWT.Application.Interfaces;
using HWT.Domain.DTOs;
using HWT.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HWT.Infrastructure.Services;

public class KillEventService : IKillEventService, IDisposable
{
    #region Fields
    private readonly IGameLogService     _logService;
    private readonly IGoogleSheetService _sheet;
    private readonly ILogger<KillEventService> _logger;
    private readonly IDisposable         _subscription;
    public event Action<KillEntry>? KillReceived;
    private KillEntry? _lastKill;
    public KillEntry? LastKill => _lastKill;
    private static readonly string OfflineLogPath = "queued_kills_offline.json";
    #endregion

    #region Constructor
    public KillEventService(
        IGameLogService     logService,
        IGoogleSheetService sheet,
        ILogger<KillEventService> logger)
    {
        _logService = logService;
        _sheet      = sheet;
        _logger     = logger;

        // Subscribe to parsed kills
        _subscription = SubscribeToLog();
    }
    #endregion

    #region Methods

    /// <summary name="SubscribeToLog">
    /// Subscribes to the kill events from the game log service,
    /// allowing the service to process kill entries as they are parsed.
    /// This method sets up an event handler that will be invoked
    /// whenever a new kill entry is parsed from the game log.
    /// </summary>
    private IDisposable SubscribeToLog()
    {
        _logger.LogInformation("Subscribing to kill events");
        _logService.KillParsed += Handler;
        return new DisposableAction(() => _logService.KillParsed -= Handler);
        void Handler(KillEntry k) => Raise(k);
    }
    
    /// <summary name="Raise">
    /// Raises a kill event, processing the provided kill entry.
    /// This method updates the last kill entry, invokes the KillReceived event,
    /// and attempts to log the kill entry to Google Sheets.
    /// It handles any exceptions that may occur during processing or logging,
    /// ensuring that errors are logged appropriately.
    /// </summary>
    public async void Raise(KillEntry entry)
    {
        try
        {
            _logger.LogInformation("Processing kill event: {KillEntry}", entry);
            _lastKill = entry;
            KillReceived?.Invoke(entry);

            try
            {
                _logger.LogInformation("Attempting to log kill to Google Sheets...");
                await _sheet.LogKillAsync(entry);
                _logger.LogInformation("Kill successfully logged.");
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning("Google credentials missing. Logging to offline queue. Exception: {Message}", ex.Message);
                SaveKillOffline(entry); // fallback method below
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log kill to Google Sheets. Logging to offline queue.");
                SaveKillOffline(entry);
            }

            _logger.LogInformation("Kill event processing complete.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in Raise method.");
        }
    }
    
    /// <summary name="Dispose">
    /// Disposes the KillEventService, unsubscribing from the kill events
    /// and releasing any resources held by the service.
    /// </summary>
    public void Dispose() => _subscription.Dispose();
    
    private void SaveKillOffline(KillEntry entry)
    {
        try
        {
            List<KillEntry> kills = [];

            if (File.Exists(OfflineLogPath))
            {
                var json = File.ReadAllText(OfflineLogPath);
                var existing = JsonSerializer.Deserialize<List<KillEntry>>(json);
                if (existing != null)
                    kills.AddRange(existing);
            }

            kills.Add(entry);

            File.WriteAllText(OfflineLogPath,
                JsonSerializer.Serialize(kills, new JsonSerializerOptions { WriteIndented = true }));

            _logger.LogInformation("Kill saved to offline queue: {Kill}", entry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write kill to offline file.");
        }
    }
    #endregion
    
    public async Task<IEnumerable<KillEntry>> GetRecentKillsAsync(string userId, int count)
    {
        try
        {
            _logger.LogInformation("Getting recent kills for user {UserId}, count: {Count}", userId, count);
        
            // You'll need to implement the actual data retrieval logic here
            // This could be from a database, repository, or other data source
            // For now, this is a placeholder - replace with your actual implementation
        
            // Example if you have a repository or DbContext:
            // return await _repository.GetRecentKillsAsync(userId, count);
        
            // Placeholder return - replace with actual implementation
            return new List<KillEntry>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent kills for user {UserId}", userId);
            throw;
        }
    }

    public async Task<KillStatsDto> GetKillStatsAsync(string userId)
    {
        try
        {
            _logger.LogInformation("Getting kill stats for user {UserId}", userId);
        
            // Implement your stats calculation logic here
            // This might involve querying your database for various kill statistics
        
            // Placeholder return - replace with actual implementation
            return new KillStatsDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting kill stats for user {UserId}", userId);
            throw;
        }
    }

    public async Task SyncKillsFromGameLogAsync(string userId)
    {
        try
        {
            _logger.LogInformation("Syncing kills from game log for user {UserId}", userId);
        
            // Implement your sync logic here
            // This might involve reading from game logs and updating your database
        
            // Placeholder implementation
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing kills for user {UserId}", userId);
            throw;
        }
    }
}

/// <summary name="DisposableAction">
/// This class provides a simple way to create a disposable action that executes
/// an action when disposed (e.g., for cleanup purposes).
/// </summary>
/// <param name="onDispose"></param>
public class DisposableAction(Action onDispose) : IDisposable
{
    public void Dispose() => onDispose();
}

