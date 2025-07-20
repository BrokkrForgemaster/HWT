namespace HWT.Domain.DTOs;

/// <summary>
/// Data Transfer Object for updating user profile information.
/// </summary>
public class UpdateUserProfileDto
{
    public string? DisplayName { get; set; } 
    public string? Theme { get; set; }
    public string? Language { get; set; }
    public string? Bio { get; set; }
    public string? StarCitizenCharacterName { get; set; }
    public string? StarCitizenOrgRank { get; set; }
}