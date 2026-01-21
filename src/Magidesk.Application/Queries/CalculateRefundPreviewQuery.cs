using Magidesk.Application.Interfaces;
using Magidesk.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Domain.Entities;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Enumerations;
using Magidesk.Application.Commands; // For RefundMode enum if shared, or define here

namespace Magidesk.Application.Queries
{
    public enum RefundMode
    {
        Full,
        Partial,
        SpecificPayments
    }

    public class CalculateRefundPreviewQuery : IQuery<RefundPreviewDto>
    {
        public Guid TicketId { get; set; }
        public RefundMode Mode { get; set; }
        public Money? PartialAmount { get; set; }
        public List<Guid> SpecificPaymentIds { get; set; } = new();
    }

    public class CalculateRefundPreviewQueryHandler : IQueryHandler<CalculateRefundPreviewQuery, RefundPreviewDto>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IPaymentRepository _paymentRepository;

        public CalculateRefundPreviewQueryHandler(ITicketRepository ticketRepository, IPaymentRepository paymentRepository)
        {
            _ticketRepository = ticketRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<RefundPreviewDto> HandleAsync(CalculateRefundPreviewQuery query, System.Threading.CancellationToken cancellationToken = default)
        {
            var ticket = await _ticketRepository.GetByIdAsync(query.TicketId);
            if (ticket == null) throw new KeyNotFoundException($"Ticket {query.TicketId} not found");

            var payments = await _paymentRepository.GetByTicketIdAsync(query.TicketId);
            var activePayments = payments.Where(p => !p.IsVoided && p.TransactionType == TransactionType.Credit).ToList();

            var dto = new RefundPreviewDto
            {
                TicketId = ticket.Id,
                OriginalTotalAmount = ticket.TotalAmount,
                OriginalPaidAmount = ticket.PaidAmount,
                OriginalDueAmount = ticket.DueAmount,
                IsFullRefund = query.Mode == RefundMode.Full
            };

            Money refundTotal = Money.Zero();
            var actions = new List<ProposedRefundAction>();

            switch (query.Mode)
            {
                case RefundMode.Full:
                    refundTotal = ticket.PaidAmount;
                    foreach (var p in activePayments)
                    {
                        actions.Add(new ProposedRefundAction
                        {
                            Description = $"Refund full payment {p.Id.ToString().Substring(0,8)}",
                            Amount = p.Amount,
                            PaymentMethod = p.PaymentType.ToString(),
                            IsGatewayRefund = p.PaymentType == PaymentType.CreditCard
                        });
                    }
                    break;

                case RefundMode.Partial:
                    if (query.PartialAmount == null || query.PartialAmount.Amount <= 0)
                    {
                        dto.ValidationWarning = "Partial amount must be greater than zero.";
                        return dto;
                    }
                    if (query.PartialAmount.Amount > ticket.PaidAmount.Amount)
                    {
                        dto.ValidationWarning = $"Partial amount cannot exceed Total Paid ({ticket.PaidAmount}).";
                        return dto;
                    }
                    refundTotal = query.PartialAmount;
                    actions.Add(new ProposedRefundAction
                    {
                        Description = "Partial refund applied to ticket balance",
                        Amount = refundTotal,
                        PaymentMethod = "Mixed/Partial",
                        IsGatewayRefund = false 
                    });
                    break;

                case RefundMode.SpecificPayments:
                    foreach(var paymentId in query.SpecificPaymentIds)
                    {
                        var p = activePayments.FirstOrDefault(x => x.Id == paymentId);
                        if(p != null)
                        {
                           refundTotal += p.Amount;
                           actions.Add(new ProposedRefundAction
                           {
                               Description = $"Refund payment {p.Id.ToString().Substring(0, 8)}",
                               Amount = p.Amount,
                               PaymentMethod = p.PaymentType.ToString(),
                               IsGatewayRefund = p.PaymentType == PaymentType.CreditCard
                           });
                        }
                    }
                    break;
            }

            dto.ProposedActions = actions;
            dto.ProjectedPaidAmount = ticket.PaidAmount - refundTotal;
            dto.ProjectedDueAmount = ticket.TotalAmount - dto.ProjectedPaidAmount; // Assuming Total doesn't change
            
            // Logic check: If refunding, DueAmount increases (money owed back to invoice if unpaid, but here we are just reversing payment)
            // Actually: PaidAmount decreases. DueAmount = Total - Paid.
            // So if Paid goes down, Due goes up. Correct.

            return dto;
        }
    }
}
