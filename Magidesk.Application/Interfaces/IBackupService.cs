using Magidesk.Application.DTOs.SystemConfig;

namespace Magidesk.Application.Interfaces;

public interface IBackupService
{
    Task<string> CreateBackupAsync(CancellationToken cancellationToken = default);
    Task<List<BackupInfoDto>> ListBackupsAsync(CancellationToken cancellationToken = default);
    Task RestoreBackupAsync(string fileName, CancellationToken cancellationToken = default);
}
