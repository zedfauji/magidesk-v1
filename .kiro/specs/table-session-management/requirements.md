# Requirements Document

## Introduction

This specification defines the Table Session Management system for the billiard club POS application. This system enables time-based billing by tracking when customers start and end table sessions, calculating charges based on elapsed time, and integrating with the existing ticket and payment systems.

The current system lacks proper session management, making time-based billing impossible. This feature is critical for billiard club operations where customers pay for table time rather than just products.

## Glossary

- **Table_Session**: A time-tracked period when a table is occupied by customers
- **Table_Type**: A category of table with specific pricing rules (e.g., Pool Table, Snooker Table)
- **Billable_Time**: The actual time charged to customer (excludes paused periods)
- **Pricing_Service**: Domain service that calculates time-based charges
- **Session_Status**: Current state of a session (Active, Paused, Ended)
- **Time_Charge**: A line item on a ticket representing table time usage
- **Guest_Count**: Number of people using the table during a session

## Requirements

### Requirement 1: Table Session Lifecycle Management

**User Story:** As a billiard club operator, I want to start, pause, resume, and end table sessions, so that I can accurately track table usage time and bill customers accordingly.

#### Acceptance Criteria

1. WHEN an operator starts a session on an available table, THE System SHALL create a new TableSession with start time and mark the table as in-use
2. WHEN an operator attempts to start a session on an occupied table, THE System SHALL prevent the action and display an error message
3. WHEN an operator pauses an active session, THE System SHALL record the pause time and stop the billing timer
4. WHEN an operator resumes a paused session, THE System SHALL calculate the pause duration and continue billing from the resume time
5. WHEN an operator ends a session, THE System SHALL calculate the total billable time excluding paused periods and create a ticket with time charges
6. THE System SHALL prevent ending a session that is currently paused without first resuming it

### Requirement 2: Table Type and Pricing Configuration

**User Story:** As a billiard club manager, I want to configure different table types with their pricing rules, so that different tables can have different hourly rates and billing rules.

#### Acceptance Criteria

1. THE System SHALL support multiple table types with unique names and descriptions
2. WHEN creating a table type, THE System SHALL require a base hourly rate greater than zero
3. THE System SHALL allow configuring first-hour pricing rates that differ from the base hourly rate
4. THE System SHALL support minimum charge rules that ensure sessions are billed for at least a specified duration
5. THE System SHALL support time rounding rules that round session time to specified intervals (e.g., 15-minute increments)
6. WHEN a table is assigned a table type, THE System SHALL use that type's pricing rules for all sessions on that table

### Requirement 3: Real-time Session Monitoring

**User Story:** As a billiard club operator, I want to see live session information on table displays, so that I can monitor active sessions and their current charges without interrupting customers.

#### Acceptance Criteria

1. WHEN a table has an active session, THE System SHALL display a live timer showing elapsed time in HH:MM:SS format
2. THE System SHALL update the timer display every second for active sessions
3. THE System SHALL display the current running charge based on elapsed billable time and table pricing
4. WHEN a session is paused, THE System SHALL display a paused indicator and stop timer updates
5. THE System SHALL exclude paused time from the running charge calculation
6. THE System SHALL display session information including customer name (if assigned) and guest count

### Requirement 4: Time-based Charge Calculation

**User Story:** As a billiard club operator, I want the system to automatically calculate time-based charges when ending sessions, so that billing is accurate and consistent.

#### Acceptance Criteria

1. WHEN calculating time charges, THE System SHALL use the table's assigned table type pricing rules
2. THE System SHALL apply first-hour pricing if configured and the session duration includes the first hour
3. THE System SHALL round session time according to the table type's rounding rules before calculating charges
4. THE System SHALL enforce minimum charge rules by billing for at least the minimum duration even if actual time is less
5. THE System SHALL exclude all paused time from billable duration calculations
6. WHEN a session ends, THE System SHALL create a time charge line item on the ticket with duration, rate, and total charge details

### Requirement 5: Session Status and Table Management

**User Story:** As a billiard club operator, I want clear visual indicators of table status, so that I can quickly identify available tables and manage customer flow.

#### Acceptance Criteria

1. THE System SHALL display tables with color-coded status indicators: Available (green), In-Use (blue), Paused (yellow)
2. WHEN a session starts, THE System SHALL immediately update the table status to In-Use
3. WHEN a session is paused, THE System SHALL update the table status to Paused
4. WHEN a session ends, THE System SHALL update the table status to Available
5. THE System SHALL prevent starting new sessions on tables that are not Available
6. THE System SHALL provide a centralized view of all active sessions across all floors

### Requirement 6: Customer and Guest Tracking

**User Story:** As a billiard club operator, I want to associate customers with table sessions and track guest counts, so that I can provide personalized service and maintain accurate occupancy records.

#### Acceptance Criteria

1. WHEN starting a session, THE System SHALL allow optional customer assignment through search
2. THE System SHALL require a guest count between 1 and 20 when starting a session
3. WHEN a customer is assigned to a session, THE System SHALL display the customer name on the table and in session lists
4. THE System SHALL allow updating the guest count during an active session
5. THE System SHALL include customer and guest information in session reports and history

### Requirement 7: Manager Override Capabilities

**User Story:** As a billiard club manager, I want to adjust session times and override charges when necessary, so that I can handle special situations and correct billing errors.

#### Acceptance Criteria

1. WHEN a manager needs to adjust session time, THE System SHALL allow time adjustments with a required reason
2. THE System SHALL require manager-level permissions for all time adjustments
3. WHEN a time adjustment is made, THE System SHALL log the adjustment with user, timestamp, amount, and reason
4. THE System SHALL recalculate charges automatically after time adjustments
5. THE System SHALL maintain an audit trail of all manager overrides for compliance purposes

### Requirement 8: Session Transfer and Table Management

**User Story:** As a billiard club operator, I want to transfer sessions between tables when needed, so that I can accommodate customer requests and optimize table usage.

#### Acceptance Criteria

1. WHEN transferring a session, THE System SHALL verify the destination table is available
2. THE System SHALL maintain session timing continuity during transfers (no time lost)
3. WHEN a transfer completes, THE System SHALL update both source and destination table statuses
4. THE System SHALL log all session transfers with authorization and reason
5. THE System SHALL notify kitchen displays and other systems of table changes if applicable

### Requirement 9: Integration with Existing Ticket System

**User Story:** As a billiard club operator, I want table sessions to integrate seamlessly with the existing POS ticket system, so that time charges appear alongside product orders.

#### Acceptance Criteria

1. WHEN ending a session, THE System SHALL create a new ticket if no existing ticket is specified
2. THE System SHALL allow adding time charges to existing tickets when requested
3. WHEN creating time charge line items, THE System SHALL include duration, hourly rate, and total charge information
4. THE System SHALL mark time charge line items as distinct from product line items for reporting purposes
5. THE System SHALL ensure time charges follow the same tax and discount rules as other line items

### Requirement 10: Error Handling and Data Integrity

**User Story:** As a billiard club operator, I want the system to handle errors gracefully and maintain data integrity, so that session data is never lost and operations can continue smoothly.

#### Acceptance Criteria

1. WHEN database errors occur during session operations, THE System SHALL display clear error messages to operators
2. THE System SHALL prevent data corruption by validating all session state transitions
3. WHEN system restarts occur, THE System SHALL restore active sessions from persistent storage
4. THE System SHALL prevent orphaned sessions by ensuring all sessions have valid table assignments
5. THE System SHALL maintain referential integrity between sessions, tables, and tickets at all times