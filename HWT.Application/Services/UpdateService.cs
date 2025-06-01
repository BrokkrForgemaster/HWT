using Octokit;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.RegularExpressions;
using HWT.Application.Interfaces;
using FileMode = System.IO.FileMode;

namespace HWT.Application.Services
{
    /// <summary>
    /// This service checks for updates by looking in the repository's "publish/" directory
    /// (instead of Releases) for a ZIP whose filename encodes a version (e.g. HouseWolfApp-0.1.3.zip).
    /// </summary>
    public class UpdateService : IUpdateService
    {
        #region Fields
        private const string Owner = "BrokkrForgemaster";
        private const string Repo  = "HWT";
        private readonly GitHubClient _github = new(
            new ProductHeaderValue("HouseWolf-App"));
        #endregion

        #region Methods

        /// <summary>
        /// Checks whether a newer ZIP exists under the "publish/" directory by comparing
        /// the version parsed from the ZIP filename to the local executable's FileVersion.
        /// </summary>
        public async Task<bool> IsUpdateAvailableAsync()
        {
            // Naming convention: "HouseWolfApp-X.Y.Z.zip"
            try
            {
                var contents = await _github.Repository.Content.GetAllContents(Owner, Repo, "publish");
                
                var versionedZips = contents
                    .Where(c => c.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    .Select(c =>
                    {
                        var m = Regex.Match(c.Name, @"HouseWolfApp-(\d+\.\d+\.\d+)\.zip", RegexOptions.IgnoreCase);
                        if (m.Success && Version.TryParse(m.Groups[1].Value, out var ver))
                        {
                            return new { Content = c, Version = ver };
                        }
                        return null;
                    })
                    .Where(x => x != null)
                    .Cast<dynamic>()
                    .ToList();

                if (!versionedZips.Any())
                    return false;
                
                var latestZip = versionedZips
                    .OrderByDescending(x => (Version)x.Version)
                    .First();

                var remoteVer = (Version)latestZip.Version;

                var exePath = Environment.ProcessPath;
                if (exePath != null)
                {
                    var fileVersion = FileVersionInfo.GetVersionInfo(exePath).FileVersion;
                    if (fileVersion != null && Version.TryParse(fileVersion, out var localVer))
                    {
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

        /// <summary>
        /// Downloads the latest ZIP from the "publish/" folder (if newer), extracts it
        /// to the application's directory, and reports progress back to the caller.
        /// </summary>
        public async Task<bool> TryUpdateAsync(Action<double, string> reportProgress)
        {
            IReadOnlyList<RepositoryContent> contents;
            try
            {
                contents = await _github.Repository.Content.GetAllContents(Owner, Repo, "publish");
            }
            catch
            {
                return false;
            }

            var versionedZips = contents
                .Where(c => c.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                .Select(c =>
                {
                    var m = Regex.Match(c.Name, @"HouseWolfApp-(\d+\.\d+\.\d+)\.zip", RegexOptions.IgnoreCase);
                    if (m.Success && Version.TryParse(m.Groups[1].Value, out var ver))
                    {
                        return new { Content = c, Version = ver };
                    }
                    return null;
                })
                .Where(x => x != null)
                .Cast<dynamic>()
                .ToList();

            if (!versionedZips.Any())
                return false;

            var latestZip = versionedZips
                .OrderByDescending(x => (Version)x.Version)
                .First();

            var remoteVer = (Version)latestZip.Version;
            
            var processModule = Process.GetCurrentProcess().MainModule;
            if (processModule != null)
            {
                var exePath = processModule.FileName;
                var fileVersion = FileVersionInfo.GetVersionInfo(exePath).FileVersion;
                if (fileVersion != null && Version.TryParse(fileVersion, out var localVer))
                {
                    if (remoteVer <= localVer)
                        return false;
                }

                var downloadUrl = latestZip.Content.BrowserDownloadUrl; 
                var tempZip = Path.Combine(Path.GetTempPath(), latestZip.Content.Name);

                using (var http = new HttpClient())
                using (var resp = await http.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    resp.EnsureSuccessStatusCode();
                    var total = resp.Content.Headers.ContentLength ?? 1L;

                    using var src = await resp.Content.ReadAsStreamAsync();
                    using var dst = new FileStream(tempZip, FileMode.Create, FileAccess.Write, FileShare.None);

                    var buffer = new byte[81920];
                    long downloaded = 0;
                    int read;

                    while ((read = await src.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await dst.WriteAsync(buffer, 0, read);
                        downloaded += read;
                        var percent = downloaded / (double)total * 100.0;
                        reportProgress(percent, $"Downloading {percent:F0}%");
                    }
                }
                
                var installDir = Path.GetDirectoryName(exePath)!;
                ZipFile.ExtractToDirectory(tempZip, installDir, overwriteFiles: true);
                File.Delete(tempZip);
                
                reportProgress(100, "Update complete!");
                return true;
            }

            return false;
        }

        #endregion
    }
}
