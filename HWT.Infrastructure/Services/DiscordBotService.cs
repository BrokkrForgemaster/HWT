using System.Net.Http.Headers;
using System.Text.Json;
using HWT.Domain.Entities;
using HWT.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HWT.Infrastructure.Services;

public class DiscordBotService : IDiscordBotService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DiscordBotService> _logger;
    private readonly string _botToken;
    private readonly string _guildId;

    public DiscordBotService(HttpClient httpClient, IConfiguration configuration, ILogger<DiscordBotService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _botToken = configuration["Discord:BotToken"] ?? throw new InvalidOperationException("Discord bot token not configured");
        _guildId = configuration["Discord:GuildId"] ?? throw new InvalidOperationException("Discord guild ID not configured");
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", _botToken);
        _httpClient.BaseAddress = new Uri("https://discord.com/api/v10/");
    }

    public async Task<List<string>> GetUserRolesAsync(string discordUserId)
    {
        try
        {
            // Get guild member info
            var memberResponse = await _httpClient.GetAsync($"guilds/{_guildId}/members/{discordUserId}");
            if (!memberResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch Discord member: {StatusCode}", memberResponse.StatusCode);
                return new List<string>();
            }

            var memberJson = await memberResponse.Content.ReadAsStringAsync();
            var member = JsonSerializer.Deserialize<DiscordGuildMember>(memberJson);
            
            if (member?.Roles == null || !member.Roles.Any())
                return new List<string>();

            // Get all guild roles to map IDs to names
            var rolesResponse = await _httpClient.GetAsync($"guilds/{_guildId}/roles");
            if (!rolesResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch Discord roles: {StatusCode}", rolesResponse.StatusCode);
                return new List<string>();
            }

            var rolesJson = await rolesResponse.Content.ReadAsStringAsync();
            var allRoles = JsonSerializer.Deserialize<List<DiscordRole>>(rolesJson);

            // Map role IDs to role names
            var userRoleNames = allRoles?
                .Where(role => member.Roles.Contains(role.Id))
                .Select(role => role.Name)
                .Where(name => name != "@everyone") // Exclude @everyone role
                .ToList() ?? new List<string>();

            return userRoleNames;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Discord roles for user {UserId}", discordUserId);
            return new List<string>();
        }
    }

    public async Task<List<DiscordRole>> GetGuildRolesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"guilds/{_guildId}/roles");
            if (!response.IsSuccessStatusCode)
                return new List<DiscordRole>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<DiscordRole>>(json) ?? new List<DiscordRole>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Discord guild roles");
            return new List<DiscordRole>();
        }
    }
}

// DTOs for Discord API responses


