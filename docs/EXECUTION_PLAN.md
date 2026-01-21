# Magidesk POS - Execution Plan (Full POS Scope)

## Overview
This is a **full POS system** rebuild. Architecture supports all features from the start, but implementation will be phased for manageability.

## Implementation Phases

### Phase 1: Foundation & Core Transaction Flow (Weeks 1-4)

#### Week 1: Domain Model Implementation
**Goal**: Implement core domain entities with all invariants

- [ ] **Day 1-2: Value Objects**
  - [ ] Money value object (with currency support)
  - [ ] UserId value object
  - [ ] Unit tests

- [ ] **Day 3-5: Core Entities**
  - [ ] Ticket entity (full model with all properties)
  - [ ] OrderLine entity (with modifiers support)
  - [ ] Payment entity (base with type support)
  - [ ] CashSession entity
  - [ ] AuditEvent entity
  - [ ] Domain exceptions
  - [ ] Unit tests for all invariants

#### Week 2: Domain Services & Events
- [ ] TicketDomainService (all methods)
- [ ] PaymentDomainService
- [ ] CashSessionDomainService
- [ ] DiscountDomainService
- [ ] Domain events (interfaces and base classes)
- [ ] Unit tests

#### Week 3: Application Layer - Core Use Cases
- [ ] Repository interfaces
- [ ] Application service interfaces
- [ ] DTOs for all entities
- [ ] Commands:
  - [ ] CreateTicketCommand
  - [ ] AddOrderLineCommand
  - [ ] RemoveOrderLineCommand
  - [ ] ModifyOrderLineCommand
  - [ ] ApplyDiscountCommand
  - [ ] ProcessPaymentCommand (cash only in Phase 1)
  - [ ] CloseTicketCommand
  - [ ] VoidTicketCommand
- [ ] Queries:
  - [ ] GetTicketQuery
  - [ ] GetMenuItemsQuery
- [ ] Unit tests

#### Week 4: Infrastructure - EF Core Setup
- [ ] PostgreSQL connection configuration
- [ ] DbContext setup
- [ ] Entity configurations (all entities)
- [ ] Value object mappings (Money)
- [ ] Optimistic concurrency (Version fields)
- [ ] Database migrations
- [ ] Repository implementations
- [ ] Integration tests

### Phase 2: Payment Expansion (Weeks 5-7)

#### Week 5: Multiple Payment Types
- [ ] CreditCardTransaction entity
- [ ] DebitCardTransaction entity
- [ ] GiftCertificateTransaction entity
- [ ] CustomPaymentTransaction entity
- [ ] Payment type factory
- [ ] Payment processing workflows
- [ ] Unit tests

#### Week 6: Split Payments & Tips
- [ ] Multiple payments per ticket support
- [ ] Partial payment handling
- [ ] Gratuity entity and workflow
- [ ] Tips on card transactions
- [ ] Payment validation logic
- [ ] Unit tests

#### Week 7: Card Processing Integration
- [ ] Card authorization workflow
- [ ] Card capture workflow
- [ ] Card void workflow
- [ ] External payment gateway interfaces
- [ ] Mock payment processor (for development)
- [ ] Integration tests

### Phase 3: Advanced Features (Weeks 8-10)

#### Week 8: Discounts & Tax
- [ ] Discount entity (reference data)
- [ ] TicketDiscount entity
- [ ] OrderLineDiscount entity
- [ ] Discount calculation logic
- [ ] Max discount selection
- [ ] Tax calculation (multiple rates, tax groups)
- [ ] Tax-exempt support
- [ ] Price-includes-tax mode
- [ ] Unit tests

#### Week 9: Refunds & Ticket Splitting
- [ ] RefundTransaction entity
- [ ] Refund workflow
- [ ] Partial refunds
- [ ] Ticket split workflow
- [ ] Split ticket validation
- [ ] Unit tests

#### Week 10: Service Charges & Adjustments
- [ ] Service charge calculation
- [ ] Delivery charge handling
- [ ] Adjustment amount support
- [ ] Advance payment support
- [ ] Unit tests

### Phase 4: Cash Management (Weeks 11-12)

#### Week 11: Cash Session Management
- [ ] Cash session open/close workflow
- [ ] Expected cash calculation
- [ ] Payout entity and workflow
- [ ] Cash drop entity and workflow
- [ ] Drawer bleed entity and workflow
- [ ] Drawer pull report generation
- [ ] Unit tests

#### Week 12: Shift Management
- [ ] Shift entity
- [ ] Shift assignment to tickets
- [ ] Shift reports
- [ ] Integration with cash sessions
- [ ] Unit tests

### Phase 5: Restaurant Features (Weeks 13-15)

#### Week 13: Order Types & Tables
- [ ] OrderType entity
- [ ] Table entity
- [ ] Table status management
- [ ] Table assignment to tickets
- [ ] Order type pricing
- [ ] Unit tests

#### Week 14: Modifiers & Add-ons
- [ ] MenuModifier entity
- [ ] ModifierGroup entity
- [ ] OrderLineModifier entity
- [ ] Modifier pricing logic
- [ ] Pizza-style modifiers (sections)
- [ ] Unit tests

#### Week 15: Kitchen & Receipt Printing
- [ ] Kitchen printing workflow
- [ ] Receipt printing workflow
- [ ] Print service interfaces
- [ ] Mock printers (for development)
- [ ] Unit tests

