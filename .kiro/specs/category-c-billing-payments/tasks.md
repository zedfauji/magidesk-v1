# Tasks Document

## Overview

This document breaks down the Category C: Billing, Payments & Pricing implementation into discrete, actionable tasks. Tasks are organized by sprint following the implementation plan, with each task referencing specific requirements from requirements.md.

**Task Format**:
- Tasks marked with "*" postfix are optional test-related tasks
- Each task includes requirement references (e.g., REQ-1.1)
- Checkpoint tasks are included at reasonable breaks
- Tasks are ordered by dependency and logical implementation flow

**Estimated Timeline**: 8 weeks (4 sprints)

---

## Sprint 1: Critical Payment Features (Weeks 1-2)

### Feature 1.1: Hold Ticket Backend (C.2)

**Task 1.1.1**: Update TicketStatus enum
- Add `Held = 2` status to `TicketStatus` enum
- Renumber subsequent statuses (Paid = 3, Voided = 4, Refunded = 5)
- Update all references to use new status values
- **Requirements**: REQ-1.1
- **Files**: `Magidesk.Domain/Enumerations/TicketStatus.cs`

**Task 1.1.2**: Enhance Ticket entity with hold support
- Add properties: `HeldAt`, `HoldReason`, `HeldBy`
- Implement `Hold(string reason, UserId userId)` method
- Implement `Release()` method
- Add validation: prevent hold if status is Closed, Voided, or Refunded
- Add validation: require non-empty reason
- **Requirements**: REQ-1.1, REQ-1.2, REQ-1.4, REQ-1.5, REQ-1.6
- **Files**: `Magidesk.Domain/Entities/Ticket.cs`

**Task 1.1.3**: Create domain events for hold operations
- Create `TicketHeldEvent` with ticket ID, reason, held by, timestamp
- Create `TicketReleasedEvent` with ticket ID, released by, timestamp
- **Requirements**: REQ-1.8
- **Files**: `Magidesk.Domain/Events/TicketHeldEvent.cs`, `Magidesk.Domain/Events/TicketReleasedEvent.cs`

**Task 1.1.4**: Create HoldTicketCommand and handler
- Create `HoldTicketCommand` record with TicketId, Reason, UserId
- Implement `HoldTicketCommandHandler`
- Load ticket, call Hold() method, save ticket
- End table session if ticket has associated table
- Publish TicketHeldEvent
- **Requirements**: REQ-1.1, REQ-1.2, REQ-1.3, REQ-1.8
- **Files**: `Magidesk.Application/Commands/HoldTicketCommand.cs`, `Magidesk.Application/Services/HoldTicketCommandHandler.cs`

**Task 1.1.5**: Create ReleaseHeldTicketCommand and handler
- Create `ReleaseHeldTicketCommand` record with TicketId, UserId
- Implement `ReleaseHeldTicketCommandHandler`
- Load ticket, call Release() method, save ticket
- Publish TicketReleasedEvent
- **Requirements**: REQ-1.6, REQ-1.8
- **Files**: `Magidesk.Application/Commands/ReleaseHeldTicketCommand.cs`, `Magidesk.Application/Services/ReleaseHeldTicketCommandHandler.cs`

**Task 1.1.6**: Create GetHeldTicketsQuery and handler
- Create `GetHeldTicketsQuery` record
- Create `HeldTicketDto` with all display fields
- Implement `GetHeldTicketsQueryHandler`
- Query tickets with Status = Held, include hold details
- **Requirements**: REQ-1.7
- **Files**: `Magidesk.Application/Queries/GetHeldTicketsQuery.cs`, `Magidesk.Application/DTOs/HeldTicketDto.cs`, `Magidesk.Application/Services/GetHeldTicketsQueryHandler.cs`

**Task 1.1.7**: Update ITicketRepository interface
- Add `Task<IReadOnlyList<Ticket>> GetHeldTicketsAsync()` method
- **Requirements**: REQ-1.7
- **Files**: `Magidesk.Application/Interfaces/ITicketRepository.cs`

**Task 1.1.8**: Implement GetHeldTicketsAsync in repository
- Implement query with EF Core: filter by Status = Held
- Include related entities (Customer, Table, OrderLines)
- Order by HeldAt descending
- **Requirements**: REQ-1.7
- **Files**: `Magidesk.Infrastructure/Repositories/TicketRepository.cs`

**Task 1.1.9**: Update TicketConfiguration for EF Core
- Add property mappings for HeldAt, HoldReason, HeldBy
- Configure HoldReason max length (500)
- Add filtered index on HeldAt where Status = Held
- **Requirements**: REQ-1.2
- **Files**: `Magidesk.Infrastructure/Data/Configurations/TicketConfiguration.cs`

**Task 1.1.10**: Create database migration script
- Create SQL script to add columns: HeldAt, HoldReason, HeldBy
- Create filtered index: IX_Tickets_HeldAt_Held
- **Requirements**: REQ-1.2
- **Files**: `add_hold_ticket_columns.sql`

**Task 1.1.11**: Apply database migration
- Execute SQL migration script against database
- Update EF Core model snapshot
- Verify columns exist in database
- **Requirements**: REQ-1.2
- **Files**: `Magidesk.Migrations/Migrations/ApplicationDbContextModelSnapshot.cs`

**Task 1.1.12***: Write unit tests for Ticket.Hold() and Release()
- Test hold with valid inputs
- Test hold with invalid status (Closed, Voided, Refunded)
- Test hold with empty reason
- Test release changes status to Open
- **Requirements**: REQ-1.1, REQ-1.4, REQ-1.5, REQ-1.6
- **Validates**: Property 1, 3, 4, 5
- **Files**: `Magidesk.Domain.Tests/Entities/TicketTests.cs`

**Task 1.1.13***: Write property-based tests for hold operations
- Property 1: Hold ticket state transition
- Property 3: Hold ticket invalid states
- Property 4: Hold ticket validation
- Property 5: Release held ticket round trip
- **Requirements**: REQ-1.1, REQ-1.4, REQ-1.5, REQ-1.6
- **Files**: `Magidesk.Domain.Tests/Properties/TicketHoldPropertiesTests.cs`

**Task 1.1.14***: Write integration tests for hold commands
- Test HoldTicketCommand end-to-end
- Test ReleaseHeldTicketCommand end-to-end
- Test GetHeldTicketsQuery returns correct results
- Verify table session ended when ticket held
- Verify audit events created
- **Requirements**: REQ-1.1, REQ-1.3, REQ-1.6, REQ-1.7, REQ-1.8
- **Validates**: Property 2, 6, 7
- **Files**: `Magidesk.Application.Tests/Commands/HoldTicketCommandTests.cs`

**CHECKPOINT 1.1**: Hold Ticket Backend Complete
- All backend code compiles without errors
- Database migration applied successfully
- All tests passing (if implemented)
- Ready for frontend integration

### Feature 1.2: Hold Ticket Frontend (C.2)

**Task 1.2.1**: Create HoldTicketDialogViewModel
- Add properties: HoldReason, SelectedReasonCode, ReasonCodes list
- Implement HoldTicketCommand (RelayCommand)
- Call HoldTicketCommand handler via mediator
- Handle success and error responses
- **Requirements**: REQ-1.1, REQ-1.5, REQ-11.4
- **Files**: `Magidesk.Presentation/ViewModels/Dialogs/HoldTicketDialogViewModel.cs`

**Task 1.2.2**: Create HoldTicketDialog view
- Create dialog with reason dropdown and text input
- Add predefined reason codes (Customer Request, Payment Issue, Other)
- Add OK and Cancel buttons
- Bind to HoldTicketDialogViewModel
- **Requirements**: REQ-1.1, REQ-1.5, REQ-11.1
- **Files**: `Magidesk.Presentation/Views/Dialogs/HoldTicketDialog.xaml`

