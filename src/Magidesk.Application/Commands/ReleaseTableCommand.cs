namespace Magidesk.Application.Commands;

/// <summary>
/// Command to release a table from its ticket.
/// </summary>
public class ReleaseTableCommand
{
    public Guid TableId { get; set; }
}

/// <summary>
/// Result of releasing a table.
/// </summary>
public class ReleaseTableResult
{
    public Guid TableId { get; set; }
}

