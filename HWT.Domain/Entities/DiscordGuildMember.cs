using System.Text.Json.Serialization;

namespace HWT.Domain.Entities;

public class DiscordGuildMember
{
    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; } = new();
    
    [JsonPropertyName("nick")]
    public string? Nick { get; set; }
}