namespace Magidesk.Application.DTOs;

/// <summary>
/// Data transfer object for cash balance information.
/// Contains current balance and running totals for cash drawer operations.
/// </summary>
public class CashBalanceDto
{
    /// <summary>
    /// Cash session ID.
    /// </summary>
    public Guid CashSessionId { get; set; }

    /// <summary>
    /// Terminal ID.
    /// </summary>
    public Guid TerminalId { get; set; }

    /// <summary>
    /// Current cash balance in the drawer.
    /// </summary>
    public decimal CurrentBalance { get; set; }

    /// <summary>
    /// Opening balance for the session.
    /// </summary>
    public decimal OpeningBalance { get; set; }

    /// <summary>
    /// Total cash sales for the session.
    /// </summary>
    public decimal TotalCashSales { get; set; }

    /// <summary>
    /// Total cash drops for the session.
    /// </summary>
    public decimal TotalCashDrops { get; set; }

    /// <summary>
    /// Total drawer bleeds for the session.
    /// </summary>
    public decimal TotalDrawerBleeds { get; set; }

    /// <summary>
    /// Total refunds processed in cash.
    /// </summary>
    public decimal TotalCashRefunds { get; set; }

    /// <summary>
    /// Expected balance based on transactions.
    /// </summary>
    public decimal ExpectedBalance => OpeningBalance + TotalCashSales - TotalCashDrops - TotalDrawerBleeds + TotalCashRefunds;

    /// <summary>
    /// Variance between current and expected balance.
    /// </summary>
    public decimal Variance => CurrentBalance - ExpectedBalance;

    /// <summary>
    /// Whether the balance has a variance.
    /// </summary>
    public bool HasVariance => Math.Abs(Variance) > 0.01m;

    /// <summary>
    /// Last updated timestamp.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Whether the session is currently active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Low cash alert threshold.
    /// </summary>
    public decimal? LowCashThreshold { get; set; }

    /// <summary>
    /// High cash alert threshold.
    /// </summary>
    public decimal? HighCashThreshold { get; set; }

    /// <summary>
    /// Whether current balance is below low cash threshold.
    /// </summary>
    public bool IsLowCash => LowCashThreshold.HasValue && CurrentBalance < LowCashThreshold.Value;

    /// <summary>
    /// Whether current balance is above high cash threshold.
    /// </summary>
    public bool IsHighCash => HighCashThreshold.HasValue && CurrentBalance > HighCashThreshold.Value;

    /// <summary>
    /// Balance status description.
    /// </summary>
    public string BalanceStatus
    {
        get
        {
            if (IsLowCash) return "LOW CASH";
            if (IsHighCash) return "HIGH CASH";
            if (CurrentBalance < 0) return "NEGATIVE";
            return "NORMAL";
        }
    }
}