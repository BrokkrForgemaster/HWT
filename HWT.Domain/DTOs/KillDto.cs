using HWT.Domain.Entities;

namespace HWT.Domain.DTOs
{
    public class KillDto
    {
        public int Id { get; set; }
        public string KillerName { get; set; } = string.Empty;
        public string VictimName { get; set; } = string.Empty;
        public string Weapon { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.MinValue;
        public string GameLogSource { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string? UserDisplayName { get; set; } = null;
        public KillType KillType { get; set; } 
        public bool IsPvp { get; set; } 
    }
}