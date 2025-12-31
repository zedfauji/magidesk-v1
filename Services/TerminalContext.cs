using System;
using System.Security.Cryptography;
using System.Text;

namespace Magidesk.Presentation.Services;

public interface ITerminalContext
{
    string? TerminalIdentity { get; }
    Guid? TerminalId { get; }
    void SetTerminalIdentity(string terminalIdentity);
}

public sealed class TerminalContext : ITerminalContext
{
    public string? TerminalIdentity { get; private set; }
    public Guid? TerminalId { get; private set; }

    public void SetTerminalIdentity(string terminalIdentity)
    {
        if (string.IsNullOrWhiteSpace(terminalIdentity))
        {
            throw new ArgumentException("Terminal identity is required.", nameof(terminalIdentity));
        }

        TerminalIdentity = terminalIdentity;
        TerminalId = CreateDeterministicGuid(terminalIdentity);
    }

    private static Guid CreateDeterministicGuid(string input)
    {
        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = md5.ComputeHash(bytes);
        return new Guid(hash);
    }
}
