# Requirements Document: Table & Game Management

## Introduction

The Table & Game Management system extends the existing table session functionality to provide comprehensive billiard club operations management. This system builds upon the current `TableSession` and `TableType` entities to add advanced pricing rules, session control features, manager overrides, and sophisticated table management capabilities.

The system focuses on completing the missing Category A features identified in the delivery plan, including frontend implementations for existing backend functionality and new features for pause/resume, advanced pricing, manager overrides, and table operations.

## Glossary

- **System**: The Table & Game Management module
- **Table_Session**: An active billiard table rental with time tracking and billing
- **Table_Type**: A category of table with specific pricing rules and characteristics
- **Pricing_Rule**: Configuration that determines how time charges are calculated
- **Session_Control**: The ability to pause, resume, and manage active sessions
- **Manager_Override**: Elevated permissions allowing managers to modify sessions and pricing
- **Time_Rounding**: Rules for rounding session duration to billing increments
- **Minimum_Charge**: The smallest amount that can be charged for a session
- **First_Hour_Pricing**: Special pricing for the initial hour of a session
- **Guest_Count**: The number of players using a table during a session
- **Table_Equipment**: Physical devices and accessories linked to tables
- **Session_Transfer**: Moving an active session from one table to another
- **Table_Merge**: Combining multiple tables for larger groups
- **Table_Split**: Dividing a merged table back into individual tables
- **Game_History**: Historical record of completed sessions and games

## Requirements

### Requirement 1: Advanced Pricing Rules Implementation

**User Story:** As a club manager, I want to configure sophisticated pricing rules for different table types, so that I can optimize revenue and provide flexible pricing options.

#### Acceptance Criteria

1. WHEN configuring first-hour pricing, THE System SHALL allow different rates for the first hour versus subsequent hours
2. WHEN setting time rounding rules, THE System SHALL support rounding to 15, 30, or 60-minute increments
3. WHEN establishing minimum charges, THE System SHALL enforce minimum billing amounts regardless of actual time used
4. THE System SHALL calculate charges using the configured pricing rules for each table type
5. WHEN pricing rules change, THE System SHALL apply new rules to future sessions while preserving existing session calculations

### Requirement 2: Session Pause and Resume Control

**User Story:** As a server, I want to pause and resume table sessions, so that customers are not charged for time when they are not actively using the table.

#### Acceptance Criteria

1. WHEN a customer requests to pause their session, THE System SHALL stop the billing timer and record the pause time
2. WHEN resuming a paused session, THE System SHALL restart the billing timer from the current time
3. WHEN calculating final charges, THE System SHALL exclude all paused time from the billable duration
4. THE System SHALL display paused sessions with clear visual indicators on the floor plan
5. WHEN a session is paused for more than 2 hours, THE System SHALL alert staff to check on the table status

### Requirement 3: Manager Override Capabilities

**User Story:** As a manager, I want to override session parameters and pricing, so that I can handle exceptional situations and provide customer service flexibility.

#### Acceptance Criteria

1. WHEN a manager needs to adjust session time, THE System SHALL allow manual time adjustments with authorization
2. WHEN overriding pricing, THE System SHALL require manager PIN and reason code entry
3. WHEN forcing session end, THE System SHALL allow managers to end sessions regardless of current state
4. THE System SHALL log all manager overrides with timestamp, user ID, and justification
5. WHEN reviewing overrides, THE System SHALL provide complete audit trails for management review

### Requirement 4: Enhanced Guest Count Management

**User Story:** As a server, I want to track and update guest counts for active sessions, so that I can apply appropriate pricing and service levels.

#### Acceptance Criteria

1. WHEN starting a session, THE System SHALL require initial guest count entry
2. WHEN guest count changes during a session, THE System SHALL allow updates with staff authorization
3. WHEN guest count affects pricing, THE System SHALL recalculate charges based on current count
4. THE System SHALL display current guest count on the floor plan for each active table
5. WHEN generating reports, THE System SHALL include guest count analytics for capacity planning

### Requirement 5: Table Type Configuration Interface

**User Story:** As a club manager, I want to configure table types and their properties through the user interface, so that I can manage table categories without technical assistance.

#### Acceptance Criteria

1. WHEN accessing table type management, THE System SHALL display all configured table types with their properties
2. WHEN creating new table types, THE System SHALL allow configuration of pricing rules, descriptions, and characteristics
3. WHEN modifying table types, THE System SHALL validate that changes don't conflict with active sessions
4. THE System SHALL allow assignment of table types to individual tables through the interface
5. WHEN table types are updated, THE System SHALL apply changes to future sessions while preserving active session pricing

### Requirement 6: Time-Based Pricing Display

**User Story:** As a server, I want to see real-time pricing information for different table types, so that I can inform customers about costs before starting sessions.

#### Acceptance Criteria

1. WHEN viewing table information, THE System SHALL display current hourly rates and pricing rules
2. WHEN starting a session, THE System SHALL show estimated costs based on expected duration
3. WHEN pricing varies by time of day, THE System SHALL display current applicable rates
4. THE System SHALL show first-hour pricing, minimum charges, and rounding rules clearly
5. WHEN customers inquire about costs, THE System SHALL provide pricing calculators for different scenarios

### Requirement 7: Equipment Linking and Management

**User Story:** As a club manager, I want to link equipment and accessories to specific tables, so that I can track inventory and ensure proper setup.

