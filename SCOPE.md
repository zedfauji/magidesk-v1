# Magidesk POS - Full Scope Definition

## Scope Philosophy
This is a **FULL POS system** rebuild, not an MVP. The architecture must support all features from the start, even if implementation is phased. We design for completeness, then implement incrementally.

## Core Features (Must Have)

### Ticket Management
- Create, edit, and manage tickets
- Multiple items per ticket
- Item modifiers and add-ons
- Item-level and ticket-level discounts
- Tax calculation (including tax-exempt)
- Service charges
- Delivery charges
- Adjustment amounts
- Ticket splitting
- Ticket voiding (with reason)
- Ticket reopening (if needed)
- Kitchen status tracking

### Payment Processing
- Multiple payment types:
  - Cash (with change calculation)
  - Credit Card (authorize/capture workflow)
  - Debit Card
  - Gift Certificate (with cash back)
  - Custom Payment (flexible fields)
- Split payments (multiple payments per ticket)
- Partial payments
- Tips/Gratuity (can be added to transactions)
- Refunds (full or partial)
- Payment voiding

### Cash Management
- Cash drawer assignment
- Cash session management (explicit open/close)
- Drawer pull reports
- Cash accountability tracking
- Payouts
- Cash drops
- Drawer bleeds
- Multi-currency support (if needed)

### Order Types
- Dine In
- Take Out
- Pickup
- Home Delivery
- Drive Through
- Bar Tab (authorize/capture workflow)
- Custom order types

### Discounts
- Item-level discounts
- Ticket-level discounts
- Discount types:
  - Fixed amount
  - Percentage
  - Fixed per item/category/order
  - Percentage per item/category/order
  - Free amount
- Minimum buy requirements
- Max discount selection (only one applies)

### Shifts
- Shift definition
- Ticket association with shifts
- Drawer pulls per shift
- Shift reports

### Additional Features
- Table management (restaurant)
- Customer management
- Kitchen printing
- Receipt printing
- Menu item management
- Modifier management
- Tax configuration
- User management and permissions
- Reports (sales, drawer pulls, etc.)

## Architecture Requirements

### Must Support From Start
1. **Multiple Payments Per Ticket**: Architecture must handle split payments
2. **All Payment Types**: Cash, Card, Gift Cert, Custom
3. **Tips/Gratuity**: Separate from payment amount
4. **Refunds**: Full and partial refunds
5. **Ticket Splitting**: Split items across multiple tickets
6. **Discounts**: Item and ticket level, multiple types
7. **Tax Complexity**: Tax-exempt, price-includes-tax, multiple tax rates
8. **Service/Delivery Charges**: Additional charges on tickets
9. **Adjustments**: Manual adjustments to totals
10. **Audit Trail**: Complete audit of all financial operations

### Implementation Phases
While architecture supports all features, implementation can be phased:

**Phase 1: Core Transaction Flow**
- Basic ticket creation
- Cash payments only
- Simple discounts
- Basic tax calculation
- Ticket closing

**Phase 2: Payment Expansion**
- Credit/Debit card processing
- Gift certificates
- Split payments
- Tips

**Phase 3: Advanced Features**
- Ticket splitting
- Refunds
- Custom payments
- Advanced discounts

**Phase 4: Restaurant Features**
- Table management
- Kitchen printing
- Order types
- Seat-based ordering

**Phase 5: Management Features**
- Reports
- User management
- Menu management
- Configuration

## Out of Scope (For Now)
- Inventory management (future)
- Multi-location support (future)
- Online ordering integration (future)
- Loyalty programs (future)
- Advanced analytics (future)

## Success Criteria

### Functional
- Can handle all payment types
- Can process split payments
- Can handle refunds
- Can split tickets
- Complete audit trail
- Accurate financial calculations
- Offline-capable

### Technical
- Clean Architecture maintained
- All invariants enforced
- Domain logic in Domain layer only
- No business logic in UI
- Testable and maintainable
- Extensible for future features

## Design Principles

1. **Design for Full Scope**: Architecture supports all features
2. **Implement Incrementally**: Build features in phases
3. **Never Compromise Architecture**: Don't take shortcuts that limit future features
4. **Test Everything**: Comprehensive tests for all business logic
5. **Audit Everything**: All financial mutations are auditable
6. **Fix Legacy Issues**: Don't replicate flawed behaviors from reference systems

