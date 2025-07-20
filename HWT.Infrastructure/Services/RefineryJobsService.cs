using System.Net.Http.Headers;
using System.Text.Json;
using HWT.Application.Interfaces;
using HWT.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HWT.Infrastructure.Services
{
    public class RefineryJobsService : IRefineryJobsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RefineryJobsService> _logger;
        private readonly ISettingsService _settingsService;
        private const string BaseUrl = "https://api.uexcorp.space/2.0/user_refineries_jobs/";

        public RefineryJobsService(ISettingsService settingsService,
            HttpClient httpClient, ILogger<RefineryJobsService> logger)
        {
            _settingsService = settingsService;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IReadOnlyList<RefineryJob>> GetRefineryJobsAsync(string? secretKey)
        {
            var settings = _settingsService?.GetSettings();
            secretKey = settings?.TradingApiSecretKey;
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentException("secretKey is required", nameof(secretKey));
           
            // Prepare request
            using var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", secretKey);

            // Send
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling UEXCorp API for refinery jobs.");
                throw;
            }

            // Handle HTTP status codes
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                // You can throw a custom exception or return empty
                _logger.LogWarning("UEX API returned status {StatusCode}", response.StatusCode);
                return Array.Empty<RefineryJob>();
            }
            else if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("UEX API returned non‐success status {StatusCode}", response.StatusCode);
                return Array.Empty<RefineryJob>();
            }

            // Deserialize JSON
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var jobs = JsonSerializer.Deserialize<List<RefineryJob>>(json, options);
                return jobs ?? new List<RefineryJob>();
            }
            catch (JsonException jex)
            {
                _logger.LogError(jex, "Failed to parse JSON from UEXCorp refinery jobs endpoint.");
                return Array.Empty<RefineryJob>();
            }
        }
    }
}
