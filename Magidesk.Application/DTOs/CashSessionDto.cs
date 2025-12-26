using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.DTOs;

/// <summary>
/// DTO for CashSession entity.
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
    public List<PayoutDto> Payouts { get; set; } = new();
    public List<CashDropDto> CashDrops { get; set; } = new();
    public List<DrawerBleedDto> DrawerBleeds { get; set; } = new();
    public int PaymentCount { get; set; }
    public decimal TotalCashReceipts { get; set; }
    public decimal TotalCashRefunds { get; set; }
}

/// <summary>
/// DTO for Payout entity.
/// </summary>
public class PayoutDto
{
    public Guid Id { get; set; }
    public Guid CashSessionId { get; set; }
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public Guid ProcessedBy { get; set; }
    public DateTime ProcessedAt { get; set; }
}

/// <summary>
/// DTO for CashDrop entity.
/// </summary>
public class CashDropDto
{
    public Guid Id { get; set; }
    public Guid CashSessionId { get; set; }
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public Guid ProcessedBy { get; set; }
    public DateTime ProcessedAt { get; set; }
}

/// <summary>
/// DTO for DrawerBleed entity.
/// </summary>
public class DrawerBleedDto
{
    public Guid Id { get; set; }
    public Guid CashSessionId { get; set; }
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public Guid ProcessedBy { get; set; }
    public DateTime ProcessedAt { get; set; }
}

/// <summary>
/// DTO for drawer pull report.
/// </summary>
public class DrawerPullReportDto
{
    public Guid CashSessionId { get; set; }
    public Guid UserId { get; set; }
    public DateTime OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal ExpectedCash { get; set; }
    public decimal? ActualCash { get; set; }
    public decimal? Difference { get; set; }
    public decimal TotalCashReceipts { get; set; }
    public decimal TotalCashRefunds { get; set; }
    public decimal TotalPayouts { get; set; }
    public decimal TotalCashDrops { get; set; }
    public decimal TotalDrawerBleeds { get; set; }
    public List<PayoutDto> Payouts { get; set; } = new();
    public List<CashDropDto> CashDrops { get; set; } = new();
    public List<DrawerBleedDto> DrawerBleeds { get; set; } = new();
    public int CashPaymentCount { get; set; }
}

