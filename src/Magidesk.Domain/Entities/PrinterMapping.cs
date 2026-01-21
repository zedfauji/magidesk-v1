using System;
using Magidesk.Domain.Enumerations;

namespace Magidesk.Domain.Entities;

/// <summary>
/// Map a virtual PrinterGroup to a physical system printer for a specific Terminal.
/// </summary>
public class PrinterMapping
{
    public Guid Id { get; private set; }
    public Guid TerminalId { get; private set; }
    public Guid PrinterGroupId { get; private set; }
    public string PhysicalPrinterName { get; private set; } = string.Empty;

    public PrinterFormat Format { get; private set; }
    public bool CutEnabled { get; private set; }

    // Physical Capabilities
    public int PaperWidthMm { get; private set; }
    public int PrintableWidthChars { get; private set; }
    public int Dpi { get; private set; }
    public bool SupportsCashDrawer { get; private set; }
    public bool SupportsImages { get; private set; }
    public bool SupportsQr { get; private set; }

    // Private constructor for EF Core
    private PrinterMapping()
    {
    }

    public static PrinterMapping Create(
        Guid terminalId,
        Guid printerGroupId,
        string physicalPrinterName,
        PrinterFormat format = PrinterFormat.Thermal80mm,
        bool cutEnabled = true)
    {
        if (terminalId == Guid.Empty) throw new ArgumentException("Terminal ID is required.");
        if (printerGroupId == Guid.Empty) throw new ArgumentException("Printer Group ID is required.");
        if (string.IsNullOrWhiteSpace(physicalPrinterName)) throw new ArgumentException("Physical printer name is required.");

        return new PrinterMapping
        {
            Id = Guid.NewGuid(),
            TerminalId = terminalId,
            PrinterGroupId = printerGroupId,
            PhysicalPrinterName = physicalPrinterName,
            Format = format,
            CutEnabled = cutEnabled,
            // Defaults based on format - can be updated later via UpdateConfiguration
            PaperWidthMm = format == PrinterFormat.Thermal58mm ? 58 : 80,
            PrintableWidthChars = format == PrinterFormat.Thermal58mm ? 32 : 48,
            Dpi = 203,
            SupportsCashDrawer = true,
            SupportsImages = true,
            SupportsQr = true
        };
    }

    public void Update(string physicalPrinterName)
    {
        if (string.IsNullOrWhiteSpace(physicalPrinterName)) throw new ArgumentException("Physical printer name is required.");
        PhysicalPrinterName = physicalPrinterName;
    }

    public void UpdateConfiguration(
        string physicalPrinterName, 
        PrinterFormat format, 
        bool cutEnabled,
        int paperWidthMm,
        int printableWidthChars,
        int dpi,
        bool supportsCashDrawer,
        bool supportsImages,
        bool supportsQr)
    {
        if (string.IsNullOrWhiteSpace(physicalPrinterName)) throw new ArgumentException("Physical printer name is required.");
        PhysicalPrinterName = physicalPrinterName;
        Format = format;
        CutEnabled = cutEnabled;
        PaperWidthMm = paperWidthMm;
        PrintableWidthChars = printableWidthChars;
        Dpi = dpi;
        SupportsCashDrawer = supportsCashDrawer;
        SupportsImages = supportsImages;
        SupportsQr = supportsQr;
    }
}
