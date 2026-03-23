using MediatR;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Commands;

public class ApplyUpdateCommandHandler
    : IRequestHandler<ApplyUpdateCommand, ApplyUpdateResult>
{
    private readonly IUpdateService _updateService;
    private readonly ILogger<ApplyUpdateCommandHandler> _logger;

    public ApplyUpdateCommandHandler(
        IUpdateService updateService,
        ILogger<ApplyUpdateCommandHandler> logger)
    {
        _updateService = updateService;
        _logger = logger;
    }

    public async Task<ApplyUpdateResult> Handle(
        ApplyUpdateCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var progress = new Progress<double>();  // ViewModel wires its own progress
            var installerPath = await _updateService.DownloadInstallerAsync(
                request.DownloadUrl, progress, cancellationToken);

            _updateService.ApplyInstaller(installerPath);

            return new ApplyUpdateResult(true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply update from {Url}", request.DownloadUrl);
            return new ApplyUpdateResult(false, ex.Message);
        }
    }
}
