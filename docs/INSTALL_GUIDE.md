# Magidesk POS - Installation Guide

**Version:** 1.0
**Target OS:** Windows 10/11 (x64)

## Prerequisites
1.  **Hardware**: Touchscreen Terminal (1920x1080 recommended).
2.  **OS**: Windows 10 Pro or Windows 11 Pro.
3.  **Database**: PostgreSQL 14+ installed locally or on a server.

## Installation Steps

1.  **Run Installer**
    *   Double-click `MagideskInstaller.msi`.
    *   Accept the default path: `C:\Program Files\Magidesk POS\`.
    *   Administrator privileges are required.

2.  **Database Setup**
    *   Open PowerShell as Admin.
    *   Navigate to the `scripts/` folder (on the install media).
    *   Run: `.\install_db.ps1`
    *   Follow prompts to enter Database Host/User/Password.

3.  **App Configuration**
    *   Navigate to `C:\Program Files\Magidesk POS\`.
    *   Open `appsettings.json` in Notepad.
    *   Update the `ConnectionStrings` section with your DB details.
    *   Set the `TerminalId` (must be unique per device).

4.  **Launch**
    *   Double-click the "Magidesk POS" icon on the desktop.

## Troubleshooting
*   **Logs**: Check `C:\ProgramData\Magidesk\Logs\` for error details.
*   **Database Error**: Ensure Port 5432 is open on the database server firewall.
