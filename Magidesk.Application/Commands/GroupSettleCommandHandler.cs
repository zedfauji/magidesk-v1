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

namespace Magidesk.Application.Commands
{
    public class GroupSettleCommandHandler : ICommandHandler<GroupSettleCommand, GroupSettleResult>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICashSessionRepository _cashSessionRepository;

        public GroupSettleCommandHandler(
            ITicketRepository ticketRepository,
            IPaymentRepository paymentRepository,
            IEventPublisher eventPublisher,
            ICashSessionRepository cashSessionRepository)
        {
            _ticketRepository = ticketRepository;
            _paymentRepository = paymentRepository;
            _eventPublisher = eventPublisher;
            _cashSessionRepository = cashSessionRepository;
        }

        public async Task<GroupSettleResult> HandleAsync(GroupSettleCommand command, CancellationToken cancellationToken = default)
        {
            // Validate all tickets exist and are open
            var tickets = new List<Ticket>();
            foreach (var ticketId in command.TicketIds)
            {
                var ticket = await _ticketRepository.GetByIdAsync(ticketId);
                if (ticket == null)
                {
                    throw new InvalidOperationException($"Ticket {ticketId} not found");
                }
                if (ticket.Status != TicketStatus.Open)
                {
                    throw new InvalidOperationException($"Ticket {ticketId} is not open");
                }
                tickets.Add(ticket);
            }

            // Calculate total due amount
            var totalDue = tickets.Sum(t => t.DueAmount.Amount);
            
            // Validate payment amount covers total due
            if (command.Amount.Amount < totalDue)
            {
                throw new InvalidOperationException($"Payment amount {command.Amount.Amount} is less than total due {totalDue}");
            }

            // Start atomic transaction
            using var transaction = await _ticketRepository.BeginTransactionAsync();
            
            try
            {
                var payments = new List<Payment>();
                var settledTickets = new List<Ticket>();

                // Process each ticket atomically
                foreach (var ticket in tickets)
                {
                    // Calculate payment amount for this ticket (proportional)
                    var ticketDue = ticket.DueAmount.Amount;
                    var ticketPaymentAmount = ticketDue;

                    // Create payment for this ticket
                    var payment = CreatePaymentForTicket(command, ticket, ticketPaymentAmount);
                    await _paymentRepository.AddAsync(payment);
                    payments.Add(payment);

                    // Update ticket with payment
                    ticket.AddPayment(payment);
                    await _ticketRepository.UpdateAsync(ticket);
                    settledTickets.Add(ticket);
                }

                // Publish GROUP_SETTLE audit event
                await PublishGroupSettleEvent(command, tickets, payments);

                // Commit transaction
                await transaction.CommitAsync();

                return new GroupSettleResult
                {
                    Success = true,
                    TicketIds = command.TicketIds,
                    PaymentIds = payments.Select(p => p.Id).ToList(),
                    TotalAmount = command.Amount,
                    ChangeAmount = new Money(command.TenderAmount.Amount - totalDue, command.TenderAmount.Currency)
                };
            }
            catch (Exception)
            {
                // Rollback on any failure
                await transaction.RollbackAsync();
                throw;
            }
        }

        private Payment CreatePaymentForTicket(GroupSettleCommand command, Ticket ticket, decimal amount)
        {
            Payment payment;

            if (command.PaymentType == PaymentType.Cash)
            {
                payment = CashPayment.Create(
                    ticket.Id,
                    new Money(amount, command.Amount.Currency),
                    command.ProcessedBy,
                    command.TerminalId,
                    command.GlobalId
                );
            }
            else if (command.PaymentType == PaymentType.CreditCard)
            {
                payment = CreditCardPayment.Create(
                    ticket.Id,
                    new Money(amount, command.Amount.Currency),
                    command.ProcessedBy,
                    command.TerminalId,
                    null, // cardNumber
                    "Group Settlement",
                    command.AuthCode,
                    null, // referenceNumber
                    command.CardType,
                    command.GlobalId
                );
                
                // Authorize the payment if auth code provided
                if (!string.IsNullOrEmpty(command.AuthCode))
                {
                    ((CreditCardPayment)payment).Authorize(command.AuthCode, null, command.CardType);
                }
            }
            else
            {
                // For other payment types, create a basic payment (this would need to be extended)
                throw new NotSupportedException($"Payment type {command.PaymentType} is not supported for group settlement");
            }

            return payment;
        }

        private async Task PublishGroupSettleEvent(GroupSettleCommand command, List<Ticket> tickets, List<Payment> payments)
        {
            var groupSettleEvent = new GroupSettled(
                command.TicketIds,
                command.Amount,
                command.PaymentType,
                command.ProcessedBy,
                payments.Select(p => p.Id).ToList()
            );

            await _eventPublisher.PublishAsync(groupSettleEvent);
        }
    }

    public class GroupSettleResult
    {
        public bool Success { get; set; }
        public List<Guid> TicketIds { get; set; } = new();
        public List<Guid> PaymentIds { get; set; } = new();
        public Money TotalAmount { get; set; } = new(0m, "USD");
        public Money ChangeAmount { get; set; } = new(0m, "USD");
        public string? ErrorMessage { get; set; }
    }
}
