# Category I: Hardware & Peripherals

## I.1 Ticket printer support

**Feature ID:** I.1  
**Feature Name:** Ticket printer support  
**Status:** FULLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: PrinterMappings, PrinterGroups tables
- Domain entities: `PrinterMapping.cs`, `PrinterGroup.cs`
- Services: `PrintingService.cs`, `ReceiptPrintService.cs`
- APIs / handlers: Print commands
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `SystemConfigPage.xaml` has printer settings
- ViewModels: Printer configuration support
- Navigation path: Settings → Printers
- User-visible workflow: Configure and print receipts

**Notes:**
- ESC/POS support in WindowsPrintingService
- Multiple printer group routing

**Risks / Gaps:**
- None significant

**Recommendation:** COMPLETE - Core printing works

---

## I.2 USB printers

**Feature ID:** I.2  
**Feature Name:** USB printers  
**Status:** FULLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: PhysicalPrinterName in PrinterMapping
- Domain entities: Printer configuration entities
- Services: `WindowsPrintingService.cs` uses Windows printer drivers
- APIs / handlers: Generic printer name handling
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): Printer selection in config
- ViewModels: GetSystemPrintersAsync lists USB printers
- Navigation path: Settings → Printer Configuration
- User-visible workflow: Select from installed printers

**Notes:**
- Uses Windows print spooler
- USB printers auto-detected

**Risks / Gaps:**
- None significant

**Recommendation:** COMPLETE - Works via Windows drivers

---

## I.3 Serial printers

**Feature ID:** I.3  
**Feature Name:** Serial printers  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: Printer configuration exists
- Domain entities: NO EVIDENCE FOUND for COM port config
- Services: NO EVIDENCE FOUND for direct serial
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND for COM port settings
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- May work via virtual COM→USB drivers
- No native serial port handling

**Risks / Gaps:**
- Legacy hardware may not work

**Recommendation:** VERIFY - Test with actual serial printers

---

## I.4 Network printers

**Feature ID:** I.4  
**Feature Name:** Network printers  
**Status:** FULLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: Printer name supports network paths
- Domain entities: PhysicalPrinterName field
- Services: Windows printer APIs support network
- APIs / handlers: Same as USB
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): Same as USB config
- ViewModels: Network printers appear in list
- Navigation path: Settings → Printers
- User-visible workflow: Select network printer

**Notes:**
- Relies on Windows network printer support
- IP printers work when added to Windows

**Risks / Gaps:**
- None for Windows-installed network printers

**Recommendation:** COMPLETE - Works via Windows

---

## I.5 Printer configuration (baud rate, paper cut)

**Feature ID:** I.5  
**Feature Name:** Printer configuration (baud rate, paper cut)  
**Status:** PARTIALLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: PrinterGroup has some settings
- Domain entities: `PrinterGroup.cs` configuration
- Services: `KitchenPrintService.cs` - ShouldCut logic
- APIs / handlers: Paper cut commands in ESC/POS
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `SystemConfigPage.xaml` - "Cash Drawer Kick" checkbox
- ViewModels: Partial configuration
- Navigation path: Settings → Printer → Options
- User-visible workflow: Some settings available

**Notes:**
- Paper cut implemented
- Baud rate via Windows driver

**Risks / Gaps:**
- Limited fine-grained control

**Recommendation:** EXTEND - Add more printer options if needed

---

## I.6 Barcode scanners

**Feature ID:** I.6  
**Feature Name:** Barcode scanners  
**Status:** FULLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: MenuItem.Barcode
- Domain entities: Barcode field exists
- Services: Search by barcode
- APIs / handlers: Item lookup by barcode
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `ItemSearchDialog.xaml` accepts barcode input
- ViewModels: Barcode search logic
- Navigation path: Order Entry → Search/Scan
- User-visible workflow: Scan item, auto-add to order

**Notes:**
- Keyboard-wedge scanners supported
- Input treated as keyboard text

**Risks / Gaps:**
- None for keyboard-wedge type

**Recommendation:** COMPLETE - Standard scanner support

---

## I.7 Cash drawer control

**Feature ID:** I.7  
**Feature Name:** Cash drawer control  
**Status:** FULLY IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: Terminal.HasCashDrawer
- Domain entities: OpenCashDrawerCommand
- Services: `CashDrawerService.cs` - pulse via printer
- APIs / handlers: OpenCashDrawer command
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): "Has Cash Drawer" toggle in config
- ViewModels: `SettleViewModel.cs` triggers drawer
- Navigation path: Payment → Cash → Drawer opens
- User-visible workflow: Drawer opens on cash payment

**Notes:**
- Drawer connected to receipt printer
- ESC/POS pulse command

**Risks / Gaps:**
- None for printer-connected drawers

**Recommendation:** COMPLETE - Standard implementation

---

## I.8 Hardware game / lamp control

**Feature ID:** I.8  
**Feature Name:** Hardware game / lamp control  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Billiard-specific feature
- Controls table lamps via relay

**Risks / Gaps:**
- Tables cannot auto-illuminate
- Manual lamp control required

**Recommendation:** IMPLEMENT - Critical for billiard clubs

---

## I.9 Powerline control

**Feature ID:** I.9  
**Feature Name:** Powerline control  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- X10 or similar powerline protocol
- Alternative to relay control

**Risks / Gaps:**
- No powerline device support

**Recommendation:** DEFER - Relay control more common

---

## I.10 Relay network control

**Feature ID:** I.10  
**Feature Name:** Relay network control  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Network-connected relay boards
- IP-based control

**Risks / Gaps:**
- Cannot control table equipment

**Recommendation:** IMPLEMENT - Needed for lamp control

---

## I.11 RS-232 / USB controllers

**Feature ID:** I.11  
**Feature Name:** RS-232 / USB controllers  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Serial relay controllers
- Alternative to network relays

**Risks / Gaps:**
- No serial relay support

**Recommendation:** IMPLEMENT - Alternative control method

---

## Category I COMPLETE

- Features audited: 11
- Fully implemented: 5
- Partially implemented: 2
- Not implemented: 4
