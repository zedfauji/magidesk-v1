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
        // TODO: Implement Kitchen logic (grouping by PrinterGroup)
        Debug.WriteLine($"[PrintingService] Printing KITCHEN Ticket #{ticket.TicketNumber}");
        await Task.CompletedTask;
    }

    public async Task PrintReceiptAsync(TicketDto ticket)
    {
        // Re-use PrintTicketAsync for receipt for now
        await PrintTicketAsync(ticket);
    }
}
