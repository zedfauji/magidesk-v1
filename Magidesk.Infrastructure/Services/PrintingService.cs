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
        try 
        {
            Debug.WriteLine($"[PrintingService] Preparing Ticket #{ticket.TicketNumber} for {(printerName ?? "Default Printer")}");
            
            // 1. Generate Layout
            string content = await _defaultAdapter.GenerateLayoutAsync(ticket);
            
            // 2. Configure Print Document
            using var printDoc = new PrintDocument();
            if (!string.IsNullOrEmpty(printerName))
            {
                printDoc.PrinterSettings.PrinterName = printerName;
            }

            // 3. Define Print Handler
            printDoc.PrintPage += (sender, e) => 
            {
                // Font: Consolas 9pt (approx 48 chars on 80mm)
                using var font = new Font("Consolas", 9);
                using var brush = new SolidBrush(Color.Black);
                
                float yPos = 0;
                float leftMargin = 0; // Thermal printers handle margins
                
                // Draw text
                // Note: Simple text drawing. Does not handle complex pagination manually, 
                // but Thermal printers act as one long page usually.
                e.Graphics.DrawString(content, font, brush, leftMargin, yPos);
                
                e.HasMorePages = false;
            };

            // 4. Print
            if (printDoc.PrinterSettings.IsValid)
            {
               printDoc.Print();
            }
            else 
            {
                Debug.WriteLine($"[PrintingService] Invalid Printer: {printerName}");
                // Fallback to debug
                Debug.WriteLine(content);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PrintingService] Error: {ex.Message}");
            throw; // Propagate for UI handling
        }

        await Task.CompletedTask;
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
