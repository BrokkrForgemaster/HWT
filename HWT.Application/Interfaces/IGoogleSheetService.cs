using HWT.Domain.Entities;

namespace HWT.Application.Interfaces;

/// <summary name="IGoogleSheetService">
/// Defines a service for logging kill entries to Google Sheets,
/// allowing for asynchronous operations.
/// </summary>
public interface IGoogleSheetService
{
    Task LogKillAsync(KillEntry kill);
}
