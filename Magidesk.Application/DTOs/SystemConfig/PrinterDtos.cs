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
        set
        {
            if (!string.IsNullOrEmpty(value) && Enum.TryParse<PrinterType>(value, out var result))
            {
                PrinterType = result;
            }
        }
    }

    // Behavior Settings
    public Domain.Enumerations.CutBehavior CutBehavior { get; set; } = Domain.Enumerations.CutBehavior.Auto;
    public bool ShowPrices { get; set; } = true;
    public int RetryCount { get; set; }
    public int RetryDelayMs { get; set; }
    public bool AllowReprint { get; set; } = true;
    public Guid? FallbackPrinterGroupId { get; set; }
}

public class PrinterMappingDto
{
    public Guid Id { get; set; }
    public Guid TerminalId { get; set; }
    public Guid PrinterGroupId { get; set; }
    public string PhysicalPrinterName { get; set; } = string.Empty;
    public PrinterFormat Format { get; set; } = PrinterFormat.Thermal80mm;
    public bool CutEnabled { get; set; } = true;

    // Detailed Capabilities
    public int PaperWidthMm { get; set; } = 80;
    public int PrintableWidthChars { get; set; } = 48;
    public int Dpi { get; set; } = 203;
    public bool SupportsCashDrawer { get; set; } = true;
    public bool SupportsImages { get; set; } = true;
    public bool SupportsQr { get; set; } = true;

    // UI Helpers
    public string PrinterGroupName { get; set; } = string.Empty;
    public string PrinterName 
    { 
        get => PhysicalPrinterName; 
        set => PhysicalPrinterName = value; 
    }
}
