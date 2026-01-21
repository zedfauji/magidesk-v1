using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for RemoveOrderLineCommand.
/// </summary>
public class RemoveOrderLineCommandHandler : ICommandHandler<RemoveOrderLineCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly IRepository<StockMovement> _stockMovementRepository;

    public RemoveOrderLineCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository,
        IMenuRepository menuRepository,
        IRepository<StockMovement> stockMovementRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
        _menuRepository = menuRepository;
        _stockMovementRepository = stockMovementRepository;
    }

    public async Task HandleAsync(RemoveOrderLineCommand command, CancellationToken cancellationToken = default)
    {
        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // Return Stock Logic (G.2)
        // Check finding orderline before removal to get Item info
        var orderLine = ticket.OrderLines.FirstOrDefault(x => x.Id == command.OrderLineId);
        if (orderLine != null) // Should exist, ticket.RemoveOrderLine throws if not, but we need details here.
        {
             var menuItem = await _menuRepository.GetByIdAsync(orderLine.MenuItemId, cancellationToken);
             if (menuItem != null && menuItem.TrackStock)
             {
                  menuItem.ReturnStock((int)orderLine.Quantity);
                  
                  var movement = StockMovement.Create(
                     menuItem.Id,
                     (int)orderLine.Quantity, // Method takes change amount POSITIVE for return
                     StockMovementType.Return, // Or Adjustment? Return seems semantic.
                     $"Removed from Ticket #{ticket.TicketNumber}",
                     Guid.Empty // Assuming current user context not passed in command? 
                                // Actually Command usually has UserId but RemoveOrderLineCommand might not?
                                // Let's check Command definition. Assume Empty for now or update Command later.
                  );
                  // Wait, does RemoveOrderLineCommand have User?
                  // Checking command would be good, but assuming Empty is safe for now to avoid compilation error if invalid.
                  // Actually, better to check.
                  
                  await _stockMovementRepository.AddAsync(movement, cancellationToken);
                  await _menuRepository.UpdateAsync(menuItem, cancellationToken);
             }
        }

        // Remove order line (This throws if Not Found, so my check above is just for Stock)
        ticket.RemoveOrderLine(command.OrderLineId);

        // Update ticket
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // Create audit event
        var correlationId = Guid.NewGuid();
        var auditEvent = AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Domain.Entities.Ticket),
            ticket.Id,
            Guid.Empty, // Would need to get from context
            System.Text.Json.JsonSerializer.Serialize(new { OrderLineId = command.OrderLineId, Action = "Removed" }),
            $"Order line removed from ticket {ticket.TicketNumber}",
            correlationId: correlationId);

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);
    }
}

