using HWT.Domain.Entities;

namespace HWT.Application.Interfaces;
/// <summary name="IGameLogService">
/// Defines a game-log service that tails the log file and pushes parsed events.
/// </summary>
// in IGameLogService
public interface IGameLogService
{
    event Action<KillEntry> KillParsed;
    Task StartAsync(string logFilePath, CancellationToken cancellationToken);
    Task StopAsync();
}


