using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using HWT.Application.Interfaces;
using HWT.Domain.Entities;
using Microsoft.Extensions.Logging;


namespace HWT.Presentation.Views;

public partial class KillTracker : UserControl
{
    private readonly ObservableCollection<KillEntry> _recentKills = new();
    private readonly IKillEventService _killEvents;
    private readonly IGameLogService _logService;
    private readonly ISettingsService _settings;
    private readonly ILogger<KillTracker> _logger;

    private const int MaxKillCount = 3;
    private int _fpsKillCount;
    private int _airKillCount;

    public KillTracker(
        IKillEventService killEvents,
        IGameLogService logService,
        ISettingsService settings,
        ILogger<KillTracker> logger)
    {
        InitializeComponent();

        _killEvents = killEvents;
        _logService = logService;
        _settings = settings;
        _logger = logger;
        var view = CollectionViewSource.GetDefaultView(_recentKills);
        view.Filter = o =>
        {
            if (o is KillEntry kill)
                return kill.Type == "FPS" || kill.Type == "Air";
            return false;
        };

        RecentKillsList.ItemsSource = view;
        _killEvents.KillReceived += OnKillReceived;

        // seed UI with the last known status (e.g. Connected)
        if (_killEvents.LastKill != null)
            OnKillReceived(_killEvents.LastKill);

        TxtFpsKillCount.Text = "0";
        TxtAirKillCount.Text = "0";

        StartLogMonitor();
    }

    private void StartLogMonitor()
    {
        var path = _settings.GetSettings().GameLogFilePath;
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            _logger.LogError("Invalid game log path: {Path}", path);
            ApplyConnectionStatus("Error", "Info");
            return;
        }

        // Run tail loop in background so UI stays responsive
        _ = Task.Run(async () =>
        {
            try
            {
                await _logService.StartAsync(path, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GameLogService crashed");
                Dispatcher.Invoke(() => ApplyConnectionStatus("Error", "Info"));
            }

            return Task.CompletedTask;
        });
    }

    private void OnKillReceived(KillEntry kill)
    {
        Dispatcher.Invoke(() =>
        {
            // Info entries drive connection status
            if (kill.Type == "Info")
            {
                ApplyConnectionStatus(kill.Summary!, kill.Type);
                LogStatusEntry(kill.Summary!);
                return;
            }

            // update counters
            if (kill.Type == "FPS")
                TxtFpsKillCount.Text = (++_fpsKillCount).ToString();
            else if (kill.Type == "Air")
                TxtAirKillCount.Text = (++_airKillCount).ToString();

            // recent kills list
            _recentKills.Insert(0, kill);
            if (_recentKills.Count > MaxKillCount)
                _recentKills.RemoveAt(_recentKills.Count - 1);
        });
    }

    private void ApplyConnectionStatus(string summary, string type = "Info")
    {
        SolidColorBrush brush;
        if (summary.Equals("Connected", StringComparison.OrdinalIgnoreCase))
        {
            brush = new SolidColorBrush(Color.FromRgb(144, 238, 144)); // same light green
        }
        else if (summary.Equals("Disconnected", StringComparison.OrdinalIgnoreCase))
        {
            brush = new SolidColorBrush(Color.FromRgb(240, 128, 128)); // same light red
        }
        else
        {
            // fallback to Info color
            brush = (SolidColorBrush)
                ((IValueConverter)FindResource("KillTypeToColorConverter"))
                .Convert("Info", typeof(Brush), null, CultureInfo.CurrentCulture)!;
        }

        Dispatcher.Invoke(() =>
        {
            ConnectionStatus.Text       = summary;
            ConnectionStatus.Foreground = brush;
        });
    }


    private void LogStatusEntry(string summary)
    {
        var entry = new KillEntry
        {
            Timestamp = DateTime.Now.ToString("T"),
            Type = "Info",
            Summary = summary
        };

        if (_recentKills.Count > 0 && _recentKills[0].Type == "Info")
            _recentKills.RemoveAt(0);

        _recentKills.Insert(0, entry);
    }
}