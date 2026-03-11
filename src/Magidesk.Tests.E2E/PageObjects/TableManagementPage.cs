using FlaUI.Core.AutomationElements;

namespace Magidesk.Tests.E2E.PageObjects;

/// <summary>
/// Page object for table management operations including pool tables and dining tables.
/// </summary>
public sealed class TableManagementPage : BasePage
{
    // Pool table control AutomationIds
    private const string StartPoolTableButtonId = "StartPoolTableButton";
    private const string StopPoolTableButtonId = "StopPoolTableButton";
    private const string PausePoolTableButtonId = "PausePoolTableButton";
    private const string ResumePoolTableButtonId = "ResumePoolTableButton";
    private const string PoolTableElapsedTimeTextBlockId = "PoolTableElapsedTimeTextBlock";
    private const string PoolTableNumberTextBoxId = "PoolTableNumberTextBox";

    // Dining table control AutomationIds
    private const string AssignDiningTableButtonId = "AssignDiningTableButton";
    private const string TransferTableButtonId = "TransferTableButton";
    private const string ClearTableButtonId = "ClearTableButton";
    private const string TableNumberTextBoxId = "TableNumberTextBox";
    private const string TicketIdTextBoxId = "TicketIdTextBox";
    private const string NewServerTextBoxId = "NewServerTextBox";
    private const string TableStatusTextBlockId = "TableStatusTextBlock";

    // Floor map navigation
    private const string FloorMapButtonId = "FloorMapButton";
    private const string FloorNameTextBoxId = "FloorNameTextBox";

    public TableManagementPage(Window window) : base(window)
    {
    }

    /// <summary>
    /// Starts the timer for a pool table.
    /// </summary>
    /// <param name="tableNumber">The pool table number to start.</param>
    public void StartPoolTable(string tableNumber)
    {
        EnterText(PoolTableNumberTextBoxId, tableNumber);
        ClickButton(StartPoolTableButtonId);
    }

    /// <summary>
    /// Stops the timer for a pool table and adds charges to the ticket.
    /// </summary>
    /// <param name="tableNumber">The pool table number to stop.</param>
    public void StopPoolTable(string tableNumber)
    {
        EnterText(PoolTableNumberTextBoxId, tableNumber);
        ClickButton(StopPoolTableButtonId);
    }

    /// <summary>
    /// Pauses the timer for a pool table.
    /// </summary>
    /// <param name="tableNumber">The pool table number to pause.</param>
    public void PausePoolTable(string tableNumber)
    {
        EnterText(PoolTableNumberTextBoxId, tableNumber);
        ClickButton(PausePoolTableButtonId);
    }

    /// <summary>
    /// Resumes the timer for a paused pool table.
    /// </summary>
    /// <param name="tableNumber">The pool table number to resume.</param>
    public void ResumePoolTable(string tableNumber)
    {
        EnterText(PoolTableNumberTextBoxId, tableNumber);
        ClickButton(ResumePoolTableButtonId);
    }

    /// <summary>
    /// Gets the elapsed time for a pool table.
    /// </summary>
    /// <param name="tableNumber">The pool table number.</param>
    /// <returns>The elapsed time as a TimeSpan.</returns>
    public TimeSpan GetPoolTableElapsedTime(string tableNumber)
    {
        EnterText(PoolTableNumberTextBoxId, tableNumber);
        var elapsedText = GetText(PoolTableElapsedTimeTextBlockId);
        return TimeSpan.Parse(elapsedText);
    }

    /// <summary>
    /// Assigns a dining table to a ticket.
    /// </summary>
    /// <param name="tableNumber">The dining table number.</param>
    /// <param name="ticketId">The ticket ID to associate with the table.</param>
    public void AssignDiningTable(string tableNumber, string ticketId)
    {
        EnterText(TableNumberTextBoxId, tableNumber);
        EnterText(TicketIdTextBoxId, ticketId);
        ClickButton(AssignDiningTableButtonId);
    }

    /// <summary>
    /// Transfers a table to a new server.
    /// </summary>
    /// <param name="tableNumber">The table number to transfer.</param>
    /// <param name="newServer">The new server name or ID.</param>
    public void TransferTable(string tableNumber, string newServer)
    {
        EnterText(TableNumberTextBoxId, tableNumber);
        EnterText(NewServerTextBoxId, newServer);
        ClickButton(TransferTableButtonId);
    }

    /// <summary>
    /// Clears a table, making it available.
    /// </summary>
    /// <param name="tableNumber">The table number to clear.</param>
    public void ClearTable(string tableNumber)
    {
        EnterText(TableNumberTextBoxId, tableNumber);
        ClickButton(ClearTableButtonId);
    }

    /// <summary>
    /// Gets the current status of a table.
    /// </summary>
    /// <param name="tableNumber">The table number.</param>
    /// <returns>The table status.</returns>
    public TableStatus GetTableStatus(string tableNumber)
    {
        EnterText(TableNumberTextBoxId, tableNumber);
        var statusText = GetText(TableStatusTextBlockId);
        
        return statusText.ToLowerInvariant() switch
        {
            "available" => TableStatus.Available,
            "occupied" => TableStatus.Occupied,
            "reserved" => TableStatus.Reserved,
            _ => throw new InvalidOperationException($"Unknown table status: {statusText}")
        };
    }

    /// <summary>
    /// Navigates to the floor map for a specific floor.
    /// </summary>
    /// <param name="floorName">The name of the floor to display.</param>
    public void NavigateToFloorMap(string floorName)
    {
        EnterText(FloorNameTextBoxId, floorName);
        ClickButton(FloorMapButtonId);
    }
}

/// <summary>
/// Represents the status of a table.
/// </summary>
public enum TableStatus
{
    Available,
    Occupied,
    Reserved
}
