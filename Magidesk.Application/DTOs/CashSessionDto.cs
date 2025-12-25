using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.DTOs;

/// <summary>
/// Data transfer object for CashSession entity.
/// </summary>
public class CashSessionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TerminalId { get; set; }
    public Guid ShiftId { get; set; }
    public DateTime OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public Guid? ClosedBy { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal ExpectedCash { get; set; }
    public decimal? ActualCash { get; set; }
    public decimal? Difference { get; set; }
    public CashSessionStatus Status { get; set; }
    public int Version { get; set; }
}

