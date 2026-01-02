using System;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Application.DTOs.SystemConfig;

public class PrinterGroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PrinterType PrinterType { get; set; }

    public string PrinterTypeString
    {
        get => PrinterType.ToString();
        set => PrinterType = Enum.Parse<PrinterType>(value);
    }
}

public class PrinterMappingDto
{
    public Guid Id { get; set; }
    public Guid TerminalId { get; set; }
    public Guid PrinterGroupId { get; set; }
    public string PhysicalPrinterName { get; set; } = string.Empty;

    // UI Helpers
    public string PrinterGroupName { get; set; } = string.Empty;
    public string PrinterName 
    { 
        get => PhysicalPrinterName; 
        set => PhysicalPrinterName = value; 
    }
}
