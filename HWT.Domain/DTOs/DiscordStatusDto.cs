namespace HWT.Domain.DTOs;

public class DiscordStatusDto
{
    public string DiscordId { get; set; } = string.Empty;
    public string DiscordName { get; set; } = string.Empty;
    public bool TokenValid { get; set; }
    public DateTime? TokenExpiresAt { get; set; }
    public string AvatarUrl { get; set; } = string.Empty;
}