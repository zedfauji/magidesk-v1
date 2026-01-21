using Magidesk.Application.DTOs.SystemConfig;

namespace Magidesk.Application.Queries.SystemConfig;

public class GetTerminalConfigQuery
{
    public string TerminalKey { get; }

    public GetTerminalConfigQuery(string terminalKey)
    {
        TerminalKey = terminalKey;
    }
}

public class GetTerminalConfigResult
{
    public TerminalDto Terminal { get; }

    public GetTerminalConfigResult(TerminalDto terminal)
    {
        Terminal = terminal;
    }
}
