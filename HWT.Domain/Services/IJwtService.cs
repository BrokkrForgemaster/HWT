using System.Security.Claims;
using HWT.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HWT.Domain.Services;

public interface IJwtService
{
    Task<string> GenerateTokenAsync(ApplicationUser user, UserManager<ApplicationUser> userManager);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    bool ValidateToken(string token);
    DateTime? GetTokenExpiration(string token);
}