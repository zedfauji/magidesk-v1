using System;
using System.Collections.Generic;
using Magidesk.Application.DTOs.SystemConfig;

namespace Magidesk.Application.Queries.SystemConfig;

public record GetPrinterGroupsResult(List<PrinterGroupDto> Groups);

public class GetPrinterGroupsQuery
{
    public GetPrinterGroupsQuery()
    {
    }
}
