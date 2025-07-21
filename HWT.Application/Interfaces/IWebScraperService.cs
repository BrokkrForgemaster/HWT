using HWT.Domain.Entities;

namespace HWT.Application.Interfaces;

/// <summary name="IWebScraperService">
/// Interface for web scraping service to fetch news articles.
/// It provides a method to scrape news articles from various sources.
/// </summary>
public interface IWebScraperService
{
    Task<List<RssFeedItem>> ScrapeNewsAsync();
}