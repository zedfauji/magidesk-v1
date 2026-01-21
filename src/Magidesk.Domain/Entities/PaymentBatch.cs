using System;
using System.Collections.Generic;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.Entities;

public class PaymentBatch
{
    public Guid Id { get; private set; }
    public Guid TerminalId { get; private set; }
    public PaymentBatchStatus Status { get; private set; }
    public DateTime OpenedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public string? GatewayBatchId { get; private set; }
    
    // In a real system, we'd link this to the Payments included in the batch.
    // For now, simpler linkage or date-range based batching is common.
    // We will assume a specific list of Payment IDs is tracked or associated via the Payment table itself.

    protected PaymentBatch() { }

    public PaymentBatch(Guid terminalId)
    {
        Id = Guid.NewGuid();
        TerminalId = terminalId;
        Status = PaymentBatchStatus.Open;
        OpenedAt = DateTime.UtcNow;
    }

    public void Close(string? gatewayBatchId = null)
    {
        Status = PaymentBatchStatus.Closed;
        ClosedAt = DateTime.UtcNow;
        GatewayBatchId = gatewayBatchId;
    }
}
