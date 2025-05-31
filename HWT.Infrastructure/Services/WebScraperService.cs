using HtmlAgilityPack;
using HWT.Domain.Entities;
using HWT.Application.Interfaces;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace HWT.Infrastructure.Services;

/// <summary name="WebScraperService">
/// Service to scrape news articles from the Roberts Space Industries website.
/// This service fetches the HTML content of the page and parses it to extract relevant information.
/// </summary>
public class WebScraperService(HttpClient httpClient, ILogger<WebScraperService> logger) : IWebScraperService
{
    #region Fields
    private const string CommLinkUrl = "https://robertsspaceindustries.com/comm-link";
    #endregion

    #region Methods
    /// <summary name="ScrapeNewsAsync">
    /// Asynchronously scrapes the latest news articles from the RSI Comm-Link page.
    /// This method fetches the HTML content, parses it using HtmlAgilityPack,
    /// and extracts the title, link, published date, summary, and image URL of each article.
    /// It returns a list of RssFeedItem objects representing the articles found.
    /// </summary>
    public async Task<List<RssFeedItem>> ScrapeNewsAsync()
    {
        var items = new List<RssFeedItem>();

        logger.LogInformation("Starting RSS feed scrape from {Url}", CommLinkUrl);
        try
        {
            var html = await httpClient.GetStringAsync(CommLinkUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var nodes = doc.DocumentNode.SelectNodes("//a[contains(@class,'content-block2')]");
            if (nodes == null)
            {
                logger.LogWarning("No RSS nodes found at {Url}", CommLinkUrl);
                return items;
            }

            foreach (var node in nodes.Take(10))
            {
                try
                {
                    var titleNode =
                        node.SelectSingleNode(".//div[@class='title-holder']/div[contains(@class,'title')]");
                    var link = node.GetAttributeValue("href", string.Empty);
                    var dateNode =
                        node.SelectSingleNode(".//div[contains(@class,'time_ago')]/span[contains(@class,'value')]");
                    var summaryNode =
                        node.SelectSingleNode(".//div[contains(@class,'over')]//div[contains(@class,'body')]/p");
                    var bgStyle = node.SelectSingleNode(".//div[@class='background']")
                        ?.GetAttributeValue("style", string.Empty);

                    if (bgStyle != null)
                    {
                        var item = new RssFeedItem
                        {
                            Title = titleNode?.InnerText.Trim() ?? "No Title",
                            Link = $"https://robertsspaceindustries.com{link}",
                            PublishedDate = ParseRelativeDate(dateNode?.InnerText ?? string.Empty),
                            Summary = summaryNode?.InnerText.Trim() ?? "No Summary",
                            ImageUrl = ExtractBackgroundImageUrl(bgStyle)
                        };
                        items.Add(item);
                        logger.LogDebug("Parsed article: {Title}", item.Title);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error parsing node: {Html}", node.OuterHtml);
                }
            }

            logger.LogInformation("Completed scraping. Total articles retrieved: {Count}", items.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to scrape RSS feed from {Url}", CommLinkUrl);
        }

        return items;
    }

    /// <summary name="ParseRelativeDate">
    /// Parses a relative date string (e.g., "2 days ago") into a DateTime object.
    /// If the string is empty or cannot be parsed, it returns the current UTC date and time.
    /// </summary>
    private DateTime ParseRelativeDate(string text)
    {
        logger.LogDebug("Parsing relative date: {Text}", text);
        if (string.IsNullOrEmpty(text)) return DateTime.UtcNow;

        try
        {
            var lower = text.ToLower().Trim();
            var match = Regex.Match(lower, @"(\d+)");
            if (!match.Success || !int.TryParse(match.Groups[1].Value, out var value))
                return DateTime.UtcNow;

            if (lower.Contains("day")) return DateTime.UtcNow.AddDays(-value);
            if (lower.Contains("week")) return DateTime.UtcNow.AddDays(-7 * value);
            if (lower.Contains("month")) return DateTime.UtcNow.AddMonths(-value);
            if (lower.Contains("year")) return DateTime.UtcNow.AddYears(-value);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to parse relative date: {Text}", text);
        }

        return DateTime.UtcNow;
    }

    /// <summary name="ExtractBackgroundImageUrl">
    /// Extracts the background image URL from a style attribute string.
    /// If the style is empty or the URL cannot be extracted, it returns an empty string.
    /// </summary>
    private string ExtractBackgroundImageUrl(string style)
    {
        logger.LogDebug("Extracting image URL from style: {Style}", style);
        if (string.IsNullOrEmpty(style)) return string.Empty;

        try
        {
            var match = Regex.Match(style, @"background-image:url\('?(.+?)'\)?");
            var url = match.Success ? match.Groups[1].Value : string.Empty;
            logger.LogDebug("Extracted image URL: {Url}", url);
            return url;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error extracting image URL from style: {Style}", style);
            return string.Empty;
        }
    }
    #endregion
}