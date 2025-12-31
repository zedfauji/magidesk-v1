# Parity Matrix

## Scope
- This matrix summarizes parity at a **feature group** level based on static code evidence.
- Ratings are conservative: **FULL** is used only where both a comparable surface and workflow were observed.

## Parity matrix

| Feature Name | FloreantPOS Behavior (summary) | Backend Parity | Frontend Parity | Overall Parity |
|---|---|---|---|---|
| Application bootstrap & initialization | DB connection check, terminal/order types/printers/plugins init, views initialization | PARTIAL | PARTIAL | PARTIAL |
| Main POS shell | Fullscreen-capable main window with status bar + timers | PARTIAL | PARTIAL | PARTIAL |
| Root navigation model | Card-based view switching with modal dialogs preserving ticket context | PARTIAL | PARTIAL | PARTIAL |
| Login + user context | Login view + password entry dialog + user session context used across permissions/audit | PARTIAL | PARTIAL | PARTIAL |
| Switchboard (open tickets) | Lists open tickets + activity, permission gated actions, order type buttons | PARTIAL | PARTIAL | PARTIAL |
| New ticket workflow | Select order type; table/customer requirements enforced; guest count; create ticket | PARTIAL | PARTIAL | PARTIAL |
| Order entry core | High-throughput ticket/menu split view; add items; hold/done; send to kitchen; misc items | PARTIAL | PARTIAL | PARTIAL |
| Modifiers (standard) | Auto-popup modifier dialogs; enforce required groups; apply pricing/tax | PARTIAL | PARTIAL | PARTIAL |
| Pizza modifiers | Half/quarter modifiers + price adjustments | PARTIAL | PARTIAL | PARTIAL |
| Item search | Search items by name/barcode; add to ticket | PARTIAL | PARTIAL | PARTIAL |
| Cooking instructions | Select/add cooking instructions per item | PARTIAL | PARTIAL | PARTIAL |
| Combo selection | Choose combo items within constraints | PARTIAL | PARTIAL | PARTIAL |
| Variable price entry | Prompt for price on variable-price items | PARTIAL | PARTIAL | PARTIAL |
| Settlement / payment keypad | Tender keypad; quick cash; next amount; exact due; tender type selection | PARTIAL | PARTIAL | PARTIAL |
| Card processing surfaces | Swipe card, manual entry, auth code, wait dialog | PARTIAL | PARTIAL | PARTIAL |
| Tax exempt | Toggle tax exempt on ticket | PARTIAL | PARTIAL | PARTIAL |
| No sale / open drawer | Drawer kick and audit/transaction logging | PARTIAL | PARTIAL | PARTIAL |
| Cash drop / payout / bleed | Record cash movements with reasons; management list | PARTIAL | PARTIAL | PARTIAL |
| Drawer pull report | Generate and print drawer pull report | PARTIAL | PARTIAL | PARTIAL |
| Split ticket | Split ticket by seat/amount/even; transfer payments; transactional integrity | PARTIAL | PARTIAL | PARTIAL |
| Void ticket | Void workflow with reasons, audit, optional prints | PARTIAL | PARTIAL | PARTIAL |
| Refunds | Refund validation and limits; refund flows | PARTIAL | NONE | PARTIAL |
| Transfer ticket | Transfer ownership/user; manager permissions | PARTIAL | NONE | PARTIAL |
| Tables / dining room | Table map + select/assign/change/release table workflows | PARTIAL | PARTIAL | PARTIAL |
| Kitchen display / KDS | Stateful kitchen order list with bump/void/status | PARTIAL | PARTIAL | PARTIAL |
| Back office shell | Explorer/config/report menus; permission gating | PARTIAL | PARTIAL | PARTIAL |
| Reports (sales balance, exceptions, journal, productivity, labor, delivery) | Generate reports over time ranges; filter by user/entity | PARTIAL | PARTIAL | PARTIAL |
| Printer configuration | Configure printers, groups, routing | PARTIAL | NONE | PARTIAL |

## Notes
- “Backend Parity” reflects existence of commands/queries/controllers, not correctness of business rules.
- “Frontend Parity” reflects presence of a user-facing surface, not UX parity.
