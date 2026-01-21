using System;
using Magidesk.Application.DTOs.SystemConfig;

namespace Magidesk.Application.Queries.SystemConfig;

public record GetCardConfigResult(CardConfigDto Config);

public class GetCardConfigQuery
{
    public Guid TerminalId { get; }

    public GetCardConfigQuery(Guid terminalId)
    {
        TerminalId = terminalId;
    }
}
