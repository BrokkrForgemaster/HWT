using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HWT.Application.Interfaces;
using HWT.Domain.DTOs;
using HWT.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HWT.Api.Controllers
{
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
        public async Task<ActionResult<List<KillDto>>> GetRecentKills([FromQuery] int count = 10)
        {
            if (count <= 0 || count > 100)
                return BadRequest("Count must be between 1 and 100.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var kills = await _killEventService.GetRecentKillsAsync(userId, count);

            var killDtos = kills
                .Where(k => k.KillType == KillType.FPS || k.KillType == KillType.AIR)
                .Select(k => new KillDto
                {
                    Id = k.Id,
                    KillerName = k.Attacker,
                    VictimName = k.Target,
                    Weapon = k.Weapon,
                    Location = k.Summary,
                    Timestamp = DateTime.TryParse(k.Timestamp, out var ts) ? ts : DateTime.MinValue,
                    KillType = k.KillType,
                })
                .ToList();

            return Ok(killDtos);
        }

        [HttpGet("stats")]
        public async Task<ActionResult<KillStatsDto>> GetKillStats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var stats = await _killEventService.GetKillStatsAsync(userId);
            return Ok(stats);
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncKillsFromGameLog()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await _killEventService.SyncKillsFromGameLogAsync(userId);
            return Ok(new { message = "Kills synced successfully" });
        }
    }
}