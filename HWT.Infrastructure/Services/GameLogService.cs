using HWT.Domain.Entities;
using HWT.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace HWT.Infrastructure.Services;

/// <summary name="GameLogService">
/// This service reads a game log file, parses kill entries,
/// and raises events for each parsed kill.
/// </summary>
public class GameLogService(ILogger<GameLogService> logger) : IGameLogService
{
    #region Fields
    private long _lastPosition;
    public event Action<KillEntry>? KillParsed;
    #endregion
    
    #region Methods
    
    /// <summary name="StartAsync">
    /// Starts the game log service by reading the specified log file,
    /// parsing kill entries, and invoking the KillParsed event.
    /// <para name="logFilePath">The path to the game log file.</para>
    /// <para name="cancellationToken">A cancellation token to stop the service.</para>
    /// </summary>
    public async Task StartAsync(string logFilePath, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting GameLogService for {Path}", logFilePath);
        
        KillParsed?.Invoke(new KillEntry { Type = "Info", Summary = "Connected", Timestamp = DateTime.Now.ToString("T") });

        await using var stream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);
        stream.Seek(_lastPosition, SeekOrigin.Begin);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync(cancellationToken);
                if (line == null)
                {
                    logger.LogDebug("No new lines in log file, waiting...");
                    await Task.Delay(500, cancellationToken);
                    continue;
                }
                logger.LogDebug("Read line: {Line}", line);
                _lastPosition = stream.Position;

                var kill = KillParser.ExtractKill(line);
                if (kill == null) continue;
                logger.LogDebug("Parsed kill: {Summary}", kill.Summary);
                KillParsed?.Invoke(kill);
            }
        }
        finally
        {
            KillParsed?.Invoke(new KillEntry { Type = "Info", Summary = "Disconnected", Timestamp = DateTime.Now.ToString("T") });
            logger.LogInformation("Stopped GameLogService for {Path}", logFilePath);
        }
    }
    public Task StopAsync() => Task.CompletedTask;
    #endregion
}