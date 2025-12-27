
namespace Magidesk.Application.Commands.SystemConfig;

public class CreateSystemBackupCommand
{
}

public class CreateSystemBackupResult
{
    public string FileName { get; }

    public CreateSystemBackupResult(string fileName)
    {
        FileName = fileName;
    }
}
