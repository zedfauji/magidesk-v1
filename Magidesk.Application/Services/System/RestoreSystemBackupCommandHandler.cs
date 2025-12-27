using Magidesk.Application.Commands;
using Magidesk.Application.Commands.SystemConfig;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Services.SystemConfig;

public class RestoreSystemBackupCommandHandler : ICommandHandler<RestoreSystemBackupCommand, RestoreSystemBackupResult>
{
    private readonly IBackupService _backupService;

    public RestoreSystemBackupCommandHandler(IBackupService backupService)
    {
        _backupService = backupService;
    }

    public async Task<RestoreSystemBackupResult> HandleAsync(RestoreSystemBackupCommand command, CancellationToken cancellationToken = default)
    {
        await _backupService.RestoreBackupAsync(command.FileName, cancellationToken);
        return new RestoreSystemBackupResult(true);
    }
}
