using System;
using Magidesk.Application.Interfaces;

namespace Magidesk.Api.Services;

public class StubTerminalContext : ITerminalContext
{
    public string? TerminalIdentity { get; private set; } = "API-Host";
    public Guid? TerminalId { get; private set; } = Guid.Empty;

    public void SetTerminalIdentity(string terminalIdentity, Guid terminalId)
    {
        TerminalIdentity = terminalIdentity;
        TerminalId = terminalId;
    }
}
