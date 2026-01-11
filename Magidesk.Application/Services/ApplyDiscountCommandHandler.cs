using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for ApplyDiscountCommand.
/// Note: This is a simplified implementation for Phase 1.
/// Full discount logic will be implemented in Phase 3.
/// </summary>
public class ApplyDiscountCommandHandler : ICommandHandler<ApplyDiscountCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IAuditEventRepository _auditEventRepository;
    private readonly Domain.DomainServices.DiscountDomainService _discountDomainService;
    private readonly IDiscountRepository _discountRepository;

    public ApplyDiscountCommandHandler(
        ITicketRepository ticketRepository,
        IAuditEventRepository auditEventRepository,
        Domain.DomainServices.DiscountDomainService discountDomainService,
        IDiscountRepository discountRepository)
    {
        _ticketRepository = ticketRepository;
        _auditEventRepository = auditEventRepository;
        _discountDomainService = discountDomainService;
        _discountRepository = discountRepository;
    }

    public async Task HandleAsync(ApplyDiscountCommand command, CancellationToken cancellationToken = default)
    {
        // 1. Validate Ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // 2. Identify Discount Strategy
        DiscountType discountType;
        decimal discountValue;
        string discountName;
        Money? minimumBuy = null;
        int? minimumQuantity = null;

        if (command.DiscountId.HasValue)
        {
            // Standard Predefined Discount
            var discount = await _discountRepository.GetByIdAsync(command.DiscountId.Value, cancellationToken);
            if (discount == null) throw new Domain.Exceptions.BusinessRuleViolationException("Invalid discount ID.");
            
            if (!discount.IsActive) throw new Domain.Exceptions.BusinessRuleViolationException("Discount is inactive.");

            discountType = discount.Type;
            discountValue = discount.Value;
            discountName = discount.Name;
            minimumBuy = discount.MinimumBuy;
            minimumQuantity = discount.MinimumQuantity;
        }
        else if (command.Type.HasValue && command.Value.HasValue)
        {
            // Ad-hoc / Override / Member Discount
            discountType = command.Type.Value;
            discountValue = command.Value.Value;

            switch (discountType)
            {
                case DiscountType.ManagerOverride:
                    if (string.IsNullOrWhiteSpace(command.Reason))
                        throw new Domain.Exceptions.BusinessRuleViolationException("Manager override requires a reason.");
                    if (!command.AuthorizingUserId.HasValue)
                        throw new Domain.Exceptions.BusinessRuleViolationException("Manager override requires authorization.");
                    discountName = $"Override: {command.Reason}";
                    break;

                case DiscountType.MemberDiscount:
                    // In a real implementation, we would validate the customer's membership tier here.
                    // For now, we assume the command issuer has validated eligibility.
                    discountName = "Member Discount";
                    break;

                case DiscountType.Promotional:
                    discountName = command.Reason ?? "Promotional Discount";
                    break;

                default:
                    throw new Domain.Exceptions.BusinessRuleViolationException("Unsupported ad-hoc discount type.");
            }
        }
        else
        {
             throw new Domain.Exceptions.BusinessRuleViolationException("Must provide either DiscountId or Type/Value.");
        }

        // 3. Apply Discount
        if (command.OrderLineId.HasValue)
        {
            await ApplyToOrderLine(ticket, command.OrderLineId.Value, discountType, discountValue, discountName, minimumQuantity);
        }
        else
        {
             await ApplyToTicket(ticket, discountType, discountValue, discountName, minimumBuy);
        }

        // 4. Audit Log (Placeholder)
        // await _auditEventRepository.AddAsync(new AuditEvent(...));

        // 5. Persist
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);
    }

    private async Task ApplyToOrderLine(Domain.Entities.Ticket ticket, Guid lineId, DiscountType type, decimal value, string name, int? minQty)
    {
        var line = ticket.OrderLines.FirstOrDefault(x => x.Id == lineId);
        if (line == null) throw new Domain.Exceptions.BusinessRuleViolationException("Order line not found.");

        // For defined discounts, check domain service eligibility
        // For overrides, we bypass this check or rely on simpler logic
        // This is a simplified check for now
        
        decimal quantity = line.Quantity > 0 ? line.Quantity : line.ItemCount;
        if (minQty.HasValue && quantity < minQty.Value)
        {
             throw new Domain.Exceptions.BusinessRuleViolationException($"Minimum quantity of {minQty.Value} required.");
        }

        // Calculate amount
        Money amount;
        if (type == DiscountType.FixedAmount)
        {
             amount = new Money(value); // Assuming currency match
        }
        else if (type == DiscountType.Percentage)
        {
             amount = line.SubtotalAmount * (value / 100m);
        }
        else if (type == DiscountType.ManagerOverride)
        {
             // Overrides could be fixed or percent. Assuming value is amount if type is Amount, else percent
             // But for C.7 specific ManagerOverride type, let's assume it's a fixed reduction for simplicity 
             // OR we need to check if Value > 1 (likely amount) or <= 1 (likely percent)? 
             // Ideally, ManagerOverride should just follow Percentage or FixedAmount logic.
             // Let's treat ManagerOverride as Fixed Amount reduction for safety unless specified otherwise.
             amount = new Money(value);
        }
        else if (type == DiscountType.MemberDiscount)
        {
             amount = line.SubtotalAmount * (value / 100m); // Usually percentage
        }
        else
        {
             // Fallback to domain service for standard calcs if needed, or implement simple calc
             // Using simple calc for new types to avoid modifying Domain Service yet
             amount = new Money(value); 
        }

        // Create OrderLineDiscount snapshot
        var lineDiscount = OrderLineDiscount.Create(
            line.Id,
            Guid.Empty, // No specific ID for ad-hoc
            name,
            type,
            value,
            amount,
            minQty
        );

        ticket.ApplyLineDiscount(line.Id, lineDiscount);
        await Task.CompletedTask;
    }

    private async Task ApplyToTicket(Domain.Entities.Ticket ticket, DiscountType type, decimal value, string name, Money? minBuy)
    {
        if (minBuy != null && ticket.SubtotalAmount < minBuy)
        {
             throw new Domain.Exceptions.BusinessRuleViolationException($"Minimum purchase of {minBuy} required.");
        }

        Money amount;
        if (type == DiscountType.FixedAmount || type == DiscountType.ManagerOverride)
        {
            amount = new Money(value);
        }
        else 
        {
            // Percentage-based (Standard, Member, etc.)
            amount = ticket.SubtotalAmount * (value / 100m);
        }

        var ticketDiscount = TicketDiscount.Create(
            ticket.Id,
            Guid.Empty,
            name,
            type,
            value,
            amount,
            minBuy
        );

        ticket.ApplyDiscount(ticketDiscount);
        await Task.CompletedTask;
    }
}

