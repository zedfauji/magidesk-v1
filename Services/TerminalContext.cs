using System;
using System.Security.Cryptography;
using System.Text;

namespace Magidesk.Presentation.Services;

public interface ITerminalContext
{
    string? TerminalIdentity { get; }
    Guid? TerminalId { get; }
    void SetTerminalIdentity(string terminalIdentity, Guid terminalId);
}

public sealed class TerminalContext : ITerminalContext
{
    public string? TerminalIdentity { get; private set; }
    public Guid? TerminalId { get; private set; }

    public void SetTerminalIdentity(string terminalIdentity, Guid terminalId)
    {
        if (string.IsNullOrWhiteSpace(terminalIdentity))
        {
            throw new ArgumentException("Terminal identity is required.", nameof(terminalIdentity));
        }

        TerminalIdentity = terminalIdentity;
        TerminalId = terminalId;
    }
}