#### Acceptance Criteria

1. WHEN configuring tables, THE System SHALL allow linking of equipment items (cues, balls, racks, etc.)
2. WHEN equipment is assigned, THE System SHALL track equipment status and availability
3. WHEN starting sessions, THE System SHALL verify required equipment is available and assigned
4. THE System SHALL alert staff when equipment needs maintenance or replacement
5. WHEN generating reports, THE System SHALL provide equipment utilization and maintenance schedules

### Requirement 8: Game History and Analytics

**User Story:** As a club owner, I want to track game history and session analytics, so that I can understand usage patterns and optimize operations.

#### Acceptance Criteria

1. WHEN sessions end, THE System SHALL record complete session details including duration, charges, and participants
2. THE System SHALL track game types, session outcomes, and customer preferences
3. WHEN analyzing usage, THE System SHALL provide reports on peak times, popular table types, and revenue patterns
4. THE System SHALL identify frequent customers and their playing habits for marketing purposes
5. WHEN reviewing performance, THE System SHALL show table utilization rates and revenue per table

### Requirement 9: Server Assignment and Management

**User Story:** As a manager, I want to assign servers to tables and track their performance, so that I can ensure good customer service and fair tip distribution.

#### Acceptance Criteria

1. WHEN starting sessions, THE System SHALL allow assignment of primary servers to tables
2. WHEN servers change shifts, THE System SHALL support server reassignment during active sessions
3. WHEN calculating tips, THE System SHALL properly allocate gratuity based on server assignments
4. THE System SHALL track server performance metrics including sales and customer satisfaction
5. WHEN generating reports, THE System SHALL provide server-specific analytics and commission calculations

### Requirement 10: Table Merge and Split Operations

**User Story:** As a server, I want to merge tables for large groups and split them back when needed, so that I can accommodate varying party sizes efficiently.

#### Acceptance Criteria

1. WHEN large groups arrive, THE System SHALL allow merging adjacent tables into a single session
2. WHEN merging tables, THE System SHALL combine billing and maintain accurate time tracking
3. WHEN splitting merged tables, THE System SHALL properly allocate charges and create separate sessions
4. THE System SHALL ensure merged tables maintain proper equipment assignments and server allocations
5. WHEN managing merged tables, THE System SHALL provide clear visual indicators on the floor plan

### Requirement 11: Session Transfer Between Tables

**User Story:** As a server, I want to transfer active sessions between tables, so that I can accommodate customer preferences and handle table maintenance.

#### Acceptance Criteria

1. WHEN customers request table changes, THE System SHALL allow transferring active sessions to available tables
2. WHEN transferring sessions, THE System SHALL preserve all timing, billing, and customer information
3. WHEN transfer is complete, THE System SHALL update table statuses and floor plan displays
4. THE System SHALL maintain audit trails for all session transfers with reasons and authorization
5. WHEN transfers affect pricing, THE System SHALL handle rate differences appropriately

### Requirement 12: Real-Time Session Monitoring

**User Story:** As a manager, I want to monitor all active sessions in real-time, so that I can oversee operations and identify issues quickly.

#### Acceptance Criteria

1. THE System SHALL display all active sessions with current duration and charges on a central dashboard
2. WHEN sessions exceed expected duration, THE System SHALL alert staff to check on customers
3. WHEN tables have been idle, THE System SHALL identify opportunities for new customer seating
4. THE System SHALL show session status indicators (active, paused, ending soon) with color coding
5. WHEN problems occur, THE System SHALL provide quick access to session management functions

### Requirement 13: Pricing Rule Validation and Testing

**User Story:** As a club manager, I want to test pricing configurations before applying them, so that I can ensure accurate billing and avoid customer disputes.

#### Acceptance Criteria

1. WHEN configuring pricing rules, THE System SHALL provide simulation tools to test different scenarios
2. THE System SHALL validate that pricing rules are mathematically consistent and logical
3. WHEN rules conflict, THE System SHALL identify conflicts and suggest resolutions
4. THE System SHALL allow preview of pricing calculations before saving configuration changes
5. WHEN implementing new rules, THE System SHALL provide rollback capabilities if issues arise

### Requirement 14: Integration with Existing Systems

**User Story:** As a system administrator, I want table management to integrate seamlessly with existing POS functions, so that operations remain smooth and data stays consistent.

#### Acceptance Criteria

1. WHEN sessions end, THE System SHALL automatically create appropriate line items in the ticket system
2. THE System SHALL integrate with the payment processing system for seamless checkout
3. WHEN generating reports, THE System SHALL coordinate with the reporting system for comprehensive analytics
4. THE System SHALL maintain data consistency with customer management and inventory systems
5. WHEN system updates occur, THE System SHALL preserve integration points and data integrity

### Requirement 15: Performance and Scalability

**User Story:** As a system administrator, I want the table management system to perform efficiently under load, so that operations continue smoothly during peak business hours.

#### Acceptance Criteria

1. WHEN managing multiple concurrent sessions, THE System SHALL maintain response times under 200 milliseconds
2. THE System SHALL support at least 50 simultaneous active sessions without performance degradation
3. WHEN updating session information, THE System SHALL propagate changes to all connected terminals within 5 seconds
4. THE System SHALL handle system recovery gracefully, preserving all active session data
5. WHEN generating reports, THE System SHALL complete processing within 10 seconds for standard date ranges