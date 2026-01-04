using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;

namespace Magidesk.Infrastructure.Services;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class PrintingService : IPrintingService
{
    public async Task<IEnumerable<string>> GetSystemPrintersAsync()
    {
        return await Task.Run(() =>
        {
            var printers = new List<string>();
            // BEH-001: No Silent Failures. Allow exception to propagate to caller for Dialog.
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                printers.Add(printer);
            }
            return printers.AsEnumerable();
        });
    }

    public async Task PrintTicketAsync(TicketDto ticket, string? printerName = null)
    {
        // Stub: Log print request
        Debug.WriteLine($"[PrintingService] Printing Ticket #{ticket.TicketNumber} to {(printerName ?? "Default Printer")}");
        
        // Simulating print delay
        await Task.Delay(500);

        // In a real implementation, this would send ESC/POS commands or usage Windows PrintManager
    }

    public async Task PrintKitchenTicketAsync(TicketDto ticket)
    {
        Debug.WriteLine($"[PrintingService] Printing KITCHEN Ticket #{ticket.TicketNumber}");
        await Task.Delay(500);
    }

    public async Task PrintReceiptAsync(TicketDto ticket)
    {
        Debug.WriteLine($"[PrintingService] Printing RECEIPT for Ticket #{ticket.TicketNumber}");
        await Task.Delay(500);
    }
}
