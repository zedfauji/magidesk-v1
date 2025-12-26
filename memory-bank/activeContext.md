# Active Context

## Current Work Focus
- **Backend Forensic Audit** 🔍
  - We are shifting from UI Audit to Backend Audit.
  - Objective: Document backend invariants, forbidden states, and concurrency guarantees for all 132 features.
  - Output: `docs/forensic-backend-audit/F-XXXX-backend.md` for each feature.

## Guardrails (See .clinerules for full list)
- **FORENSIC PARITY**: `docs/forensic-ui-audit/features/F-XXXX.md` is the Source of Truth.
- **FINANCIAL INTEGRITY**: `decimal` only. Domain-layer naming only.
- **ARCHITECTURE**: Clean Architecture (Domain -> Application -> Infrastructure -> Presentation).
- **MEMORY BANK**: Keep this directory updated.

## Recent Changes
- ✅ **Forensic UI Audit COMPLETE**: All 132 features mapped to Linear Documents.
- ✅ **Memory Bank Initialized**: Guardrails and Context files established.
- ✅ Phase 2 COMPLETE (Payment Expansion).
- ✅ Phase 3 Weeks 8-10 COMPLETE (Tax, Refunds, Service Charges).

## Active Documents
- `docs/forensic-ui-audit/features/` (Reference)
- `docs/forensic-backend-audit/` (Target Output)
- `.clinerules` (Rules)
nsactions
  - Added gratuity workflow methods to Ticket
  - Enhanced PaymentDomainService with tip validation and calculation
- ✅ Phase 2 Week 7: Card Processing Integration
  - Created IPaymentGateway interface for external payment gateway integration
  - Implemented MockPaymentGateway for development/testing
  - Card authorization workflow (AuthorizeCardPaymentCommand)
  - Card capture workflow (CaptureCardPaymentCommand)
  - Card void workflow (VoidCardPaymentCommand)
  - Add tips to card payment workflow (AddTipsToCardPaymentCommand)
  - All command handlers with audit event logging
- ✅ Database: `magidesk_new` created and migrated (postgres/postgres)
- ✅ All entity configurations completed
- ✅ Repository implementations completed
- ✅ Dependency Injection setup completed
- ✅ Integration tests created (4+ tests passing)
- ✅ Payment.Create factory method added

## Next Steps
1. Phase 2 Week 5: Multiple Payment Types
   - CreditCardTransaction entity
   - DebitCardTransaction entity
   - GiftCertificateTransaction entity
   - CustomPaymentTransaction entity
   - TPH (Table Per Hierarchy) for Payment

## Active Decisions
- Using `magidesk_new` database to avoid mixing with legacy
- Mapping Modifiers/AddOns through single relationship with domain-level splitting
- Payment is concrete for Phase 1 (will become abstract in Phase 2)

## Important Patterns
- Clean Architecture: Domain → Application → Infrastructure → Presentation
- Repository pattern for data access
- CQRS for commands and queries
- Domain events for audit trail

## Workspace / MCP Filesystem Path (Source of Truth)
- **MCP filesystem root**: `/projects`
- **Repository path (use for MCP file tools)**: `/projects/Code/Redesign-POS/Windows Based POS/Magidesk`

