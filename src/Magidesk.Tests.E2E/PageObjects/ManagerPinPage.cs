using FlaUI.Core.AutomationElements;

namespace Magidesk.Tests.E2E.PageObjects;

/// <summary>
/// Page object for the Manager PIN Authorization dialog (ManagerPinDialog.xaml).
/// Handles entering a PIN digit-by-digit using the on-screen keypad buttons.
/// </summary>
public sealed class ManagerPinPage : BasePage
{
    // AutomationIds matching ManagerPinDialog.xaml digit buttons
    private const string Btn1 = "Pin_1";
    private const string Btn2 = "Pin_2";
    private const string Btn3 = "Pin_3";
    private const string Btn4 = "Pin_4";
    private const string Btn5 = "Pin_5";
    private const string Btn6 = "Pin_6";
    private const string Btn7 = "Pin_7";
    private const string Btn8 = "Pin_8";
    private const string Btn9 = "Pin_9";
    private const string Btn0 = "Pin_0";
    private const string BtnClear = "Pin_Clear";

    // The ContentDialog primary button "Authorize" is rendered by WinUI as a named button.
    // WinUI ContentDialog renders PrimaryButton with the text as AutomationId fallback;
    // we find it by name text as a reliable cross-version approach.
    private const string AuthorizeButtonName = "Authorize";

    public ManagerPinPage(Window window) : base(window)
    {
    }

    /// <summary>
    /// Waits for the Manager PIN dialog to be visible by waiting for the digit button "1" to appear.
    /// </summary>
    public void WaitForDialogVisible()
    {
        Infrastructure.WaitHelpers.WaitForElementByAutomationId(Window, Btn1, DefaultTimeout);
    }

    /// <summary>
    /// Enters each digit of the PIN by clicking on-screen keypad buttons, then clicks Authorize.
    /// </summary>
    public void EnterPinAndAuthorize(string pin)
    {
        foreach (char digit in pin)
        {
            var buttonId = digit switch
            {
                '0' => Btn0,
                '1' => Btn1,
                '2' => Btn2,
                '3' => Btn3,
                '4' => Btn4,
                '5' => Btn5,
                '6' => Btn6,
                '7' => Btn7,
                '8' => Btn8,
                '9' => Btn9,
                _ => throw new ArgumentException($"Invalid PIN digit: '{digit}'. Only 0-9 are supported.")
            };

            ClickButton(buttonId);
            Thread.Sleep(80); // Small delay between digit clicks for stability
        }

        // Click the Authorize button — WinUI ContentDialog primary button
        // Try by AutomationId "Authorize" first (WinUI sets name as automation fallback)
        try
        {
            var authorizeBtn = Window.FindFirstDescendant(
                cf => cf.ByName(AuthorizeButtonName).And(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button)));
            authorizeBtn?.AsButton().Invoke();
        }
        catch
        {
            // Fall back: find any enabled button with text "Authorize"
            var btn = FindButtonByName(AuthorizeButtonName);
            btn?.Invoke();
        }

        Thread.Sleep(500); // Allow dialog dismissal and navigation
    }

    /// <summary>
    /// Finds a button descendant by its Name property.
    /// </summary>
    private FlaUI.Core.AutomationElements.Button? FindButtonByName(string name)
    {
        try
        {
            var element = Window.FindFirstDescendant(cf => cf.ByName(name));
            return element?.AsButton();
        }
        catch
        {
            return null;
        }
    }
}
