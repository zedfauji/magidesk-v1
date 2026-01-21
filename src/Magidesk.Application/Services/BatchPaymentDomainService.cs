using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Events;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Enumerations;
using Magidesk.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Magidesk.Application.Services
{
    public class BatchPaymentDomainService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICashSessionRepository _cashSessionRepository;
        private readonly IEventPublisher _eventPublisher;

        public BatchPaymentDomainService(
            ITicketRepository ticketRepository,
            IPaymentRepository paymentRepository,
            ICashSessionRepository cashSessionRepository,
            IEventPublisher eventPublisher)
        {
            _ticketRepository = ticketRepository;
            _paymentRepository = paymentRepository;
            _cashSessionRepository = cashSessionRepository;
            _eventPublisher = eventPublisher;
        }

        /// <summary>
        /// Processes a batch payment across multiple tickets atomically
        /// </summary>
        public async Task<BatchPaymentResult> ProcessBatchPaymentAsync(BatchPaymentRequest request)
        {
            // Validate all tickets exist and are in correct state
            var tickets = await ValidateAndGetTickets(request.TicketIds);
            
            // Calculate payment distribution
            var paymentDistribution = CalculatePaymentDistribution(tickets, request.TotalAmount);
            
            // Start atomic transaction
            using var transaction = await _ticketRepository.BeginTransactionAsync();
            
            try
            {
                var payments = new List<Payment>();
                var updatedTickets = new List<Ticket>();

                // Process each ticket payment
                foreach (var distribution in paymentDistribution)
                {
                    var ticket = tickets.First(t => t.Id == distribution.TicketId);
                    
                    // Create payment for this ticket
                    var payment = CreatePayment(request, ticket, distribution.Amount);
                    await _paymentRepository.AddAsync(payment);
                    payments.Add(payment);

                    // Update ticket
                    ticket.AddPayment(payment);
                    await _ticketRepository.UpdateAsync(ticket);
                    updatedTickets.Add(ticket);
                }

                // Publish audit events
                await PublishBatchPaymentEvents(request, tickets, payments);

                // Commit transaction
                await transaction.CommitAsync();

                return new BatchPaymentResult
                {
                    Success = true,
                    TicketIds = request.TicketIds,
                    PaymentIds = payments.Select(p => p.Id).ToList(),
                    TotalProcessed = payments.Sum(p => p.Amount.Amount),
                    ChangeAmount = CalculateChange(request, paymentDistribution.Sum(d => d.Amount.Amount))
                };
            }
            catch (Exception ex)
            {
                // Rollback on any failure
                await transaction.RollbackAsync();
                return new BatchPaymentResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    TicketIds = request.TicketIds
                };
            }
        }

        private async Task<List<Ticket>> ValidateAndGetTickets(List<Guid> ticketIds)
        {
            var tickets = new List<Ticket>();
            
            foreach (var ticketId in ticketIds)
            {
                var ticket = await _ticketRepository.GetByIdAsync(ticketId);
                if (ticket == null)
                {
                    throw new InvalidOperationException($"Ticket {ticketId} not found");
                }
                if (ticket.Status != TicketStatus.Open)
                {
                    throw new InvalidOperationException($"Ticket {ticketId} is not in Open state");
                }
                if (ticket.DueAmount.Amount <= 0)
                {
                    throw new InvalidOperationException($"Ticket {ticketId} has no due amount");
                }
                tickets.Add(ticket);
            }

            return tickets;
        }

        private List<PaymentDistribution> CalculatePaymentDistribution(List<Ticket> tickets, Money totalAmount)
        {
            var distributions = new List<PaymentDistribution>();
            var totalDue = tickets.Sum(t => t.DueAmount.Amount);
            
            // Distribute payment proportionally based on each ticket's due amount
            foreach (var ticket in tickets)
            {
                var ticketDue = ticket.DueAmount.Amount;
                var proportion = ticketDue / totalDue;
                var paymentAmount = Math.Round(totalAmount.Amount * proportion, 2);
                
                distributions.Add(new PaymentDistribution
                {
                    TicketId = ticket.Id,
                    Amount = new Money(paymentAmount, totalAmount.Currency),
                    DueAmount = ticket.DueAmount
                });
            }

            return distributions;
        }

        private Payment CreatePayment(BatchPaymentRequest request, Ticket ticket, Money amount)
        {
            Payment payment;

            if (request.PaymentType == PaymentType.Cash)
            {
                payment = CashPayment.Create(
                    ticket.Id,
                    amount,
                    request.ProcessedBy,
                    request.TerminalId,
                    request.GlobalId
                );
            }
            else if (request.PaymentType == PaymentType.CreditCard)
            {
                payment = CreditCardPayment.Create(
                    ticket.Id,
                    amount,
                    request.ProcessedBy,
                    request.TerminalId,
                    null, // cardNumber
                    "Batch Payment",
                    request.AuthCode,
                    null, // referenceNumber
                    request.CardType,
                    request.GlobalId
                );
                
                // Authorize the payment if auth code provided
                if (!string.IsNullOrEmpty(request.AuthCode))
                {
                    ((CreditCardPayment)payment).Authorize(request.AuthCode, null, request.CardType);
                }
            }
            else
            {
                // For other payment types, create a basic payment (this would need to be extended)
                throw new NotSupportedException($"Payment type {request.PaymentType} is not supported for batch payment");
            }

            return payment;
        }

        private async Task PublishBatchPaymentEvents(BatchPaymentRequest request, List<Ticket> tickets, List<Payment> payments)
        {
            // Publish GROUP_SETTLE event
            var groupSettleEvent = new GroupSettled(
                request.TicketIds,
                request.TotalAmount,
                request.PaymentType,
                request.ProcessedBy,
                payments.Select(p => p.Id).ToList()
            );

            await _eventPublisher.PublishAsync(groupSettleEvent);

            // Publish individual payment events if needed
            foreach (var payment in payments)
            {
                var paymentEvent = new PaymentProcessed(
                    payment.Id,
                    payment.TicketId,
                    payment.Amount,
                    payment.PaymentType,
                    payment.ProcessedBy
                );

                await _eventPublisher.PublishAsync(paymentEvent);
            }
        }

        private Money CalculateChange(BatchPaymentRequest request, decimal totalProcessed)
        {
            var change = request.TenderAmount.Amount - totalProcessed;
            return new Money(Math.Max(0, change), request.TenderAmount.Currency);
        }
    }

    public class BatchPaymentRequest
    {
        public List<Guid> TicketIds { get; set; } = new();
        public Money TotalAmount { get; set; } = new(0m, "USD");
        public Money TenderAmount { get; set; } = new(0m, "USD");
        public PaymentType PaymentType { get; set; }
        public UserId ProcessedBy { get; set; } = null!;
        public Guid TerminalId { get; set; }
        public string GlobalId { get; set; } = string.Empty;
        public string? AuthCode { get; set; }
        public string? CardType { get; set; }
        public string? AuthMethod { get; set; }
    }

    public class BatchPaymentResult
    {
        public bool Success { get; set; }
        public List<Guid> TicketIds { get; set; } = new();
        public List<Guid> PaymentIds { get; set; } = new();
        public decimal TotalProcessed { get; set; }
        public Money ChangeAmount { get; set; } = new(0m, "USD");
        public string? ErrorMessage { get; set; }
    }

    internal class PaymentDistribution
    {
        public Guid TicketId { get; set; }
        public Money Amount { get; set; } = new(0m, "USD");
        public Money DueAmount { get; set; } = new(0m, "USD");
    }
}
