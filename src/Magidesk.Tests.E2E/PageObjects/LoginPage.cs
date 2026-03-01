using FlaUI.Core.AutomationElements;

namespace Magidesk.Tests.E2E.PageObjects;

/// <summary>
/// Page object for the login page with PIN-based authentication.
/// </summary>
public sealed class LoginPage : BasePage
{
    private const string LoginButtonId = "LoginButton";
    private const string ErrorMessageTextBlockId = "ErrorMessageTextBlock";
    private const string PinDisplayTextBlockId = "PinDisplayTextBlock";

    public LoginPage(Window window) : base(window)
    {
    }

    /// <summary>
    /// Enters a PIN by clicking digit buttons.
    /// </summary>
    /// <param name="pin">The PIN to enter (e.g., "1234")</param>
    public void EnterPin(string pin)
    {
        foreach (char digit in pin)
        {
            string buttonId = $"Digit{digit}Button";
            ClickButton(buttonId);
            Thread.Sleep(100); // Small delay between digit presses
        }
    }

    /// <summary>
    /// Clicks the login button to submit the PIN.
    /// </summary>
    public void ClickLogin()
    {
        ClickButton(LoginButtonId);
    }

    /// <summary>
    /// Enters PIN and clicks login in one operation.
    /// </summary>
    /// <param name="pin">The PIN to enter (e.g., "1234")</param>
    public void LoginWithPin(string pin)
    {
        EnterPin(pin);
        ClickLogin();
        Thread.Sleep(500); // Wait for login to process
    }

    public string GetErrorMessage()
    {
        return GetText(ErrorMessageTextBlockId);
    }

    public bool IsLoginButtonEnabled()
    {
        return IsElementEnabled(LoginButtonId);
    }

    public void ClickBackspace()
    {
        ClickButton("BackspaceButton");
    }
}
