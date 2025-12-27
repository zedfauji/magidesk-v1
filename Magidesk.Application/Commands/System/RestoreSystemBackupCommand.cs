
namespace Magidesk.Application.Commands.SystemConfig;

public class RestoreSystemBackupCommand
{
    public string FileName { get; }

    public RestoreSystemBackupCommand(string fileName)
    {
        FileName = fileName;
    }
}

public class RestoreSystemBackupResult
{
    public bool Success { get; }

    public RestoreSystemBackupResult(bool success)
    {
        Success = success;
    }
}