**Task 1.2.3**: Add Hold Ticket button to SettlePage
- Add "Hold Ticket" button to settle page toolbar
- Show button only when ticket status is Open
- Open HoldTicketDialog on click
- Refresh ticket after successful hold
- **Requirements**: REQ-11.1
- **Files**: `Magidesk.Presentation/Views/SettlePage.xaml`, `Magidesk.Presentation/ViewModels/SettlePageViewModel.cs`

**Task 1.2.4**: Create HeldTicketsViewModel
- Add HeldTickets observable collection
- Implement RefreshCommand to load held tickets
- Implement ReleaseTicketCommand for individual ticket
- Implement ViewTicketDetailsCommand
- Handle loading states and errors
- **Requirements**: REQ-1.6, REQ-1.7, REQ-11.5
- **Files**: `Magidesk.Presentation/ViewModels/HeldTicketsViewModel.cs`

**Task 1.2.5**: Create HeldTicketsPage view
- Create page with DataGrid showing held tickets
- Display columns: Ticket #, Customer, Table, Amount, Held At, Reason, Held By
- Add "Release" button for each ticket
- Add "View Details" button for each ticket
- Add refresh button in toolbar
- **Requirements**: REQ-1.7, REQ-11.1
- **Files**: `Magidesk.Presentation/Views/HeldTicketsPage.xaml`

**Task 1.2.6**: Add navigation route for Held Tickets page
- Register HeldTicketsPage in navigation service
- Add menu item in main navigation
- Add icon for Held Tickets (e.g., pause icon)
- **Requirements**: REQ-11.7
- **Files**: `Magidesk.Presentation/Services/NavigationService.cs`, `MainWindow.xaml`

**Task 1.2.7**: Update ticket status indicators
- Add visual indicator for "Held" status
- Use distinct color (e.g., orange) for held tickets
- Show hold icon in ticket lists
- Display hold reason in tooltip
- **Requirements**: REQ-11.2
- **Files**: `Magidesk.Presentation/Converters/TicketStatusToColorConverter.cs`, `Magidesk.Presentation/Styles/TicketStyles.xaml`

**Task 1.2.8***: Test Hold Ticket UI flow
- Manual test: Hold ticket from settle page
- Verify dialog opens and captures reason
- Verify ticket appears in Held Tickets page
- Verify table is released
- Manual test: Release ticket from Held Tickets page
- Verify ticket status changes to Open
- **Requirements**: REQ-1.1, REQ-1.3, REQ-1.6, REQ-1.7

**CHECKPOINT 1.2**: Hold Ticket Frontend Complete
- UI compiles and runs without errors
- Hold ticket dialog functional
- Held Tickets page displays correctly
- Navigation working
- Ready for end-to-end testing

### Feature 1.3: Split Payment Processing (C.4)

**Task 1.3.1**: Enhance Payment entity for split support
- Add properties: SplitGroupId (Guid?), SplitSequence (int?)
- Add properties: RefundedAmount, IsRefunded
- Update constructor to support split payment fields
- **Requirements**: REQ-2.7, REQ-2.8
- **Files**: `Magidesk.Domain/Entities/Payment.cs`

**Task 1.3.2**: Create SplitPaymentEntry value object
- Create record with PaymentMethod and Amount
- Add validation: Amount must be positive
- **Requirements**: REQ-2.1
- **Files**: `Magidesk.Domain/ValueObjects/SplitPaymentEntry.cs`

**Task 1.3.3**: Create ProcessSplitPaymentCommand
- Create command with TicketId, List<SplitPaymentEntry>, ProcessedBy
- Add validation: payments list not empty
- **Requirements**: REQ-2.1
- **Files**: `Magidesk.Application/Commands/ProcessSplitPaymentCommand.cs`

**Task 1.3.4**: Implement ProcessSplitPaymentCommandHandler
- Calculate sum of all payment amounts
- Validate sum equals ticket total (or handle overpayment)
- Reject if sum < total, return remaining amount
- Generate unique SplitGroupId for the payment group
- Create Payment entity for each entry with sequence number
- Add all payments to ticket
- Mark ticket as Paid if fully paid
- Calculate and return change if overpayment
- **Requirements**: REQ-2.1, REQ-2.2, REQ-2.3, REQ-2.4, REQ-2.7, REQ-2.8
- **Files**: `Magidesk.Application/Services/ProcessSplitPaymentCommandHandler.cs`

**Task 1.3.5**: Update PaymentConfiguration for EF Core
- Add property mappings for SplitGroupId, SplitSequence
- Add property mappings for RefundedAmount, IsRefunded
- Create filtered index on SplitGroupId where not null
- **Requirements**: REQ-2.7, REQ-2.8
- **Files**: `Magidesk.Infrastructure/Data/Configurations/PaymentConfiguration.cs`

**Task 1.3.6**: Create database migration for split payments
- Create SQL script to add columns: SplitGroupId, SplitSequence, RefundedAmount, IsRefunded
- Create filtered index: IX_Payments_SplitGroupId
- **Requirements**: REQ-2.7, REQ-2.8
- **Files**: `add_split_payment_columns.sql`

**Task 1.3.7***: Write unit tests for split payment validation
- Test sum equals total (valid)
- Test sum exceeds total (calculate change)
- Test sum less than total (reject with remaining)
- Test empty payments list (reject)
- Test negative payment amount (reject)
- **Requirements**: REQ-2.2, REQ-2.3, REQ-2.4
- **Validates**: Property 8, 9, 10
- **Files**: `Magidesk.Application.Tests/Commands/ProcessSplitPaymentCommandTests.cs`

**Task 1.3.8***: Write property-based tests for split payments
- Property 8: Split payment sum equals total
- Property 9: Split payment overpayment change
- Property 10: Split payment underpayment rejection
- Property 11: Split payment record count
- **Requirements**: REQ-2.2, REQ-2.3, REQ-2.4, REQ-2.8
- **Files**: `Magidesk.Application.Tests/Properties/SplitPaymentPropertiesTests.cs`

**Task 1.3.9**: Create SplitPaymentViewModel
- Add Payments observable collection (PaymentEntry items)
- Add RemainingAmount property (calculated)
- Add TotalEntered property (calculated)
- Implement AddPaymentCommand
- Implement RemovePaymentCommand
- Implement ProcessSplitPaymentCommand
- Update RemainingAmount on payment changes
- **Requirements**: REQ-2.1, REQ-2.4, REQ-11.3
- **Files**: `Magidesk.Presentation/ViewModels/SplitPaymentViewModel.cs`

**Task 1.3.10**: Create SplitPaymentDialog view
- Create dialog with payment entry grid
- Show payment method dropdown and amount input for each entry
- Display running total and remaining amount
- Add "Add Payment" button
- Add quick split buttons (2-way, 3-way, 4-way, custom)
- Add "Process Payment" button (enabled when remaining = 0)
- Show change amount if overpayment
- **Requirements**: REQ-2.1, REQ-2.4, REQ-2.5, REQ-11.2, REQ-11.3
- **Files**: `Magidesk.Presentation/Views/Dialogs/SplitPaymentDialog.xaml`

**Task 1.3.11**: Integrate split payment into SettlePage
- Add "Split Payment" button to settle page
- Open SplitPaymentDialog on click
- Pre-populate with ticket total
- Handle success response and refresh ticket
- Handle error response (underpayment, validation)
- **Requirements**: REQ-11.1
- **Files**: `Magidesk.Presentation/Views/SettlePage.xaml`, `Magidesk.Presentation/ViewModels/SettlePageViewModel.cs`

**CHECKPOINT 1.3**: Split Payment Complete
- Backend split payment logic working
- Database migration applied
- UI functional with quick split options
- Validation working (sum = total)
- Change calculation working

---

## Sprint 2: Enhanced Payment Features (Weeks 3-4)

### Feature 2.1: Discount Application (C.7)

