using System;
using System.Collections.Generic;
using Magidesk.Application.DTOs.SystemConfig;

namespace Magidesk.Application.Queries.SystemConfig;

public record GetPrinterMappingsResult(List<PrinterMappingDto> Mappings);

public class GetPrinterMappingsQuery
{
    public Guid TerminalId { get; }

    public GetPrinterMappingsQuery(Guid terminalId)
    {
        TerminalId = terminalId;
    }
}