### Phase 6: UI Development (Weeks 16-22)

#### Week 16: UI Foundation
- [ ] WinUI 3 project setup
- [ ] Dependency injection configuration
- [ ] Navigation setup
- [ ] Basic layout and styling
- [ ] ViewModel base classes

#### Week 17: Cash Session UI
- [ ] CashSessionViewModel
- [ ] CashSessionView
- [ ] Open/close session workflow
- [ ] Drawer pull report display

#### Week 18: Ticket Entry UI
- [ ] TicketViewModel
- [ ] TicketView
- [ ] OrderLineViewModel
- [ ] Menu item selection
- [ ] Item quantity entry
- [ ] Totals display

#### Week 19: Payment UI
- [ ] PaymentViewModel
- [ ] PaymentView
- [ ] Payment type selection
- [ ] Cash payment (with change)
- [ ] Card payment (authorize/capture)
- [ ] Split payment support
- [ ] Tips entry

#### Week 20: Discount & Tax UI
- [ ] Discount selection dialog
- [ ] Tax display
- [ ] Service charge display
- [ ] Adjustment entry

#### Week 21: Ticket Management UI
- [ ] Ticket list view
- [ ] Ticket detail view
- [ ] Void ticket dialog
- [ ] Refund dialog
- [ ] Split ticket dialog

#### Week 22: Reports & Settings UI
- [ ] Drawer pull report view
- [ ] Sales reports
- [ ] Settings views
- [ ] User management UI

### Phase 7: Testing & Refinement (Weeks 23-24)

#### Week 23: Comprehensive Testing
- [ ] Domain layer unit tests (>90% coverage)
- [ ] Application layer unit tests (>80% coverage)
- [ ] Integration tests
- [ ] End-to-end tests
- [ ] Performance testing
- [ ] Error scenario testing

#### Week 24: Bug Fixes & Documentation
- [ ] Fix identified bugs
- [ ] Performance optimizations
- [ ] Code review and refactoring
- [ ] User documentation
- [ ] Developer documentation
- [ ] Deployment preparation

## Database Setup

### PostgreSQL Configuration
- **Database**: `magidesk_pos` (already exists)
- **Connection**: Local passwordless PostgreSQL server
- **Schema**: `public` (or create dedicated schema)
- **Migrations**: EF Core migrations for all schema changes

### Initial Database Structure
- All tables will be created via EF Core migrations
- Seed data for:
  - OrderTypes (Dine In, Take Out, etc.)
  - Shifts (default shifts)
  - Menu items (sample data)
  - Users (admin user)

## Technology Stack (Confirmed)

- **.NET**: 8.0
- **UI**: WinUI 3
- **MVVM**: CommunityToolkit.Mvvm
- **ORM**: EF Core
- **Database**: PostgreSQL (local, passwordless)
- **Testing**: xUnit, FluentAssertions, Moq
- **Validation**: FluentValidation

## Success Metrics

### Code Quality
- [ ] Domain layer >90% test coverage
- [ ] Application layer >80% test coverage
- [ ] Zero business logic in Presentation layer
- [ ] All invariants have unit tests
- [ ] All dependencies point inward

### Functionality
- [ ] Complete transaction flow (all payment types)
- [ ] Split payments working
- [ ] Refunds working
- [ ] Ticket splitting working
- [ ] All discounts working
- [ ] Cash session management working
- [ ] All invariants enforced
- [ ] Complete audit trail

### Performance
- [ ] UI operations <100ms
- [ ] Can handle 100+ items per ticket
- [ ] Database queries optimized
- [ ] No memory leaks

## Risk Mitigation

### Risk: Complexity Overwhelming
- **Mitigation**: Phased implementation, start with core flow
- **Checkpoint**: Review after Phase 1

### Risk: Database Schema Changes
- **Mitigation**: EF Core migrations, version control
- **Checkpoint**: Review migrations before applying

### Risk: Payment Gateway Integration
- **Mitigation**: Abstract interfaces, mock implementations first
- **Checkpoint**: Test with mocks before real integration

### Risk: UI Complexity
- **Mitigation**: Incremental UI development, reusable components
- **Checkpoint**: Review after each UI phase

## Immediate Next Steps (Today)

1. **Update Domain Model**
   - Replace DOMAIN_MODEL.md with full version
   - Ensure all entities from FloreantPOS analysis are included

2. **Update Invariants**
   - Replace INVARIANTS.md with full version
   - Include all invariants for split payments, refunds, etc.

3. **Database Setup**
   - Verify PostgreSQL connection
   - Plan initial schema (will be created via EF Core migrations)

4. **Start Domain Implementation**
   - Begin with Money value object
   - Then Ticket entity
   - Build incrementally

## Questions to Resolve

1. **Database Schema**: Use existing tables or create new schema?
2. **Payment Gateway**: Which payment processor to integrate?
3. **Receipt Printer**: Which printer model/brand?
4. **Card Reader**: Which card reader hardware?
5. **Multi-terminal**: Required for Phase 1 or later?

## Notes

- Architecture supports all features from start
- Implementation is phased for manageability
- Each phase builds on previous phases
- Can adjust phases based on priorities
- Database already exists - will use EF Core migrations to create/update schema

