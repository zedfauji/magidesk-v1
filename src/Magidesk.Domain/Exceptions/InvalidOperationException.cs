using System;

namespace Magidesk.Domain.Exceptions;

/// <summary>
/// Exception thrown when an operation is not allowed in the current state.
/// </summary>
public sealed class InvalidOperationException : DomainException
{
    public InvalidOperationException(string message) : base(message)
    {
    }

    public InvalidOperationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Domain-scoped alias for InvalidOperationException.
/// Used across Ticket partial classes for state-violation exceptions.
/// </summary>
public sealed class DomainInvalidOperationException : DomainException
{
    public DomainInvalidOperationException(string message) : base(message)
    {
    }

    public DomainInvalidOperationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
