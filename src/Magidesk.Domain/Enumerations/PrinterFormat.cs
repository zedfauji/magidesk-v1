namespace Magidesk.Domain.Enumerations;

/// <summary>
/// Defines the physical format of the printer.
/// Determines the layout engines and strategies used for printing.
/// </summary>
public enum PrinterFormat
{
    /// <summary>
    /// Standard 80mm Thermal Receipt Printer (Default).
    /// Target width: 42-48 columns.
    /// </summary>
    Thermal80mm = 1,

    /// <summary>
    /// Narrow 58mm Thermal Receipt Printer.
    /// Target width: 32 columns.
    /// </summary>
    Thermal58mm = 2,

    /// <summary>
    /// Full page standard printer (A4/Letter).
    /// Uses graphical layout engine.
    /// </summary>
    StandardPage = 3
}
