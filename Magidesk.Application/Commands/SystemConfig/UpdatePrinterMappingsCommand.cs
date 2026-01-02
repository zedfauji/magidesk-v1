using System;
using System.Collections.Generic;
using Magidesk.Application.DTOs.SystemConfig;

namespace Magidesk.Application.Commands.SystemConfig;

public record UpdatePrinterMappingsResult(bool Success, string Message);

public class UpdatePrinterMappingsCommand
{
    public Guid TerminalId { get; }
    public List<PrinterMappingDto> Mappings { get; }

    public UpdatePrinterMappingsCommand(Guid terminalId, List<PrinterMappingDto> mappings)
    {
        TerminalId = terminalId;
        Mappings = mappings;
    }
}
