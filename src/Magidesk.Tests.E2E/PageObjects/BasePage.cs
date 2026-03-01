using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;

namespace Magidesk.Tests.E2E.PageObjects;

/// <summary>
/// Base class for all page objects providing common UI interaction methods.
/// </summary>
public abstract class BasePage
{
    protected Window Window { get; }
    protected TimeSpan DefaultTimeout { get; }

    protected BasePage(Window window, TimeSpan? defaultTimeout = null)
    {
        Window = window ?? throw new ArgumentNullException(nameof(window));
        DefaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(10);
    }

    protected AutomationElement FindElement(string automationId)
    {
        return Infrastructure.WaitHelpers.WaitForElementByAutomationId(
            Window,
            automationId,
            DefaultTimeout);
    }

    protected void ClickButton(string automationId)
    {
        var button = FindElement(automationId);
        Infrastructure.WaitHelpers.WaitForElementEnabled(button, DefaultTimeout);
        button.AsButton().Invoke();
    }

    protected void EnterText(string automationId, string text)
    {
        var textBox = FindElement(automationId);
        Infrastructure.WaitHelpers.WaitForElementEnabled(textBox, DefaultTimeout);
        textBox.AsTextBox().Text = text;
    }

    protected string GetText(string automationId)
    {
        var element = FindElement(automationId);
        return element.AsTextBox().Text;
    }

    protected bool IsElementEnabled(string automationId)
    {
        try
        {
            var element = FindElement(automationId);
            return element.IsEnabled;
        }
        catch
        {
            return false;
        }
    }

    protected void WaitForElementToDisappear(string automationId)
    {
        Infrastructure.WaitHelpers.WaitForElementToDisappear(
            () => Window.FindFirstDescendant(cf => cf.ByAutomationId(automationId)),
            DefaultTimeout,
            $"Element with AutomationId '{automationId}'");
    }
}
