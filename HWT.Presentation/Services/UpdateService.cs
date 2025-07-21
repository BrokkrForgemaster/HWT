using Squirrel;
using HWT.Application.Interfaces;
using Microsoft.Extensions.Logging;

public class UpdateService : IUpdateService
{
    private readonly ILogger<UpdateService> _logger;

    public UpdateService(ILogger<UpdateService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> IsUpdateAvailableAsync()
    {
        try
        {
            using var mgr = await UpdateManager.GitHubUpdateManager(
                repoUrl: "https://github.com/BrokkrForgemaster/HWT");

            var updateInfo = await mgr.CheckForUpdate();
            return updateInfo.ReleasesToApply.Count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check for updates.");
            return false;
        }
    }

    public async Task ApplyUpdateAsync(Action<double, string>? reportProgress = null)
    {
        try
        {
            using var mgr = await UpdateManager.GitHubUpdateManager(
                repoUrl: "https://github.com/BrokkrForgemaster/HWT");

            await mgr.UpdateApp();
            UpdateManager.RestartApp();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply update.");
        }
    }
}