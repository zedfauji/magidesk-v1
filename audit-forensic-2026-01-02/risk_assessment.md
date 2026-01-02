# Risk & Impact Assessment
**Audit Date:** 2026-01-01

## 1. Critical Risks (Immediate Action Required)
- **Data Loss / Business Continuity (Financial Risk):**
  - **Gap:** Missing Database Backup / Restore Tools.
  - **Impact:** If the SQLite/SQL Server DB corrupts, the restaurant loses all sales data, open tabs, and inventory. No recovery path exists in the UI.
- **Revenue Loss (Financial Risk):**
  - **Gap:** Multi-Currency Support Missing.
  - **Impact:** If deployed in a border region (e.g., US/Canada, US/Mexico) or tourist hub accepting Euros/Dollars, the system cannot function.

## 2. High Risks (Operational Blocks)
- **Kitchen Chaos (Operational Risk):**
  - **Gap:** Pizza Builder / Complex Modifier Logic.
  - **Impact:** Servers will enter "Cheese Pizza" + "Add Peperoni (Left)" + "Add Mushroom (Right)" using generic notes or multiple line items. The Kitchen ticket will be unreadable or confusing, leading to food waste and angry customers.
- **Booking Failures (UX Risk):**
  - **Gap:** Missing Reservations.
  - **Impact:** Hostess cannot block tables. Walk-ins will be seated at reserved tables, causing conflicts.

## 3. Medium Risks (Friction / Inefficiency)
- **Waiter Efficiency (UX Risk):**
  - **Gap:** Navigation "Back" button behavior (Table Map vs Switchboard).
  - **Impact:** High-volume nights will see slower table turn times as waiters fight the UI navigation.
- **Inventory Drift (Operational Risk):**
  - **Gap:** Missing Purchase Orders.
  - **Impact:** Stock is only adjusted manually. No "Receiving" workflow means theft during delivery is harder to catch.
