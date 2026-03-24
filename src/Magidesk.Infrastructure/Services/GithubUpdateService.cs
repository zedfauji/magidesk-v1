using System.Diagnostics;
using System.Net.Http;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;

namespace Magidesk.Infrastructure.Services;

public class GithubUpdateService : IUpdateService
{
    private readonly UpdateSettings _settings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GithubUpdateService> _logger;

    private readonly GitHubClient _github = new(
        new ProductHeaderValue("Magidesk-POS-Updater"));

    public GithubUpdateService(
        IOptions<UpdateSettings> settings,
        IHttpClientFactory httpClientFactory,
        ILogger<GithubUpdateService> logger)
    {
        _settings = settings.Value;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<UpdateAvailableDto?> CheckForUpdatesAsync(
        string currentVersion,
        CancellationToken cancellationToken = default)
    {
        var latest = await _github.Repository.Release.GetLatest(
            _settings.RepositoryOwner,
            _settings.RepositoryName);

        var latestVersion = latest.TagName.TrimStart('v');

        if (!IsNewer(latestVersion, currentVersion))
            return null;

        var asset = latest.Assets.FirstOrDefault(a =>
            a.Name.Contains(_settings.InstallerAssetNamePattern,
                StringComparison.OrdinalIgnoreCase));

        if (asset is null)
        {
            _logger.LogWarning(
                "Release {Tag} found but no asset matches pattern '{Pattern}'.",
                latest.TagName, _settings.InstallerAssetNamePattern);
            return null;
        }

        return new UpdateAvailableDto(
            LatestVersion: latestVersion,
            CurrentVersion: currentVersion,
            ReleaseNotes: latest.Body ?? string.Empty,
            DownloadUrl: asset.BrowserDownloadUrl,
            AssetName: asset.Name,
            PublishedAt: latest.PublishedAt ?? DateTimeOffset.UtcNow);
    }

    public async Task<string> DownloadInstallerAsync(
        string releaseDownloadUrl,
        IProgress<double> progress,
        CancellationToken cancellationToken = default)
    {
        var fileName = Path.GetFileName(new Uri(releaseDownloadUrl).LocalPath);
        var destPath = Path.Combine(Path.GetTempPath(), fileName);

        using var client = _httpClientFactory.CreateClient("GitHubDownload");
        using var response = await client.GetAsync(
            releaseDownloadUrl,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? -1L;
        var bytesRead = 0L;

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var fileStream = new FileStream(
            destPath, System.IO.FileMode.Create, FileAccess.Write, FileShare.None);

        var buffer = new byte[81920];
        int read;
        while ((read = await stream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            bytesRead += read;
            if (totalBytes > 0)
                progress.Report((double)bytesRead / totalBytes);
        }

        return destPath;
    }

    public void ApplyInstaller(string installerPath)
    {
        var logPath = Path.Combine(Path.GetTempPath(), "magidesk_update.log");

        Process.Start(new ProcessStartInfo
        {
            FileName = "msiexec.exe",
            Arguments = $"/i \"{installerPath}\" /qn /norestart /log \"{logPath}\"",
            UseShellExecute = true
        });
    }

    internal static bool IsNewer(string candidate, string current)
    {
        if (!System.Version.TryParse(candidate.TrimStart('v'), out var candidateV)) return false;
        if (!System.Version.TryParse(current.TrimStart('v'), out var currentV)) return false;
        return candidateV > currentV;
    }
}
