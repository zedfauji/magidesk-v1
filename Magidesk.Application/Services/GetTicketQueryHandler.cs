using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Application.Queries;
using Magidesk.Domain.ValueObjects;

namespace Magidesk.Application.Services;

/// <summary>
/// Handler for GetTicketQuery.
/// </summary>
public class GetTicketQueryHandler : IQueryHandler<GetTicketQuery, TicketDto?>
{
    private readonly ITicketRepository _ticketRepository;

    public GetTicketQueryHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<TicketDto?> HandleAsync(GetTicketQuery query, CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(query.TicketId, cancellationToken);
        if (ticket == null)
        {
            return null;
        }

        return MapToDto(ticket);
    }

    private static TicketDto MapToDto(Domain.Entities.Ticket ticket)
    {
        return new TicketDto
        {
            Id = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            GlobalId = ticket.GlobalId,
            CreatedAt = ticket.CreatedAt,
            OpenedAt = ticket.OpenedAt,
            ClosedAt = ticket.ClosedAt,
            ActiveDate = ticket.ActiveDate,
            DeliveryDate = ticket.DeliveryDate,
            Status = ticket.Status,
            CreatedBy = ticket.CreatedBy.Value,
            ClosedBy = ticket.ClosedBy?.Value,
            VoidedBy = ticket.VoidedBy?.Value,
            TerminalId = ticket.TerminalId,
            ShiftId = ticket.ShiftId,
            OrderTypeId = ticket.OrderTypeId,
            CustomerId = ticket.CustomerId,
            AssignedDriverId = ticket.AssignedDriverId,
            TableNumbers = ticket.TableNumbers.ToList(),
            NumberOfGuests = ticket.NumberOfGuests,
            SubtotalAmount = ticket.SubtotalAmount.Amount,
            DiscountAmount = ticket.DiscountAmount.Amount,
            TaxAmount = ticket.TaxAmount.Amount,
            ServiceChargeAmount = ticket.ServiceChargeAmount.Amount,
            DeliveryChargeAmount = ticket.DeliveryChargeAmount.Amount,
            AdjustmentAmount = ticket.AdjustmentAmount.Amount,
            TotalAmount = ticket.TotalAmount.Amount,
            PaidAmount = ticket.PaidAmount.Amount,
            DueAmount = ticket.DueAmount.Amount,
            AdvanceAmount = ticket.AdvanceAmount.Amount,
            IsTaxExempt = ticket.IsTaxExempt,
            IsBarTab = ticket.IsBarTab,
            IsReOpened = ticket.IsReOpened,
            DeliveryAddress = ticket.DeliveryAddress,
            ExtraDeliveryInfo = ticket.ExtraDeliveryInfo,
            CustomerWillPickup = ticket.CustomerWillPickup,
            OrderLines = ticket.OrderLines.Select(MapOrderLineToDto).ToList(),
            Payments = ticket.Payments.Select(MapPaymentToDto).ToList(),
            Gratuity = ticket.Gratuity != null ? MapGratuityToDto(ticket.Gratuity) : null,
            Version = ticket.Version
        };
    }

    private static OrderLineDto MapOrderLineToDto(Domain.Entities.OrderLine orderLine)
    {
        return new OrderLineDto
        {
            Id = orderLine.Id,
            TicketId = orderLine.TicketId,
            MenuItemId = orderLine.MenuItemId,
            MenuItemName = orderLine.MenuItemName,
            CategoryName = orderLine.CategoryName,
            GroupName = orderLine.GroupName,
            Quantity = orderLine.Quantity,
            ItemCount = orderLine.ItemCount,
            ItemUnitName = orderLine.ItemUnitName,
            IsFractionalUnit = orderLine.IsFractionalUnit,
            UnitPrice = orderLine.UnitPrice.Amount,
            SubtotalAmount = orderLine.SubtotalAmount.Amount,
            DiscountAmount = orderLine.DiscountAmount.Amount,
            TaxRate = orderLine.TaxRate,
            TaxAmount = orderLine.TaxAmount.Amount,
            TotalAmount = orderLine.TotalAmount.Amount,
            IsBeverage = orderLine.IsBeverage,
            ShouldPrintToKitchen = orderLine.ShouldPrintToKitchen,
            PrintedToKitchen = orderLine.PrintedToKitchen,
            Instructions = orderLine.Instructions, // F-0036
            SeatNumber = orderLine.SeatNumber,
            TreatAsSeat = orderLine.TreatAsSeat,
            Modifiers = orderLine.Modifiers.Select(MapModifierToDto).ToList(),
            AddOns = orderLine.AddOns.Select(MapModifierToDto).ToList(),
            CreatedAt = orderLine.CreatedAt
        };
    }

    private static PaymentDto MapPaymentToDto(Domain.Entities.Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            GlobalId = payment.GlobalId,
            TicketId = payment.TicketId,
            TransactionType = payment.TransactionType,
            PaymentType = payment.PaymentType,
            Amount = payment.Amount.Amount,
            TipsAmount = payment.TipsAmount.Amount,
            TenderAmount = payment.TenderAmount.Amount,
            ChangeAmount = payment.ChangeAmount.Amount,
            TransactionTime = payment.TransactionTime,
            ProcessedBy = payment.ProcessedBy.Value,
            TerminalId = payment.TerminalId,
            IsCaptured = payment.IsCaptured,
            IsVoided = payment.IsVoided,
            IsAuthorizable = payment.IsAuthorizable,
            CashSessionId = payment.CashSessionId,
            Note = payment.Note
        };
    }

    private static GratuityDto MapGratuityToDto(Domain.Entities.Gratuity gratuity)
    {
        return new GratuityDto
        {
            Id = gratuity.Id,
            TicketId = gratuity.TicketId,
            Amount = gratuity.Amount.Amount,
            Paid = gratuity.Paid,
            Refunded = gratuity.Refunded,
            TerminalId = gratuity.TerminalId,
            OwnerId = gratuity.OwnerId.Value,
            CreatedAt = gratuity.CreatedAt
        };
    }

    private static OrderLineModifierDto MapModifierToDto(Domain.Entities.OrderLineModifier modifier)
    {
        return new OrderLineModifierDto
        {
            Id = modifier.Id,
            OrderLineId = modifier.OrderLineId,
            ModifierId = modifier.ModifierId,
            Name = modifier.Name,
            ModifierType = modifier.ModifierType,
            ItemCount = modifier.ItemCount,
            UnitPrice = modifier.UnitPrice.Amount,
            TaxRate = modifier.TaxRate,
            TaxAmount = modifier.TaxAmount.Amount,
            TotalAmount = modifier.TotalAmount.Amount,
            SectionName = modifier.SectionName, // F-0037
            ShouldPrintToKitchen = modifier.ShouldPrintToKitchen,
            CreatedAt = modifier.CreatedAt
        };
    }
}

