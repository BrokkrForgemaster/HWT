using HWT.Domain.Entities;

namespace HWT.Application.Interfaces;

/// <summary name="IKillEventService">
/// Defines a service for handling kill events.
/// </summary>
public interface IKillEventService
{
    event Action<KillEntry> KillReceived;
    KillEntry? LastKill { get; }
    void Raise(KillEntry killEntry);
}