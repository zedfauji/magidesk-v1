# Printing System Contract

## 1. No Silent Failures
*   Any print job that fails **MUST** throw an exception back to the UI.
*   The UI **MUST** display an error dialog allowing Retry or Cancel.
*   **NEVER** swallow `PrinterException` or `Win32Exception`.

## 2. Source of Truth
*   The **Backend** (Domain/Services) dictates *what* to print.
*   The **Database** stores *where* to print (`PrinterMapping`).
*   The **Frontend** allows configuration but does not contain print logic (except invoking the service).

## 3. Deployment Topology
*   **Server**: Does NOT print (normally).
*   **Terminal**: Prints to locally connected printers (USB/Network) mapped in `PrinterMapping`.
*   **Routing**: Logic occurs at the Application Service layer (`PrintingService`).

## 4. Hardware Abstraction
*   Use `System.Drawing.Printing` for Windows generic printing.
*   Use RawPrinterHelper (ESC/POS) for Cash Drawer kicks if driver doesn't support it directly.
