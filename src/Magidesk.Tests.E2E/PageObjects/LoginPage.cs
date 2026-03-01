using FlaUI.Core.AutomationElements;

namespace Magidesk.Tests.E2E.PageObjects;

/// <summary>
/// Page object for the login page.
/// </summary>
public sealed class LoginPage : BasePage
{
    private const string UsernameTextBoxId = "UsernameTextBox";
    private const string PasswordTextBoxId = "PasswordTextBox";
    private const string LoginButtonId = "LoginButton";
    private const string ErrorMessageTextBlockId = "ErrorMessageTextBlock";

    public LoginPage(Window window) : base(window)
    {
    }

    public void EnterUsername(string username)
    {
        EnterText(UsernameTextBoxId, username);
    }

    public void EnterPassword(string password)
    {
        EnterText(PasswordTextBoxId, password);
    }

    public void ClickLogin()
    {
        ClickButton(LoginButtonId);
    }

    public string GetErrorMessage()
    {
        return GetText(ErrorMessageTextBlockId);
    }

    public bool IsLoginButtonEnabled()
    {
        return IsElementEnabled(LoginButtonId);
    }
}
