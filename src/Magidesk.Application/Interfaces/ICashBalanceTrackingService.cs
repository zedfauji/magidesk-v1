using Magidesk.Application.DTOs;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Service for real-time cash balance tracking and monitoring.
/// Provides live updates of cash drawer status and alerts.
/// </summary>
public interface ICashBalanceTrackingService
{
    /// <summary>
    /// Event raised when cash balance is updated.
    /// </summary>
    event EventHandler<CashBalanceUpdatedEventArgs>? CashBalanceUpdated;

    /// <summary>
    /// Event raised when cash balance alert conditions are met.
    /// </summary>
    event EventHandler<CashBalanceAlertEventArgs>? CashBalanceAlert;

    /// <summary>
    /// Gets the current cash balance for the specified terminal.
    /// </summary>
    /// <param name="terminalId">Terminal ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current cash balance information</returns>
    Task<CashBalanceDto?> GetCurrentCashBalanceAsync(Guid terminalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the cash balance after a transaction.
    /// </summary>
    /// <param name="terminalId">Terminal ID</param>
    /// <param name="amount">Transaction amount (positive for additions, negative for subtractions)</param>
    /// <param name="transactionType">Type of transaction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateCashBalanceAsync(Guid terminalId, decimal amount, CashTransactionType transactionType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts real-time monitoring for the specified terminal.
    /// </summary>
    /// <param name="terminalId">Terminal ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task StartMonitoringAsync(Guid terminalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops real-time monitoring for the specified terminal.
    /// </summary>
    /// <param name="terminalId">Terminal ID</param>
    Task StopMonitoringAsync(Guid terminalId);

    /// <summary>
    /// Configures cash balance alert thresholds.
    /// </summary>
    /// <param name="terminalId">Terminal ID</param>
    /// <param name="lowCashThreshold">Low cash alert threshold</param>
    /// <param name="highCashThreshold">High cash alert threshold</param>
    Task ConfigureAlertsAsync(Guid terminalId, decimal lowCashThreshold, decimal highCashThreshold);
}

/// <summary>
/// Event arguments for cash balance updates.
/// </summary>
public class CashBalanceUpdatedEventArgs : EventArgs
{
    public Guid TerminalId { get; set; }
    public CashBalanceDto Balance { get; set; } = null!;
}

/// <summary>
/// Event arguments for cash balance alerts.
/// </summary>
public class CashBalanceAlertEventArgs : EventArgs
{
    public Guid TerminalId { get; set; }
    public CashBalanceAlertType AlertType { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal Threshold { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Types of cash balance alerts.
/// </summary>
public enum CashBalanceAlertType
{
    LowCash,
    HighCash,
    NegativeBalance,
    SessionNotFound
}

/// <summary>
/// Types of cash transactions for balance tracking.
/// </summary>
public enum CashTransactionType
{
    Sale,
    Refund,
    CashDrop,
    DrawerBleed,
    Opening,
    Closing,
    Adjustment
}