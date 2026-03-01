using FlaUI.Core.AutomationElements;

namespace Magidesk.Tests.E2E.PageObjects;

/// <summary>
/// Page object for the switchboard (main menu) page.
/// </summary>
public sealed class SwitchboardPage : BasePage
{
    private const string OrderEntryButtonId = "OrderEntryButton";
    private const string SettlementButtonId = "SettlementButton";
    private const string CashSessionButtonId = "CashSessionButton";
    private const string ReportsButtonId = "ReportsButton";
    private const string LogoutButtonId = "LogoutButton";
    private const string CurrentUserTextBlockId = "CurrentUserTextBlock";

    public SwitchboardPage(Window window) : base(window)
    {
    }

    public void NavigateToOrderEntry()
    {
        ClickButton(OrderEntryButtonId);
    }

    public void NavigateToSettlement()
    {
        ClickButton(SettlementButtonId);
    }

    public void NavigateToCashSession()
    {
        ClickButton(CashSessionButtonId);
    }

    public void NavigateToReports()
    {
        ClickButton(ReportsButtonId);
    }

    public void Logout()
    {
        ClickButton(LogoutButtonId);
    }

    public string GetCurrentUserName()
    {
        return GetText(CurrentUserTextBlockId);
    }
}
