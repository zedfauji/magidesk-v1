namespace Magidesk.Domain.Exceptions;

/// <summary>
/// Exception thrown when a business rule (invariant) is violated.
/// </summary>
public sealed class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string message) : base(message)
    {
    }

    public BusinessRuleViolationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

