# Backend Feature Gaps
**Audit Date:** 2026-01-01

## 1. Domain Entities
| Entity | Status | Impact |
| :--- | :--- | :--- |
| `PurchaseOrder` | **MISSING** | Cannot track stock procurement lifecycle. |
| `Currency` | **MISSING** | Cannot handle multi-currency payments. |
| `TableBooking`/`Reservation` | **MISSING** | Cannot manage future table availability. |
| `PizzaPrice`/`PizzaModifier` | **MISSING** | Complex pricing logic must be shoehorned into generic modifiers. |
| `ZipCodeVsDeliveryCharge` | **MISSING** | Delivery charge logic is likely flat-rate or missing auto-calculation based on zone. |

## 2. Infrastructure / Services
- **Database Backup Service:** Floreant includes `DataUpdateInfo` and backup utilities. Magidesk lacks an integrated backup scheduler/runner.
- **License/Key Management:** Floreant is Open Source but has "Pro" plugins. Magidesk is custom, but if it needs to enforce licensing or unique terminal IDs (`TerminalKey`), that logic is not explicitly found.
