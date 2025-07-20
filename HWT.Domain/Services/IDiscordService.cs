using HWT.Domain.Entities;
using HWT.Domain.Entities.Discord;

namespace HWT.Domain.Services;

public interface IDiscordService
{
    string GetAuthorizationUrl(string state);
    Task<DiscordTokenResponse> ExchangeCodeForTokenAsync(string code);
    Task<DiscordUser> GetUserAsync(string accessToken);
    Task<DiscordTokenResponse> RefreshTokenAsync(string refreshToken);
    Task<bool> ValidateTokenAsync(string accessToken);
}