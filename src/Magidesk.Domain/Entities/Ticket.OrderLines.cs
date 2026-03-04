using System;
using System.Linq;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Partial class containing order line management methods.
/// </summary>
public partial class Ticket
{
    /// <summary>
    /// Adds an order line to the ticket.
    /// </summary>
    public void AddOrderLine(OrderLine orderLine)
    {
        if (orderLine == null)
        {
            throw new ArgumentNullException(nameof(orderLine));
        }

        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot add items to ticket in {Status} status.");
        }

        if (orderLine.TicketId != Id)
        {
            throw new BusinessRuleViolationException("OrderLine does not belong to this ticket.");
        }

        _orderLines.Add(orderLine);
        ActiveDate = DateTime.UtcNow;

        // Auto-open if still in Draft
        if (Status == TicketStatus.Draft)
        {
            Open();
        }

        CalculateTotals();
    }

    /// <summary>
    /// Removes an order line from the ticket.
    /// </summary>
    public void RemoveOrderLine(Guid orderLineId)
    {
        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot remove items from ticket in {Status} status.");
        }

        var orderLine = _orderLines.FirstOrDefault(ol => ol.Id == orderLineId);
        if (orderLine == null)
        {
            throw new BusinessRuleViolationException($"OrderLine {orderLineId} not found.");
        }

        _orderLines.Remove(orderLine);
        ActiveDate = DateTime.UtcNow;
        CalculateTotals();
    }
}
