using System.Security.Claims;
using HWT.Domain.DTOs;
using HWT.Domain.Entities;
using HWT.Domain.Entities.Discord;
using HWT.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HWT.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IDiscordService _discordService;
    private readonly IJwtService _jwtService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AuthController> _logger;
    private readonly IDiscordBotService? _discordBotService = null;

    public AuthController(
        IDiscordService discordService,
        IJwtService jwtService,
        UserManager<ApplicationUser> userManager,
        ILogger<AuthController> logger)
    {
        _discordService = discordService;
        _jwtService = jwtService;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Initiates Discord OAuth2 login flow
    /// </summary>
    /// <param name="returnUrl">Optional URL to redirect to after successful authentication</param>
    /// <returns>Discord authorization URL</returns>
    [HttpGet("discord/login")]
    public IActionResult DiscordLogin([FromQuery] string? returnUrl = null)
    {
        try
        {
            var state = Guid.NewGuid().ToString();
            
            // Store state and returnUrl in session for validation
            HttpContext.Session.SetString("discord_oauth_state", state);
            if (!string.IsNullOrEmpty(returnUrl))
            {
                HttpContext.Session.SetString("discord_return_url", returnUrl);
            }

            var authUrl = _discordService.GetAuthorizationUrl(state);
            
            _logger.LogInformation("Discord login initiated with state: {State}", state);
            
            return Redirect(authUrl);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initiate Discord login");
            return BadRequest(new { error = "Failed to initiate Discord login" });
        }
    }

    /// <summary>
    /// Handles Discord OAuth2 callback and completes authentication
    /// </summary>
    /// <param name="code">Authorization code from Discord</param>
    /// <param name="state">State parameter for CSRF protection</param>
    /// <returns>JWT token and user information</returns>
    [HttpGet("discord/callback")]
public async Task<IActionResult> DiscordCallback([FromQuery] string code, [FromQuery] string state)
{
    try
    {
        // Validate required parameters
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest(new { error = "Authorization code is required" });
        }

        if (string.IsNullOrEmpty(state))
        {
            return BadRequest(new { error = "State parameter is required" });
        }

        // Validate state parameter for CSRF protection
        var storedState = HttpContext.Session.GetString("discord_oauth_state");
        if (string.IsNullOrEmpty(storedState) || storedState != state)
        {
            _logger.LogWarning("Invalid state parameter. Expected: {Expected}, Received: {Received}", storedState, state);
            return BadRequest(new { error = "Invalid state parameter" });
        }

        // Exchange authorization code for access token
        var tokenResponse = await _discordService.ExchangeCodeForTokenAsync(code);
        
        // Get Discord user information
        var discordUser = await _discordService.GetUserAsync(tokenResponse.AccessToken);
        
        // Find existing user by Discord ID
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.DiscordId == discordUser.Id);

        if (user == null)
        {
            // Create new user
            user = new ApplicationUser
            {
                UserName = discordUser.Username,
                Email = discordUser.Email ?? $"{discordUser.Username}@discord.local",
                EmailConfirmed = discordUser.Verified ?? false,
                
                // Discord properties
                DiscordId = discordUser.Id,
                DiscordName = discordUser.Username,
                DiscordDiscriminator = discordUser.Discriminator,
                DiscordAvatar = discordUser.Avatar,
                DiscordAccessToken = tokenResponse.AccessToken,
                DiscordRefreshToken = tokenResponse.RefreshToken,
                DiscordTokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                
                // Profile properties
                DisplayName = discordUser.Username,
                AvatarUrl = GetDiscordAvatarUrl(discordUser),
                
                // Audit properties
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to create user: {Errors}", errors);
                return BadRequest(new { error = "Failed to create user account", details = errors });
            }

            _logger.LogInformation("Created new user with Discord ID: {DiscordId}, Username: {Username}", 
                discordUser.Id, discordUser.Username);
        }
        else
        {
            // Update existing user with latest Discord information
            user.DiscordName = discordUser.Username;
            user.DiscordDiscriminator = discordUser.Discriminator;
            user.DiscordAvatar = discordUser.Avatar;
            user.DiscordAccessToken = tokenResponse.AccessToken;
            user.DiscordRefreshToken = tokenResponse.RefreshToken;
            user.DiscordTokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            user.AvatarUrl = GetDiscordAvatarUrl(discordUser);
            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            // Update email if not set or if Discord email is verified
            if (string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(discordUser.Email))
            {
                user.Email = discordUser.Email;
                user.EmailConfirmed = discordUser.Verified ?? false;
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                _logger.LogError("Failed to update user: {Errors}", errors);
                return BadRequest(new { error = "Failed to update user account", details = errors });
            }

            _logger.LogInformation("Updated existing user with Discord ID: {DiscordId}, Username: {Username}", 
                discordUser.Id, discordUser.Username);
        }

        // **NEW: Fetch Discord server roles**
        try
        {
            if (_discordBotService != null)
            {
                var discordRoles = await _discordBotService.GetUserRolesAsync(discordUser.Id);
                user.SetDiscordRoles(discordRoles);
                await _userManager.UpdateAsync(user);
            
                _logger.LogInformation("Updated Discord roles for user {UserId}: {Roles}", 
                    user.Id, string.Join(", ", discordRoles));
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch Discord roles for user {UserId}", user.Id);
            // Continue with login even if role fetching fails
        }

        // Generate JWT token for the application
        var token = await _jwtService.GenerateTokenAsync(user, _userManager);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Store refresh token (using SecurityStamp for simplicity)
        user.SecurityStamp = refreshToken;
        await _userManager.UpdateAsync(user);

        // Prepare response
        var authResponse = new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60), // Match your JWT expiration
            User = new UserProfileDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                DisplayName = user.DisplayName,
                Email = user.Email ?? string.Empty,
                AvatarUrl = user.AvatarUrl,
                DiscordId = user.DiscordId,
                DiscordName = user.DiscordName ?? string.Empty,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                DiscordRoles = user.GetDiscordRolesList() // **NEW: Include Discord roles in response**
            }
        };

        // Handle return URL if provided
        var returnUrl = HttpContext.Session.GetString("discord_return_url");
        
        // Clear session data
        HttpContext.Session.Remove("discord_oauth_state");
        HttpContext.Session.Remove("discord_return_url");

        // For web applications, redirect to frontend with token
        if (!string.IsNullOrEmpty(returnUrl))
        {
            var redirectUrl = $"{returnUrl}?token={Uri.EscapeDataString(token)}&refresh_token={Uri.EscapeDataString(refreshToken)}";
            return Redirect(redirectUrl);
        }

        return Ok(authResponse);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Discord authentication failed for code: {Code}", code);
        return BadRequest(new { error = "Authentication failed", details = ex.Message });
    }
}

    /// <summary>
    /// Refreshes an expired JWT token using a refresh token
    /// </summary>
    /// <param name="request">Refresh token request containing the expired token and refresh token</param>
    /// <returns>New JWT token and refresh token</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new { error = "Token and refresh token are required" });
            }

            // Validate the expired token and extract claims
            var principal = _jwtService.GetPrincipalFromExpiredToken(request.Token);
            if (principal == null)
            {
                return BadRequest(new { error = "Invalid token" });
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { error = "Invalid token claims" });
            }

            // Find user and validate refresh token
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.SecurityStamp != request.RefreshToken)
            {
                return BadRequest(new { error = "Invalid refresh token" });
            }

            // Check if user is still active
            if (!user.IsActive || user.IsDeleted)
            {
                return BadRequest(new { error = "User account is inactive" });
            }

            // Generate new tokens
            var newJwtToken = _jwtService.GenerateTokenAsync(user, _userManager);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // Update user with new refresh token
            user.SecurityStamp = newRefreshToken;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Token refreshed for user: {UserId}", userId);

            return Ok(new
            {
                token = newJwtToken,
                refreshToken = newRefreshToken,
                expiresAt = DateTime.UtcNow.AddMinutes(60)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token refresh failed");
            return BadRequest(new { error = "Token refresh failed", details = ex.Message });
        }
    }

    /// <summary>
    /// Logs out the current user and invalidates their refresh token
    /// </summary>
    /// <returns>Success message</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.LastLogoutAt = DateTime.UtcNow;
                    user.UpdatedAt = DateTime.UtcNow;
                    user.SecurityStamp = Guid.NewGuid().ToString(); // Invalidate refresh token
                    await _userManager.UpdateAsync(user);

                    _logger.LogInformation("User logged out: {UserId}", userId);
                }
            }

            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout failed for user: {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return BadRequest(new { error = "Logout failed" });
        }
    }

    /// <summary>
    /// Gets the current authenticated user's profile information
    /// </summary>
    /// <returns>User profile data</returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User ID not found in token" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            if (!user.IsActive || user.IsDeleted)
            {
                return BadRequest(new { error = "User account is inactive" });
            }

            var userProfile = new UserProfileDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                DisplayName = user.DisplayName,
                Email = user.Email ?? string.Empty,
                AvatarUrl = user.AvatarUrl,
                DiscordId = user.DiscordId,
                DiscordName = user.DiscordName ?? string.Empty,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };

            return Ok(userProfile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get current user profile");
            return BadRequest(new { error = "Failed to retrieve user profile" });
        }
    }

    /// <summary>
    /// Checks if the current user's Discord token is still valid
    /// </summary>
    /// <returns>Token validity status</returns>
    [HttpGet("discord/status")]
    [Authorize]
    public async Task<IActionResult> GetDiscordStatus()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var isTokenValid = user.DiscordTokenExpiresAt.HasValue && 
                              user.DiscordTokenExpiresAt.Value > DateTime.UtcNow;

            return Ok(new
            {
                discordId = user.DiscordId,
                discordName = user.DiscordName,
                tokenValid = isTokenValid,
                tokenExpiresAt = user.DiscordTokenExpiresAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Discord status");
            return BadRequest(new { error = "Failed to get Discord status" });
        }
    }

    /// <summary>
    /// Generates Discord avatar URL from user information
    /// </summary>
    /// <param name="discordUser">Discord user data</param>
    /// <returns>Avatar URL</returns>
    private static string GetDiscordAvatarUrl(DiscordUser discordUser)
    {
        if (string.IsNullOrEmpty(discordUser.Avatar))
        {
            // Default Discord avatar based on discriminator
            if (int.TryParse(discordUser.Discriminator, out var discriminator))
            {
                return $"https://cdn.discordapp.com/embed/avatars/{discriminator % 5}.png";
            }
            return "https://cdn.discordapp.com/embed/avatars/0.png";
        }

        // Custom avatar - check if it's animated (starts with "a_")
        var format = discordUser.Avatar.StartsWith("a_") ? "gif" : "png";
        return $"https://cdn.discordapp.com/avatars/{discordUser.Id}/{discordUser.Avatar}.{format}?size=256";
    }
}