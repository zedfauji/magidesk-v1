using System;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Interfaces;

public interface IMerchantBatchService
{
    /// <summary>
    /// Opens a new payment batch for the terminal if none exists.
    /// </summary>
    Task<PaymentBatch> OpenBatchAsync(Guid terminalId);

    /// <summary>
    /// Closes the current batch for the terminal.
    /// Finds all captured but unbatched payments, sends CloseBatch to gateway, and updates records.
    /// </summary>
    Task<PaymentBatch> CloseBatchAsync(Guid terminalId, UserId closedBy);
}
