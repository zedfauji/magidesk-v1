namespace Magidesk.Domain.Exceptions;

/// <summary>
/// Exception thrown when an optimistic concurrency conflict occurs.
/// </summary>
public sealed class ConcurrencyException : DomainException
{
    public ConcurrencyException(string message) : base(message)
    {
    }

    public ConcurrencyException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

