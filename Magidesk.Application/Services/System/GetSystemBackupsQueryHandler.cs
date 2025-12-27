using Magidesk.Application.DTOs.SystemConfig;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Application.Queries.SystemConfig;

namespace Magidesk.Application.Services.SystemConfig;

public class GetSystemBackupsQueryHandler : IQueryHandler<GetSystemBackupsQuery, GetSystemBackupsResult>
{
    private readonly IBackupService _backupService;

    public GetSystemBackupsQueryHandler(IBackupService backupService)
    {
        _backupService = backupService;
    }

    public async Task<GetSystemBackupsResult> HandleAsync(GetSystemBackupsQuery query, CancellationToken cancellationToken = default)
    {
        var backups = await _backupService.ListBackupsAsync(cancellationToken);
        return new GetSystemBackupsResult(backups);
    }
}
