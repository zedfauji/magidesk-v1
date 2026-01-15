namespace Magidesk.Api.Dtos.Orders;

using Magidesk.Api.Dtos.Sessions; // For DraftOrderLineDto

public class AddLinesRequest
{
    public List<DraftOrderLineDto> Items { get; set; } = new();
}

public class CreateTicketRequest
{
    public string TableId { get; set; } = string.Empty;
    public int GuestCount { get; set; } = 1;
}

public class TicketResultDto
{
    public bool Success { get; set; }
    public string TicketId { get; set; } = string.Empty;
    public int UpdatedVersion { get; set; }
}
