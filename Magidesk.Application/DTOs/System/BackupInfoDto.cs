namespace Magidesk.Application.DTOs.SystemConfig;

public class BackupInfoDto
{
    public string FileName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public long SizeBytes { get; set; }
    public string Path { get; set; } = string.Empty;
}
