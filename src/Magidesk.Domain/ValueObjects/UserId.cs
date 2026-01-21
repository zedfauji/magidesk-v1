using System;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Represents a user identifier.
/// Immutable value object that ensures non-empty Guid.
/// </summary>
public sealed record UserId
{
    /// <summary>
    /// Gets the user identifier value.
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserId"/> class.
    /// </summary>
    /// <param name="value">The user identifier Guid.</param>
    /// <exception cref="ArgumentException">Thrown when value is empty Guid.</exception>
    public UserId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty Guid.", nameof(value));
        }

        Value = value;
    }

    /// <summary>
    /// Implicitly converts a Guid to UserId.
    /// </summary>
    /// <param name="value">The Guid value.</param>
    public static implicit operator UserId(Guid value) => new(value);

    /// <summary>
    /// Implicitly converts a UserId to Guid.
    /// </summary>
    /// <param name="userId">The UserId instance.</param>
    public static implicit operator Guid(UserId userId) => userId.Value;

    /// <summary>
    /// Creates a new UserId from a Guid string.
    /// </summary>
    /// <param name="value">The Guid string.</param>
    /// <returns>A new UserId instance.</returns>
    /// <exception cref="ArgumentException">Thrown when value is not a valid Guid.</exception>
    public static UserId FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("UserId string cannot be null or empty.", nameof(value));
        }

        if (!Guid.TryParse(value, out var guid))
        {
            throw new ArgumentException($"Invalid Guid format: {value}", nameof(value));
        }

        return new UserId(guid);
    }

    /// <summary>
    /// Returns a string representation of the UserId.
    /// </summary>
    public override string ToString() => Value.ToString();
}

