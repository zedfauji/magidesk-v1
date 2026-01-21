using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.DTOs;

/// <summary>
/// Data transfer object for Ticket entity.
/// </summary>
public class TicketDto
{
    public Guid Id { get; set; }
    public int TicketNumber { get; set; }
    public string? GlobalId { get; set; }
    public string? TableName { get; set; } // Added for UI
    public string OwnerName { get; set; } = string.Empty; // Added for UI
    public string? Note { get; set; } // F-0125
    public DateTime CreatedAt { get; set; }
    public DateTime? OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public DateTime ActiveDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public TicketStatus Status { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? ClosedBy { get; set; }
    public Guid? VoidedBy { get; set; }
    public Guid TerminalId { get; set; }
    public Guid ShiftId { get; set; }
    public Guid OrderTypeId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? AssignedDriverId { get; set; }
    public List<int> TableNumbers { get; set; } = new();
    public int NumberOfGuests { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ServiceChargeAmount { get; set; }
    public decimal DeliveryChargeAmount { get; set; }
    public decimal AdjustmentAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public decimal AdvanceAmount { get; set; }
    public bool IsTaxExempt { get; set; }
    public bool IsBarTab { get; set; }
    public bool IsReOpened { get; set; }
    public string? DeliveryAddress { get; set; }
    public string? ExtraDeliveryInfo { get; set; }
    public bool CustomerWillPickup { get; set; }
    public List<OrderLineDto> OrderLines { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
    public GratuityDto? Gratuity { get; set; }
    
    // Session-related properties (BE-A.15-03)
    public Guid? SessionId { get; set; }
    public DateTime? SessionStartTime { get; set; }
    public decimal? SessionHourlyRate { get; set; }
    public TimeSpan? SessionElapsedTime { get; set; }
    public decimal? SessionRunningCharge { get; set; }
    public TableSessionStatus? SessionStatus { get; set; } // BE-A.16-01
    public DateTime? SessionPausedAt { get; set; } // BE-A.16-01
    public bool HasActiveSession => SessionId.HasValue;
    
    public int Version { get; set; }
}

