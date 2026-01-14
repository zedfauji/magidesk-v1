using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for ApplyDiscountCommand.
/// Task 2.1.5: Enhanced to use Ticket.ApplyDiscount(Discount, UserId, UserId?) method.
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
        // Task 2.1.5: New path for predefined discounts using enhanced Ticket.ApplyDiscount method
        if (command.DiscountId != Guid.Empty && command.AppliedBy != null)
        {
            await HandlePredefinedDiscountAsync(command, cancellationToken);
            return;
        }

        // Legacy path for ad-hoc discounts (backward compatibility)
        await HandleLegacyDiscountAsync(command, cancellationToken);
    }

    /// <summary>
    /// Task 2.1.5: Handle predefined discount application using the enhanced Ticket.ApplyDiscount method.
    /// </summary>
    private async Task HandlePredefinedDiscountAsync(ApplyDiscountCommand command, CancellationToken cancellationToken)
    {
        // 1. Load ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // 2. Load discount
        var discount = await _discountRepository.GetByIdAsync(command.DiscountId, cancellationToken);
        if (discount == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Discount {command.DiscountId} not found.");
        }

        // 3. Check if discount is active
        if (!discount.IsActive)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException("Cannot apply inactive discount.");
        }

        // 4. Calculate discount amount to check if authorization is required
        var discountAmount = discount.CalculateDiscount(ticket.SubtotalAmount);
        var discountPercentage = ticket.SubtotalAmount.Amount > 0
            ? (discountAmount.Amount / ticket.SubtotalAmount.Amount) * 100m
            : 0m;

        // 5. Check if authorization is required (> 50% of subtotal)
        if (discountPercentage > 50m && command.AuthorizedBy == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException(
                $"Discount of {discountPercentage:F1}% requires manager authorization. Please provide AuthorizedBy.");
        }

        // 6. Apply discount using the enhanced Ticket.ApplyDiscount method
        // This method will:
        // - Validate discount doesn't result in negative total
        // - Create TicketDiscount snapshot
        // - Recalculate totals
        // - Raise DiscountAppliedEvent (when event raising is implemented)
        ticket.ApplyDiscount(discount, command.AppliedBy, command.AuthorizedBy);

        // 7. Create audit event
        var auditDetails = $"DiscountId={discount.Id}, Name={discount.Name}, Type={discount.Type}, Value={discount.Value}, Amount={discountAmount.Amount}, Percentage={discountPercentage:F2}%, AuthorizedBy={command.AuthorizedBy?.Value.ToString() ?? "N/A"}";
        
        var auditEvent = AuditEvent.Create(
            AuditEventType.Modified,
            nameof(Ticket),
            ticket.Id,
            command.AppliedBy.Value,
            auditDetails,
            $"Applied discount '{discount.Name}' ({discountAmount}) to ticket #{ticket.TicketNumber}",
            beforeState: $"Subtotal={ticket.SubtotalAmount.Amount}, Total={ticket.TotalAmount.Amount}",
            correlationId: Guid.NewGuid()
        );

        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);

        // 8. Save ticket
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);
    }

    /// <summary>
    /// Legacy handler for ad-hoc discounts (backward compatibility).
    /// </summary>
    private async Task HandleLegacyDiscountAsync(ApplyDiscountCommand command, CancellationToken cancellationToken)
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

        if (command.DiscountId != Guid.Empty)
        {
            // Standard Predefined Discount (legacy path)
            var discount = await _discountRepository.GetByIdAsync(command.DiscountId, cancellationToken);
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
            await ApplyToOrderLine(ticket, command.OrderLineId.Value, discountType, discountValue, discountName, minimumQuantity, command);
        }
        else
        {
             await ApplyToTicket(ticket, discountType, discountValue, discountName, minimumBuy, command);
        }

        // 4. Persist
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);
    }

    private async Task ApplyToOrderLine(Domain.Entities.Ticket ticket, Guid lineId, DiscountType type, decimal value, string name, int? minQty, ApplyDiscountCommand command)
    {
        var line = ticket.OrderLines.FirstOrDefault(x => x.Id == lineId);
        if (line == null) throw new Domain.Exceptions.BusinessRuleViolationException("Order line not found.");

        decimal quantity = line.Quantity > 0 ? line.Quantity : line.ItemCount;
        if (minQty.HasValue && quantity < minQty.Value)
        {
             throw new Domain.Exceptions.BusinessRuleViolationException($"Minimum quantity of {minQty.Value} required.");
        }

        // Calculate amount
        Money amount;
        if (type == DiscountType.FixedAmount)
        {
             amount = new Money(value);
        }
        else if (type == DiscountType.Percentage)
        {
             amount = line.SubtotalAmount * (value / 100m);
        }
        else if (type == DiscountType.ManagerOverride)
        {
             amount = new Money(value);
        }
        else if (type == DiscountType.MemberDiscount)
        {
             amount = line.SubtotalAmount * (value / 100m);
        }
        else
        {
             amount = new Money(value); 
        }

        // Create OrderLineDiscount snapshot
        var lineDiscount = OrderLineDiscount.Create(
            line.Id,
            Guid.Empty,
            name,
            type,
            value,
            amount,
            minQty
        );

        ticket.ApplyLineDiscount(line.Id, lineDiscount);
        await Task.CompletedTask;
    }

    private async Task ApplyToTicket(Domain.Entities.Ticket ticket, DiscountType type, decimal value, string name, Money? minBuy, ApplyDiscountCommand command)
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
            amount = ticket.SubtotalAmount * (value / 100m);
        }

        var appliedBy = command.AppliedBy ?? (command.AuthorizingUserId.HasValue ? new UserId(command.AuthorizingUserId.Value) : new UserId(Guid.NewGuid()));
        var authorizedBy = command.AuthorizedBy ?? (command.AuthorizingUserId.HasValue ? new UserId(command.AuthorizingUserId.Value) : null);

        var ticketDiscount = TicketDiscount.Create(
            ticket.Id,
            Guid.Empty,
            name,
            type,
            value,
            amount,
            appliedBy: appliedBy,
            authorizedBy: authorizedBy,
            minimumAmount: minBuy
        );

        ticket.ApplyDiscount(ticketDiscount);
        await Task.CompletedTask;
    }
}

