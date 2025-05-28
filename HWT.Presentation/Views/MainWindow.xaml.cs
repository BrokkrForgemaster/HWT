using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using HWT.Application.Interfaces;

namespace HWT.Presentation.Views;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _clockTimer;
    private readonly INavigationService _nav;

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
    }

    private void Navigate_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string viewName)
            _nav.Navigate(viewName);
    }

    private void Exit_Click(object sender, RoutedEventArgs e) => Close();

}