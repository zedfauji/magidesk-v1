namespace Magidesk.Presentation.ViewModels;

/// <summary>
/// Navigation context for OrderPageView (formerly OrderEntryPage).
/// Used when navigating from table map or table explorer.
/// </summary>
public class OrderEntryNavigationContext
{
    public Guid? TicketId { get; set; }
    public bool IsExistingTicket { get; set; }

    public OrderEntryNavigationContext(Guid ticketId, bool isExistingTicket = true)
    {
        TicketId = ticketId;
        IsExistingTicket = isExistingTicket;
    }
}
