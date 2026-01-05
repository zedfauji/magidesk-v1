using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;

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
        // Get ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException($"Ticket {command.TicketId} not found.");
        }

        // Get Discount
        var discount = await _discountRepository.GetByIdAsync(command.DiscountId, cancellationToken);
        if (discount == null)
        {
            throw new Domain.Exceptions.BusinessRuleViolationException("Invalid discount.");
        }

        if (command.OrderLineId.HasValue)
        {
            // Line Item Discount
            var line = ticket.OrderLines.FirstOrDefault(x => x.Id == command.OrderLineId.Value);
            if (line == null) throw new Domain.Exceptions.BusinessRuleViolationException("Order line not found.");

            if (!_discountDomainService.IsEligible(discount, line))
            {
                throw new Domain.Exceptions.BusinessRuleViolationException("Discount is not eligible for this item.");
            }

            var amount = _discountDomainService.CalculateDiscountAmount(discount, line.SubtotalAmount);
            
            var lineDiscount = OrderLineDiscount.Create(
                line.Id,
                discount.Id,
                discount.Name,
                discount.Type,
                discount.Value,
                amount,
                discount.MinimumQuantity
            );

            ticket.ApplyLineDiscount(line.Id, lineDiscount);
        }
        else
        {
            // Ticket Level Discount
            if (!_discountDomainService.IsEligible(discount, ticket))
            {
                 throw new Domain.Exceptions.BusinessRuleViolationException("Discount is not eligible for this ticket.");
            }

            var amount = _discountDomainService.CalculateDiscountAmount(discount, ticket.SubtotalAmount);

            var ticketDiscount = TicketDiscount.Create(
                ticket.Id,
                discount.Id,
                discount.Name,
                discount.Type,
                discount.Value,
                amount,
                discount.MinimumBuy
            );

            ticket.ApplyDiscount(ticketDiscount);
        }

        await _ticketRepository.UpdateAsync(ticket, cancellationToken);
    }
}

