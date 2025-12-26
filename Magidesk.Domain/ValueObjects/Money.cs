using System;

namespace Magidesk.Domain.ValueObjects;

/// <summary>
/// Represents a monetary amount with currency.
/// Immutable value object that enforces 2 decimal places precision.
/// </summary>
public sealed record Money : IComparable<Money>, IComparable
{
    private const int DecimalPlaces = 2;
    private const string DefaultCurrency = "USD";

    /// <summary>
    /// Gets the monetary amount (rounded to 2 decimal places).
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Gets the currency code (ISO 4217, e.g., "USD").
    /// </summary>
    public string Currency { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Money"/> class.
    /// </summary>
    /// <param name="amount">The monetary amount.</param>
    /// <param name="currency">The currency code (ISO 4217). Defaults to "USD".</param>
    /// <exception cref="ArgumentException">Thrown when amount is negative or currency is invalid.</exception>
    public Money(decimal amount, string currency = DefaultCurrency)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Money amount cannot be negative.", nameof(amount));
        }

        // NOTE:
        // We default missing/blank currency to USD. This prevents persistence/materialization issues
        // (e.g., older rows with NULL/blank currency) from crashing the domain at read time.
        if (string.IsNullOrWhiteSpace(currency))
        {
            currency = DefaultCurrency;
        }

        // Round to 2 decimal places
        Amount = Math.Round(amount, DecimalPlaces, MidpointRounding.AwayFromZero);
        Currency = currency.ToUpperInvariant();
    }

    /// <summary>
    /// Creates a Money instance with zero amount.
    /// </summary>
    /// <param name="currency">The currency code. Defaults to "USD".</param>
    /// <returns>A Money instance with zero amount.</returns>
    public static Money Zero(string currency = DefaultCurrency) => new(0m, currency);

    /// <summary>
    /// Adds two Money instances.
    /// </summary>
    /// <param name="left">The first Money instance.</param>
    /// <param name="right">The second Money instance.</param>
    /// <returns>A new Money instance with the sum.</returns>
    /// <exception cref="InvalidOperationException">Thrown when currencies don't match.</exception>
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new InvalidOperationException($"Cannot add money with different currencies: {left.Currency} and {right.Currency}.");
        }

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    /// <summary>
    /// Subtracts two Money instances.
    /// </summary>
    /// <param name="left">The first Money instance.</param>
    /// <param name="right">The second Money instance.</param>
    /// <returns>A new Money instance with the difference.</returns>
    /// <exception cref="InvalidOperationException">Thrown when currencies don't match or result is negative.</exception>
    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new InvalidOperationException($"Cannot subtract money with different currencies: {left.Currency} and {right.Currency}.");
        }

        var result = left.Amount - right.Amount;
        if (result < 0)
        {
            throw new InvalidOperationException("Money subtraction result cannot be negative.");
        }

        return new Money(result, left.Currency);
    }

    /// <summary>
    /// Multiplies a Money instance by a decimal factor.
    /// </summary>
    /// <param name="money">The Money instance.</param>
    /// <param name="factor">The multiplication factor.</param>
    /// <returns>A new Money instance with the product.</returns>
    /// <exception cref="ArgumentException">Thrown when factor is negative.</exception>
    public static Money operator *(Money money, decimal factor)
    {
        if (factor < 0)
        {
            throw new ArgumentException("Multiplication factor cannot be negative.", nameof(factor));
        }

        return new Money(money.Amount * factor, money.Currency);
    }

    /// <summary>
    /// Multiplies a Money instance by a decimal factor.
    /// </summary>
    /// <param name="factor">The multiplication factor.</param>
    /// <param name="money">The Money instance.</param>
    /// <returns>A new Money instance with the product.</returns>
    public static Money operator *(decimal factor, Money money) => money * factor;

    /// <summary>
    /// Divides a Money instance by a decimal divisor.
    /// </summary>
    /// <param name="money">The Money instance.</param>
    /// <param name="divisor">The divisor.</param>
    /// <returns>A new Money instance with the quotient.</returns>
    /// <exception cref="ArgumentException">Thrown when divisor is zero or negative.</exception>
    public static Money operator /(Money money, decimal divisor)
    {
        if (divisor <= 0)
        {
            throw new ArgumentException("Divisor must be greater than zero.", nameof(divisor));
        }

        return new Money(money.Amount / divisor, money.Currency);
    }


    /// <summary>
    /// Compares two Money instances.
    /// </summary>
    public static bool operator <(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new InvalidOperationException($"Cannot compare money with different currencies: {left.Currency} and {right.Currency}.");
        }

        return left.Amount < right.Amount;
    }

    /// <summary>
    /// Compares two Money instances.
    /// </summary>
    public static bool operator >(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new InvalidOperationException($"Cannot compare money with different currencies: {left.Currency} and {right.Currency}.");
        }

        return left.Amount > right.Amount;
    }

    /// <summary>
    /// Compares two Money instances.
    /// </summary>
    public static bool operator <=(Money left, Money right) => left < right || left == right;

    /// <summary>
    /// Compares two Money instances.
    /// </summary>
    public static bool operator >=(Money left, Money right) => left > right || left == right;

    /// <summary>
    /// Returns a string representation of the Money instance.
    /// </summary>
    public override string ToString() => $"{Currency} {Amount:F2}";

    public int CompareTo(Money? other)
    {
        if (other is null) return 1;
        if (Currency != other.Currency)
            throw new ArgumentException($"Cannot compare money with different currencies: {Currency} and {other.Currency}.");
        return Amount.CompareTo(other.Amount);
    }

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is Money other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(Money)}");
    }
}