using System;

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

    // Private constructor for EF Core
    private PrinterMapping()
    {
    }

    public static PrinterMapping Create(Guid terminalId, Guid printerGroupId, string physicalPrinterName)
    {
        if (terminalId == Guid.Empty) throw new ArgumentException("Terminal ID is required.");
        if (printerGroupId == Guid.Empty) throw new ArgumentException("Printer Group ID is required.");
        if (string.IsNullOrWhiteSpace(physicalPrinterName)) throw new ArgumentException("Physical printer name is required.");

        return new PrinterMapping
        {
            Id = Guid.NewGuid(),
            TerminalId = terminalId,
            PrinterGroupId = printerGroupId,
            PhysicalPrinterName = physicalPrinterName
        };
    }

    public void Update(string physicalPrinterName)
    {
        if (string.IsNullOrWhiteSpace(physicalPrinterName)) throw new ArgumentException("Physical printer name is required.");
        PhysicalPrinterName = physicalPrinterName;
    }
}