**Task 2.1.1**: Create Discount entity
- Add properties: Id, Name, Type (Percentage/FixedAmount), Value
- Add properties: ApplicationType (Ticket/Item), RequiresAuthorization, IsActive
- Implement CalculateDiscount(Money amount) method
- Add validation: Value must be positive
- Add validation: Percentage must be 0-100
- **Requirements**: REQ-3.1, REQ-3.2
- **Files**: `Magidesk.Domain/Entities/Discount.cs`

**Task 2.1.2**: Create TicketDiscount entity
- Link Discount to Ticket with application details
- Add properties: DiscountId, TicketId, AppliedBy, AuthorizedBy, AppliedAt
- Track discount amount calculated at application time
- **Requirements**: REQ-3.2, REQ-3.7
- **Files**: `Magidesk.Domain/Entities/TicketDiscount.cs`

**Task 2.1.3**: Enhance Ticket entity with discount support
- Add Discounts collection (IReadOnlyList<TicketDiscount>)
- Implement ApplyDiscount(Discount, UserId, UserId?) method
- Validate discount doesn't result in negative total
- Check if discount requires authorization (>50%)
- Recalculate TotalAmount after discount
- Raise DiscountAppliedEvent
- Implement RemoveDiscount(DiscountId) method
- **Requirements**: REQ-3.1, REQ-3.5, REQ-3.7, REQ-3.8
- **Files**: `Magidesk.Domain/Entities/Ticket.cs`

**Task 2.1.4**: Create domain events for discounts
- Create DiscountAppliedEvent with ticket ID, discount ID, amount, applied by, authorized by
- Create DiscountRemovedEvent with ticket ID, discount ID, removed by
- **Requirements**: REQ-3.7
- **Files**: `Magidesk.Domain/Events/DiscountAppliedEvent.cs`, `Magidesk.Domain/Events/DiscountRemovedEvent.cs`

**Task 2.1.5**: Create ApplyDiscountCommand and handler
- Create command with TicketId, DiscountId, AppliedBy, AuthorizedBy (optional)
- Implement handler: load ticket and discount
- Check if authorization required (discount > 50% of total)
- If required and not provided, return error
- Call Ticket.ApplyDiscount() method
- Save ticket and publish event
- **Requirements**: REQ-3.1, REQ-3.2, REQ-3.5, REQ-3.7
- **Files**: `Magidesk.Application/Commands/ApplyDiscountCommand.cs`, `Magidesk.Application/Services/ApplyDiscountCommandHandler.cs`

**Task 2.1.6**: Create RemoveDiscountCommand and handler
- Create command with TicketId, DiscountId, RemovedBy
- Implement handler: load ticket, call RemoveDiscount()
- Recalculate ticket total
- Save ticket and publish event
- **Requirements**: REQ-3.7
- **Files**: `Magidesk.Application/Commands/RemoveDiscountCommand.cs`, `Magidesk.Application/Services/RemoveDiscountCommandHandler.cs`

**Task 2.1.7**: Implement member discount auto-application
- Create MemberDiscountService
- Implement GetMemberDiscount(CustomerId) method
- Auto-apply member discount when ticket created for member
- **Requirements**: REQ-3.3
- **Files**: `Magidesk.Application/Services/MemberDiscountService.cs`

**Task 2.1.8**: Create IDiscountRepository interface
- Add GetByIdAsync(DiscountId) method
- Add GetActiveDiscountsAsync() method
- Add GetMemberDiscountAsync(CustomerId) method
- **Requirements**: REQ-3.1, REQ-3.3
- **Files**: `Magidesk.Application/Interfaces/IDiscountRepository.cs`

**Task 2.1.9**: Implement DiscountRepository
- Implement all interface methods with EF Core
- Query active discounts only
- **Requirements**: REQ-3.1, REQ-3.3
- **Files**: `Magidesk.Infrastructure/Repositories/DiscountRepository.cs`

**Task 2.1.10**: Create database tables for discounts
- Create Discounts table with all fields
- Create TicketDiscounts junction table
- Add foreign keys and indexes
- **Requirements**: REQ-3.1, REQ-3.2
- **Files**: `create_discount_tables.sql`

**Task 2.1.11***: Write unit tests for discount calculation
- Test percentage discount calculation
- Test fixed amount discount calculation
- Test discount doesn't result in negative total
- Test member discount auto-application
- Test large discount requires authorization
- **Requirements**: REQ-3.1, REQ-3.3, REQ-3.5, REQ-3.8
- **Validates**: Property 12, 13, 14, 15
- **Files**: `Magidesk.Domain.Tests/Entities/TicketDiscountTests.cs`

**Task 2.1.12***: Write property-based tests for discounts
- Property 12: Discount calculation correctness
- Property 13: Discount non-negative total
- Property 14: Member discount auto-application
- Property 15: Large discount authorization
- Property 16: Discount audit trail
- **Requirements**: REQ-3.1, REQ-3.3, REQ-3.5, REQ-3.7, REQ-3.8
- **Files**: `Magidesk.Domain.Tests/Properties/DiscountPropertiesTests.cs`

**Task 2.1.13**: Create DiscountSelectionViewModel
- Add AvailableDiscounts observable collection
- Add SelectedDiscount property
- Implement ApplyDiscountCommand
- Check if authorization required, prompt for manager PIN
- Call ApplyDiscountCommand handler
- **Requirements**: REQ-3.1, REQ-3.5, REQ-11.4
- **Files**: `Magidesk.Presentation/ViewModels/DiscountSelectionViewModel.cs`

**Task 2.1.14**: Create DiscountSelectionDialog view
- Display list of available discounts
- Show discount name, type, and value
- Add quick discount buttons (10%, 20%, 50%, Custom)
- Add "Apply" button
- Show manager PIN dialog if required
- **Requirements**: REQ-3.1, REQ-3.5, REQ-11.1
- **Files**: `Magidesk.Presentation/Views/Dialogs/DiscountSelectionDialog.xaml`

**Task 2.1.15**: Integrate discount into SettlePage
- Add "Apply Discount" button to settle page
- Open DiscountSelectionDialog on click
- Display applied discounts in ticket summary
- Show original and discounted amounts
- Add "Remove Discount" button for each applied discount
- **Requirements**: REQ-3.6, REQ-11.1
- **Files**: `Magidesk.Presentation/Views/SettlePage.xaml`, `Magidesk.Presentation/ViewModels/SettlePageViewModel.cs`

**Task 2.1.16**: Add member discount indicator
- Show member discount badge on ticket if customer is member
- Auto-apply member discount when ticket opened
- Display member discount in ticket summary
- **Requirements**: REQ-3.3
- **Files**: `Magidesk.Presentation/Views/SettlePage.xaml`

**CHECKPOINT 2.1**: Discount Application Complete
- Discount entity and calculation working
- Member discount auto-applies
- Large discounts require authorization
- UI shows discounts clearly
- Audit trail created

### Feature 2.2: Price Override with Authorization (C.12)

**Task 2.2.1**: Create PriceOverride entity
- Add properties: Id, OrderLineId, OriginalPrice, OverridePrice, Variance
- Add properties: Reason, AppliedBy, AuthorizedBy, AppliedAt
- Calculate Variance in constructor (OverridePrice - OriginalPrice)
- Add validation: OverridePrice must be >= 0
- **Requirements**: REQ-4.3, REQ-4.6
- **Files**: `Magidesk.Domain/Entities/PriceOverride.cs`

**Task 2.2.2**: Enhance OrderLine entity with override support
- Add PriceOverride property (nullable)
- Add EffectivePrice property (returns override price if exists, else original)
- Implement ApplyPriceOverride(Money newPrice, string reason, UserId appliedBy, UserId authorizedBy) method
- Raise PriceOverriddenEvent
- **Requirements**: REQ-4.2, REQ-4.3, REQ-4.7
- **Files**: `Magidesk.Domain/Entities/OrderLine.cs`

