using MediatR;

namespace Magidesk.Application.Commands;

/// <summary>
/// Downloads the installer from DownloadUrl and launches it silently.
/// The caller is responsible for closing the application afterwards.
/// </summary>
public record ApplyUpdateCommand(
    string DownloadUrl,
    string AssetName
) : IRequest<ApplyUpdateResult>;

public record ApplyUpdateResult(bool Success, string? ErrorMessage);
