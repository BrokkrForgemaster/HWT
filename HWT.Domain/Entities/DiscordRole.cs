using System.Text.Json.Serialization;

namespace HWT.Domain.Entities;

public class DiscordRole
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("color")]
    public int Color { get; set; }
    
    [JsonPropertyName("position")]
    public int Position { get; set; }
}