namespace HWT.Domain.DTOs;

/// <summary>
/// Data Transfer Object for kill statistics.
/// </summary>
public class KillStatsDto
{
    public int TotalKills { get; set; } 
    public int TotalPvpKills { get; set; }
    public int TotalAirKills { get; set; }
    public int TotalFpsKills { get; set; }
    public int TotalDeaths { get; set; }
    public int UniqueVictims { get; set; }
    public int UniqueWeaponsUsed { get; set; }
    public DateTime? FirstKillTimestamp { get; set; }
    public DateTime? LastKillTimestamp { get; set; }
}