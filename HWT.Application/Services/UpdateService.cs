using Octokit;
using System.Diagnostics;
using System.IO.Compression;
using HWT.Application.Interfaces;

namespace HWT.Application.Services;

/// <summary name="UpdateService">
/// This service checks for updates to the application
/// and performs the update if available. It uses the GitHub Releases API
/// to check for the latest release and download it if necessary.
/// </summary>
public class UpdateService : IUpdateService
{
    #region Fields
    private const string Owner = "BrokkrForgemaster";
    private const string Repo = "HWT";
    private readonly GitHubClient _github = new(
        new ProductHeaderValue("HouseWolf-App"));
    #endregion

    #region Methods
    /// <summary name="IsUpdateAvailableAsync">
    /// Checks if an update is available for the application,
    /// comparing the local version with the latest release on GitHub.
    /// <returns> True if an update is available, and the remote version
    /// is greater than the local version. Otherwise, false.</returns>
    /// <exception cref="Exception"> Thrown when there is an error with the GitHub API,
    /// such as network issues or invalid responses.</exception>
    /// <remarks> This method retrieves the latest release from the GitHub repository,
    /// parses the version from the release tag, and compares it with the local
    /// version of the application. If the remote version is greater than the local version,
    /// it returns true, indicating that an update is available.
    /// If the remote version is less than or equal to the local version, it returns false.
    /// If there is an error while fetching the release information, it returns false.</remarks>
    /// </summary>
    public async Task<bool> IsUpdateAvailableAsync()
    {
        try
        {
            var release = await _github.Repository.Release.GetLatest(Owner, Repo);
            var remoteVer = new Version(release.TagName.TrimStart('v'));

            var exePath = Environment.ProcessPath;
            if (exePath != null)
            {
                var fileVersion = FileVersionInfo.GetVersionInfo(exePath).FileVersion;
                if (fileVersion != null)
                {
                    var localVer = new Version(fileVersion);

                    return remoteVer > localVer;
                }
            }
        }
        catch
        {
            return false;
        }
        return false;
    }

    /// <summary name="TryUpdateAsync">
    /// Attempts to update the application by downloading the latest release
    /// from GitHub and extracting it to the application's directory.
    /// <param name="reportProgress">An action to report progress during the update process.</param>
    /// <returns>True if the update was successful, otherwise false.</returns>
    /// <remarks> This method retrieves the latest release from the GitHub repository,
    /// downloads the release asset (expected to be a ZIP file),
    /// and extracts it to the application's directory. It reports progress
    /// during the download process using the provided `reportProgress` action. 
    /// </remarks>
    /// </summary>
    public async Task<bool> TryUpdateAsync(Action<double, string> reportProgress)
    {
        Release release;
        try
        {
            release = await _github.Repository.Release.GetLatest(Owner, Repo);
        }
        catch
        {
            return false;
        }

        var asset = release.Assets.FirstOrDefault(a => a.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase));
        if (asset == null)
            return false;

        var remoteVer = new Version(release.TagName.TrimStart('v'));
        var processModule = Process.GetCurrentProcess().MainModule;
        if (processModule != null)
        {
            var exePath = processModule.FileName;
            var fileVersion = FileVersionInfo.GetVersionInfo(exePath).FileVersion;
            if (fileVersion != null)
            {
                var localVer = new Version(fileVersion);
                if (remoteVer <= localVer)
                    return false;
            }

            var tempZip = Path.Combine(Path.GetTempPath(), asset.Name);
            using var http = new HttpClient();
            using var resp = await http.GetAsync(asset.BrowserDownloadUrl, HttpCompletionOption.ResponseHeadersRead);
            resp.EnsureSuccessStatusCode();

            var total = resp.Content.Headers.ContentLength ?? 1L;
            using var src = await resp.Content.ReadAsStreamAsync();
            using var dst = new FileStream(tempZip, System.IO.FileMode.Create, FileAccess.Write, FileShare.None);

            var buffer = new byte[81920];
            long downloaded = 0;
            int read;
            while ((read = await src.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await dst.WriteAsync(buffer, 0, read);
                downloaded += read;
                reportProgress(downloaded / (double)total * 100, $"Downloading {downloaded * 100 / total:F0}%");
            }

            var installDir = Path.GetDirectoryName(exePath)!;
            ZipFile.ExtractToDirectory(tempZip, installDir, overwriteFiles: true);
            File.Delete(tempZip);
        }

        reportProgress(100, "Update complete!");
        return true;
    }
    #endregion
}