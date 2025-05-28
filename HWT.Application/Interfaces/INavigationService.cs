using System.Windows.Controls;

namespace HWT.Application.Interfaces;

/// <summary name="INavigationService">
/// Defines a navigation service for managing navigation within the application.
/// </summary>
public interface INavigationService
{
    void Initialize(Frame frame);
    void Navigate(string key);
}