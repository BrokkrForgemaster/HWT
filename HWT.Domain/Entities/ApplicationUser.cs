using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace HWT.Domain.Entities;

/// <summary name="ApplicationUser">
/// This class represents a user in the application,
/// extending the IdentityUser class to include additional
/// properties related to Discord, Star Citizen, and
/// user profile information.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public Guid UserId { get; set; } = Guid.NewGuid();

    // Discord-related properties
    public string? DiscordId { get; set; }  // Made nullable
    public string? DiscordName { get; set; } = string.Empty;
    public string? DiscordDiscriminator { get; set; } = string.Empty;
    public string? DiscordAvatar { get; set; } = string.Empty;
    public string? DiscordAccessToken { get; set; } = string.Empty;
    public string? DiscordRefreshToken { get; set; } = string.Empty;
    public string? DiscordRoles { get; set; } = string.Empty;
    public DateTime? DiscordTokenExpiresAt { get; set; } = null;
    
    // Star Citizen-related properties
    public string StarCitizenCharacterName { get; set; } = string.Empty;
    public string GameLogFilePath { get; set; } = string.Empty;
    public string StarCitizenApiKey { get; set; } = string.Empty;
    public string StarCitizenApiUrl { get; set; } = string.Empty;
    public string StarCitizenOrgRank { get; set; } = string.Empty;
    public string TradingApiKey { get; set; } = string.Empty;
    public string TradingApiUrl { get; set; } = string.Empty;
    
    // Profile properties
    public string DisplayName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    
    // Audit properties
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = null;
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime? LastLoginAt { get; set; } = null;
    public DateTime? LastLogoutAt { get; set; } = null;
    
    // Preferences
    public string Theme { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    
    public List<string> GetDiscordRolesList()
    {
        if (string.IsNullOrEmpty(DiscordRoles))
            return new List<string>();
    
        try
        {
            return JsonSerializer.Deserialize<List<string>>(DiscordRoles) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    public void SetDiscordRoles(List<string> roles)
    {
        DiscordRoles = JsonSerializer.Serialize(roles);
    }
}

