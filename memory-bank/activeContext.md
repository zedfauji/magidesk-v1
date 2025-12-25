# Active Context

## Current Work Focus
- **Phase 2 COMPLETE** ✅ (Payment Expansion - Weeks 5-7)
- **Phase 3 Week 8: IN PROGRESS** - Discounts & Tax
  - ✅ Created TaxRate and TaxGroup value objects
  - ✅ Created TaxDomainService for tax calculations
  - ✅ Enhanced TicketDomainService to use TaxDomainService
  - ✅ Ticket.cs file restored (all properties, methods, and behaviors)
  - ✅ Added PriceIncludesTax property to Ticket
  - ⏳ Need to complete tax calculation integration and price-includes-tax logic

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

