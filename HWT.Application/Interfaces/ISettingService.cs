using HWT.Domain.Entities;

namespace HWT.Application.Interfaces;

/// <summary name="ISettingsService">
/// Interface for managing application settings.
/// It provides methods to retrieve and save settings for the application.
/// </summary>
public interface ISettingsService
{
    AppSettings GetSettings();
    Task SaveAsync(AppSettings current);
}