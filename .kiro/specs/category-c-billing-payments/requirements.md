# Requirements Document

## Introduction

This specification defines the requirements for Category C: Billing, Payments & Pricing features for the Magidesk POS system. These features build upon the existing basic payment flow to provide comprehensive billing capabilities including deferred payments, split payments, discounts, promotional pricing, group billing, and audit trails.

## Glossary

- **Billing_System**: The complete system handling all payment, pricing, discount, and billing operations
- **Ticket**: An order that tracks items, amounts, and payment status
- **Held_Ticket**: A ticket marked for deferred payment with the table released
- **Split_Payment**: A payment divided across multiple payment methods or payers
- **Discount**: A reduction in price applied to items or tickets
- **Price_Override**: A manual change to an item's price requiring authorization
- **Promotion**: A scheduled discount or pricing rule applied automatically
- **Group_Settlement**: A billing strategy for multiple tickets paid together
- **Manager_Authorization**: Permission verification using manager PIN
- **Audit_Trail**: A record of all pricing and payment modifications

## Requirements

### Requirement 1: Hold Ticket for Deferred Payment (C.2)

**User Story:** As a staff member, I want to hold a ticket for later payment, so that I can free up the table while keeping the order open.

#### Acceptance Criteria

1. WHEN a staff member holds an open ticket with a reason, THE Billing_System SHALL change the ticket status to "Held"
2. WHEN a ticket is held, THE Billing_System SHALL record the hold timestamp, reason, and staff member ID
3. WHEN a ticket is held, THE Billing_System SHALL release the associated table for other customers
4. WHEN a staff member attempts to hold a closed, voided, or refunded ticket, THE Billing_System SHALL prevent the action and display an error
5. WHEN a staff member attempts to hold a ticket without providing a reason, THE Billing_System SHALL prevent the action and display a validation error
6. WHEN a staff member releases a held ticket, THE Billing_System SHALL change the ticket status back to "Open"
7. WHEN a staff member queries held tickets, THE Billing_System SHALL return all tickets with status "Held" including hold details
8. WHEN a ticket is held or released, THE Billing_System SHALL create an audit event

### Requirement 2: Split Payment Processing (C.4)

**User Story:** As a staff member, I want to split a payment across multiple payment methods or payers, so that customers can pay their preferred way.

#### Acceptance Criteria

1. WHEN a staff member processes a split payment, THE Billing_System SHALL accept multiple payment entries
2. WHEN processing split payments, THE Billing_System SHALL validate that the sum of all payments equals the ticket total
3. WHEN the sum of split payments exceeds the ticket total, THE Billing_System SHALL calculate and return change
4. WHEN the sum of split payments is less than the ticket total, THE Billing_System SHALL prevent completion and display the remaining amount
5. WHEN a staff member selects "Split by Amount", THE Billing_System SHALL provide quick split options (2-way, 3-way, 4-way, custom)
6. WHEN a staff member selects "Split by Item", THE Billing_System SHALL allow assigning items to different payers
7. WHEN processing split payments, THE Billing_System SHALL track each payment method and amount separately
8. WHEN a split payment is completed, THE Billing_System SHALL create separate payment records for each portion

### Requirement 3: Discount Application (C.7)

**User Story:** As a staff member, I want to apply discounts to tickets or items, so that I can provide price reductions to customers.

#### Acceptance Criteria

1. WHEN a staff member applies a discount to a ticket, THE Billing_System SHALL reduce the total by the discount amount or percentage
2. WHEN a discount is applied, THE Billing_System SHALL record the discount type, value, and reason
3. WHEN a member discount is available, THE Billing_System SHALL automatically apply it to the ticket
4. WHEN multiple discounts are applied, THE Billing_System SHALL follow stacking rules (if configured)
5. WHEN a discount exceeds 50% of the total, THE Billing_System SHALL require manager authorization
6. WHEN a staff member applies a discount, THE Billing_System SHALL display the original and discounted amounts
7. WHEN a discount is applied or removed, THE Billing_System SHALL create an audit event
8. THE Billing_System SHALL NOT allow discounts that result in negative totals

### Requirement 4: Price Override with Authorization (C.12)

**User Story:** As a manager, I want to override item prices with authorization, so that I can handle special pricing situations.

#### Acceptance Criteria

