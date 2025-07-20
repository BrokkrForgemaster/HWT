using System.Text.Json.Serialization;

namespace HWT.Domain.Entities;

public class DiscordGuildMember
{
    [JsonPropertyName("id")]
    public int Id { get; set; } 
    
    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; } = new();
    
    [JsonPropertyName("nick")]
    public string? Nick { get; set; }
    
    [JsonPropertyName("discordUserId")]
    public string DiscordUserId { get; set; } = string.Empty;
}