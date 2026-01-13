using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Magidesk.Infrastructure.Services;

public class CashDrawerService : ICashDrawerService
{
    private readonly IRawPrintService _rawPrintService;
    private readonly ILogger<CashDrawerService> _logger;

    // Standard EPSON Pulse Command: ESC p m t1 t2
    // m = 0 (2pin), t1 = 25 (50ms), t2 = 250 (500ms)
    // HEX: 1B 70 00 19 FA
    private static readonly byte[] OpenDrawerCommand = new byte[] { 0x1B, 0x70, 0x00, 0x19, 0xFA };

    public CashDrawerService(IRawPrintService rawPrintService, ILogger<CashDrawerService> logger)
    {
        _rawPrintService = rawPrintService;
        _logger = logger;
    }

    public async Task OpenDrawerAsync(string printerName)
    {
        _logger.LogInformation("Attempting to open cash drawer on printer: {PrinterName}", printerName);

        try
        {
            await _rawPrintService.PrintRawBytesAsync(printerName, OpenDrawerCommand);
            _logger.LogInformation("Cash drawer pulse sent to {PrinterName}", printerName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open cash drawer on {PrinterName}", printerName);
            throw; // Propagate up to UI to show error dialog as per policy
        }
    }

    public async Task<bool> IsConnectedAsync()
    {
        try
        {
            // Check if we can communicate with the default cash drawer printer
            // This is a simplified check - in production, you'd want to check specific drawer hardware
            var printers = await _rawPrintService.GetInstalledPrintersAsync();
            
            // Assume cash drawer is connected if we have any printers available
            // In a real implementation, you'd check for specific drawer hardware or test communication
            return printers.Any();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check cash drawer connectivity");
            return false;
        }
    }
}
