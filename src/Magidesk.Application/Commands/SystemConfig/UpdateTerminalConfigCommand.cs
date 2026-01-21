using Magidesk.Application.DTOs.SystemConfig;

namespace Magidesk.Application.Commands.SystemConfig;

public class UpdateTerminalConfigCommand
{
    public TerminalDto Terminal { get; }

    public UpdateTerminalConfigCommand(TerminalDto terminal)
    {
        Terminal = terminal;
    }
}

public class UpdateTerminalConfigResult
{
    public bool Success { get; }
    public string Message { get; }

    public UpdateTerminalConfigResult(bool success, string message = "")
    {
        Success = success;
        Message = message;
    }
}
