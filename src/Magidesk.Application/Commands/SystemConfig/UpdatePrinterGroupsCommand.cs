using System;
using System.Collections.Generic;
using Magidesk.Application.DTOs.SystemConfig;

namespace Magidesk.Application.Commands.SystemConfig;

public record UpdatePrinterGroupsResult(bool Success, string Message);

public class UpdatePrinterGroupsCommand
{
    public List<PrinterGroupDto> Groups { get; }

    public UpdatePrinterGroupsCommand(List<PrinterGroupDto> groups)
    {
        Groups = groups;
    }
}
