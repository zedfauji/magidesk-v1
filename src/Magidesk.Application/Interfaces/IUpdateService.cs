using Magidesk.Application.DTOs;

namespace Magidesk.Application.Interfaces;

public interface IUpdateService
{
    /// <summary>
    /// Checks GitHub Releases for a version newer than currentVersion.
    /// Returns null if already up to date or check fails (log, never throw).
    /// </summary>
    Task<UpdateAvailableDto?> CheckForUpdatesAsync(
        string currentVersion,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the MSI asset from releaseDownloadUrl to a temp file.
    /// Reports download progress (0.0–1.0) via the progress callback.
    /// Returns the local path to the downloaded MSI.
    /// </summary>
    Task<string> DownloadInstallerAsync(
        string releaseDownloadUrl,
        IProgress<double> progress,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Launches the downloaded MSI silently via msiexec.
    /// Does NOT wait for installation to complete — returns immediately.
    /// The application should close itself after calling this.
    /// </summary>
    void ApplyInstaller(string installerPath);
}
