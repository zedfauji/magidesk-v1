# Printer Format Contract

This document defines the canonical "Printer Format Contract" for Magidesk POS. It establishes the supported printer types, their physical constraints, and the formatting rules that the printing engine must adhere to.

## 1. Printer Types

The system supports three distinct printer format types. These are "Format Types", distinct from "Usage Types" (Receipt, Kitchen, etc.).

| Format Type | Description | Common Hardware |
| :--- | :--- | :--- |
| **THERMAL_58MM** | Narrow format thermal receipt printers. Highly constrained width. | 58mm POS Printers (e.g., portable, mini) |
| **THERMAL_80MM** | Standard format thermal receipt printers. The industry standard. | Epson TM-T88, Star TSP100, etc. |
| **STANDARD_PAGE** | Full page document printers (A4 / Letter). Used for reports. | Laser, Inkjet, or Windows Virtual Printers (PDF) |

---

## 2. Physical Constraints & Logic

### THERMAL_58MM
*   **Physical Width:** ~58mm
*   **Printable Width:** ~48mm
*   **Character Columns (A):** 32 chars (Font A, standard sizing)
*   **Character Columns (B):** 42 chars (Font B, condensed)
*   **Design Target:** **32 Columns** (Safe Default)
*   **Default Margins:** 0 or Minimal (Hardware handles margins)
*   **Wrapping:** Aggressive wrapping required. Item names > 16 chars usually wrap.

### THERMAL_80MM
*   **Physical Width:** ~80mm
*   **Printable Width:** ~72mm
*   **Character Columns (A):** 42-48 chars (Font A)
*   **Character Columns (B):** 56-64 chars (Font B)
*   **Design Target:** **42 Columns** (Standard Safe Default) / **48 Columns** (Optimized)
*   **Default Margins:** 0 or Minimal
*   **Wrapping:** Standard wrapping. Item names can utilize ~20-30 chars.

### STANDARD_PAGE
*   **Physical Width:** A4 (210mm) / Letter (216mm)
*   **Printable Width:** Variable (Driver dependent), typically ~190mm
*   **Character Columns:** 80+ chars (Monospace equivalent)
*   **Font Strategy:** **Scalable** (TrueType/OpenType via Windows Driver) rather than ESC/POS bytes.
*   **Layout:** Table-based layouts are possible.
*   **Handling:** Rendered as a graphics document (Graphics object) rather than a raw byte stream, OR a text stream utilizing driver fonts.

---

## 3. Formatting Strategy

The `PrintLayoutEngine` must abstract these types into a unified interface.

### Abstracted Commands
The application code should emit logical commands, not physical coordinates.

*   `Header(string text)`
*   `LineItem(string qty, string name, string price, List<string> modifiers)`
*   `Separator()`
*   `Total(string label, string amount, bool isEmphasis)`

### Adapter Logic

#### Thermal Adapter (58mm/80mm)
*   **Protocol:** ESC/POS (Raw Bytes)
*   **Positioning:** Character counting and padding.
*   **Alignment:** 
    *   Left: No padding.
    *   Right: Pad Left = (Line Width - Text Length).
    *   Center: Pad Left = (Line Width - Text Length) / 2.
*   **Truncation/Wrap:** 
    *   Columns must never overflow the `Design Target` width.
    *   Wrapped lines must be indented to visually group with the parent item.

#### Standard Adapter
*   **Protocol:** GDI+ / `System.Drawing` (Graphics Object)
*   **Positioning:** Coordinate-based (pixels/inches).
*   **Alignment:** StringFormat flags.
*   **Scaling:** Content scales to fit PrintableArea width.

---

## 4. Hardware Behavior Rules

### Cutters
*   **Thermal:** Explicit `GS V` (Cut) command sent at end of job.
*   **Standard:** Driver handles page breaks. No cut command.

### Cash Drawer
*   **Thermal:** Driven by Pulse Command (`ESC p`). Only sent if `UsesCashDrawer` is true for the specific printer mapping.
*   **Standard:** Generally does NOT support cash drawer kicks (unless specific driver features exist, which are out of scope).

### Images / Logos
*   **Thermal:** Bit-image rasterization required. High complexity.
    *   *Phase 1 Implementation:* Text-only headers.
    *   *Phase 2 Implementation:* Pre-stored logo support (memory key).
*   **Standard:** Standard Image rendering.
