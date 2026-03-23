using MediatR;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Queries;

public class CheckForUpdatesQueryHandler
    : IRequestHandler<CheckForUpdatesQuery, UpdateAvailableDto?>
{
    private readonly IUpdateService _updateService;
    private readonly ILogger<CheckForUpdatesQueryHandler> _logger;

    public CheckForUpdatesQueryHandler(
        IUpdateService updateService,
        ILogger<CheckForUpdatesQueryHandler> logger)
    {
        _updateService = updateService;
        _logger = logger;
    }

    public async Task<UpdateAvailableDto?> Handle(
        CheckForUpdatesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _updateService.CheckForUpdatesAsync(
                request.CurrentVersion, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Update check failed — will retry next cycle.");
            return null;
        }
    }
}