**Task 2.2.3**: Create PriceOverriddenEvent
- Add properties: OrderLineId, OriginalPrice, OverridePrice, Reason, AppliedBy, AuthorizedBy
- **Requirements**: REQ-4.5
- **Files**: `Magidesk.Domain/Events/PriceOverriddenEvent.cs`

**Task 2.2.4**: Create OverrideLinePriceCommand and handler
- Create command with OrderLineId, NewPrice, Reason, AppliedBy, AuthorizedBy
- Implement handler: load order line
- Validate manager authorization provided
- Validate new price >= 0
- Call OrderLine.ApplyPriceOverride()
- Recalculate ticket total
- Save and publish event
- **Requirements**: REQ-4.1, REQ-4.2, REQ-4.3, REQ-4.5, REQ-4.6
- **Files**: `Magidesk.Application/Commands/OverrideLinePriceCommand.cs`, `Magidesk.Application/Services/OverrideLinePriceCommandHandler.cs`

**Task 2.2.5**: Create IPriceOverrideRepository interface
- Add GetOverridesAsync(startDate, endDate, staffMember, manager, minVariance) method
- **Requirements**: REQ-10.1, REQ-10.3
- **Files**: `Magidesk.Application/Interfaces/IPriceOverrideRepository.cs`

**Task 2.2.6**: Implement PriceOverrideRepository
- Implement query with EF Core
- Apply filters dynamically based on parameters
- Order by AppliedAt descending
- **Requirements**: REQ-10.1, REQ-10.3
- **Files**: `Magidesk.Infrastructure/Repositories/PriceOverrideRepository.cs`

**Task 2.2.7**: Update OrderLineConfiguration for EF Core
- Add navigation property for PriceOverride
- Configure one-to-one relationship
- **Requirements**: REQ-4.3
- **Files**: `Magidesk.Infrastructure/Data/Configurations/OrderLineConfiguration.cs`

**Task 2.2.8**: Create PriceOverrides database table
- Create table with all fields
- Add foreign key to OrderLines
- Add indexes on AppliedAt, AppliedBy, Variance
- **Requirements**: REQ-4.3
- **Files**: `create_price_overrides_table.sql`

**Task 2.2.9***: Write unit tests for price override
- Test override with valid authorization
- Test override without authorization (reject)
- Test negative price override (reject)
- Test variance calculation
- Test effective price calculation
- **Requirements**: REQ-4.1, REQ-4.3, REQ-4.6, REQ-4.7
- **Validates**: Property 17, 18, 19, 20
- **Files**: `Magidesk.Domain.Tests/Entities/OrderLinePriceOverrideTests.cs`

**Task 2.2.10***: Write property-based tests for price override
- Property 17: Price override authorization required
- Property 18: Price override non-negative
- Property 19: Price override data integrity
- Property 20: Price override total calculation
- Property 21: Price override audit trail
- **Requirements**: REQ-4.1, REQ-4.3, REQ-4.5, REQ-4.6, REQ-4.7
- **Files**: `Magidesk.Domain.Tests/Properties/PriceOverridePropertiesTests.cs`

**Task 2.2.11**: Create PriceOverrideDialogViewModel
- Add properties: OriginalPrice, NewPrice, Reason
- Implement OverridePriceCommand
- Prompt for manager PIN authorization
- Call OverrideLinePriceCommand handler
- **Requirements**: REQ-4.1, REQ-4.2, REQ-11.4
- **Files**: `Magidesk.Presentation/ViewModels/Dialogs/PriceOverrideDialogViewModel.cs`

**Task 2.2.12**: Create PriceOverrideDialog view
- Display original price (read-only)
- Add numeric input for new price with numpad
- Add reason text input
- Show variance calculation (new - original)
- Add "Override" button
- Trigger manager PIN dialog on submit
- **Requirements**: REQ-4.1, REQ-4.2, REQ-4.4
- **Files**: `Magidesk.Presentation/Views/Dialogs/PriceOverrideDialog.xaml`

**Task 2.2.13**: Add price override to order line items
- Add "Edit Price" context menu item to order lines
- Open PriceOverrideDialog on click
- Show visual indicator for overridden prices (e.g., different color)
- Display original price with strikethrough
- Display override price prominently
- **Requirements**: REQ-4.4, REQ-11.2
- **Files**: `Magidesk.Presentation/Views/SettlePage.xaml`, `Magidesk.Presentation/ViewModels/SettlePageViewModel.cs`

**CHECKPOINT 2.2**: Price Override Complete
- Price override entity and logic working
- Manager authorization required
- UI shows original and override prices
- Variance calculated correctly
- Audit trail created

### Feature 2.3: Void and Refund Processing (C.15)

**Task 2.3.1**: Enhance Ticket entity with void/refund support
- Implement Void(string reason, UserId voidedBy) method
- Validate ticket status is Open (not Paid)
- Change status to Voided
- Raise TicketVoidedEvent
- Implement Refund(Money amount, string reason, UserId refundedBy) method
- Validate amount <= PaidAmount
- Update RefundedAmount on payments
- Change status to Refunded (if full refund)
- Raise TicketRefundedEvent
- **Requirements**: REQ-5.1, REQ-5.2, REQ-5.3, REQ-5.4, REQ-5.5, REQ-5.9
- **Files**: `Magidesk.Domain/Entities/Ticket.cs`

**Task 2.3.2**: Create domain events for void/refund
- Create TicketVoidedEvent with ticket ID, reason, voided by
- Create TicketRefundedEvent with ticket ID, amount, reason, refunded by, is partial
- **Requirements**: REQ-5.8
- **Files**: `Magidesk.Domain/Events/TicketVoidedEvent.cs`, `Magidesk.Domain/Events/TicketRefundedEvent.cs`

**Task 2.3.3**: Create VoidTicketCommand and handler
- Create command with TicketId, Reason, VoidedBy, AuthorizedBy
- Implement handler: validate manager authorization
- Load ticket, validate status is Open
- If Paid, return error suggesting refund
- Call Ticket.Void() method
- Save and publish event
- **Requirements**: REQ-5.1, REQ-5.2, REQ-5.3, REQ-5.8
- **Files**: `Magidesk.Application/Commands/VoidTicketCommand.cs`, `Magidesk.Application/Services/VoidTicketCommandHandler.cs`

**Task 2.3.4**: Create RefundTicketCommand and handler
- Create command with TicketId, Amount, Reason, RefundedBy, AuthorizedBy, IsPartial
- Implement handler: validate manager authorization
- Load ticket, validate amount <= PaidAmount
- Call Ticket.Refund() method
- Update payment records with refunded amounts
- Generate refund receipt
- Save and publish event
- **Requirements**: REQ-5.4, REQ-5.5, REQ-5.6, REQ-5.7, REQ-5.8, REQ-5.9
- **Files**: `Magidesk.Application/Commands/RefundTicketCommand.cs`, `Magidesk.Application/Services/RefundTicketCommandHandler.cs`

**Task 2.3.5**: Implement refund receipt generation
- Create RefundReceiptService
- Generate receipt with refund details
- Include original ticket info, refund amount, reason
- Support printing and email
- **Requirements**: REQ-5.7
- **Files**: `Magidesk.Application/Services/RefundReceiptService.cs`

**Task 2.3.6***: Write unit tests for void/refund
- Test void open ticket (success)
- Test void paid ticket (reject, suggest refund)
- Test void requires authorization
- Test full refund (status changes to Refunded)
- Test partial refund (status remains Paid)
- Test refund amount exceeds paid (reject)
- **Requirements**: REQ-5.1, REQ-5.2, REQ-5.3, REQ-5.4, REQ-5.5, REQ-5.9
- **Validates**: Property 22, 23, 24, 25, 26
- **Files**: `Magidesk.Domain.Tests/Entities/TicketVoidRefundTests.cs`

