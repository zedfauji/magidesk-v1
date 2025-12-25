using Magidesk.Domain.Entities;
using Magidesk.Domain.Exceptions;

namespace Magidesk.Domain.DomainServices;

/// <summary>
/// Domain service for cash session operations.
/// Stateless service that coordinates cash session business logic.
/// </summary>
public class CashSessionDomainService
{
    /// <summary>
    /// Calculates the expected cash amount for a cash session.
    /// ExpectedCash = OpeningBalance + CashReceipts - CashRefunds - Payouts - CashDrops - Bleeds
    /// </summary>
    public void CalculateExpectedCash(CashSession cashSession)
    {
        if (cashSession == null)
        {
            throw new ArgumentNullException(nameof(cashSession));
        }

        cashSession.CalculateExpectedCash();
    }

    /// <summary>
    /// Validates if a cash session can be closed.
    /// </summary>
    public bool CanCloseSession(CashSession cashSession)
    {
        if (cashSession == null)
        {
            throw new ArgumentNullException(nameof(cashSession));
        }

        return cashSession.CanClose();
    }
}

