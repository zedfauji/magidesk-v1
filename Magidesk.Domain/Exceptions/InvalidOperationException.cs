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

