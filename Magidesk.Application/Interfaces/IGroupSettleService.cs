using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Interfaces;

public interface IGroupSettleService
{
    /// <summary>
    /// Settles a group of tickets with a single payment method.
    /// Distributes payments to each ticket to satisfy their balances.
    /// </summary>
    /// <param name="ticketIds">List of Ticket IDs to settle.</param>
    /// <param name="paymentType">The payment method (Cash, CreditCard, etc).</param>
    /// <param name="totalAmount">The total amount collected (should match sum of tickets).</param>
    /// <param name="processedBy">User performing the settlement.</param>
    /// <param name="terminalId">Terminal ID.</param>
    /// <param name="globalId">External Gateway Transaction ID (optional).</param>
    /// <returns>The created GroupSettlement entity.</returns>
    Task<GroupSettlement> SettleGroupAsync(
        List<Guid> ticketIds,
        PaymentType paymentType,
        Money totalAmount,
        UserId processedBy,
        Guid terminalId,
        string? globalId = null);
}
