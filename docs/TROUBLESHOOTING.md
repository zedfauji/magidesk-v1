# Troubleshooting Guide

Solutions to common issues you might encounter.

## Login Issues

### "Login Failed" or "System Critical Error"
- **Cause**: The system cannot verify your PIN, likely due to a database connection loss.
- **Solution**:
  1. Check if the server computer is turned on.
  2. Check your network cable/Wi-Fi.
  3. Restart Magidesk.

### Forgot PIN
- **Solution**: Ask a Manager to log in and reset your PIN in the **Admin > Users** screen.
- **If Admin PIN is lost**: Contact Magidesk Support immediately.

## Printer / Hardware

### Receipt Not Printing
1. Check if the printer has paper and the lid is closed.
2. Turn the printer OFF and ON again.
3. Check Windows Printers settings to ensure the printer is "Online".
4. In Magidesk, go to **Configuration** and re-select the printer.

### Cash Drawer Won't Open
- The cash drawer connects to the printer. Ensure the phone-line style cable is securely plugged into the **printer**, not the computer and not the wall/router.

## Database Connection

### "Setup Required" Screen Appears Suddenly
- This means Magidesk lost its configuration file or cannot find the database.
- Provide the Host IP and Credentials again as per the [Database Setup Guide](DATABASE_SETUP.md).

### System Sluggishness
- If the system feels slow, restart the **Main Database Server** computer. It may be running updates or have high memory usage.
