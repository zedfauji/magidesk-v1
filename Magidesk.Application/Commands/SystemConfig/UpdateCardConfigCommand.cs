using System;
using Magidesk.Application.DTOs.SystemConfig;

namespace Magidesk.Application.Commands.SystemConfig;

public record UpdateCardConfigResult(bool Success, string Message);

public class UpdateCardConfigCommand
{
    public CardConfigDto Config { get; }

    public UpdateCardConfigCommand(CardConfigDto config)
    {
        Config = config;
    }
}
