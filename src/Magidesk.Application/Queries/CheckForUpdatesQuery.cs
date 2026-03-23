using MediatR;
using Magidesk.Application.DTOs;

namespace Magidesk.Application.Queries;

/// <summary>
/// Returns UpdateAvailableDto if a newer version exists on GitHub,
/// or null if the app is already up to date or the check fails.
/// </summary>
public record CheckForUpdatesQuery(string CurrentVersion) : IRequest<UpdateAvailableDto?>;