**Task 2.3.7***: Write property-based tests for void/refund
- Property 22: Void ticket state transition
- Property 23: Void paid ticket rejection
- Property 24: Full refund processing
- Property 25: Refund amount constraint
- Property 26: Void/refund authorization required
- Property 27: Void/refund audit trail
- **Requirements**: REQ-5.1, REQ-5.2, REQ-5.3, REQ-5.4, REQ-5.6, REQ-5.8, REQ-5.9
- **Files**: `Magidesk.Domain.Tests/Properties/VoidRefundPropertiesTests.cs`

**Task 2.3.8**: Create VoidTicketDialogViewModel
- Add properties: Reason, SelectedReasonCode
- Implement VoidTicketCommand
- Prompt for manager PIN authorization
- Call VoidTicketCommand handler
- **Requirements**: REQ-5.2, REQ-11.4
- **Files**: `Magidesk.Presentation/ViewModels/Dialogs/VoidTicketDialogViewModel.cs`

**Task 2.3.9**: Create VoidTicketDialog view
- Display ticket summary
- Add reason dropdown and text input
- Add "Void Ticket" button
- Trigger manager PIN dialog
- **Requirements**: REQ-5.2
- **Files**: `Magidesk.Presentation/Views/Dialogs/VoidTicketDialog.xaml`

**Task 2.3.10**: Create RefundWizardViewModel
- Add properties: RefundMode (Full/Partial/Specific), RefundAmount, RefundReason
- Add Payments collection for specific payment refunds
- Implement CalculateRefundPreview() method
- Implement ProcessRefundCommand
- Prompt for manager PIN authorization
- **Requirements**: REQ-5.4, REQ-5.5, REQ-5.6, REQ-11.3
- **Files**: `Magidesk.Presentation/ViewModels/RefundWizardViewModel.cs`

**Task 2.3.11**: Create RefundWizard view (4-step wizard)
- Step 1: Select refund mode (Full/Partial/Specific)
- Step 2: Enter refund amount or select payments
- Step 3: Enter reason and preview
- Step 4: Confirm and process
- Show refund calculation preview
- Trigger manager PIN dialog on final step
- **Requirements**: REQ-5.4, REQ-5.5, REQ-5.6
- **Files**: `Magidesk.Presentation/Views/Dialogs/RefundWizard.xaml`

**Task 2.3.12**: Add void/refund buttons to SettlePage
- Add "Void Ticket" button (visible for Open tickets)
- Add "Refund" button (visible for Paid tickets)
- Open VoidTicketDialog or RefundWizard on click
- Refresh ticket after successful operation
- **Requirements**: REQ-11.1
- **Files**: `Magidesk.Presentation/Views/SettlePage.xaml`, `Magidesk.Presentation/ViewModels/SettlePageViewModel.cs`

**Task 2.3.13**: Add reprint receipt functionality
- Add "Reprint Receipt" button for refunded tickets
- Generate and print refund receipt
- **Requirements**: REQ-5.7
- **Files**: `Magidesk.Presentation/Views/SettlePage.xaml`

**CHECKPOINT 2.3**: Void/Refund Complete
- Void ticket working with authorization
- Full and partial refunds working
- Refund wizard functional
- Refund receipts generated
- Audit trail created
- Sprint 2 complete

---

## Sprint 3: Promotional Pricing (Weeks 5-6)

### Feature 3.1: Happy Hour / Promotional Pricing (C.9)

**Task 3.1.1**: Create PromotionSchedule entity
- Add properties: Id, Name, DiscountId, StartTime, EndTime
- Add properties: Recurrence, QualifyingItems, IsActive
- Implement IsActiveAt(DateTime) method
- Implement OverlapsWith(PromotionSchedule) method
- Add validation: StartTime < EndTime
- **Requirements**: REQ-6.2, REQ-6.5
- **Files**: `Magidesk.Domain/Entities/PromotionSchedule.cs`

**Task 3.1.2**: Enhance OrderLine entity with promotion support
- Add PromotionScheduleId property (nullable)
- Add PromotionalPrice property (nullable)
- Add EffectivePrice property (returns promotional price if exists, else regular price)
- Implement ApplyPromotion(PromotionSchedule, Discount) method
- **Requirements**: REQ-6.1, REQ-6.3, REQ-6.4
- **Files**: `Magidesk.Domain/Entities/OrderLine.cs`

**Task 3.1.3**: Create PromotionApplicationService
- Implement GetActivePromotions(DateTime) method
- Implement ApplyPromotionsToOrderLine(OrderLine, DateTime) method
- Select best discount if multiple promotions apply
- Record promotion ID in order line
- **Requirements**: REQ-6.1, REQ-6.6, REQ-6.7
- **Files**: `Magidesk.Application/Services/PromotionApplicationService.cs`

**Task 3.1.4**: Create IPromotionScheduleRepository interface
- Add GetActivePromotionsAsync(DateTime) method
- Add HasOverlappingPromotionsAsync(PromotionSchedule) method
- Add GetByIdAsync(PromotionScheduleId) method
- **Requirements**: REQ-6.1, REQ-7.2
- **Files**: `Magidesk.Application/Interfaces/IPromotionScheduleRepository.cs`

**Task 3.1.5**: Implement PromotionScheduleRepository
- Implement all interface methods with EF Core
- Query active promotions at specific time
- Check for overlapping promotions
- **Requirements**: REQ-6.1, REQ-7.2
- **Files**: `Magidesk.Infrastructure/Repositories/PromotionScheduleRepository.cs`

**Task 3.1.6**: Create PromotionSchedules database table
- Create table with all fields
- Add foreign key to Discounts
- Add indexes on IsActive, StartTime, EndTime
- Store QualifyingItems as array
- **Requirements**: REQ-6.2
- **Files**: `create_promotion_schedules_table.sql`

**Task 3.1.7**: Integrate promotion application into order flow
- When order line created, check for active promotions
- Auto-apply promotion if item qualifies
- Update order line with promotional price
- **Requirements**: REQ-6.1
- **Files**: `Magidesk.Application/Services/OrderService.cs`

**Task 3.1.8***: Write unit tests for promotion application
- Test promotion applies during active window
- Test promotion does not apply outside window
- Test best discount selected when multiple apply
- Test promotion recorded in order line
- **Requirements**: REQ-6.1, REQ-6.5, REQ-6.6, REQ-6.7
- **Validates**: Property 28, 29
- **Files**: `Magidesk.Domain.Tests/Services/PromotionApplicationTests.cs`

**Task 3.1.9***: Write property-based tests for promotions
- Property 28: Promotion time-based application
- Property 29: Promotion best discount selection
- **Requirements**: REQ-6.1, REQ-6.6
- **Files**: `Magidesk.Domain.Tests/Properties/PromotionPropertiesTests.cs`

**Task 3.1.10**: Add happy hour indicator to UI
- Show banner when promotions are active
- Display "Happy Hour" or promotion name
- Show promotion end time countdown
- **Requirements**: REQ-6.3
- **Files**: `Magidesk.Presentation/Views/OrderPage.xaml`

**Task 3.1.11**: Show promotional pricing on order lines
- Display original price with strikethrough
- Display promotional price prominently
- Add promotion badge/icon
- Show promotion name in tooltip
- **Requirements**: REQ-6.3, REQ-6.4
- **Files**: `Magidesk.Presentation/Views/OrderPage.xaml`, `Magidesk.Presentation/Converters/PromotionalPriceConverter.cs`

**CHECKPOINT 3.1**: Happy Hour Pricing Complete
- Promotion schedule entity working
- Auto-application functional
- Best discount selection working
- UI shows promotional pricing
- Promotion recorded in order lines

### Feature 3.2: Automatic Promotion Scheduling (C.10)

**Task 3.2.1**: Enhance PromotionSchedule with recurrence
- Add RecurrencePattern enum (Daily, Weekly, Custom)
- Add DaysOfWeek property for weekly recurrence
- Implement GetNextOccurrence(DateTime) method
- Implement IsActiveOn(DateTime) method considering recurrence
- **Requirements**: REQ-7.1, REQ-7.4
- **Files**: `Magidesk.Domain/Entities/PromotionSchedule.cs`

