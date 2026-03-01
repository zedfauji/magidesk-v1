using FlaUI.Core.AutomationElements;

namespace Magidesk.Tests.E2E.PageObjects;

/// <summary>
/// Page object for the cash session management page.
/// </summary>
public sealed class CashSessionPage : BasePage
{
    private const string OpenSessionButtonId = "OpenSessionButton";
    private const string CloseSessionButtonId = "CloseSessionButton";
    private const string CashDropButtonId = "CashDropButton";
    private const string PayoutButtonId = "PayoutButton";
    private const string StartingCashTextBoxId = "StartingCashTextBox";
    private const string ExpectedCashTextBlockId = "ExpectedCashTextBlock";
    private const string ActualCashTextBlockId = "ActualCashTextBlock";
    private const string SessionStatusTextBlockId = "SessionStatusTextBlock";

    public CashSessionPage(Window window) : base(window)
    {
    }

    public void OpenSession(decimal startingCash)
    {
        EnterText(StartingCashTextBoxId, startingCash.ToString("F2"));
        ClickButton(OpenSessionButtonId);
    }

    public void CloseSession()
    {
        ClickButton(CloseSessionButtonId);
    }

    public void RecordCashDrop(decimal amount)
    {
        ClickButton(CashDropButtonId);
        // Additional dialog interaction would go here
    }

    public void RecordPayout(decimal amount, string reason)
    {
        ClickButton(PayoutButtonId);
        // Additional dialog interaction would go here
    }

    public decimal GetExpectedCash()
    {
        var expectedText = GetText(ExpectedCashTextBlockId);
        return decimal.Parse(expectedText.Replace("$", "").Trim());
    }

    public decimal GetActualCash()
    {
        var actualText = GetText(ActualCashTextBlockId);
        return decimal.Parse(actualText.Replace("$", "").Trim());
    }

    public string GetSessionStatus()
    {
        return GetText(SessionStatusTextBlockId);
    }
}