1. WHEN a staff member attempts to override an item price, THE Billing_System SHALL require manager PIN authorization
2. WHEN a price override is authorized, THE Billing_System SHALL update the item price to the new value
3. WHEN a price override is applied, THE Billing_System SHALL record the original price, new price, reason, and authorizing manager
4. WHEN displaying an overridden item, THE Billing_System SHALL show both original and override prices
5. WHEN a price override is applied, THE Billing_System SHALL create an audit event
6. THE Billing_System SHALL NOT allow negative price overrides
7. WHEN calculating ticket totals, THE Billing_System SHALL use override prices where applicable

### Requirement 5: Void and Refund Processing (C.15)

**User Story:** As a manager, I want to void or refund tickets with authorization, so that I can handle cancellations and returns.

#### Acceptance Criteria

1. WHEN a manager voids an open ticket, THE Billing_System SHALL change the ticket status to "Voided"
2. WHEN voiding a ticket, THE Billing_System SHALL require manager authorization and a reason
3. WHEN a manager attempts to void a paid ticket, THE Billing_System SHALL prevent the action and suggest refund instead
4. WHEN a manager processes a full refund, THE Billing_System SHALL refund all payments and change status to "Refunded"
5. WHEN a manager processes a partial refund, THE Billing_System SHALL refund the specified amount and update payment records
6. WHEN processing a refund, THE Billing_System SHALL require manager authorization and a reason
7. WHEN a refund is processed, THE Billing_System SHALL generate a refund receipt
8. WHEN a ticket is voided or refunded, THE Billing_System SHALL create an audit event
9. THE Billing_System SHALL NOT allow refund amounts exceeding the paid amount

### Requirement 6: Happy Hour and Promotional Pricing (C.9)

**User Story:** As a manager, I want to schedule promotional pricing, so that I can offer time-based discounts automatically.

#### Acceptance Criteria

1. WHEN a promotion schedule is active, THE Billing_System SHALL automatically apply promotional pricing to qualifying items
2. WHEN creating a promotion, THE Billing_System SHALL require start time, end time, discount type, and qualifying items
3. WHEN a promotion is active, THE Billing_System SHALL display promotional pricing on order lines
4. WHEN displaying promotional items, THE Billing_System SHALL show both original and promotional prices
5. WHEN a promotion period ends, THE Billing_System SHALL revert to standard pricing
6. WHEN multiple promotions apply to an item, THE Billing_System SHALL apply the best discount for the customer
7. WHEN a promotion is applied, THE Billing_System SHALL record the promotion ID in the order line

### Requirement 7: Automatic Promotion Scheduling (C.10)

**User Story:** As a manager, I want to schedule recurring promotions, so that I can automate happy hour and special pricing.

#### Acceptance Criteria

1. WHEN creating a promotion schedule, THE Billing_System SHALL support daily, weekly, and custom recurrence patterns
2. WHEN scheduling promotions, THE Billing_System SHALL detect and prevent overlapping promotions for the same items
3. WHEN a promotion schedule is created, THE Billing_System SHALL validate start/end times and recurrence rules
4. WHEN a promotion schedule is active, THE Billing_System SHALL automatically activate and deactivate based on the schedule
5. WHEN viewing promotion schedules, THE Billing_System SHALL display all active and upcoming promotions
6. WHEN editing a promotion schedule, THE Billing_System SHALL require manager authorization

### Requirement 8: Manual Promotion Override (C.11)

**User Story:** As a manager, I want to manually override automatic promotions, so that I can handle exceptions.

#### Acceptance Criteria

1. WHEN a manager disables a promotion for a ticket, THE Billing_System SHALL revert to standard pricing
2. WHEN disabling a promotion, THE Billing_System SHALL require manager authorization and a reason
3. WHEN a promotion is manually disabled, THE Billing_System SHALL record the override in the audit trail
4. WHEN displaying a ticket with disabled promotions, THE Billing_System SHALL indicate the manual override
5. WHEN a promotion is manually disabled, THE Billing_System SHALL create an audit event

### Requirement 9: Group Billing (C.5)

**User Story:** As a staff member, I want to bill multiple tickets together, so that I can handle group payments efficiently.

#### Acceptance Criteria

