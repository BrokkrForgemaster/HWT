namespace HWT.Domain.DTOs;

/// <summary name="UpdateUserProfileDto">
/// Data Transfer Object for updating user profile information.
/// </summary>
public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string DiscordId { get; set; } = string.Empty;
    public string DiscordName { get; set; } = string.Empty;
    public List<string> DiscordRoles { get; set; } = new List<string>();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
    public string Theme { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    
    // Star Citizen related (optional)
    public string StarCitizenCharacterName { get; set; } = string.Empty;
    public string StarCitizenOrgRank { get; set; } = string.Empty;
}