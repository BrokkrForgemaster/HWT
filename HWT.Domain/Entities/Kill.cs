namespace HWT.Domain.Entities;

public class Kill
{
    public int Id { get; set; }
    public string KillerId { get; set; } = string.Empty;
    public string KillerName { get; set; } = string.Empty;
    public string VictimId { get; set; } = string.Empty;
    public string VictimName { get; set; } = string.Empty;
    public string Weapon { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string GameLogSource { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public KillType KillType { get; set; } // <-- NEW
    public bool IsPvp { get; set; } // <-- NEW
    public ApplicationUser User { get; set; }
}