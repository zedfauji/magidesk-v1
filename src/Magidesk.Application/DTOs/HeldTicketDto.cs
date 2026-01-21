namespace Magidesk.Application.DTOs;

/// <summary>
/// DTO for held ticket information.
/// </summary>
public record HeldTicketDto(
    Guid Id,
    int TicketNumber,
    DateTime HeldAt,
    string HoldReason,
    string HeldByUserName,
    decimal TotalAmount,
    string? CustomerName,
    int? TableNumber
);
