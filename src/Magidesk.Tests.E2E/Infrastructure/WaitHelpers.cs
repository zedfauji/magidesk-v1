using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;

namespace Magidesk.Tests.E2E.Infrastructure;

/// <summary>
/// Provides deterministic waiting strategies for UI automation tests.
/// All methods use retry + timeout pattern with detailed error messages.
/// </summary>
public static class WaitHelpers
{
    private const int DefaultPollingIntervalMs = 100;

    /// <summary>
    /// Waits until a condition becomes true within the specified timeout.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="timeout">Maximum time to wait.</param>
    /// <param name="errorMessage">Custom error message if timeout occurs.</param>
    /// <exception cref="TimeoutException">Thrown when condition is not met within timeout.</exception>
    public static void WaitUntil(Func<bool> condition, TimeSpan timeout, string? errorMessage = null)
    {
        if (condition == null)
            throw new ArgumentNullException(nameof(condition));

        var endTime = DateTime.UtcNow.Add(timeout);
        var lastException = default(Exception);

        while (DateTime.UtcNow < endTime)
        {
            try
            {
                if (condition())
                    return;
            }
            catch (Exception ex)
            {
                lastException = ex;
            }

            Task.Delay(DefaultPollingIntervalMs).Wait();
        }

        var message = errorMessage ?? $"Condition was not met within {timeout.TotalSeconds:F1} seconds.";
        if (lastException != null)
            message += $" Last exception: {lastException.Message}";

        throw new TimeoutException(message);
    }

    /// <summary>
    /// Waits for an element to exist and be available by AutomationId.
    /// </summary>
    /// <param name="parent">The parent element to search within.</param>
    /// <param name="automationId">The AutomationId of the element to find.</param>
    /// <param name="timeout">Maximum time to wait.</param>
    /// <returns>The found element.</returns>
    /// <exception cref="TimeoutException">Thrown when element is not found within timeout.</exception>
    public static AutomationElement WaitForElementByAutomationId(
        AutomationElement parent,
        string automationId,
        TimeSpan timeout)
    {
        if (parent == null)
            throw new ArgumentNullException(nameof(parent));
        if (string.IsNullOrWhiteSpace(automationId))
            throw new ArgumentException("AutomationId cannot be null or empty.", nameof(automationId));

        AutomationElement? element = null;
        var endTime = DateTime.UtcNow.Add(timeout);

        while (DateTime.UtcNow < endTime)
        {
            try
            {
                element = parent.FindFirstDescendant(cf => cf.ByAutomationId(automationId));
                if (element != null && element.IsAvailable)
                    return element;
            }
            catch
            {
                // Element not ready yet
            }

            Task.Delay(DefaultPollingIntervalMs).Wait();
        }

        throw new TimeoutException(
            $"Element with AutomationId '{automationId}' was not found or not available within {timeout.TotalSeconds:F1} seconds. " +
            $"Parent: {GetElementDescription(parent)}");
    }

