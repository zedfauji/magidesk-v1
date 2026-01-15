namespace Magidesk.Api.Dtos.Sessions;

// Used as a return type for EndSession and GetTicket (which returns ActiveSession state)
public class ActiveSessionDto
{
    public string TableId { get; set; } = string.Empty;
    public string TicketId { get; set; } = string.Empty;
    public string TicketNumber { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty; // ISO
    public bool IsPaused { get; set; }
    public decimal HourlyRate { get; set; }
    
    public string DraftState { get; set; } = "Idle"; // Idle, Dirty, Submitting, Error
    public List<DraftOrderLineDto> DraftItems { get; set; } = new();
    public List<CommittedOrderLineDto> CommittedItems { get; set; } = new();
    
    public SessionTotalsDto Totals { get; set; } = new();
    public int Version { get; set; }
}

public class SessionTotalsDto
{
    public decimal SessionTimeAmount { get; set; }
    public decimal FnBSubtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal GrandTotal { get; set; }
}

public class DraftOrderLineDto
{
    public string TempId { get; set; } = string.Empty;
    public string MenuItemId { get; set; } = string.Empty;
    public string? Name { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public List<SelectedModifierDto> Modifiers { get; set; } = new();
    public string? Instructions { get; set; }
}

public class SelectedModifierDto
{
    public string GroupId { get; set; } = string.Empty;
    public string OptionId { get; set; } = string.Empty;
    public decimal PriceDelta { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CommittedOrderLineDto
{
    public string Id { get; set; } = string.Empty;
    public string MenuItemId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
    public int Version { get; set; }
    public List<SelectedModifierDto> Modifiers { get; set; } = new();
}
