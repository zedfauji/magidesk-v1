using FlaUI.Core.AutomationElements;

namespace Magidesk.Tests.E2E.PageObjects;

/// <summary>
/// Page object for the settlement (payment) page.
/// </summary>
public sealed class SettlementPage : BasePage
{
    private const string PaymentMethodComboBoxId = "PaymentMethodComboBox";
    private const string PaymentAmountTextBoxId = "PaymentAmountTextBox";
    private const string ProcessPaymentButtonId = "ProcessPaymentButton";
    private const string SplitPaymentButtonId = "SplitPaymentButton";
    private const string VoidTicketButtonId = "VoidTicketButton";
    private const string TicketTotalTextBlockId = "TicketTotalTextBlock";
    private const string AmountDueTextBlockId = "AmountDueTextBlock";
    private const string AmountPaidTextBlockId = "AmountPaidTextBlock";

    public SettlementPage(Window window) : base(window)
    {
    }

    public void SelectPaymentMethod(string methodName)
    {
        var comboBox = FindElement(PaymentMethodComboBoxId).AsComboBox();
        comboBox.Select(methodName);
    }

    public void EnterPaymentAmount(decimal amount)
    {
        EnterText(PaymentAmountTextBoxId, amount.ToString("F2"));
    }

    public void ProcessPayment()
    {
        ClickButton(ProcessPaymentButtonId);
    }

    public void SplitPayment()
    {
        ClickButton(SplitPaymentButtonId);
    }

    public void VoidTicket()
    {
        ClickButton(VoidTicketButtonId);
    }

    public decimal GetTicketTotal()
    {
        var totalText = GetText(TicketTotalTextBlockId);
        return decimal.Parse(totalText.Replace("$", "").Trim());
    }

    public decimal GetAmountDue()
    {
        var dueText = GetText(AmountDueTextBlockId);
        return decimal.Parse(dueText.Replace("$", "").Trim());
    }

    public decimal GetAmountPaid()
    {
        var paidText = GetText(AmountPaidTextBlockId);
        return decimal.Parse(paidText.Replace("$", "").Trim());
    }

    public bool IsProcessPaymentEnabled()
    {
        return IsElementEnabled(ProcessPaymentButtonId);
    }
}
