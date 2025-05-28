namespace HWT.Domain.Entities;


// <summary name="KillEntry">
/// This class represents a kill entry in the game log.
/// It contains information about the time of the kill,
/// the type of kill, the attacker, the target, the weapon used,
/// and an optional summary. Represents a kill entry in the game.
/// </summary>
public class KillEntry
{
    public string Timestamp { get; set; } = "";
    public string Type { get; set; } = "";
    public string Attacker { get; set; } = "";
    public string Target { get; set; } = "";
    public string Weapon { get; set; } = "";
    public string? Summary { get; set; } 
}