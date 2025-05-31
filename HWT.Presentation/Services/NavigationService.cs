using System.Windows.Controls;
using HWT.Application.Interfaces;


namespace HWT.Presentation.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _provider;
    private Frame? _hostFrame;
    private readonly Dictionary<string, Type> _map;

    public NavigationService(IServiceProvider provider)
    {
        _provider = provider;
        
        _map = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            ["Dashboard"] = typeof(Views.DashboardView), 
            ["Kill Tracker"] = typeof(Views.KillTracker), 
            ["Settings"] = typeof(Views.SettingsForm), 
            // other views…
        };
    }

    public void Initialize(Frame hostFrame)
    {
        _hostFrame = hostFrame
                     ?? throw new ArgumentNullException(nameof(hostFrame));
    }

    public void Navigate(string key)
    {
        if (_hostFrame is null)
            throw new InvalidOperationException("NavigationService not initialized.");

        if (!_map.TryGetValue(key, out var viewType))
            throw new ArgumentException($"No view mapped for '{key}'.");
        
        var viewObj = _provider.GetService(viewType)
                      ?? Activator.CreateInstance(viewType)
                      ?? throw new InvalidOperationException($"Cannot create {viewType.Name}");

        switch (viewObj)
        {
            case Page page:
                _hostFrame.Navigate(page);
                break;

            case UserControl uc:
                _hostFrame.Content = uc;
                break;

            default:
                throw new InvalidOperationException(
                    $"Mapped type '{viewType.Name}' is neither Page nor UserControl.");
        }
    }
}