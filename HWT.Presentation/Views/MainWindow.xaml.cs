using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using HWT.Application.Interfaces;
using HWT.Application.Services;

namespace HWT.Presentation.Views;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _clockTimer;
    private readonly INavigationService _nav;
    private readonly UpdateService _updater = new();


    public MainWindow(INavigationService nav)
    {
        InitializeComponent();
        _nav = nav;
        _nav.Initialize(ContentFrame);
        _nav.Navigate("Dashboard");

        _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _clockTimer.Tick += (s, e) =>
        {
            var now = DateTime.Now;
            TxtLocalTime.Text = now.ToString("MM/dd/yyyy") + "\n" + now.ToString("h:mm:ss tt");
        };
        _clockTimer.Start();
        
        Loaded += async (_, __) =>
        {
            // As soon as the window finishes loading, trigger a check:
            bool updated = await _updater.TryUpdateAsync( (pct, status) =>
            {
                Dispatcher.Invoke(() =>
                {
                    UpdateProgressBar.Value = pct;
                    UpdateStatusText.Text = status;
                });
            });
            if (updated)
            {
                MessageBox.Show(
                    "An update was found and installed. Please restart the app.",
                    "Update Applied",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        };
    }

    private void Navigate_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string viewName)
            _nav.Navigate(viewName);
    }
    
    private async void UpdateButton_Click(object sender, RoutedEventArgs e)
    {
        UpdateProgressBar.Visibility = Visibility.Visible;
        UpdateStatusText.Visibility   = Visibility.Visible;
        UpdateButton.IsEnabled  = false;

        UpdateProgressBar.Value      = 0;
        UpdateStatusText.Text        = "Checking for updates…";
        
        bool updated = await _updater.TryUpdateAsync((pct, status) =>
        {
            Dispatcher.Invoke(() =>
            {
                UpdateProgressBar.Value     = pct;
                UpdateStatusText.Text       = status;
            });
        });
        
        UpdateProgressBar.Visibility = Visibility.Collapsed;
        UpdateStatusText.Visibility   = Visibility.Collapsed;
        UpdateButton.IsEnabled = true;
        UpdateProgressBar.Value = 0;
        UpdateStatusText.Text = string.Empty;
        if (updated)
        {
            UpdateStatusText.Text = "Relaunching…";
            var exe = Process.GetCurrentProcess().MainModule!.FileName;
            Process.Start(new ProcessStartInfo(exe) { UseShellExecute = true });
            System.Windows.Application.Current.Shutdown();
        }
        else
        {
            UpdateStatusText.Text = "Already up to date.";
            await Task.Delay(1500);
            UpdateProgressBar.Visibility = Visibility.Collapsed;
            UpdateStatusText.Visibility  = Visibility.Collapsed;
            UpdateButton.IsEnabled = true;
        }
    }
    private void Exit_Click(object sender, RoutedEventArgs e) => Close();

}