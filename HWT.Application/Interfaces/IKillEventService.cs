using HWT.Domain.Entities;
using HWT.Domain.DTOs;

namespace HWT.Application.Interfaces;

/// <summary name="IKillEventService">
/// Defines a service for handling kill events.
/// </summary>
public interface IKillEventService
{
    event Action<KillEntry> KillReceived;
    KillEntry? LastKill { get; }
    void Raise(KillEntry killEntry);
    
    // New methods for the controller
    Task<IEnumerable<KillEntry>> GetRecentKillsAsync(string userId, int count);
    Task<KillStatsDto> GetKillStatsAsync(string userId);
    Task SyncKillsFromGameLogAsync(string userId);
}