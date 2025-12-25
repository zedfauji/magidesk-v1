using System;
using System.Collections.Generic;
using System.Linq;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Represents a cash drawer session (explicit open/close).
/// Aggregate root for cash management.
/// </summary>
public class CashSession
{
    private readonly List<Payment> _payments = new();
    private readonly List<Payout> _payouts = new();
    private readonly List<CashDrop> _cashDrops = new();
    private readonly List<DrawerBleed> _drawerBleeds = new();

    public Guid Id { get; private set; }
    public UserId UserId { get; private set; } = null!;
    public Guid TerminalId { get; private set; }
    public Guid ShiftId { get; private set; }
    public DateTime OpenedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public UserId? ClosedBy { get; private set; }
    public Money OpeningBalance { get; private set; }
    public Money ExpectedCash { get; private set; }
    public Money? ActualCash { get; private set; }
    public Money? Difference { get; private set; }
    public CashSessionStatus Status { get; private set; }
    public int Version { get; private set; }

    // Collections
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();
    public IReadOnlyCollection<Payout> Payouts => _payouts.AsReadOnly();
    public IReadOnlyCollection<CashDrop> CashDrops => _cashDrops.AsReadOnly();
    public IReadOnlyCollection<DrawerBleed> DrawerBleeds => _drawerBleeds.AsReadOnly();

    // Private constructor for EF Core
    private CashSession()
    {
        OpeningBalance = Money.Zero();
        ExpectedCash = Money.Zero();
        Status = CashSessionStatus.Open;
    }

    /// <summary>
    /// Opens a new cash session.
    /// </summary>
    public static CashSession Open(
        UserId userId,
        Guid terminalId,
        Guid shiftId,
        Money openingBalance)
    {
        if (openingBalance < Money.Zero())
        {
            throw new BusinessRuleViolationException("Opening balance cannot be negative.");
        }

        var session = new CashSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TerminalId = terminalId,
            ShiftId = shiftId,
            OpenedAt = DateTime.UtcNow,
            OpeningBalance = openingBalance,
            Status = CashSessionStatus.Open,
            Version = 1
        };

        session.CalculateExpectedCash();
        return session;
    }

    /// <summary>
    /// Closes the cash session.
    /// </summary>
    public void Close(UserId closedBy, Money actualCash)
    {
        if (Status == CashSessionStatus.Closed)
        {
            throw new Exceptions.InvalidOperationException("Cash session is already closed.");
        }

        if (actualCash < Money.Zero())
        {
            throw new BusinessRuleViolationException("Actual cash cannot be negative.");
        }

        Status = CashSessionStatus.Closed;
        ClosedAt = DateTime.UtcNow;
        ClosedBy = closedBy;
        ActualCash = actualCash;
        CalculateExpectedCash();
        Difference = ActualCash - ExpectedCash;
    }

    /// <summary>
    /// Calculates the expected cash amount.
    /// ExpectedCash = OpeningBalance + CashReceipts - CashRefunds - Payouts - CashDrops - Bleeds
    /// </summary>
    public void CalculateExpectedCash()
    {
        var cashReceipts = _payments
            .Where(p => p.PaymentType == PaymentType.Cash && !p.IsVoided)
            .Aggregate(Money.Zero(), (sum, p) => sum + p.Amount);

        var cashRefunds = _payments
            .Where(p => p.PaymentType == PaymentType.Cash && p.IsVoided && p.TransactionType == TransactionType.Debit)
            .Aggregate(Money.Zero(), (sum, p) => sum + p.Amount);

        var payouts = _payouts.Aggregate(Money.Zero(), (sum, p) => sum + p.Amount);
        var cashDrops = _cashDrops.Aggregate(Money.Zero(), (sum, d) => sum + d.Amount);
        var bleeds = _drawerBleeds.Aggregate(Money.Zero(), (sum, b) => sum + b.Amount);

        ExpectedCash = OpeningBalance + cashReceipts - cashRefunds - payouts - cashDrops - bleeds;
    }

    /// <summary>
    /// Checks if the session can be closed.
    /// </summary>
    public bool CanClose()
    {
        return Status == CashSessionStatus.Open;
    }

    /// <summary>
    /// Adds a cash payment to the session.
    /// </summary>
    public void AddPayment(Payment payment)
    {
        if (payment == null)
        {
            throw new ArgumentNullException(nameof(payment));
        }

        if (Status == CashSessionStatus.Closed)
        {
            throw new Exceptions.InvalidOperationException("Cannot add payment to closed session.");
        }

        if (payment.PaymentType != PaymentType.Cash)
        {
            throw new BusinessRuleViolationException("Only cash payments can be added to cash session.");
        }

        if (payment.CashSessionId != Id)
        {
            throw new BusinessRuleViolationException("Payment does not belong to this cash session.");
        }

        _payments.Add(payment);
        CalculateExpectedCash();
    }

    /// <summary>
    /// Adds a payout to the session.
    /// </summary>
    public void AddPayout(Payout payout)
    {
        if (payout == null)
        {
            throw new ArgumentNullException(nameof(payout));
        }

        if (Status == CashSessionStatus.Closed)
        {
            throw new Exceptions.InvalidOperationException("Cannot add payout to closed session.");
        }

        if (payout.CashSessionId != Id)
        {
            throw new BusinessRuleViolationException("Payout does not belong to this cash session.");
        }

        _payouts.Add(payout);
        CalculateExpectedCash();
    }

    /// <summary>
    /// Adds a cash drop to the session.
    /// </summary>
    public void AddCashDrop(CashDrop cashDrop)
    {
        if (cashDrop == null)
        {
            throw new ArgumentNullException(nameof(cashDrop));
        }

        if (Status == CashSessionStatus.Closed)
        {
            throw new Exceptions.InvalidOperationException("Cannot add cash drop to closed session.");
        }

        if (cashDrop.CashSessionId != Id)
        {
            throw new BusinessRuleViolationException("Cash drop does not belong to this cash session.");
        }

        _cashDrops.Add(cashDrop);
        CalculateExpectedCash();
    }

    /// <summary>
    /// Adds a drawer bleed to the session.
    /// </summary>
    public void AddDrawerBleed(DrawerBleed drawerBleed)
    {
        if (drawerBleed == null)
        {
            throw new ArgumentNullException(nameof(drawerBleed));
        }

        if (Status == CashSessionStatus.Closed)
        {
            throw new Exceptions.InvalidOperationException("Cannot add drawer bleed to closed session.");
        }

        if (drawerBleed.CashSessionId != Id)
        {
            throw new BusinessRuleViolationException("Drawer bleed does not belong to this cash session.");
        }

        _drawerBleeds.Add(drawerBleed);
        CalculateExpectedCash();
    }
}

