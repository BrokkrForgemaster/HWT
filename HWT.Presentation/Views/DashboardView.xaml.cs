using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using HWT.Application.Interfaces;
using HWT.Application.Services;
using HWT.Presentation.Controls;
using Microsoft.Extensions.Logging;

namespace HWT.Presentation.Views;

/// <summary>
/// This class represents the DashboardView in the application.
/// It is responsible for displaying a grid of news feed cards
/// and dynamically resizing them based on the available space.
/// It uses a responsive wrap layout to adjust the card sizes.
/// </summary>
public partial class DashboardView : UserControl
{
    private readonly IWebScraperService _scraper;
    private readonly ILogger<DashboardView> _logger;
    private readonly ILogger<NewsFeedItemControl> _newsFeedLogger;

    public DashboardView(IWebScraperService scraper,
        ILogger<DashboardView> logger,
        ILogger<NewsFeedItemControl> newsFeedLogger)
    {
        InitializeComponent();
        _scraper = scraper;
        _logger = logger;
        _newsFeedLogger = newsFeedLogger;

        Loaded   += async (_,_) => await LoadNewsFeed();
        SizeChanged += (_,_) => AdjustCardLayout();
    }

    /// <summary>
    /// Loads the latest news feed items and adds them as cards to the dashboard panel.
    /// </summary>
    private async Task LoadNewsFeed()
    {
        _newsFeedLogger.LogInformation("Loading news feed...");
        var items = await _scraper.ScrapeNewsAsync();
        
        NewsFeedPanel.Items.Clear();
        foreach (var item in items)
        {
            var card = new NewsFeedItemControl(item, _newsFeedLogger)
            {
                Width = 370,
                Height = 200,
                Margin = new Thickness(10),
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 12,
                    Opacity = 0.3,
                    ShadowDepth = 2
                }
            };
            {
                Margin = new Thickness(10);
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 12,
                    Opacity = 0.3,
                    ShadowDepth = 2
                };
            };
            NewsFeedPanel.Items.Add(card);
        }

        AdjustCardLayout();
    }

    /// <summary>
    /// Dynamically adjusts the width of each feed card based on available container width.
    /// </summary>
    private void AdjustCardLayout()
    {
        var panel = GetWrapPanel();
        if (panel == null || NewsFeedPanel.Items.Count == 0)
            return;

        double availableWidth = ActualWidth - 60; // Adjust for padding/margin
        int columns = Math.Max(1, (int)(availableWidth / 370));
        double newWidth = availableWidth / columns;

        foreach (var item in NewsFeedPanel.Items)
        {
            var container = NewsFeedPanel.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;

            if (container != null && VisualTreeHelper.GetChildrenCount(container) > 0)
            {
                var child = VisualTreeHelper.GetChild(container, 0);
                if (child is NewsFeedItemControl card)
                {
                    card.Width = newWidth - 20;
                }
            }
        }
    }



    /// <summary>
    /// Recursively finds a visual child of a given type in the visual tree.
    /// </summary>
    private static T? FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(obj, i);
            if (child is T childOfT)
                return childOfT;

            T? childRecursive = FindVisualChild<T>(child);
            if (childRecursive != null)
                return childRecursive;
        }
        return null;
    }

    /// <summary>
    /// Finds the internal WrapPanel used in the ItemsControl's ItemsPanelTemplate.
    /// </summary>
    private WrapPanel? GetWrapPanel()
    {
        return FindVisualChild<WrapPanel>(NewsFeedPanel);
    }
    
    
    
}
