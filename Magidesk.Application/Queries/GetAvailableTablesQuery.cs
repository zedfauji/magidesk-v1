using Magidesk.Application.DTOs;

namespace Magidesk.Application.Queries;

/// <summary>
/// Query to get all available tables.
/// </summary>
public class GetAvailableTablesQuery
{
    public Guid? FloorId { get; set; }
}

/// <summary>
/// Result of getting available tables.
/// </summary>
public class GetAvailableTablesResult
{
    public List<TableDto> Tables { get; set; } = new();
}

