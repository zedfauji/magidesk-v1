using Magidesk.Application.DTOs.SystemConfig;


namespace Magidesk.Application.Queries.SystemConfig;

public class GetSystemBackupsQuery
{
}

public class GetSystemBackupsResult
{
    public List<BackupInfoDto> Backups { get; }

    public GetSystemBackupsResult(List<BackupInfoDto> backups)
    {
        Backups = backups;
    }
}
