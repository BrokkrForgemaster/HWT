using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using HWT.Domain.Entities;
using Microsoft.Extensions.Logging;


namespace HWT.Presentation.Controls;

/// <summary name="NewsFeedItemControl">
/// This control is used to display a news feed item,
/// including its title, publication date, summary, and image.
/// It also provides a button to open the news link in the default web browser.
/// </summary>
public partial class NewsFeedItemControl : UserControl
{
    private readonly string _newsUrl;
    private readonly ILogger<NewsFeedItemControl> _log;

    public NewsFeedItemControl(RssFeedItem item, ILogger<NewsFeedItemControl> log)
    {
        _log = log;
        _log.LogInformation("Creating NewsFeedItemControl for {Title}", item.Title);
        InitializeComponent();

        _newsUrl = item.Link;
        TitleText.Text = item.Title;
        DateText.Text = item.PublishedDate.ToString("MMM dd, yyyy");
        SummaryText.Text = item.Summary;

        if (!string.IsNullOrWhiteSpace(item.ImageUrl))
        {
            _log.LogInformation("Loading image from {ImageUrl}", item.ImageUrl);
            try
            {
                NewsImage.Source = new BitmapImage(new Uri(item.ImageUrl, UriKind.RelativeOrAbsolute));
            }
            catch
            {
                _log.LogWarning("Failed to load image from URL: {ImageUrl}", item.ImageUrl);
            }
            _log.LogInformation("Image loaded successfully");
        }

        ImageButton.Click += OpenNewsUrl;
    }
    

    private void OpenNewsUrl(object sender, RoutedEventArgs e)
    {
        _log.LogInformation("Opening news URL: {NewsUrl}", _newsUrl);
        if (!string.IsNullOrEmpty(_newsUrl))
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = _newsUrl,
                    UseShellExecute = true
                });
            }
            catch
            {
                _log.LogError("Failed to open news URL: {NewsUrl}", _newsUrl);
                MessageBox.Show("Unable to open the news link.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            _log.LogInformation("News URL opened successfully");
        }
    }
}