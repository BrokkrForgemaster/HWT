namespace HWT.Domain.Entities;

public class KillEntry
{
    public string Timestamp { get; set; } = string.Empty;
    public string Attacker { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string Weapon { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "FPS", "Air", "Unknown"
    public string Summary { get; set; } = string.Empty;
    
    // Helper property to convert to KillType enum
    public KillType KillType => Type.ToLowerInvariant() switch
    {
        "fps" => KillType.FPS,
        "air" => KillType.AIR,
        _ => KillType.UNKNOWN,
    };
}