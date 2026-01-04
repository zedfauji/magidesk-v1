using System.Drawing.Printing;

namespace Magidesk.Application.Interfaces;

/// <summary>
/// Low-level service for interacting with physical printers.
/// Handles raw data sending and printer enumeration.
/// </summary>
public interface IRawPrintService
{
    /// <summary>
    /// Gets a list of installed printer names.
    /// </summary>
    Task<IEnumerable<string>> GetInstalledPrintersAsync();

    /// <summary>
    /// Sends raw string data to a printer (useful for ESC/POS).
    /// </summary>
    Task PrintRawStringAsync(string printerName, string data);

    /// <summary>
    /// Sends raw bytes to a printer (useful for drawer kick commands).
    /// </summary>
    Task PrintRawBytesAsync(string printerName, byte[] data);
    
    /// <summary>
    /// Checks if a printer exists and is valid.
    /// </summary>
    Task<bool> IsPrinterValidAsync(string printerName);
}
