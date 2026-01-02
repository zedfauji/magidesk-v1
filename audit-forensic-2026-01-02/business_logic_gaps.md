# Business Logic Gaps
**Audit Date:** 2026-01-01

## 1. Complex Pricing Models
- **Gap:** **Pizza Pricing Logic**
  - **FloreantPOS:** Has dedicated `PizzaPrice` and `PizzaModifierPrice` logic, handling complex fraction pricing (1/2 toppings), size multipliers, and crust types deeply integrated into the line item calculation.
  - **Magidesk:** Uses generic `FractionalModifier` and `ModifierAttributes`. While flexible, it may not enforce the strict "Pizza Style" pricing rules (e.g., "First 2 toppings free", "Average price of halves") without custom scripting or logic integration that appears missing in the current `OrderLine` calculation service.
  - **Risk:** Revenue loss or overcharging on complex pizza orders.

## 2. Financial Precision & currency
- **Gap:** **Multi-Currency Support**
  - **FloreantPOS:** Explicit support for `Currency` entity, exchange rates, and `MultiCurrencyTender`.
  - **Magidesk:** No evidence of `Currency` entity. Assumes single base currency.
  - **Risk:** Critical blocker for international markets or border regions accepting multiple currencies.

## 3. Inventory Control
- **Gap:** **Purchase Order & Receiving**
  - **FloreantPOS:** Full `PurchaseOrder` lifecycle (Create -> Approve -> Receive). Updates inventory counts upon receipt.
  - **Magidesk:** Has `InventoryItem` and `InventoryPage`, but lack of `PurchaseOrder` entity suggests reliance on simple "Manual Adjustment" or "Stock Take" rather than a rigorous procurement workflow.
  - **Risk:** Lack of audit trail for incoming stock; theft/shrinkage easier to hide.

## 4. Operational Invariants
- **Gap:** **Drawer Assignment Enforcement**
  - **FloreantPOS:** Strict `DrawerAssignedHistory`. Prevents transactions if drawer not assigned to user.
  - **Magidesk:** Has `CashSession`. Need to verify if `ProcessPaymentCommand` *strictly* enforces an open session. The existence of `CashSessionService` suggests yes, but the strict "One Drawer = One User" rule (critical in some jurisdictions) needs verification against `CashSession` logic which might be pooled.

## 5. Tax & Compliance
- **Gap:** **Tax Exemption Workflows**
  - **FloreantPOS:** `TaxSelectionDialog` allows ad-hoc tax changing/exemption per ticket with specific reasons.
  - **Magidesk:** `SetTaxExemptCommandHandler` exists, but the granular logic for *associating a reason* or specific tax ID override seems less developed in the domain model.
