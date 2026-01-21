
# WPA API Contracts

This directory contains the formal HTTP API contracts derived from the WPA Web frontend implementation.

## Structure
- `auth.md`: Authentication endpoints.
- `tables.md`: Table management and session control.
- `menu.md`: Product catalog and search.
- `orders.md`: Ticket management and order submission.
- `validation_report.md`: Analysis of assumptions and potential risks.

## Source of Truth
These contracts are derived from:
- `WPA.Web/src/types/index.ts`
- `WPA.Web/src/services/interfaces.ts`
- `WPA.Web/src/services/mock/*.ts`

## Rules
- The Backend MUST implement these endpoints.
- The Frontend MUST NOT rely on fields not defined here.
- Any change to these contracts requires a synchronized update to both Frontend and Backend.
