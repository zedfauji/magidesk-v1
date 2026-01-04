using System.Drawing.Printing;
using System.Runtime.Versioning;
using System.Text;
using Magidesk.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Magidesk.Infrastructure.Printing;

/// <summary>
/// Implementation of IRawPrintService using System.Drawing.Printing and RawPrinterHelper.
/// </summary>
[SupportedOSPlatform("windows")]
public class WindowsPrintingService : IRawPrintService
{
    private readonly ILogger<WindowsPrintingService> _logger;

    public WindowsPrintingService(ILogger<WindowsPrintingService> logger)
    {
        _logger = logger;
    }

    public Task<IEnumerable<string>> GetInstalledPrintersAsync()
    {
        return Task.Run(() =>
        {
            var printers = new List<string>();
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                printers.Add(printer);
            }
            return (IEnumerable<string>)printers;
        });
    }

    public Task<bool> IsPrinterValidAsync(string printerName)
    {
        return Task.Run(() =>
        {
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                if (printer.Equals(printerName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        });
    }

    public async Task PrintRawBytesAsync(string printerName, byte[] data)
    {
        if (string.IsNullOrWhiteSpace(printerName))
            throw new ArgumentException("Printer name is required.");

        if (!await IsPrinterValidAsync(printerName))
            throw new InvalidOperationException($"Printer '{printerName}' not found.");

        await Task.Run(() =>
        {
            try
            {
                // SendBytesToPrinter is a P/Invoke helper we need to implement or assume exists.
                // Since this is a forensic fix, I will implement the helper class inside this file or reference it.
                // Typically this requires 'RawPrinterHelper' class using Win32 Spooler API.
                
                if (!RawPrinterHelper.SendBytesToPrinter(printerName, data))
                {
                    throw new Exception("Win32 Spooler failed to write bytes.");
                }
                
                _logger.LogInformation("Sent {Count} bytes to printer {PrinterName}", data.Length, printerName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to print raw bytes to {PrinterName}", printerName);
                throw new Exception($"Printing failed: {ex.Message}", ex);
            }
        });
    }

    public async Task PrintRawStringAsync(string printerName, string data)
    {
        var bytes = Encoding.ASCII.GetBytes(data); // ESC/POS usually expects ASCII/ANSI
        await PrintRawBytesAsync(printerName, bytes);
    }
}

// Internal Helper for Win32 Spooler API
internal static class RawPrinterHelper
{
    // Structure and API declarations:
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
    public class DOCINFOA
    {
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]
        public string pDocName;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]
        public string pOutputFile;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]
        public string pDataType;
    }

    [System.Runtime.InteropServices.DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, ExactSpelling = true, CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
    public static extern bool OpenPrinter([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

    [System.Runtime.InteropServices.DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
    public static extern bool ClosePrinter(IntPtr hPrinter);

    [System.Runtime.InteropServices.DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, ExactSpelling = true, CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
    public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [System.Runtime.InteropServices.In, System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStruct)] DOCINFOA di);

    [System.Runtime.InteropServices.DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
    public static extern bool EndDocPrinter(IntPtr hPrinter);

    [System.Runtime.InteropServices.DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
    public static extern bool StartPagePrinter(IntPtr hPrinter);

    [System.Runtime.InteropServices.DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
    public static extern bool EndPagePrinter(IntPtr hPrinter);

    [System.Runtime.InteropServices.DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
    public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

    public static bool SendBytesToPrinter(string szPrinterName, byte[] data)
    {
        Int32 dwError = 0, dwWritten = 0;
        IntPtr hPrinter = new IntPtr(0);
        DOCINFOA di = new DOCINFOA();
        bool bSuccess = false;

        di.pDocName = "Magidesk RAW Document";
        di.pDataType = "RAW";

        if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
        {
            if (StartDocPrinter(hPrinter, 1, di))
            {
                if (StartPagePrinter(hPrinter))
                {
                    IntPtr pUnmanagedBytes = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(data.Length);
                    System.Runtime.InteropServices.Marshal.Copy(data, 0, pUnmanagedBytes, data.Length);
                    bSuccess = WritePrinter(hPrinter, pUnmanagedBytes, data.Length, out dwWritten);
                    System.Runtime.InteropServices.Marshal.FreeCoTaskMem(pUnmanagedBytes);
                    EndPagePrinter(hPrinter);
                }
                EndDocPrinter(hPrinter);
            }
            ClosePrinter(hPrinter);
        }
        
        if (bSuccess == false)
        {
            dwError = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
        }
        return bSuccess;
    }
}
