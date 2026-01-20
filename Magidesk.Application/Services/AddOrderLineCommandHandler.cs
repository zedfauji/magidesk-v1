using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Microsoft.Extensions.Logging;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for AddOrderLineCommand.
/// Enhanced with automatic kitchen routing (requirement 9.1).
/// </summary>
public class AddOrderLineCommandHandler : ICommandHandler<AddOrderLineCommand, AddOrderLineResult>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly IAuditEventRepository _auditEventRepository;
    private readonly IRepository<StockMovement> _stockMovementRepository;
    private readonly IKitchenRoutingService _kitchenRoutingService;
    private readonly IUserService _userService;
    private readonly ILogger<AddOrderLineCommandHandler> _logger;

    public AddOrderLineCommandHandler(
        ITicketRepository ticketRepository,
        IMenuRepository menuRepository,
        IAuditEventRepository auditEventRepository,
        IRepository<StockMovement> stockMovementRepository,
        IKitchenRoutingService kitchenRoutingService,
        IUserService userService,
        ILogger<AddOrderLineCommandHandler> logger)
    {
        _ticketRepository = ticketRepository;
        _menuRepository = menuRepository;
        _auditEventRepository = auditEventRepository;
        _stockMovementRepository = stockMovementRepository;
        _kitchenRoutingService = kitchenRoutingService;
        _userService = userService;
        _logger = logger;
    }

    public async Task<AddOrderLineResult> HandleAsync(AddOrderLineCommand command, CancellationToken cancellationToken = default)
    {
        int maxRetries = 3;
        int currentRetry = 0;
        
        while (true)
        {
            try
            {
                // Create a transaction for atomicity (Ticket Update + Stock + Audit)
                using var transaction = await _ticketRepository.BeginTransactionAsync(cancellationToken);

                // Get ticket
                var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
                if (ticket == null)
                {
                    throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
                }

                // 1. Get MenuItem and Handle Stock (G.2)
                var menuItem = await _menuRepository.GetByIdAsync(command.MenuItemId, cancellationToken);
                OrderLine orderLine;

                // Scope Stock operations in a try-block? No, transaction handles it.
                if (menuItem != null) // Should check null? Yes.
                {
                     // Deduct Stock if tracked
                     if (menuItem.TrackStock)
                     {
                         // This throws BusinessRuleViolationException if insufficient
                         menuItem.DeductStock((int)command.Quantity); 

                         // Record Movement
                         var movement = StockMovement.Create(
                             menuItem.Id,
                             -(int)command.Quantity, // Method takes change amount? No, Constructor takes change. Sale is negative.
                             StockMovementType.Sale,
                             $"Ticket #{ticket.TicketNumber}",
                             command.AddedBy?.Value
                         );
                         
                         await _stockMovementRepository.AddAsync(movement, cancellationToken);
                         await _menuRepository.UpdateAsync(menuItem, cancellationToken);
                     }
                }
                
                // Create order line
                orderLine = OrderLine.Create(
                    command.TicketId,
                    command.MenuItemId,
                    command.MenuItemName,
                    command.Quantity,
                    new Domain.ValueObjects.Money(command.UnitPrice.Amount, command.UnitPrice.Currency), // Deep Clone
                    command.TaxRate,
                    command.CategoryName,
                    command.GroupName);

                // Populate PrinterGroupId (F-0014)
                if (menuItem != null)
                {
                    if (menuItem.PrinterGroupId.HasValue)
                    {
                        orderLine.SetPrinterGroup(menuItem.PrinterGroupId);
                    }
                    else if (menuItem.Group?.PrinterGroupId.HasValue == true)
                    {
                         orderLine.SetPrinterGroup(menuItem.Group.PrinterGroupId);
                    }
                    else if (menuItem.Category?.PrinterGroupId.HasValue == true)
                    {
                         orderLine.SetPrinterGroup(menuItem.Category.PrinterGroupId);
                    }
                }

                // Add Modifiers
                foreach (var mod in command.Modifiers)
                {
                    var orderLineModifier = OrderLineModifier.Create(
                        orderLineId: orderLine.Id,
                        modifierId: mod.Id,
                        name: mod.Name,
                        modifierType: mod.ModifierType,
                        itemCount: 1, // Default to 1 for now
                        unitPrice: new Domain.ValueObjects.Money(mod.BasePrice.Amount, mod.BasePrice.Currency), // Deep Clone
                        basePrice: new Domain.ValueObjects.Money(mod.BasePrice.Amount, mod.BasePrice.Currency), // Deep Clone
                        taxRate: mod.TaxRate,
                        modifierGroupId: mod.ModifierGroupId,
                        shouldPrintToKitchen: mod.ShouldPrintToKitchen
                    );
                    orderLine.AddModifier(orderLineModifier);
                }

                // Add to ticket
                ticket.AddOrderLine(orderLine);

                // Update ticket
                await _ticketRepository.UpdateAsync(ticket, cancellationToken);

                // Create audit event
                // IMPORTANT: Never use Guid.Empty for UserId - always get from command or fallback to current user
                var currentUser = _userService.CurrentUser;
                var userId = command.AddedBy?.Value 
                          ?? currentUser?.Id
                          ?? throw new Domain.Exceptions.BusinessRuleViolationException("Cannot create audit event without a valid user context. Please ensure a user is logged in.");
                
                var isMisc = (command.CategoryName?.Contains("Misc", StringComparison.OrdinalIgnoreCase) == true) || 
                             (command.MenuItemName.Contains("Misc", StringComparison.OrdinalIgnoreCase));
                
                var action = isMisc ? "Misc Item Added" : "Item Added";
                var details = isMisc ? $"Misc/Ad-hoc item '{command.MenuItemName}' equal to {command.UnitPrice} added to ticket {ticket.TicketNumber}" 
                                     : $"Order line added to ticket {ticket.TicketNumber}";
        
                var correlationId = Guid.NewGuid();
                var auditEvent = AuditEvent.Create(
                    AuditEventType.Modified,
                    nameof(Ticket),
                    ticket.Id,
                    userId,
                    System.Text.Json.JsonSerializer.Serialize(new { OrderLineId = orderLine.Id, Action = action, IsMisc = isMisc }),
                    details,
                    correlationId: correlationId);

                await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

                // Commit Transaction
                await transaction.CommitAsync(cancellationToken);

                // Automatic kitchen routing (requirement 9.1)
                // Route the newly added order line to kitchen if it should be printed
                // This is done AFTER commit to ensure data persists first
                if (orderLine.ShouldPrintToKitchen)
                {
                    try
                    {
                        var autoRouted = await _kitchenRoutingService.AutoRouteOrderLinesAsync(
                            ticket.Id, 
                            new List<Guid> { orderLine.Id });

                        if (autoRouted)
                        {
                            _logger.LogInformation("Automatically routed order line {OrderLineId} to kitchen for ticket {TicketId}", 
                                orderLine.Id, ticket.Id);
                        }
                        else
                        {
                            _logger.LogWarning("Failed to automatically route order line {OrderLineId} to kitchen for ticket {TicketId}", 
                                orderLine.Id, ticket.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Don't fail the order line addition if kitchen routing fails
                        // This ensures the order is still recorded even if kitchen systems are down
                        _logger.LogError(ex, "Error during automatic kitchen routing for order line {OrderLineId} on ticket {TicketId}", 
                            orderLine.Id, ticket.Id);
                    }
                }

                return new AddOrderLineResult
                {
                    OrderLineId = orderLine.Id
                };
            }
            catch (Domain.Exceptions.ConcurrencyException)
            {
                currentRetry++;
                if (currentRetry > maxRetries)
                    throw; // Exhausted retries

                _logger.LogWarning("Concurrency conflict adding order line. Retrying {Retry}/{MaxRetries}...", currentRetry, maxRetries);
                
                // Clear tracker to fetch fresh entity
                _ticketRepository.ClearChangeTracker();
                
                // Backoff
                await Task.Delay(50 * currentRetry, cancellationToken); 
            }
            catch (Exception)
            {
                throw; // Rethrow other exceptions (e.g. business rule violations)
            }
        }
    }
}