1. WHEN a staff member creates a group settlement, THE Billing_System SHALL link multiple tickets to a master payment
2. WHEN creating a group settlement, THE Billing_System SHALL support equal split, by-item split, and custom split strategies
3. WHEN processing a group settlement, THE Billing_System SHALL calculate the total across all linked tickets
4. WHEN a group settlement is paid, THE Billing_System SHALL distribute the payment across all linked tickets
5. WHEN displaying a group settlement, THE Billing_System SHALL show all included tickets and their amounts
6. WHEN a group settlement is completed, THE Billing_System SHALL close all linked tickets
7. WHEN a group settlement is created, THE Billing_System SHALL create an audit event

### Requirement 10: Price Override Audit Trail (C.13)

**User Story:** As a manager, I want to view all price overrides, so that I can monitor pricing exceptions.

#### Acceptance Criteria

1. WHEN a manager queries price overrides, THE Billing_System SHALL return all override records with details
2. WHEN displaying price overrides, THE Billing_System SHALL show original price, override price, variance, staff member, manager, reason, and timestamp
3. WHEN filtering price overrides, THE Billing_System SHALL support filtering by date range, staff member, manager, and variance threshold
4. WHEN calculating variance, THE Billing_System SHALL compute the difference between original and override prices
5. WHEN displaying large variances, THE Billing_System SHALL highlight overrides exceeding configured thresholds
6. WHEN exporting price overrides, THE Billing_System SHALL generate a report in Excel format

### Requirement 11: User Interface Integration

**User Story:** As a staff member, I want intuitive UI for all billing features, so that I can process payments efficiently.

#### Acceptance Criteria

1. WHEN a staff member is on the settle page, THE Billing_System SHALL display buttons for Hold Ticket, Split Payment, Apply Discount, and Void/Refund
2. WHEN displaying payment options, THE Billing_System SHALL show clear visual indicators for each payment method
3. WHEN processing payments, THE Billing_System SHALL display real-time calculation updates
4. WHEN an error occurs, THE Billing_System SHALL display user-friendly error messages with actionable guidance
5. WHEN operations are in progress, THE Billing_System SHALL display loading indicators
6. WHEN operations complete successfully, THE Billing_System SHALL display success notifications
7. WHEN navigating between billing features, THE Billing_System SHALL maintain context and allow easy return to previous screens

### Requirement 12: Authorization and Security

**User Story:** As a manager, I want secure authorization for sensitive operations, so that I can control access to pricing and payment modifications.

#### Acceptance Criteria

1. WHEN a sensitive operation is attempted, THE Billing_System SHALL require manager PIN authorization
2. WHEN manager authorization is required, THE Billing_System SHALL display a PIN entry dialog
3. WHEN an incorrect PIN is entered, THE Billing_System SHALL increment failed attempt counter
4. WHEN failed attempts exceed the threshold, THE Billing_System SHALL lock the authorization for a configured period
5. WHEN manager authorization succeeds, THE Billing_System SHALL record the authorizing manager in the audit trail
6. THE Billing_System SHALL require manager authorization for: large discounts (>50%), price overrides, voids, refunds, promotion overrides

### Requirement 13: Audit Trail and Reporting

**User Story:** As a manager, I want comprehensive audit trails, so that I can track all billing and payment modifications.

#### Acceptance Criteria

1. WHEN any billing operation is performed, THE Billing_System SHALL create an audit event
2. WHEN creating audit events, THE Billing_System SHALL record entity type, entity ID, event type, user ID, timestamp, before state, and after state
3. WHEN querying audit events, THE Billing_System SHALL support filtering by date range, entity type, event type, and user
4. WHEN displaying audit events, THE Billing_System SHALL show all relevant details in a readable format
5. WHEN exporting audit trails, THE Billing_System SHALL generate reports in Excel format
6. THE Billing_System SHALL retain audit events for a configured retention period

### Requirement 14: Performance and Scalability

**User Story:** As a system administrator, I want the billing system to perform well under load, so that staff can process payments quickly.

#### Acceptance Criteria

1. WHEN processing a payment, THE Billing_System SHALL complete the operation within 2 seconds under normal load
2. WHEN querying held tickets, THE Billing_System SHALL return results within 1 second
3. WHEN applying discounts or promotions, THE Billing_System SHALL recalculate totals within 500 milliseconds
4. WHEN loading the settle page, THE Billing_System SHALL display the page within 1 second
5. THE Billing_System SHALL support concurrent payment processing for multiple tickets
6. THE Billing_System SHALL handle database connection failures gracefully with retry logic