**Task 3.2.2**: Create CreatePromotionScheduleCommand and handler
- Create command with all schedule fields
- Implement handler: validate times and recurrence
- Check for overlapping promotions
- If overlap detected, return error
- Save promotion schedule
- **Requirements**: REQ-7.1, REQ-7.2, REQ-7.3
- **Files**: `Magidesk.Application/Commands/CreatePromotionScheduleCommand.cs`, `Magidesk.Application/Services/CreatePromotionScheduleCommandHandler.cs`

**Task 3.2.3**: Create UpdatePromotionScheduleCommand and handler
- Create command with schedule ID and updated fields
- Implement handler: require manager authorization
- Validate changes don't create overlaps
- Save updated schedule
- **Requirements**: REQ-7.6
- **Files**: `Magidesk.Application/Commands/UpdatePromotionScheduleCommand.cs`, `Magidesk.Application/Services/UpdatePromotionScheduleCommandHandler.cs`

**Task 3.2.4**: Create GetPromotionSchedulesQuery and handler
- Create query with optional filters (active, date range)
- Implement handler: return all matching schedules
- Include upcoming promotions
- **Requirements**: REQ-7.5
- **Files**: `Magidesk.Application/Queries/GetPromotionSchedulesQuery.cs`, `Magidesk.Application/Services/GetPromotionSchedulesQueryHandler.cs`

**Task 3.2.5***: Write unit tests for promotion scheduling
- Test daily recurrence
- Test weekly recurrence
- Test overlap detection
- Test schedule validation
- **Requirements**: REQ-7.1, REQ-7.2, REQ-7.3
- **Validates**: Property 30
- **Files**: `Magidesk.Domain.Tests/Entities/PromotionScheduleTests.cs`

**Task 3.2.6***: Write property-based tests for scheduling
- Property 30: Promotion schedule overlap detection
- **Requirements**: REQ-7.2
- **Files**: `Magidesk.Domain.Tests/Properties/PromotionSchedulingPropertiesTests.cs`

**Task 3.2.7**: Create PromotionScheduleViewModel
- Add Schedules observable collection
- Implement CreateScheduleCommand
- Implement EditScheduleCommand
- Implement DeleteScheduleCommand
- Implement RefreshCommand
- **Requirements**: REQ-7.5, REQ-7.6
- **Files**: `Magidesk.Presentation/ViewModels/PromotionScheduleViewModel.cs`

**Task 3.2.8**: Create PromotionSchedulePage view
- Display list of all promotion schedules
- Show schedule name, times, recurrence, status
- Add "Create Schedule" button
- Add "Edit" and "Delete" buttons for each schedule
- Add calendar view showing active promotions
- **Requirements**: REQ-7.5
- **Files**: `Magidesk.Presentation/Views/PromotionSchedulePage.xaml`

**Task 3.2.9**: Create PromotionScheduleEditorDialog
- Add fields for all schedule properties
- Add time pickers for start/end times
- Add recurrence pattern selector
- Add item selector for qualifying items
- Show overlap warning if detected
- **Requirements**: REQ-7.1, REQ-7.2, REQ-7.3
- **Files**: `Magidesk.Presentation/Views/Dialogs/PromotionScheduleEditorDialog.xaml`

**CHECKPOINT 3.2**: Promotion Scheduling Complete
- Recurrence patterns working
- Overlap detection functional
- CRUD operations for schedules
- UI for schedule management
- Calendar view showing promotions

### Feature 3.3: Manual Promotion Override (C.11)

**Task 3.3.1**: Enhance Ticket entity with promotion override
- Add DisabledPromotions collection (List<PromotionScheduleId>)
- Implement DisablePromotion(PromotionScheduleId, string reason, UserId disabledBy, UserId authorizedBy) method
- Raise PromotionDisabledEvent
- **Requirements**: REQ-8.1, REQ-8.2, REQ-8.5
- **Files**: `Magidesk.Domain/Entities/Ticket.cs`

**Task 3.3.2**: Create PromotionDisabledEvent
- Add properties: TicketId, PromotionScheduleId, Reason, DisabledBy, AuthorizedBy
- **Requirements**: REQ-8.3, REQ-8.5
- **Files**: `Magidesk.Domain/Events/PromotionDisabledEvent.cs`

**Task 3.3.3**: Create DisablePromotionCommand and handler
- Create command with TicketId, PromotionScheduleId, Reason, DisabledBy, AuthorizedBy
- Implement handler: validate manager authorization
- Load ticket, call DisablePromotion()
- Recalculate order line prices to standard pricing
- Save and publish event
- **Requirements**: REQ-8.1, REQ-8.2, REQ-8.3, REQ-8.5
- **Files**: `Magidesk.Application/Commands/DisablePromotionCommand.cs`, `Magidesk.Application/Services/DisablePromotionCommandHandler.cs`

**Task 3.3.4***: Write unit tests for promotion override
- Test disable promotion with authorization
- Test disable promotion without authorization (reject)
- Test pricing reverts to standard
- Test audit event created
- **Requirements**: REQ-8.1, REQ-8.2, REQ-8.3, REQ-8.5
- **Validates**: Property 31, 32
- **Files**: `Magidesk.Domain.Tests/Entities/TicketPromotionOverrideTests.cs`

**Task 3.3.5***: Write property-based tests for promotion override
- Property 31: Promotion manual override
- Property 32: Promotion override audit trail
- **Requirements**: REQ-8.1, REQ-8.3, REQ-8.5
- **Files**: `Magidesk.Domain.Tests/Properties/PromotionOverridePropertiesTests.cs`

**Task 3.3.6**: Add promotion override to UI
- Add "Remove Promotion" button to order lines with promotions
- Open reason dialog on click
- Prompt for manager PIN authorization
- Call DisablePromotionCommand handler
- Update order line display to show standard pricing
- Add visual indicator for manual override
- **Requirements**: REQ-8.1, REQ-8.2, REQ-8.4
- **Files**: `Magidesk.Presentation/Views/OrderPage.xaml`, `Magidesk.Presentation/ViewModels/OrderPageViewModel.cs`

**CHECKPOINT 3.3**: Promotion Override Complete
- Manual override working with authorization
- Pricing reverts to standard
- Audit trail created
- UI shows override indicator
- Sprint 3 complete

---

## Sprint 4: Advanced Features (Weeks 7-8)

### Feature 4.1: Group Billing (C.5)

**Task 4.1.1**: Create GroupSettlement entity
- Add properties: Id, TicketIds, Strategy, MasterPaymentId, TotalAmount
- Add properties: CreatedAt, CreatedBy
- Implement AddTicket(TicketId) method
- Implement RemoveTicket(TicketId) method
- Implement CalculateDistribution(List<Ticket>) method
- Support strategies: EqualSplit, ByItem, Custom
- **Requirements**: REQ-9.1, REQ-9.2, REQ-9.3
- **Files**: `Magidesk.Domain/Entities/GroupSettlement.cs`

**Task 4.1.2**: Create GroupSettlementStrategy enum
- Add values: EqualSplit, ByItem, Custom
- **Requirements**: REQ-9.2
- **Files**: `Magidesk.Domain/Enumerations/GroupSettlementStrategy.cs`

**Task 4.1.3**: Create CreateGroupSettlementCommand and handler
- Create command with TicketIds, Strategy, CreatedBy
- Implement handler: load all tickets
- Validate all tickets are Open or Held
- Calculate total across all tickets
- Create GroupSettlement entity
- Link tickets to settlement
- Save and publish event
- **Requirements**: REQ-9.1, REQ-9.2, REQ-9.3, REQ-9.7
- **Files**: `Magidesk.Application/Commands/CreateGroupSettlementCommand.cs`, `Magidesk.Application/Services/CreateGroupSettlementCommandHandler.cs`

