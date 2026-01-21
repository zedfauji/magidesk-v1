using System;
using System.Linq;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Exceptions;
using AuditEventType = Magidesk.Domain.Enumerations.AuditEventType;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for SplitTicketCommand.
/// </summary>
public class SplitTicketCommandHandler : ICommandHandler<SplitTicketCommand, SplitTicketResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;
    private readonly Domain.DomainServices.TicketDomainService _ticketDomainService;

    public SplitTicketCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository,
        Domain.DomainServices.TicketDomainService ticketDomainService)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
        _ticketDomainService = ticketDomainService;
    }

    public async Task<SplitTicketResult> HandleAsync(
        SplitTicketCommand command,
        CancellationToken cancellationToken = default)
    {
        // Get original ticket
        var originalTicket = await _ticketRepository.GetByIdAsync(command.OriginalTicketId, cancellationToken);
        if (originalTicket == null)
        {
            return new SplitTicketResult
            {
                Success = false,
                ErrorMessage = $"Ticket {command.OriginalTicketId} not found."
            };
        }

        // Validate can split
        if (!_ticketDomainService.CanSplitTicket(originalTicket))
        {
            return new SplitTicketResult
            {
                Success = false,
                ErrorMessage = $"Ticket {originalTicket.TicketNumber} cannot be split."
            };
        }

        // Validate order lines to split
        if (command.OrderLineIdsToSplit == null || !command.OrderLineIdsToSplit.Any())
        {
            return new SplitTicketResult
            {
                Success = false,
                ErrorMessage = "At least one order line must be selected for split."
            };
        }

        // Validate that not all order lines are being split
        if (command.OrderLineIdsToSplit.Count >= originalTicket.OrderLines.Count)
        {
            return new SplitTicketResult
            {
                Success = false,
                ErrorMessage = "Cannot split all order lines. Original ticket must retain at least one order line."
            };
        }

        // Validate all order line IDs exist on the ticket
        var orderLinesToSplit = originalTicket.OrderLines
            .Where(ol => command.OrderLineIdsToSplit.Contains(ol.Id))
            .ToList();

        if (orderLinesToSplit.Count != command.OrderLineIdsToSplit.Count)
        {
            return new SplitTicketResult
            {
                Success = false,
                ErrorMessage = "One or more order line IDs not found on the ticket."
            };
        }

        // Get next ticket number for new ticket
        var newTicketNumber = await _ticketRepository.GetNextTicketNumberAsync(cancellationToken);

        // Create new ticket
        var newTicket = Ticket.Create(
            newTicketNumber,
            command.SplitBy,
            command.TerminalId,
            command.ShiftId,
            command.OrderTypeId,
            command.GlobalId);

        // Copy relevant properties from original ticket
        // (Note: Properties like TableNumbers, CustomerId, etc. could be copied or left empty)
        // For now, we'll create a clean new ticket

        // Move order lines to new ticket
        int orderLinesMoved = 0;
        foreach (var orderLine in orderLinesToSplit)
        {
            // Create new OrderLine for new ticket (copying data)
            var newOrderLine = OrderLine.Create(
                newTicket.Id,
                orderLine.MenuItemId,
                orderLine.MenuItemName,
                orderLine.Quantity,
                orderLine.UnitPrice,
                orderLine.TaxRate,
                orderLine.CategoryName,
                orderLine.GroupName);

            // Note: Modifiers, add-ons, and discounts are not copied in this initial implementation
            // They can be added later if needed, or the split can be enhanced to copy them

            // Add to new ticket first
            newTicket.AddOrderLine(newOrderLine);

            // Remove from original ticket
            originalTicket.RemoveOrderLine(orderLine.Id);

            orderLinesMoved++;
        }

        // Recalculate totals for both tickets
        _ticketDomainService.CalculateTotals(originalTicket);
        _ticketDomainService.CalculateTotals(newTicket);

        // Save both tickets
        await _ticketRepository.AddAsync(newTicket, cancellationToken);
        await _ticketRepository.UpdateAsync(originalTicket, cancellationToken);

        // Create audit events
        var correlationId = Guid.NewGuid();
        
        var originalAuditEvent = AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Ticket),
            originalTicket.Id,
            command.SplitBy.Value,
            System.Text.Json.JsonSerializer.Serialize(new { 
                Action = "Split",
                OrderLinesRemoved = orderLinesMoved,
                NewTicketId = newTicket.Id
            }),
            $"Ticket {originalTicket.TicketNumber} split. {orderLinesMoved} order lines moved to ticket {newTicketNumber}",
            correlationId: correlationId);

        var newAuditEvent = AuditEvent.Create(
            AuditEventType.Created,
            nameof(Ticket),
            newTicket.Id,
            command.SplitBy.Value,
            System.Text.Json.JsonSerializer.Serialize(new { 
                Action = "SplitFrom",
                OriginalTicketId = originalTicket.Id,
                OrderLinesMoved = orderLinesMoved
            }),
            $"Ticket {newTicketNumber} created from split of ticket {originalTicket.TicketNumber}",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(originalAuditEvent, cancellationToken);
        await _auditEventRepository.AddAsync(newAuditEvent, cancellationToken);

        return new SplitTicketResult
        {
            Success = true,
            NewTicketId = newTicket.Id,
            NewTicketNumber = newTicketNumber,
            OrderLinesMoved = orderLinesMoved
        };
    }
}

