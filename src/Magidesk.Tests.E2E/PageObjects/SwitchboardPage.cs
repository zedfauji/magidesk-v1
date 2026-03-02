using FlaUI.Core.AutomationElements;

namespace Magidesk.Tests.E2E.PageObjects;

/// <summary>
/// Page object for the switchboard (main menu) page.
/// </summary>
public sealed class SwitchboardPage : BasePage
{
    // Button labels from SwitchboardViewModel
    private const string NewTicketButtonLabel = "New Ticket";
    private const string OpenTicketsButtonLabel = "Open Tickets";
    private const string TablesButtonLabel = "Tables";
    private const string KitchenDisplayButtonLabel = "Kitchen Display";
    private const string ManagerFunctionsButtonLabel = "Manager Functions";
    private const string BackOfficeButtonLabel = "Back Office";
    private const string CashDropButtonLabel = "Cash Drop";
    private const string DrawerPullButtonLabel = "Drawer Pull";
    private const string ClockInButtonLabel = "Clock In";
    private const string ClockOutButtonLabel = "Clock Out";
    private const string LogoutButtonLabel = "Logout";

    public SwitchboardPage(Window window) : base(window)
    {
    }

    public void NavigateToOrderEntry()
    {
        ClickButtonByName(NewTicketButtonLabel);
    }

    public void NavigateToOpenTickets()
    {
        ClickButtonByName(OpenTicketsButtonLabel);
    }

    public void NavigateToTables()
    {
        ClickButtonByName(TablesButtonLabel);
    }

    public void NavigateToKitchenDisplay()
    {
        ClickButtonByName(KitchenDisplayButtonLabel);
    }

    public void NavigateToManagerFunctions()
    {
        ClickButtonByName(ManagerFunctionsButtonLabel);
    }

    public void NavigateToBackOffice()
    {
        ClickButtonByName(BackOfficeButtonLabel);
    }

    public void NavigateToCashDrop()
    {
        ClickButtonByName(CashDropButtonLabel);
    }

    public void NavigateToDrawerPull()
    {
        ClickButtonByName(DrawerPullButtonLabel);
    }

    public void ClockIn()
    {
        ClickButtonByName(ClockInButtonLabel);
    }

    public void ClockOut()
    {
        ClickButtonByName(ClockOutButtonLabel);
    }

    public void Logout()
    {
        ClickButtonByName(LogoutButtonLabel);
    }

    // Legacy method names for backward compatibility with existing tests
    public void NavigateToSettlement()
    {
        // Settlement is accessed through Manager Functions or after creating a ticket
        // For now, navigate to Manager Functions as a placeholder
        NavigateToManagerFunctions();
    }

    public void NavigateToCashSession()
    {
        // Cash session management is accessed through Manager Functions
        NavigateToManagerFunctions();
    }

    public void NavigateToReports()
    {
        // Reports are accessed through Back Office
        NavigateToBackOffice();
    }

    public string GetCurrentUserName()
    {
        return GetText("CurrentUserDisplay");
    }

    public int GetOpenTicketCount()
    {
        var text = GetText("OpenTicketCountDisplay");
        return int.TryParse(text, out var count) ? count : 0;
    }

    public int GetActiveSessionCount()
    {
        var text = GetText("ActiveSessionCountDisplay");
        return int.TryParse(text, out var count) ? count : 0;
    }
}
