using Magidesk.Application.Commands;
using Magidesk.Application.Commands.SystemConfig;
using Magidesk.Application.Interfaces;

namespace Magidesk.Application.Services.SystemConfig;

public class CreateSystemBackupCommandHandler : ICommandHandler<CreateSystemBackupCommand, CreateSystemBackupResult>
{
    private readonly IBackupService _backupService;

    public CreateSystemBackupCommandHandler(IBackupService backupService)
    {
        _backupService = backupService;
    }

    public async Task<CreateSystemBackupResult> HandleAsync(CreateSystemBackupCommand command, CancellationToken cancellationToken = default)
    {
        var fileName = await _backupService.CreateBackupAsync(cancellationToken);
        return new CreateSystemBackupResult(fileName);
    }
}
