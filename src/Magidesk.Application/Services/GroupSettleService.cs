using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Enumerations;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

public class GroupSettleService : IGroupSettleService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IPaymentRepository _paymentRepository; // Checks if this exists or I need to create/use generic
    private readonly IGroupSettlementRepository _groupSettlementRepository;
    private readonly ICashSessionRepository _cashSessionRepository;

    public GroupSettleService(
        ITicketRepository ticketRepository,
        IPaymentRepository paymentRepository,
        IGroupSettlementRepository groupSettlementRepository,
        ICashSessionRepository cashSessionRepository)
    {
        _ticketRepository = ticketRepository;
        _paymentRepository = paymentRepository;
        _groupSettlementRepository = groupSettlementRepository;
        _cashSessionRepository = cashSessionRepository;
    }

    public async Task<GroupSettlement> SettleGroupAsync(
        List<Guid> ticketIds,
        PaymentType paymentType,
        Money totalAmount,
        UserId processedBy,
        Guid terminalId,
        string? globalId = null)
    {
        // 1. Validate
        if (ticketIds == null || !ticketIds.Any())
            throw new ArgumentException("No tickets provided for group settlement.");

        var tickets = new List<Ticket>();
        foreach (var id in ticketIds)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null) throw new Exception($"Ticket {id} not found.");
            if (ticket.Status == TicketStatus.Paid || ticket.Status == TicketStatus.Closed)
                 throw new Exception($"Ticket {ticket.TicketNumber} is already paid.");
            tickets.Add(ticket);
        }

        // 2. Calculate Split (Simple Equal Split for MVP)
        int count = tickets.Count;
        decimal totalDecimal = totalAmount.Amount;
        decimal baseShare = Math.Floor((totalDecimal / count) * 100) / 100; // Truncate to 2 decimals
        decimal remainder = totalDecimal - (baseShare * count);

        // 3. Process Each Ticket
        var payments = new List<Payment>();
        CashSession? activeSession = null;

        // If Cash, verify session
        if (paymentType == PaymentType.Cash)
        {
            activeSession = await _cashSessionRepository.GetOpenSessionByTerminalIdAsync(terminalId);
            if (activeSession == null)
                throw new Exception($"No active cash session found for terminal {terminalId}.");
        }

        for (int i = 0; i < count; i++)
        {
            var ticket = tickets[i];
            decimal shareAmount = baseShare;
            
            // Distribute pennies to the first N tickets
            if (remainder > 0) 
            {
                shareAmount += 0.01m;
                remainder -= 0.01m;
            }

            var moneyShare = new Money(shareAmount, totalAmount.Currency);

            // Create Payment
            Payment payment;
            if (paymentType == PaymentType.Cash)
            {
                 payment = CashPayment.Create(
                    ticket.Id,
                    moneyShare,
                    processedBy,
                    terminalId,
                    globalId: globalId ?? Guid.NewGuid().ToString() // Distinct ID if not provided, or share?
                    // Usually for group settle, if it's one card swipe, globalId matches.
                 );
                 // Need to add to session
                 // Reflection approach or public method?
                 // CashSession.AddPayment requires the object.
                 // Payment.SetCashSessionId requires reflection or protected access.
                 // Domain Service would be better here, but for MVP mirroring Handler:
                 
                 // accessing protected SetCashSessionId via reflection
                 var setCashSessionMethod = typeof(Payment).GetMethod("SetCashSessionId", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                 setCashSessionMethod?.Invoke(payment, new object[] { activeSession.Id });
                 
                 activeSession.AddPayment(payment);
            }
            else
            {
                // Generalized for CreditCard etc if needed, simplified for MVP structure
                 payment = Payment.Create(
                    ticket.Id,
                    paymentType,
                    moneyShare,
                    processedBy,
                    terminalId,
                    globalId
                );
            }

            ticket.AddPayment(payment);
            payments.Add(payment);

            // Persistence
            await _paymentRepository.AddAsync(payment);
            await _ticketRepository.UpdateAsync(ticket);
        }

        if (activeSession != null)
        {
            await _cashSessionRepository.UpdateAsync(activeSession);
        }

        // 4. Create Group Settlement Record
        // Use the first payment as "Master" for reference, or none?
        // Entity says `MasterPaymentId`.
        var masterId = payments.First().Id;
        
        var groupSettlement = new GroupSettlement(masterId, ticketIds);
        
        await _groupSettlementRepository.AddAsync(groupSettlement);

        return groupSettlement;
    }
}
