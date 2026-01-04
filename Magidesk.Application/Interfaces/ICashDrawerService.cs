namespace Magidesk.Application.Interfaces;

public interface ICashDrawerService
{
    /// <summary>
    /// Opens the cash drawer connected to the specified printer.
    /// </summary>
    /// <param name="printerName">Name of the printer (drawer connected via RJ11).</param>
    Task OpenDrawerAsync(string printerName);
}
