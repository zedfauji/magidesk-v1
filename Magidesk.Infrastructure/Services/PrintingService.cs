using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Magidesk.Application.DTOs;
using Magidesk.Application.Interfaces;
using Magidesk.Infrastructure.Printing.Layouts;

namespace Magidesk.Infrastructure.Services;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class PrintingService : IPrintingService
{
    private readonly IPrintLayoutAdapter _defaultAdapter = new Thermal80mmAdapter();

    public async Task<IEnumerable<string>> GetSystemPrintersAsync()
    {
        return await Task.Run(() =>
        {
            var printers = new List<string>();
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                printers.Add(printer);
            }
            return printers.AsEnumerable();
        });
    }

    public async Task PrintTicketAsync(TicketDto ticket, string? printerName = null)
    {
        // 1. Generate Layout (Async)
        string content = await _defaultAdapter.GenerateLayoutAsync(ticket);
        
        // 2. Offload Printing to Backgound Thread (T-003)
        // GDI+ Print() is blocking and can freeze UI if not offloaded.
        await Task.Run(() => 
        {
            try 
            {
                Debug.WriteLine($"[PrintingService] Preparing Ticket #{ticket.TicketNumber} for {(printerName ?? "Default Printer")}");
                
                using var printDoc = new PrintDocument();
                if (!string.IsNullOrEmpty(printerName))
                {
                    printDoc.PrinterSettings.PrinterName = printerName;
                }

                printDoc.PrintPage += (sender, e) => 
                {
                    using var font = new Font("Consolas", 9);
                    using var brush = new SolidBrush(Color.Black);
                    float yPos = 0;
                    float leftMargin = 0;
                    
                    if (e.Graphics != null)
                        e.Graphics.DrawString(content, font, brush, leftMargin, yPos);
                    
                    e.HasMorePages = false;
                };

                if (printDoc.PrinterSettings.IsValid)
                {
                   printDoc.Print();
                }
                else 
                {
                    Debug.WriteLine($"[PrintingService] Invalid Printer: {printerName}");
                    Debug.WriteLine(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PrintingService] Error: {ex.Message}");
                // Rethrow to allow UI handling (captured by Task)
                throw; 
            }
        });
    }

    public async Task PrintKitchenTicketAsync(TicketDto ticket)
    {
        Debug.WriteLine($"[PrintingService] Printing KITCHEN Ticket #{ticket.TicketNumber}");
        
        // Route kitchen printing through the proper KitchenPrintService
        // This method is a legacy wrapper - the real kitchen routing happens via PrintToKitchenCommand
        // which uses KitchenPrintService for proper printer group routing
        
        // For now, we'll print to default printer as a fallback
        // In production, this should route through PrintToKitchenCommand instead
        await PrintTicketAsync(ticket);
    }

    public async Task PrintReceiptAsync(TicketDto ticket)
    {
        // Re-use PrintTicketAsync for receipt for now
        await PrintTicketAsync(ticket);
    }

    public async Task<bool> IsPrinterOnlineAsync(string printerName)
    {
        return await Task.Run(() =>
        {
            try
            {
                using var printDoc = new PrintDocument();
                printDoc.PrinterSettings.PrinterName = printerName;
                
                // Check if printer exists and is valid
                return printDoc.PrinterSettings.IsValid;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PrintingService] Error checking printer status for {printerName}: {ex.Message}");
                return false;
            }
        });
    }
}
