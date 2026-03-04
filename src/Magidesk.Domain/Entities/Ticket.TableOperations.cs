using System;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Partial class containing table and delivery operations.
/// </summary>
public partial class Ticket
{
    /// <summary>
    /// Adds a table number to the ticket.
    /// </summary>
    public void AddTableNumber(int tableNumber)
    {
        if (tableNumber <= 0)
        {
            throw new BusinessRuleViolationException("Table number must be greater than zero.");
        }

        if (!_tableNumbers.Contains(tableNumber))
        {
            _tableNumbers.Add(tableNumber);
        }
    }

    /// <summary>
    /// Removes a table number from the ticket.
    /// </summary>
    public void RemoveTableNumber(int tableNumber)
    {
        if (_tableNumbers.Remove(tableNumber))
        {
        }
    }

    /// <summary>
    /// Assigns the ticket to a specific table, removing any previous assignments.
    /// </summary>
    public void AssignTable(int tableNumber)
    {
        if (tableNumber <= 0)
        {
            throw new BusinessRuleViolationException("Table number must be greater than zero.");
        }

        if (_tableNumbers.Count == 1 && _tableNumbers[0] == tableNumber)
        {
            return;
        }

        _tableNumbers.Clear();
        _tableNumbers.Add(tableNumber);
    }

    /// <summary>
    /// Links the ticket to a table session.
    /// </summary>
    public void SetSession(Guid sessionId)
    {
        if (sessionId == Guid.Empty)
        {
            throw new ArgumentException("Session ID cannot be empty.", nameof(sessionId));
        }

        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot link session to ticket in {Status} status.");
        }

        SessionId = sessionId;
        ActiveDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the ticket as ready for pickup or delivery.
    /// </summary>
    public void MarkAsReady()
    {
        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot mark ticket as ready in {Status} status.");
        }

        ReadyTime = DateTime.UtcNow;
        ActiveDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the ticket as dispatched for delivery.
    /// </summary>
    public void MarkAsDispatched(Guid? driverId)
    {
        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot mark ticket as dispatched in {Status} status.");
        }

        if (CustomerWillPickup)
        {
            throw new DomainInvalidOperationException("Cannot dispatch a pickup ticket.");
        }

        DispatchedTime = DateTime.UtcNow;
        AssignedDriverId = driverId;
        ActiveDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Transfers the ticket to a new owner.
    /// </summary>
    public void Transfer(UserId newOwner)
    {
        if (Status == TicketStatus.Closed || Status == TicketStatus.Voided || Status == TicketStatus.Refunded)
        {
            throw new DomainInvalidOperationException($"Cannot transfer ticket in {Status} status.");
        }

        if (newOwner == null)
        {
            throw new ArgumentNullException(nameof(newOwner));
        }

        CreatedBy = newOwner;
        ActiveDate = DateTime.UtcNow;
    }
}
