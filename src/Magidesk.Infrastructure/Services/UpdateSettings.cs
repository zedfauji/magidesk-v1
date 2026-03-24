namespace Magidesk.Infrastructure.Services;

public class UpdateSettings
{
    public const string SectionName = "UpdateSettings";

    public string RepositoryOwner { get; set; } = "zedfauji";
    public string RepositoryName { get; set; } = "magidesk-v1";

    /// <summary>
    /// Name pattern used to identify the MSI asset in a GitHub Release.
    /// Supports simple substring match (case-insensitive).
    /// </summary>
    public string InstallerAssetNamePattern { get; set; } = "Magidesk-Setup";

    /// <summary>How often the background service checks, in hours.</summary>
    public int CheckIntervalHours { get; set; } = 4;
}
