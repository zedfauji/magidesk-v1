using System;
using System.Threading;
using System.Threading.Tasks;
using Magidesk.Application.Commands;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.DomainServices;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.Exceptions;

namespace Magidesk.Application.Services;

public class ApplyCouponCommandHandler : ICommandHandler<ApplyCouponCommand>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly TicketDomainService _ticketDomainService;
    private readonly IAuditEventRepository _auditEventRepository;

    public ApplyCouponCommandHandler(
        ITicketRepository ticketRepository,
        IDiscountRepository discountRepository,
        TicketDomainService ticketDomainService,
        IAuditEventRepository auditEventRepository)
    {
        _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
        _discountRepository = discountRepository ?? throw new ArgumentNullException(nameof(discountRepository));
        _ticketDomainService = ticketDomainService ?? throw new ArgumentNullException(nameof(ticketDomainService));
        _auditEventRepository = auditEventRepository ?? throw new ArgumentNullException(nameof(auditEventRepository));
    }

    public async Task HandleAsync(ApplyCouponCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        // 1. Load Ticket
        var ticket = await _ticketRepository.GetByIdAsync(command.TicketId, cancellationToken);
        if (ticket == null)
        {
            throw new NotFoundException($"Ticket {command.TicketId} not found.");
        }

        // 2. Load Coupon
        if (string.IsNullOrWhiteSpace(command.CouponCode))
        {
            throw new BusinessRuleViolationException("Coupon code cannot be empty.");
        }

        var discount = await _discountRepository.GetByCouponCodeAsync(command.CouponCode, cancellationToken);
        if (discount == null)
        {
            throw new BusinessRuleViolationException($"Invalid coupon code: '{command.CouponCode}'.");
        }

        // 3. Validate
        _ticketDomainService.ValidateCouponApplication(ticket, discount);

        // 4. Calculate Amount
        var amount = _ticketDomainService.CalculateDiscountAmount(ticket, discount);

        // 5. Create TicketDiscount
        var ticketDiscount = TicketDiscount.Create(
            ticket.Id,
            discount.Id,
            discount.Name,
            discount.Type,
            discount.Value,
            amount,
            discount.MinimumBuy);

        // 6. Apply to Ticket
        ticket.ApplyDiscount(ticketDiscount);

        // 7. Save
        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        // 8. Audit
        var auditEvent = AuditEvent.Create(
            AuditEventType.Modified,
            "Ticket",
            ticket.Id,
            command.AppliedBy?.Value ?? Guid.Empty,
            "Coupon Applied",
            $"Applied coupon '{discount.CouponCode}' ({discount.Name}) for {amount}.",
            correlationId: Guid.NewGuid());
            
        await _auditEventRepository.AddAsync(auditEvent, cancellationToken);
    }
}
