using System.Windows.Media.Imaging;
using HWT.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HWT.Presentation.Controls;

/// <summary name="RssCard">
/// This class represents a card that displays RSS feed item information.
/// It includes properties for the item's title, published date, summary, and image.
/// It also handles mouse events to open the item's link in a web browser.
/// </summary>
public abstract partial class RssCard
{
    public abstract RssFeedItem Item { set; }
    private readonly ILogger<RssCard> _log;

    public RssCard(RssFeedItem item, ILogger<RssCard> log)
    {
        _log = log;
        InitializeComponent();
        Item = item;

        TitleText.Text = item.Title;
        DateText.Text = item.PublishedDate.ToString("MMM dd, yyyy");
        SummaryText.Text = item.Summary.Length > 160 ? item.Summary[..157] + "..." : item.Summary;

        if (!string.IsNullOrEmpty(item.ImageUrl))
        {
            try
            {
                NewsImage.Source = new BitmapImage(new Uri(item.ImageUrl));
            }
            catch
            {
                _log.LogWarning( "Failed to load image from URL: {ImageUrl}", item.ImageUrl);
            }
        }

        MouseLeftButtonUp += (_, _) =>
        {
            _log.LogInformation("Opening link in browser: {Link}", item.Link);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = item.Link,
                UseShellExecute = true
            });
        };
        _log.LogInformation("RssCard created for {Title}", item.Title);
    }
}