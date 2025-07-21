using System.Text.Json;
using HWT.Domain.Entities;
using HWT.Domain.Entities.Discord;
using HWT.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HWT.Infrastructure.Services;

public class DiscordService : IDiscordService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DiscordService> _logger;

    private const string DiscordApiBaseUrl = "https://discord.com/api/v10";
    private const string DiscordOAuthBaseUrl = "https://discord.com/api/oauth2";

    public DiscordService(HttpClient httpClient, IConfiguration configuration, ILogger<DiscordService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public string GetAuthorizationUrl(string state)
    {
        var clientId = _configuration["ClientId"];
        var redirectUri = _configuration["RedirectUri"];
        if (string.IsNullOrEmpty(redirectUri))
        {
            throw new InvalidOperationException("Discord RedirectUri is not configured");
        }
        var scope = "identify email";

        if (string.IsNullOrEmpty(clientId))
        {
            throw new InvalidOperationException("Discord ClientId is not configured");
        }

        var url = $"{DiscordOAuthBaseUrl}/authorize?" +
                  $"client_id={clientId}&" +
                  $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                  $"response_type=code&" +
                  $"scope={Uri.EscapeDataString(scope)}&" +
                  $"state={state}";

        _logger.LogDebug("Generated Discord authorization URL for state: {State}", state);
        return url;
    }

    public async Task<DiscordTokenResponse> ExchangeCodeForTokenAsync(string code)
    {
        var clientId = _configuration["ClientId"];
        var clientSecret = _configuration["ClientSecret"];
        var redirectUri = _configuration["RedirectUri"];
   

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new InvalidOperationException("Discord ClientId or ClientSecret is not configured");
        }

        var parameters = new Dictionary<string, string>
        {
            {"client_id", clientId},
            {"client_secret", clientSecret},
            {"grant_type", "authorization_code"},
            {"code", code},
            {"redirect_uri", redirectUri}
        };

        var content = new FormUrlEncodedContent(parameters);

        try
        {
            _logger.LogDebug("Exchanging Discord authorization code for tokens");
            
            var response = await _httpClient.PostAsync($"{DiscordOAuthBaseUrl}/token", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Discord token exchange failed. Status: {Status}, Response: {Response}", 
                    response.StatusCode, jsonResponse);
                throw new HttpRequestException($"Discord token exchange failed: {response.StatusCode}");
            }

            var tokenResponse = JsonSerializer.Deserialize<DiscordTokenResponse>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });

            if (tokenResponse == null)
            {
                throw new InvalidOperationException("Failed to deserialize Discord token response");
            }

            _logger.LogDebug("Successfully exchanged Discord code for tokens");
            return tokenResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to exchange Discord code for token");
            throw;
        }
    }

    public async Task<DiscordUser> GetUserAsync(string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new ArgumentException("Access token cannot be null or empty", nameof(accessToken));
        }

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{DiscordApiBaseUrl}/users/@me");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            _logger.LogDebug("Fetching Discord user information");

            var response = await _httpClient.SendAsync(request);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Discord user fetch failed. Status: {Status}, Response: {Response}", 
                    response.StatusCode, jsonResponse);
                throw new HttpRequestException($"Discord user fetch failed: {response.StatusCode}");
            }

            var user = JsonSerializer.Deserialize<DiscordUser>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });

            if (user == null)
            {
                throw new InvalidOperationException("Failed to deserialize Discord user response");
            }

            _logger.LogDebug("Successfully fetched Discord user: {Username}#{Discriminator}", 
                user.Username, user.Discriminator);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Discord user information");
            throw;
        }
    }

    public async Task<DiscordTokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var clientId = _configuration["ClientId"];
        var clientSecret = _configuration["ClientSecret"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new InvalidOperationException("Discord ClientId or ClientSecret is not configured");
        }

        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new ArgumentException("Refresh token cannot be null or empty", nameof(refreshToken));
        }

        var parameters = new Dictionary<string, string>
        {
            {"client_id", clientId},
            {"client_secret", clientSecret},
            {"grant_type", "refresh_token"},
            {"refresh_token", refreshToken}
        };

        var content = new FormUrlEncodedContent(parameters);

        try
        {
            _logger.LogDebug("Refreshing Discord access token");

            var response = await _httpClient.PostAsync($"{DiscordOAuthBaseUrl}/token", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Discord token refresh failed. Status: {Status}, Response: {Response}", 
                    response.StatusCode, jsonResponse);
                throw new HttpRequestException($"Discord token refresh failed: {response.StatusCode}");
            }

            var tokenResponse = JsonSerializer.Deserialize<DiscordTokenResponse>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });

            if (tokenResponse == null)
            {
                throw new InvalidOperationException("Failed to deserialize Discord refresh token response");
            }

            _logger.LogDebug("Successfully refreshed Discord access token");
            return tokenResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh Discord token");
            throw;
        }
    }

    public async Task<bool> ValidateTokenAsync(string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            return false;
        }

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{DiscordApiBaseUrl}/users/@me");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Discord token validation failed");
            return false;
        }
    }
}