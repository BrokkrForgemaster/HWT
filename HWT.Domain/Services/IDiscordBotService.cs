using HWT.Domain.Entities;
using HWT.Domain.Entities.Discord;

namespace HWT.Domain.Services;

public interface IDiscordBotService
{
    Task<List<string>> GetUserRolesAsync(string discordUserId);
    Task<List<DiscordRole>> GetGuildRolesAsync();
}