using System;

namespace Magidesk.Domain.Exceptions;

/// <summary>
/// Thrown when a runtime printing contract is violated (e.g. missing mapping, invalid configuration).
/// This indicates a configuration error that prevents printing, distinguishable from hardware failures.
/// </summary>
public class PrintingContractViolationException : Exception
{
    public string Contract { get; }
    public string RequiredValue { get; }

    public PrintingContractViolationException(string message) 
        : base(message)
    {
        Contract = "General";
        RequiredValue = "Unknown";
    }

    public PrintingContractViolationException(string contract, string message, string requiredValue = "") 
        : base($"Printing Contract Violation [{contract}]: {message}")
    {
        Contract = contract;
        RequiredValue = requiredValue;
    }
}
