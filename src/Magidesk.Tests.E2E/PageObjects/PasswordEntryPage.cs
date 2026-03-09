using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;

namespace Magidesk.Tests.E2E.PageObjects;

/// <summary>
/// Page object for the Password Entry dialog (PasswordEntryDialog.xaml).
/// This dialog appears when accessing manager functions like Back Office.
/// Uses a numeric keypad with buttons identified by their Content property.
/// </summary>
public sealed class PasswordEntryPage : BasePage
{
    public PasswordEntryPage(Window window) : base(window)
    {
    }

    /// <summary>
    /// Waits for the Password Entry dialog to be visible by waiting for a numpad button.
    /// </summary>
    public void WaitForDialogVisible()
    {
        // Wait for any numpad button to appear (using button with content "1")
        Infrastructure.WaitHelpers.WaitUntil(
            () => Window.FindFirstDescendant(cf => 
                cf.ByControlType(ControlType.Button)
                  .And(cf.ByName("1"))) != null,
            DefaultTimeout,
            "Password Entry dialog did not appear");
    }

    /// <summary>
    /// Enters a PIN by clicking numpad buttons, then clicks OK.
    /// </summary>
    public void EnterPinAndConfirm(string pin)
    {
        foreach (char digit in pin)
        {
            if (!char.IsDigit(digit))
            {
                throw new ArgumentException($"Invalid PIN digit: '{digit}'. Only 0-9 are supported.");
            }

            // Find button by its Content (the digit)
            var button = Window.FindFirstDescendant(cf => 
                cf.ByControlType(ControlType.Button)
                  .And(cf.ByName(digit.ToString())));

            if (button == null)
            {
                throw new InvalidOperationException($"Could not find numpad button for digit '{digit}'");
            }

            button.AsButton().Invoke();
            Thread.Sleep(100); // Small delay between clicks
        }

        // Click OK button (PrimaryButton of ContentDialog)
        var okButton = Window.FindFirstDescendant(cf => 
            cf.ByControlType(ControlType.Button)
              .And(cf.ByName("OK")));

        if (okButton != null)
        {
            okButton.AsButton().Invoke();
            Thread.Sleep(500); // Allow dialog dismissal
        }
        else
        {
            throw new InvalidOperationException("Could not find OK button in Password Entry dialog");
        }
    }
}
