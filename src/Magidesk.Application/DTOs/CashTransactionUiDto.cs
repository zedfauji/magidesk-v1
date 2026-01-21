using System;

namespace Magidesk.Application.DTOs;

public class CashTransactionUiDto
{
    public Guid Id { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string ProcessedBy { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public string Type { get; set; } = "Drop"; // Drop or Bleed
    
    public string TimeDisplay => ProcessedAt.ToLocalTime().ToString("HH:mm");
}
