using Magidesk.Application.DTOs;

namespace Magidesk.Application.Queries;

/// <summary>
/// Query to get a table by ID.
/// </summary>
public class GetTableQuery
{
    public Guid TableId { get; set; }
}

/// <summary>
/// Result of getting a table.
/// </summary>
public class GetTableResult
{
    public TableDto? Table { get; set; }
}

