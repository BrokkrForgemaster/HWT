using System.Security.Claims;
using HWT.Application.Interfaces;
using HWT.Domain.DTOs;
using HWT.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HWT.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class KillController : ControllerBase
{
    private readonly IKillEventService _killEventService;

    public KillController(IKillEventService killEventService)
    {
        _killEventService = killEventService;
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentKills([FromQuery] int count = 10)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var kills = await _killEventService.GetRecentKillsAsync(userId, count);

        var killDtos = kills
            .Where(k => k.KillType == KillType.FPS || k.KillType == KillType.AIR)
            .Select(k => new KillDto
            {
                Id = 0, 
                KillerName = k.Attacker,
                VictimName = k.Target,
                Weapon = k.Weapon,
                Location = string.Empty, 
                Timestamp = DateTime.TryParse(k.Timestamp, out var ts) ? ts : DateTime.MinValue,
                GameLogSource = string.Empty, 
                UserId = string.Empty, 
                UserDisplayName = null, 
                KillType = k.KillType,
                IsPvp = false 
            }).ToList();

        return Ok(killDtos);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetKillStats()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var stats = await _killEventService.GetKillStatsAsync(userId);
        return Ok(stats);
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncKillsFromGameLog()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        await _killEventService.SyncKillsFromGameLogAsync(userId);
        return Ok(new { message = "Kills synced successfully" });
    }
}