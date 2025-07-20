using System.Windows.Controls;
using HWT.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;


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
            ["Industry Tracker"] = typeof(Views.LocopsView), 
            ["Settings"] = typeof(Views.SettingsForm), 
            // other views…
        };
    }

    public void Initialize(Frame hostFrame)
    {
        _hostFrame = hostFrame
                     ?? throw new ArgumentNullException(nameof(hostFrame));
    }

    public void Navigate(string viewKey)
    {
        if (!_map.TryGetValue(viewKey, out var viewType))
            throw new InvalidOperationException($"No view mapped for key '{viewKey}'");

        // Resolve the view from DI (so its constructor dependencies are injected)
        var viewInstance = (UserControl)_provider.GetRequiredService(viewType);
        if (_hostFrame != null) _hostFrame.Content = viewInstance;
    }
}