**Task 4.1.4**: Create ProcessGroupSettlementPaymentCommand and handler
- Create command with GroupSettlementId, Payment details
- Implement handler: load settlement and tickets
- Process master payment
- Distribute payment across tickets based on strategy
- Mark all tickets as Paid
- Save all tickets and settlement
- **Requirements**: REQ-9.4, REQ-9.6
- **Files**: `Magidesk.Application/Commands/ProcessGroupSettlementPaymentCommand.cs`, `Magidesk.Application/Services/ProcessGroupSettlementPaymentCommandHandler.cs`

**Task 4.1.5**: Create IGroupSettlementRepository interface
- Add GetByIdAsync(GroupSettlementId) method
- Add SaveAsync(GroupSettlement) method
- **Requirements**: REQ-9.1
- **Files**: `Magidesk.Application/Interfaces/IGroupSettlementRepository.cs`

**Task 4.1.6**: Implement GroupSettlementRepository
- Implement all interface methods with EF Core
- Include related tickets when loading
- **Requirements**: REQ-9.1
- **Files**: `Magidesk.Infrastructure/Repositories/GroupSettlementRepository.cs`

**Task 4.1.7**: Create GroupSettlements database table
- Create table with all fields
- Add foreign key to Payments (MasterPaymentId)
- Store TicketIds as array
- Add index on CreatedAt
- **Requirements**: REQ-9.1
- **Files**: `create_group_settlements_table.sql`

**Task 4.1.8***: Write unit tests for group settlement
- Test equal split calculation
- Test by-item split calculation
- Test total equals sum of tickets
- Test payment distribution
- Test all tickets marked as Paid
- **Requirements**: REQ-9.2, REQ-9.3, REQ-9.4, REQ-9.6
- **Validates**: Property 33, 34, 35
- **Files**: `Magidesk.Domain.Tests/Entities/GroupSettlementTests.cs`

**Task 4.1.9***: Write property-based tests for group settlement
- Property 33: Group settlement total calculation
- Property 34: Group settlement payment distribution
- Property 35: Group settlement ticket closure
- Property 36: Group settlement audit trail
- **Requirements**: REQ-9.3, REQ-9.4, REQ-9.6, REQ-9.7
- **Files**: `Magidesk.Domain.Tests/Properties/GroupSettlementPropertiesTests.cs`

**Task 4.1.10**: Create GroupBillingViewModel
- Add AvailableTickets observable collection
- Add SelectedTickets observable collection
- Add SelectedStrategy property
- Add GroupTotal property (calculated)
- Implement AddTicketCommand
- Implement RemoveTicketCommand
- Implement CreateGroupSettlementCommand
- Implement ProcessPaymentCommand
- **Requirements**: REQ-9.1, REQ-9.2, REQ-9.5
- **Files**: `Magidesk.Presentation/ViewModels/GroupBillingViewModel.cs`

**Task 4.1.11**: Create GroupBillingDialog view
- Display table selection for group
- Show selected tickets with amounts
- Add strategy selector (Equal Split, By Item, Custom)
- Display group total
- Show payment distribution preview
- Add "Process Payment" button
- **Requirements**: REQ-9.1, REQ-9.2, REQ-9.5
- **Files**: `Magidesk.Presentation/Views/Dialogs/GroupBillingDialog.xaml`

**Task 4.1.12**: Add group billing to main navigation
- Add "Group Billing" menu item
- Add icon for group billing
- Open GroupBillingDialog on click
- **Requirements**: REQ-9.1
- **Files**: `MainWindow.xaml`

**CHECKPOINT 4.1**: Group Billing Complete
- Group settlement entity working
- Split strategies functional
- Payment distribution correct
- UI for group billing
- All tickets closed after payment

### Feature 4.2: Price Override Audit Trail (C.13)

**Task 4.2.1**: Create GetPriceOverridesQuery and handler
- Create query with filters: StartDate, EndDate, StaffMember, Manager, MinVariance
- Implement handler: call repository with filters
- Map to PriceOverrideDto
- Calculate variance percentage
- **Requirements**: REQ-10.1, REQ-10.2, REQ-10.3, REQ-10.4
- **Files**: `Magidesk.Application/Queries/GetPriceOverridesQuery.cs`, `Magidesk.Application/Services/GetPriceOverridesQueryHandler.cs`

**Task 4.2.2**: Create PriceOverrideDto
- Add all display fields: Id, OrderLineId, ItemName, OriginalPrice, OverridePrice
- Add Variance, VariancePercentage
- Add Reason, AppliedByUserName, AuthorizedByUserName, AppliedAt
- **Requirements**: REQ-10.2
- **Files**: `Magidesk.Application/DTOs/PriceOverrideDto.cs`

**Task 4.2.3**: Enhance IPriceOverrideRepository with filtering
- Update GetOverridesAsync to support all filter parameters
- Implement dynamic query building based on provided filters
- **Requirements**: REQ-10.3
- **Files**: `Magidesk.Application/Interfaces/IPriceOverrideRepository.cs`, `Magidesk.Infrastructure/Repositories/PriceOverrideRepository.cs`

**Task 4.2.4**: Create ExportPriceOverridesCommand and handler
- Create command with same filters as query
- Implement handler: load overrides, generate Excel file
- Include all fields in export
- Format currency and percentages
- **Requirements**: REQ-10.6
- **Files**: `Magidesk.Application/Commands/ExportPriceOverridesCommand.cs`, `Magidesk.Application/Services/ExportPriceOverridesCommandHandler.cs`

**Task 4.2.5***: Write unit tests for audit queries
- Test variance calculation
- Test filtering by date range
- Test filtering by staff member
- Test filtering by manager
- Test filtering by variance threshold
- **Requirements**: REQ-10.3, REQ-10.4
- **Validates**: Property 37, 38
- **Files**: `Magidesk.Application.Tests/Queries/GetPriceOverridesQueryTests.cs`

**Task 4.2.6***: Write property-based tests for audit
- Property 37: Price override variance calculation
- Property 38: Price override query completeness
- **Requirements**: REQ-10.3, REQ-10.4
- **Files**: `Magidesk.Application.Tests/Properties/PriceOverrideAuditPropertiesTests.cs`

**Task 4.2.7**: Create PriceOverrideAuditViewModel
- Add Overrides observable collection
- Add filter properties: StartDate, EndDate, StaffMember, Manager, MinVariance
- Implement FilterCommand
- Implement ExportCommand
- Implement RefreshCommand
- Highlight large variances (>20%)
- **Requirements**: REQ-10.1, REQ-10.3, REQ-10.5, REQ-10.6
- **Files**: `Magidesk.Presentation/ViewModels/PriceOverrideAuditViewModel.cs`

**Task 4.2.8**: Create PriceOverrideAuditPage view
- Display DataGrid with all override fields
- Add filter controls in toolbar (date pickers, dropdowns)
- Highlight rows with large variances (different color)
- Add "Export to Excel" button
- Add search functionality
- Show variance as both amount and percentage
- **Requirements**: REQ-10.1, REQ-10.2, REQ-10.3, REQ-10.5, REQ-10.6
- **Files**: `Magidesk.Presentation/Views/PriceOverrideAuditPage.xaml`

**Task 4.2.9**: Add audit page to navigation
- Add "Price Override Audit" menu item under Reports
- Add icon for audit
- Register page in navigation service
- **Requirements**: REQ-10.1
- **Files**: `MainWindow.xaml`, `Magidesk.Presentation/Services/NavigationService.cs`

**CHECKPOINT 4.2**: Price Override Audit Complete
- Query with filtering working
- Variance calculation correct
- Export to Excel functional
- UI shows all override details
- Large variances highlighted

---

## Cross-Cutting Tasks

### Authorization and Security (REQ-12)

