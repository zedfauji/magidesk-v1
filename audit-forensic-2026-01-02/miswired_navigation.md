# Miswired Navigation Analysis
**Audit Date:** 2026-01-01

## 1. Manager Authentication Flow
- **Issue:** Bypass of "Manager Password" intercept.
- **Floreant Flow:** Click Manager -> **Password Dialog (Loop until correct)** -> Open Backoffice.
- **Magidesk Flow:** Click Manager -> `ManagerFunctionsDialog`. The password check might be inside the dialog command execution, but the *navigation* to the dialog itself seems unprotected or protected differently. The "Gate" should be before the UI reveals options.

## 2. Table Selection vs Order Entry
- **Issue:** Circular Dependency / Dead End.
- **Floreant Flow:** Table Map -> Click Table -> Ticket Created -> Order Screen -> "Back" button returns to **Table Map**.
- **Magidesk Flow:** Table Map -> Click Table -> Ticket Created -> Order Screen -> "Back/Save" button often returns to **Switchboard**.
- **Impact:** High friction for waiters entering multiple table orders. They lose context.

## 3. Backoffice Tab Navigation
- **Issue:** Deep linking missing.
- **Floreant Flow:** Specific buttons like "Edit Menu" can open the Backoffice *directly to the Menu tab*.
- **Magidesk Flow:** Generic "Backoffice" entry. User must manually click "Menu" tab. Lowers operational velocity.