    /// <summary>
    /// Waits for an element to exist and be available by Name.
    /// </summary>
    /// <param name="parent">The parent element to search within.</param>
    /// <param name="name">The Name of the element to find.</param>
    /// <param name="timeout">Maximum time to wait.</param>
    /// <returns>The found element.</returns>
    /// <exception cref="TimeoutException">Thrown when element is not found within timeout.</exception>
    public static AutomationElement WaitForElementByName(
        AutomationElement parent,
        string name,
        TimeSpan timeout)
    {
        if (parent == null)
            throw new ArgumentNullException(nameof(parent));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));

        AutomationElement? element = null;
        var endTime = DateTime.UtcNow.Add(timeout);

        while (DateTime.UtcNow < endTime)
        {
            try
            {
                element = parent.FindFirstDescendant(cf => cf.ByName(name));
                if (element != null && element.IsAvailable)
                    return element;
            }
            catch
            {
                // Element not ready yet
            }

            Task.Delay(DefaultPollingIntervalMs).Wait();
        }

        throw new TimeoutException(
            $"Element with Name '{name}' was not found or not available within {timeout.TotalSeconds:F1} seconds. " +
            $"Parent: {GetElementDescription(parent)}");
    }

    /// <summary>
    /// Waits for an element to exist and be available by ControlType.
    /// </summary>
    /// <param name="parent">The parent element to search within.</param>
    /// <param name="controlType">The ControlType of the element to find.</param>
    /// <param name="timeout">Maximum time to wait.</param>
    /// <returns>The found element.</returns>
    /// <exception cref="TimeoutException">Thrown when element is not found within timeout.</exception>
    public static AutomationElement WaitForElementByControlType(
        AutomationElement parent,
        ControlType controlType,
        TimeSpan timeout)
    {
        if (parent == null)
            throw new ArgumentNullException(nameof(parent));

        AutomationElement? element = null;
        var endTime = DateTime.UtcNow.Add(timeout);

        while (DateTime.UtcNow < endTime)
        {
            try
            {
                element = parent.FindFirstDescendant(cf => cf.ByControlType(controlType));
                if (element != null && element.IsAvailable)
                    return element;
            }
            catch
            {
                // Element not ready yet
            }

            Task.Delay(DefaultPollingIntervalMs).Wait();
        }

        throw new TimeoutException(
            $"Element with ControlType '{controlType}' was not found or not available within {timeout.TotalSeconds:F1} seconds. " +
            $"Parent: {GetElementDescription(parent)}");
    }

    /// <summary>
    /// Waits for an element to become enabled.
    /// </summary>
    /// <param name="element">The element to check.</param>
    /// <param name="timeout">Maximum time to wait.</param>
    /// <exception cref="TimeoutException">Thrown when element does not become enabled within timeout.</exception>
    public static void WaitForElementEnabled(AutomationElement element, TimeSpan timeout)
    {
        if (element == null)
            throw new ArgumentNullException(nameof(element));

        WaitUntil(
            () => element.IsEnabled,
            timeout,
            $"Element did not become enabled within {timeout.TotalSeconds:F1} seconds. " +
            $"Element: {GetElementDescription(element)}");
    }

    /// <summary>
    /// Waits for an element to disappear (become unavailable or null).
    /// </summary>
    /// <param name="elementGetter">Function that returns the element to check.</param>
    /// <param name="timeout">Maximum time to wait.</param>
    /// <param name="elementDescription">Description of the element for error messages.</param>
    /// <exception cref="TimeoutException">Thrown when element does not disappear within timeout.</exception>
    public static void WaitForElementToDisappear(
        Func<AutomationElement?> elementGetter,
        TimeSpan timeout,
        string elementDescription)
    {
        if (elementGetter == null)
            throw new ArgumentNullException(nameof(elementGetter));

        WaitUntil(
            () =>
            {
                try
                {
                    var element = elementGetter();
                    return element == null || !element.IsAvailable;
                }
                catch
                {
                    return true; // Element is gone
                }
            },
            timeout,
            $"Element did not disappear within {timeout.TotalSeconds:F1} seconds. Element: {elementDescription}");
    }

    /// <summary>
    /// Waits for a window with the specified title to appear.
    /// </summary>
    /// <param name="application">The application to search within.</param>
    /// <param name="windowTitle">The title of the window to find.</param>
    /// <param name="timeout">Maximum time to wait.</param>
    /// <returns>The found window.</returns>
    /// <exception cref="TimeoutException">Thrown when window is not found within timeout.</exception>
    public static Window WaitForWindowByTitle(
        FlaUI.Core.Application application,
        string windowTitle,
        TimeSpan timeout)
    {
        if (application == null)
            throw new ArgumentNullException(nameof(application));
        if (string.IsNullOrWhiteSpace(windowTitle))
            throw new ArgumentException("Window title cannot be null or empty.", nameof(windowTitle));

        Window? window = null;
        var endTime = DateTime.UtcNow.Add(timeout);

        while (DateTime.UtcNow < endTime)
        {
            try
            {
                var windows = application.GetAllTopLevelWindows(new FlaUI.UIA3.UIA3Automation());
                window = windows.FirstOrDefault(w => w.Title == windowTitle);
                if (window != null && window.IsAvailable)
                    return window;
            }
            catch
            {
                // Window not ready yet
            }

            Task.Delay(DefaultPollingIntervalMs).Wait();
        }

        throw new TimeoutException(
            $"Window with title '{windowTitle}' was not found within {timeout.TotalSeconds:F1} seconds.");
    }

    /// <summary>
    /// Gets a descriptive string for an element to use in error messages.
    /// </summary>
    private static string GetElementDescription(AutomationElement element)
    {
        if (element == null)
            return "null";

        try
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(element.AutomationId))
                parts.Add($"AutomationId='{element.AutomationId}'");

            if (!string.IsNullOrEmpty(element.Name))
                parts.Add($"Name='{element.Name}'");

            parts.Add($"ControlType={element.ControlType}");

            return string.Join(", ", parts);
        }
        catch
        {
            return "unavailable";
        }
    }
}
