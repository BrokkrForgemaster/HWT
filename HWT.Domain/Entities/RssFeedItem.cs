namespace HWT.Domain.Entities;

/// <summary name="RssFeedItem">
/// This class represents an item in an RSS feed
/// with properties for the title, link, published date, summary, and image URL.
/// Represents an item in an RSS feed.
/// </summary>
public class RssFeedItem
{
    public string Title { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty; // Add this line
}