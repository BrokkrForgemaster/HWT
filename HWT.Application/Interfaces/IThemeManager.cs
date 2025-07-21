namespace HWT.Application.Interfaces;

/// <summary name="IThemeManager">
/// Interface for managing themes in the application.
/// </summary>
public interface IThemeManager
{
    string[] AvailableThemes { get; }
    string CurrentTheme { get; }
    void ApplyTheme(string themeName);
}