namespace Magidesk.Application.Commands;

/// <summary>
/// Command to print order lines to kitchen.
/// </summary>
public class PrintToKitchenCommand
{
    public Guid TicketId { get; set; }
    public Guid? OrderLineId { get; set; } // If null, prints all unprinted order lines
}

/// <summary>
/// Result of printing to kitchen.
/// </summary>
public class PrintToKitchenResult
{
    public int OrderLinesPrinted { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}

