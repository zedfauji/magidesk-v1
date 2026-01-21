using System;

namespace Magidesk.Application.Interfaces;

public interface ITerminalContext
{
    string? TerminalIdentity { get; }
    Guid? TerminalId { get; }
    void SetTerminalIdentity(string terminalIdentity, Guid terminalId);
}
