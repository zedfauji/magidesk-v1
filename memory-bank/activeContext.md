# Active Context

## Current Work Focus
- **Phase 2 COMPLETE** ✅ (Payment Expansion - Weeks 5-7)
- **Phase 3 Week 8: COMPLETE** ✅ (Discounts & Tax)
  - ✅ Created TaxRate and TaxGroup value objects
  - ✅ Created TaxDomainService for tax calculations
  - ✅ Enhanced TicketDomainService to use TaxDomainService
  - ✅ Ticket.cs file restored (all properties, methods, and behaviors)
  - ✅ Added PriceIncludesTax property to Ticket
  - ✅ Completed tax calculation integration with price-includes-tax mode
  - ✅ Tax-exempt support implemented
  - ✅ EF Core configuration updated for PriceIncludesTax
  - ✅ 40 comprehensive tax tests passing
- **Phase 3 Week 9: COMPLETE** ✅ (Refunds & Ticket Splitting)
  - ✅ Payment.CreateRefund() factory method for all payment types
  - ✅ Ticket.ProcessRefund() method
  - ✅ Updated RecalculatePaidAmount() to handle Debit transactions
  - ✅ RefundPaymentCommand and RefundTicketCommand handlers
  - ✅ SplitTicketCommand handler
  - ✅ 14 comprehensive refund/split tests passing
- **Phase 3 Week 10: COMPLETE** ✅ (Service Charges & Adjustments)
  - ✅ Ticket methods: SetServiceCharge, SetDeliveryCharge, SetAdjustment, SetAdvancePayment
  - ✅ ServiceChargeDomainService for percentage and per-guest calculations
  - ✅ Commands and handlers for all charges/adjustments
  - ✅ 15 comprehensive service charge/adjustment tests passing
  - ✅ **Total: 105 tests passing**

## Recent Changes
- ✅ Phase 1 COMPLETE: Foundation & Core Transaction Flow
- ✅ Phase 2 Week 5: Multiple Payment Types (TPH implementation)
  - Created payment type hierarchy (CashPayment, CreditCardPayment, DebitCardPayment, GiftCertificatePayment, CustomPayment)
  - Configured EF Core TPH for payment types
  - Created migration `PaymentTypesTPH`
- ✅ Phase 2 Week 6: Split Payments & Tips
  - Enhanced payment validation for split payments
  - Added tips support on card transactions
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

