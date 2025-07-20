using System.Security.Claims;
using HWT.Domain.DTOs;
using HWT.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HWT.Api.Controllers;


[Authorize]
[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // GET: api/user/profile
    [HttpGet("profile")]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return NotFound();

        var profile = new UserProfileDto
        {
            Id = user.Id,
            Username = user.UserName,
            DisplayName = user.DisplayName,
            Email = user.Email,
            AvatarUrl = user.AvatarUrl,
            DiscordId = user.DiscordId,
            DiscordName = user.DiscordName,
            DiscordRoles = user.GetDiscordRolesList(),
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            IsActive = user.IsActive,
            Theme = user.Theme,
            Language = user.Language,
            Bio = user.Bio,
            StarCitizenCharacterName = user.StarCitizenCharacterName,
            StarCitizenOrgRank = user.StarCitizenOrgRank
        };

        return Ok(profile);
    }

    // PUT: api/user/profile
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return NotFound();

        // Only update fields that are present in the DTO
        if (dto.DisplayName != null) user.DisplayName = dto.DisplayName;
        if (dto.Theme != null) user.Theme = dto.Theme;
        if (dto.Language != null) user.Language = dto.Language;
        if (dto.Bio != null) user.Bio = dto.Bio;
        if (dto.StarCitizenCharacterName != null) user.StarCitizenCharacterName = dto.StarCitizenCharacterName;
        if (dto.StarCitizenOrgRank != null) user.StarCitizenOrgRank = dto.StarCitizenOrgRank;

        await _userManager.UpdateAsync(user);

        return NoContent();
    }
}