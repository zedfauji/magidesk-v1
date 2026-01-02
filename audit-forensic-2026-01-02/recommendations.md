# Recommendations
**Audit Date:** 2026-01-01

## 1. Must-Fix (Critical Path to Parity)
1.  **Implement Database Backup UI:** Create a simple `BackupService` and `BackUpDialog` in the Manager menu immediately.
2.  **Restore Table Map Navigation:** Ensure the "Back" or "Save" button on Order Entry returns to the *calling context* (Table Map) if the order started there.
3.  **Pizza Builder MVP:** If Pizza is a core menu item, implement a specialized `PizzaModifierDialog` that visually handles Halves/Quarters, even if backend storage remains simple.

## 2. Can-Defer (Post-MVP Enhancements)
1.  **Purchase Orders:** Use external tools or manual inventory adjustments for now. Complex PO logic is a heavy lift.
2.  **Reservations:** Only needed if the client explicitly requests Booking management. Many fast-casual places don't need this.
3.  **Multi-Currency:** Defer unless the specific deployment target requires it.

## 3. Safe-to-Ignore (Legacy/Bloat)
1.  **JasperReports:** The WPF/WinUI reporting engine is sufficient. Exact pixel-perfect parity with old Jasper templates is not worth the engineering effort.
2.  **ZipCode Delivery Logic:** Replaced by simpler flat-rate or UberEats/DoorDash integrations in modern contexts.
