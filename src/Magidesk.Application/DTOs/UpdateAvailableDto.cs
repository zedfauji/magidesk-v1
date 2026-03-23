namespace Magidesk.Application.DTOs;

public record UpdateAvailableDto(
    string LatestVersion,
    string CurrentVersion,
    string ReleaseNotes,
    string DownloadUrl,
    string AssetName,
    DateTimeOffset PublishedAt
);
