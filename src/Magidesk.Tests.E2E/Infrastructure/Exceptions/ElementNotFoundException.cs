namespace Magidesk.Tests.E2E.Infrastructure.Exceptions;

/// <summary>
/// Thrown when a UI element is not found within the specified timeout period.
/// Inherits from TimeoutException to maintain compatibility with timeout-based error handling.
/// </summary>
public class ElementNotFoundException : TimeoutException
{
    /// <summary>
    /// Gets the AutomationId of the element that was not found, if specified.
    /// </summary>
    public string? AutomationId { get; }

    /// <summary>
    /// Gets the Name of the element that was not found, if specified.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Gets the ControlType of the element that was not found, if specified.
    /// </summary>
    public string? ControlType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElementNotFoundException"/> class.
    /// </summary>
    /// <param name="message">
    /// The error message that describes the element search failure.
    /// Should include what went wrong (element not found), context (parent element, timeout),
    /// and next steps (verify navigation, check AutomationId).
    /// </param>
    /// <param name="automationId">The AutomationId of the element that was not found, if available.</param>
    /// <param name="name">The Name of the element that was not found, if available.</param>
    /// <param name="controlType">The ControlType of the element that was not found, if available.</param>
    public ElementNotFoundException(
        string message,
        string? automationId = null,
        string? name = null,
        string? controlType = null)
        : base(message)
    {
        AutomationId = automationId;
        Name = name;
        ControlType = controlType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElementNotFoundException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">
    /// The error message that describes the element search failure.
    /// Should include what went wrong (element not found), context (parent element, timeout),
    /// and next steps (verify navigation, check AutomationId).
    /// </param>
    /// <param name="automationId">The AutomationId of the element that was not found, if available.</param>
    /// <param name="name">The Name of the element that was not found, if available.</param>
    /// <param name="controlType">The ControlType of the element that was not found, if available.</param>
    /// <param name="innerException">The exception that caused the element search failure.</param>
    public ElementNotFoundException(
        string message,
        string? automationId,
        string? name,
        string? controlType,
        Exception innerException)
        : base(message, innerException)
    {
        AutomationId = automationId;
        Name = name;
        ControlType = controlType;
    }
}
