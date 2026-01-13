using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Magidesk.Infrastructure.Services;

/// <summary>
/// Service for real-time cash balance tracking and monitoring.
/// Provides live updates and alerts for cash drawer operations.
/// </summary>
public class CashBalanceTrackingService : ICashBalanceTrackingService, IDisposable
{
    private readonly ICashSessionRepository _cashSessionRepository;
    private readonly ILogger<CashBalanceTrackingService> _logger;
    private readonly ConcurrentDictionary<Guid, CashBalanceDto> _balanceCache = new();
    private readonly ConcurrentDictionary<Guid, Timer> _monitoringTimers = new();
    private readonly ConcurrentDictionary<Guid, (decimal Low, decimal High)> _alertThresholds = new();
    private bool _disposed = false;

    public event EventHandler<CashBalanceUpdatedEventArgs>? CashBalanceUpdated;
    public event EventHandler<CashBalanceAlertEventArgs>? CashBalanceAlert;

    public CashBalanceTrackingService(
        ICashSessionRepository cashSessionRepository,
        ILogger<CashBalanceTrackingService> logger)
    {
        _cashSessionRepository = cashSessionRepository;
        _logger = logger;
    }

    public async Task<CashBalanceDto?> GetCurrentCashBalanceAsync(Guid terminalId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check cache first
            if (_balanceCache.TryGetValue(terminalId, out var cachedBalance))
            {
                // Return cached balance if it's recent (within 30 seconds)
                if (DateTime.UtcNow - cachedBalance.LastUpdated < TimeSpan.FromSeconds(30))
                {
                    return cachedBalance;
                }
            }

            // Fetch from database
            var session = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
            if (session == null)
            {
                return null;
            }

            var balance = CreateBalanceDto(session, terminalId);
            
            // Update cache
            _balanceCache.AddOrUpdate(terminalId, balance, (key, oldValue) => balance);

            return balance;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get current cash balance for terminal {TerminalId}", terminalId);
            return null;
        }
    }

    public async Task UpdateCashBalanceAsync(Guid terminalId, decimal amount, CashTransactionType transactionType, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentBalance = await GetCurrentCashBalanceAsync(terminalId, cancellationToken);
            if (currentBalance == null)
            {
                _logger.LogWarning("Cannot update cash balance - no active session for terminal {TerminalId}", terminalId);
                
                // Raise alert for session not found
                CashBalanceAlert?.Invoke(this, new CashBalanceAlertEventArgs
                {
                    TerminalId = terminalId,
                    AlertType = CashBalanceAlertType.SessionNotFound,
                    CurrentBalance = 0,
                    Threshold = 0,
                    Message = "No active cash session found for terminal"
                });
                
                return;
            }

            // Update balance based on transaction type
            switch (transactionType)
            {
                case CashTransactionType.Sale:
                    currentBalance.TotalCashSales += amount;
                    currentBalance.CurrentBalance += amount;
                    break;
                case CashTransactionType.Refund:
                    currentBalance.TotalCashRefunds += amount;
                    currentBalance.CurrentBalance -= amount;
                    break;
                case CashTransactionType.CashDrop:
                    currentBalance.TotalCashDrops += amount;
                    currentBalance.CurrentBalance -= amount;
                    break;
                case CashTransactionType.DrawerBleed:
                    currentBalance.TotalDrawerBleeds += amount;
                    currentBalance.CurrentBalance -= amount;
                    break;
                case CashTransactionType.Adjustment:
                    currentBalance.CurrentBalance += amount;
                    break;
            }

            currentBalance.LastUpdated = DateTime.UtcNow;

            // Update cache
            _balanceCache.AddOrUpdate(terminalId, currentBalance, (key, oldValue) => currentBalance);

            // Raise balance updated event
            CashBalanceUpdated?.Invoke(this, new CashBalanceUpdatedEventArgs
            {
                TerminalId = terminalId,
                Balance = currentBalance
            });

            // Check for alerts
            await CheckAlertsAsync(terminalId, currentBalance);

            _logger.LogInformation("Cash balance updated for terminal {TerminalId}: {TransactionType} {Amount:C}, New Balance: {Balance:C}",
                terminalId, transactionType, amount, currentBalance.CurrentBalance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update cash balance for terminal {TerminalId}", terminalId);
        }
    }

    public async Task StartMonitoringAsync(Guid terminalId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Stop existing monitoring if any
            await StopMonitoringAsync(terminalId);

            // Start periodic monitoring (every 60 seconds)
            var timer = new Timer(async _ => await RefreshBalanceAsync(terminalId), null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
            _monitoringTimers.TryAdd(terminalId, timer);

            _logger.LogInformation("Started cash balance monitoring for terminal {TerminalId}", terminalId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start monitoring for terminal {TerminalId}", terminalId);
        }
    }

    public async Task StopMonitoringAsync(Guid terminalId)
    {
        try
        {
            if (_monitoringTimers.TryRemove(terminalId, out var timer))
            {
                timer.Dispose();
                _logger.LogInformation("Stopped cash balance monitoring for terminal {TerminalId}", terminalId);
            }

            // Remove from cache
            _balanceCache.TryRemove(terminalId, out _);
            _alertThresholds.TryRemove(terminalId, out _);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop monitoring for terminal {TerminalId}", terminalId);
        }
    }

    public async Task ConfigureAlertsAsync(Guid terminalId, decimal lowCashThreshold, decimal highCashThreshold)
    {
        try
        {
            _alertThresholds.AddOrUpdate(terminalId, (lowCashThreshold, highCashThreshold), 
                (key, oldValue) => (lowCashThreshold, highCashThreshold));

            // Update cached balance with new thresholds
            if (_balanceCache.TryGetValue(terminalId, out var balance))
            {
                balance.LowCashThreshold = lowCashThreshold;
                balance.HighCashThreshold = highCashThreshold;
                
                // Check alerts with new thresholds
                await CheckAlertsAsync(terminalId, balance);
            }

            _logger.LogInformation("Configured cash balance alerts for terminal {TerminalId}: Low={LowThreshold:C}, High={HighThreshold:C}",
                terminalId, lowCashThreshold, highCashThreshold);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to configure alerts for terminal {TerminalId}", terminalId);
        }
    }

    private async Task RefreshBalanceAsync(Guid terminalId)
    {
        try
        {
            // Force refresh from database
            _balanceCache.TryRemove(terminalId, out _);
            var balance = await GetCurrentCashBalanceAsync(terminalId);
            
            if (balance != null)
            {
                // Apply alert thresholds if configured
                if (_alertThresholds.TryGetValue(terminalId, out var thresholds))
                {
                    balance.LowCashThreshold = thresholds.Low;
                    balance.HighCashThreshold = thresholds.High;
                }

                // Raise balance updated event
                CashBalanceUpdated?.Invoke(this, new CashBalanceUpdatedEventArgs
                {
                    TerminalId = terminalId,
                    Balance = balance
                });

                // Check for alerts
                await CheckAlertsAsync(terminalId, balance);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh balance for terminal {TerminalId}", terminalId);
        }
    }

    private async Task CheckAlertsAsync(Guid terminalId, CashBalanceDto balance)
    {
        try
        {
            // Check for negative balance
            if (balance.CurrentBalance < 0)
            {
                CashBalanceAlert?.Invoke(this, new CashBalanceAlertEventArgs
                {
                    TerminalId = terminalId,
                    AlertType = CashBalanceAlertType.NegativeBalance,
                    CurrentBalance = balance.CurrentBalance,
                    Threshold = 0,
                    Message = $"Cash drawer has negative balance: {balance.CurrentBalance:C}"
                });
            }

            // Check for low cash
            if (balance.IsLowCash)
            {
                CashBalanceAlert?.Invoke(this, new CashBalanceAlertEventArgs
                {
                    TerminalId = terminalId,
                    AlertType = CashBalanceAlertType.LowCash,
                    CurrentBalance = balance.CurrentBalance,
                    Threshold = balance.LowCashThreshold!.Value,
                    Message = $"Cash drawer is low: {balance.CurrentBalance:C} (threshold: {balance.LowCashThreshold:C})"
                });
            }

            // Check for high cash
            if (balance.IsHighCash)
            {
                CashBalanceAlert?.Invoke(this, new CashBalanceAlertEventArgs
                {
                    TerminalId = terminalId,
                    AlertType = CashBalanceAlertType.HighCash,
                    CurrentBalance = balance.CurrentBalance,
                    Threshold = balance.HighCashThreshold!.Value,
                    Message = $"Cash drawer is high: {balance.CurrentBalance:C} (threshold: {balance.HighCashThreshold:C})"
                });
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check alerts for terminal {TerminalId}", terminalId);
        }
    }

    private CashBalanceDto CreateBalanceDto(Domain.Entities.CashSession session, Guid terminalId)
    {
        var totalCashSales = session.Payments?.Where(p => p.PaymentType == Domain.Enumerations.PaymentType.Cash).Sum(p => p.Amount.Amount) ?? 0m;
        var totalCashDrops = session.CashDrops?.Sum(d => d.Amount.Amount) ?? 0m;
        var totalDrawerBleeds = session.DrawerBleeds?.Sum(b => b.Amount.Amount) ?? 0m;
        var totalCashRefunds = 0m; // TODO: Calculate from refund payments

        return new CashBalanceDto
        {
            CashSessionId = session.Id,
            TerminalId = terminalId,
            OpeningBalance = session.OpeningBalance.Amount,
            TotalCashSales = totalCashSales,
            TotalCashDrops = totalCashDrops,
            TotalDrawerBleeds = totalDrawerBleeds,
            TotalCashRefunds = totalCashRefunds,
            CurrentBalance = session.OpeningBalance.Amount + totalCashSales - totalCashDrops - totalDrawerBleeds + totalCashRefunds,
            LastUpdated = DateTime.UtcNow,
            IsActive = !session.ClosedAt.HasValue
        };
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            foreach (var timer in _monitoringTimers.Values)
            {
                timer.Dispose();
            }
            _monitoringTimers.Clear();
            _balanceCache.Clear();
            _alertThresholds.Clear();
            _disposed = true;
        }
    }
}