**Task 5.1**: Enhance ManagerAuthorizationService
- Track failed PIN attempts per user
- Implement lockout after threshold exceeded (default: 3 attempts)
- Implement lockout duration (default: 15 minutes)
- Record authorization in audit trail
- **Requirements**: REQ-12.1, REQ-12.2, REQ-12.3, REQ-12.4, REQ-12.5
- **Files**: `Magidesk.Application/Services/ManagerAuthorizationService.cs`

**Task 5.2**: Update ManagerPinDialog
- Display failed attempt count
- Show lockout message if locked
- Show remaining lockout time
- Clear PIN input after failed attempt
- **Requirements**: REQ-12.3, REQ-12.4
- **Files**: `Magidesk.Presentation/Views/Dialogs/ManagerPinDialog.xaml`

**Task 5.3***: Write unit tests for authorization
- Test PIN validation
- Test failed attempt tracking
- Test lockout after threshold
- Test lockout duration
- Test audit trail recording
- **Requirements**: REQ-12.1, REQ-12.3, REQ-12.4, REQ-12.5
- **Validates**: Property 39, 40, 41
- **Files**: `Magidesk.Application.Tests/Services/ManagerAuthorizationServiceTests.cs`

**Task 5.4***: Write property-based tests for authorization
- Property 39: Manager authorization failed attempts
- Property 40: Manager authorization lockout
- Property 41: Manager authorization audit trail
- **Requirements**: REQ-12.3, REQ-12.4, REQ-12.5
- **Files**: `Magidesk.Application.Tests/Properties/AuthorizationPropertiesTests.cs`

### Audit Trail and Reporting (REQ-13)

**Task 5.5**: Enhance AuditEventService
- Ensure all billing operations create audit events
- Record entity type, entity ID, event type, user ID, timestamp
- Record before and after state (JSON serialization)
- **Requirements**: REQ-13.1, REQ-13.2
- **Files**: `Magidesk.Application/Services/AuditEventService.cs`

**Task 5.6**: Create GetAuditEventsQuery and handler
- Create query with filters: StartDate, EndDate, EntityType, EventType, UserId
- Implement handler: query with filters
- Map to AuditEventDto
- **Requirements**: REQ-13.3, REQ-13.4
- **Files**: `Magidesk.Application/Queries/GetAuditEventsQuery.cs`, `Magidesk.Application/Services/GetAuditEventsQueryHandler.cs`

**Task 5.7**: Create ExportAuditEventsCommand and handler
- Create command with same filters as query
- Implement handler: load events, generate Excel file
- Include all fields in export
- **Requirements**: REQ-13.5
- **Files**: `Magidesk.Application/Commands/ExportAuditEventsCommand.cs`, `Magidesk.Application/Services/ExportAuditEventsCommandHandler.cs`

**Task 5.8***: Write unit tests for audit trail
- Test audit event creation for each operation
- Test audit event data integrity
- Test filtering
- **Requirements**: REQ-13.1, REQ-13.2, REQ-13.3
- **Validates**: Property 42, 43, 44
- **Files**: `Magidesk.Application.Tests/Services/AuditEventServiceTests.cs`

**Task 5.9***: Write property-based tests for audit trail
- Property 42: Comprehensive audit trail
- Property 43: Audit event data integrity
- Property 44: Audit event query filtering
- **Requirements**: REQ-13.1, REQ-13.2, REQ-13.3
- **Files**: `Magidesk.Application.Tests/Properties/AuditTrailPropertiesTests.cs`

**Task 5.10**: Create AuditLogViewModel
- Add AuditEvents observable collection
- Add filter properties
- Implement FilterCommand
- Implement ExportCommand
- Implement RefreshCommand
- **Requirements**: REQ-13.3, REQ-13.4, REQ-13.5
- **Files**: `Magidesk.Presentation/ViewModels/AuditLogViewModel.cs`

**Task 5.11**: Create AuditLogPage view
- Display DataGrid with all audit event fields
- Add filter controls
- Add "Export to Excel" button
- Show before/after state in expandable rows
- **Requirements**: REQ-13.3, REQ-13.4, REQ-13.5
- **Files**: `Magidesk.Presentation/Views/AuditLogPage.xaml`

### Performance and Scalability (REQ-14)

**Task 5.12***: Write performance tests
- Test payment processing time (< 2 seconds)
- Test held tickets query time (< 1 second)
- Test discount calculation time (< 500ms)
- Test settle page load time (< 1 second)
- Test concurrent payment processing
- **Requirements**: REQ-14.1, REQ-14.2, REQ-14.3, REQ-14.4, REQ-14.5
- **Files**: `Magidesk.Tests.Workflows/Performance/BillingPerformanceTests.cs`

**Task 5.13**: Optimize database queries
- Add missing indexes identified during performance testing
- Optimize EF Core queries (use AsNoTracking where appropriate)
- Implement query result caching for frequently accessed data
- **Requirements**: REQ-14.1, REQ-14.2, REQ-14.3, REQ-14.4
- **Files**: Various repository files

**Task 5.14**: Implement retry logic for database operations
- Add retry policy for transient database failures
- Configure exponential backoff
- Log retry attempts
- **Requirements**: REQ-14.6
- **Files**: `Magidesk.Infrastructure/Data/ApplicationDbContext.cs`

### UI Polish and User Experience (REQ-11)

**Task 5.15**: Implement loading indicators
- Add loading spinners for all async operations
- Show progress for long-running operations
- Disable buttons during processing
- **Requirements**: REQ-11.5
- **Files**: Various ViewModel and View files

**Task 5.16**: Implement success notifications
- Show toast notifications for successful operations
- Include operation details in notification
- Auto-dismiss after 3 seconds
- **Requirements**: REQ-11.6
- **Files**: `Magidesk.Presentation/Services/NotificationService.cs`

**Task 5.17**: Enhance error messages
- Create user-friendly error messages for all domain exceptions
- Provide actionable guidance in error messages
- Add "Learn More" links where appropriate
- **Requirements**: REQ-11.4
- **Files**: `Magidesk.Presentation/Services/ErrorMessageService.cs`

**Task 5.18**: Implement navigation context preservation
- Maintain context when navigating between pages
- Allow easy return to previous screen
- Preserve filter and sort settings
- **Requirements**: REQ-11.7
- **Files**: `Magidesk.Presentation/Services/NavigationService.cs`

---

## Final Integration and Testing

**Task 6.1***: End-to-end integration tests
- Test complete hold ticket workflow
- Test complete split payment workflow
- Test complete discount workflow
- Test complete price override workflow
- Test complete void/refund workflow
- Test complete promotion workflow
- Test complete group billing workflow
- **Requirements**: All requirements
- **Files**: `Magidesk.Tests.Workflows/Integration/CategoryCIntegrationTests.cs`

**Task 6.2***: User acceptance testing
- Conduct UAT with stakeholders
- Gather feedback on UI/UX
- Identify any missing functionality
- **Requirements**: All requirements

**Task 6.3**: Documentation updates
- Update user manual with new features
- Create training materials
- Document configuration options
- **Requirements**: All requirements
- **Files**: `docs/user-guide/billing-payments.md`

**Task 6.4**: Deployment preparation
- Create deployment checklist
- Prepare database migration scripts
- Update release notes
- **Requirements**: All requirements
- **Files**: `DEPLOYMENT-CHECKLIST.md`, `RELEASE-NOTES.md`

**FINAL CHECKPOINT**: Category C Complete
- All P0 and P1 features implemented
- All tests passing
- Documentation complete
- Ready for production deployment

---

## Summary

**Total Tasks**: 150+ tasks across 4 sprints
**Estimated Duration**: 8 weeks
**Test Tasks**: 30+ optional test tasks (marked with *)
**Checkpoints**: 15 checkpoints for progress tracking

**Key Milestones**:
- End of Sprint 1: Hold Ticket and Split Payment working
- End of Sprint 2: Discounts, Price Override, Void/Refund complete
- End of Sprint 3: Promotional pricing fully functional
- End of Sprint 4: Group billing and audit trails complete

