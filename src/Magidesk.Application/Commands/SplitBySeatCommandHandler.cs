using Magidesk.Application.Commands;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Domain.Entities;
using Magidesk.Domain.Events;
using Magidesk.Domain.ValueObjects;
using Magidesk.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Magidesk.Application.Commands
{
    public class SplitBySeatCommandHandler : ICommandHandler<SplitBySeatCommand, SplitBySeatResult>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IEventPublisher _eventPublisher;

        public SplitBySeatCommandHandler(
            ITicketRepository ticketRepository,
            IEventPublisher eventPublisher)
        {
            _ticketRepository = ticketRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task<SplitBySeatResult> HandleAsync(SplitBySeatCommand command, CancellationToken cancellationToken = default)
        {
            // Get the original ticket
            var originalTicket = await _ticketRepository.GetByIdAsync(command.OriginalTicketId, cancellationToken);
            if (originalTicket == null)
            {
                return new SplitBySeatResult
                {
                    Success = false,
                    OriginalTicketId = command.OriginalTicketId,
                    ErrorMessage = "Original ticket not found"
                };
            }

            // Validate ticket can be split
            var validationResult = ValidateTicketForSplit(originalTicket);
            if (!validationResult.IsValid)
            {
                return new SplitBySeatResult
                {
                    Success = false,
                    OriginalTicketId = command.OriginalTicketId,
                    ErrorMessage = validationResult.ErrorMessage
                };
            }

            // Group items by seat
            var seatGroups = GroupItemsBySeat(originalTicket.OrderLines);
            if (seatGroups.Count <= 1)
            {
                return new SplitBySeatResult
                {
                    Success = false,
                    OriginalTicketId = command.OriginalTicketId,
                    ErrorMessage = "Cannot split: only one seat or all items unassigned"
                };
            }

            // Start transaction
            using var transaction = await _ticketRepository.BeginTransactionAsync(cancellationToken);
            
            try
            {
                var newTickets = new List<Ticket>();
                var seatToTicketMapping = new Dictionary<int, Guid>();

                // Create new tickets for each seat
                foreach (var seatGroup in seatGroups)
                {
                    var newTicket = CreateTicketForSeat(originalTicket, seatGroup.Key, seatGroup.Value, command.ProcessedBy, command.TerminalId);
                    await _ticketRepository.AddAsync(newTicket, cancellationToken);
                    newTickets.Add(newTicket);
                    seatToTicketMapping[seatGroup.Key] = newTicket.Id;
                }

                // Close original ticket
                originalTicket.Close(command.ProcessedBy);
                await _ticketRepository.UpdateAsync(originalTicket, cancellationToken);

                // Publish audit event
                await PublishSplitBySeatEvent(command, originalTicket, newTickets, seatToTicketMapping);

                // Commit transaction
                await transaction.CommitAsync(cancellationToken);

                return new SplitBySeatResult
                {
                    Success = true,
                    OriginalTicketId = command.OriginalTicketId,
                    NewTicketIds = newTickets.Select(t => t.Id).ToList(),
                    SeatToTicketMapping = seatToTicketMapping
                };
            }
            catch (Exception)
            {
                // Rollback on any failure
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private SplitValidationResult ValidateTicketForSplit(Ticket ticket)
        {
            if (ticket.Status != TicketStatus.Open)
            {
                return new SplitValidationResult { IsValid = false, ErrorMessage = "Ticket is not open" };
            }

            if (ticket.Payments.Any())
            {
                return new SplitValidationResult { IsValid = false, ErrorMessage = "Cannot split ticket with payments" };
            }

            if (!ticket.OrderLines.Any())
            {
                return new SplitValidationResult { IsValid = false, ErrorMessage = "Ticket has no items to split" };
            }

            return new SplitValidationResult { IsValid = true };
        }

        private Dictionary<int, List<OrderLine>> GroupItemsBySeat(IReadOnlyCollection<OrderLine> orderLines)
        {
            var seatGroups = new Dictionary<int, List<OrderLine>>();
            
            foreach (var orderLine in orderLines)
            {
                var seatNumber = orderLine.SeatNumber ?? 0; // Unassigned items go to seat 0
                
                if (!seatGroups.ContainsKey(seatNumber))
                {
                    seatGroups[seatNumber] = new List<OrderLine>();
                }
                
                seatGroups[seatNumber].Add(orderLine);
            }

            return seatGroups;
        }

        private Ticket CreateTicketForSeat(Ticket originalTicket, int seatNumber, List<OrderLine> items, UserId processedBy, Guid terminalId)
        {
            // Get next ticket number from repository (for now, use a placeholder)
            var nextTicketNumber = originalTicket.TicketNumber + 1; // This should come from repository

            var newTicket = Ticket.Create(
                nextTicketNumber,
                processedBy,
                terminalId,
                originalTicket.ShiftId,
                originalTicket.OrderTypeId,
                Guid.NewGuid().ToString()
            );

            // Open the ticket
            newTicket.Open();

            // Add items to new ticket
            foreach (var item in items)
            {
                // Create new order line for the new ticket using the factory method
                var newItem = OrderLine.Create(
                    newTicket.Id,
                    item.MenuItemId,
                    item.MenuItemName,
                    item.Quantity,
                    item.UnitPrice,
                    item.TaxRate,
                    item.CategoryName,
                    item.GroupName
                );

                // Copy modifiers if any
                foreach (var modifier in item.Modifiers)
                {
                    newItem.AddModifier(modifier);
                }

                newTicket.AddOrderLine(newItem);
            }

            return newTicket;
        }

        private async Task PublishSplitBySeatEvent(SplitBySeatCommand command, Ticket originalTicket, List<Ticket> newTickets, Dictionary<int, Guid> seatToTicketMapping)
        {
            var splitEvent = new TicketSplitBySeatEvent(
                originalTicket.Id,
                newTickets.Select(t => t.Id).ToList(),
                seatToTicketMapping.Count,
                seatToTicketMapping.ToDictionary(
                    kvp => kvp.Key,
                    kvp => newTickets.First(t => t.Id == kvp.Value).OrderLines.Count
                ),
                command.ProcessedBy,
                Guid.NewGuid() // correlation ID
            );

            await _eventPublisher.PublishAsync(splitEvent);
        }
    }

    internal class SplitValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
