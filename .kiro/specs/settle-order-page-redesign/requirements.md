# Requirements Document

## Introduction

This specification defines the requirements for redesigning the Settle Page and Order Page in the Magidesk POS system. The redesign aims to modernize the user interface, improve usability, enhance workflow efficiency, and provide a more intuitive experience for restaurant staff during order entry and payment settlement operations.

## Glossary

- **Settle_Page**: The user interface where payment settlement occurs for a ticket
- **Order_Page**: The user interface where staff enter and manage customer orders
- **Ticket**: A collection of order items associated with a table or customer
- **Tender_Amount**: The amount of money being offered as payment
- **Balance_Due**: The remaining amount to be paid on a ticket
- **Payment_Method**: The type of payment (Cash, Credit Card, Gift Card)
- **Quick_Cash**: Predefined cash amount buttons for rapid payment entry
- **Order_Item**: A product added to a ticket with optional modifiers
- **Modifier**: A customization applied to an order item (e.g., "Extra Cheese")
- **Category**: A grouping of products (e.g., Food, Drinks, Desserts)
- **Session**: A period of active POS operation by a user
- **Split_Payment**: Dividing a ticket's payment across multiple payment methods
- **Tax_Exempt**: A status indicating a ticket is not subject to sales tax

## Requirements

### Requirement 1: Settle Page Layout and Navigation

**User Story:** As a cashier, I want a clear and organized settle page layout, so that I can quickly process payments without confusion.

#### Acceptance Criteria

1. WHEN the settle page loads, THE Settle_Page SHALL display ticket information including ticket number and table number in the header
2. WHEN the settle page loads, THE Settle_Page SHALL display the current user and terminal information in the header
3. THE Settle_Page SHALL provide a back button to return to the order page
4. THE Settle_Page SHALL display a settings button for accessing configuration options
5. THE Settle_Page SHALL organize content into three distinct sections: ticket summary (left), tender entry (center), and payment actions (right)

### Requirement 2: Ticket Summary Display

**User Story:** As a cashier, I want to see all relevant ticket financial information at a glance, so that I can verify amounts before processing payment.

#### Acceptance Criteria

1. THE Settle_Page SHALL display the total amount labeled as "Total Amount" in the left sidebar
2. THE Settle_Page SHALL display the tax amount labeled as "Tax Amount" below the total amount
3. THE Settle_Page SHALL display the paid amount labeled as "Paid Amount" with green color styling
4. THE Settle_Page SHALL display the balance due with the label "BALANCE DUE" in uppercase, small font, and primary color
5. THE Settle_Page SHALL display the balance due amount in 4xl font size below the label
6. THE Settle_Page SHALL separate the balance due section from other amounts with a top border
7. WHEN multiple payments have been applied, THE Settle_Page SHALL update the paid amount and balance due in real-time
8. THE Settle_Page SHALL display all amounts in currency format with two decimal places
9. THE Settle_Page SHALL display amount labels in gray color and amounts in white (dark mode)

### Requirement 3: Tender Amount Entry

**User Story:** As a cashier, I want to enter tender amounts using a numeric keypad, so that I can quickly input payment values.

#### Acceptance Criteria

1. THE Settle_Page SHALL provide a numeric keypad with digits 0-9 arranged in a 3x4 grid
2. THE Settle_Page SHALL arrange keypad buttons in standard calculator layout (7-8-9, 4-5-6, 1-2-3, C-0-.)
3. THE Settle_Page SHALL provide a decimal point button in the bottom right position
4. THE Settle_Page SHALL provide a clear button labeled "C" in red styling in the bottom left position
5. WHEN a keypad button is pressed, THE Settle_Page SHALL append the digit to the tender amount display
6. WHEN the clear button is pressed, THE Settle_Page SHALL reset the tender amount to $0.00
7. THE Settle_Page SHALL display the tender amount in a rounded rectangle panel above the keypad
8. THE Settle_Page SHALL display "Tender Amount" label in small uppercase text above the amount
9. THE Settle_Page SHALL display the tender amount in 6xl font size with light font weight
10. WHEN the tender amount is zero, THE Settle_Page SHALL display "$0.00" in the tender amount field
11. THE Settle_Page SHALL provide tactile feedback on keypad button press with scale animation

### Requirement 4: Quick Cash Entry

**User Story:** As a cashier, I want quick access to common cash denominations, so that I can speed up cash transactions.

#### Acceptance Criteria

1. THE Settle_Page SHALL provide quick cash buttons for $1, $5, $10, $20, $50, and $100
2. WHEN a quick cash button is pressed, THE Settle_Page SHALL set the tender amount to that denomination
3. THE Settle_Page SHALL arrange quick cash buttons in a 3x2 grid layout
4. THE Settle_Page SHALL label each quick cash button with the dollar amount in bold font
5. THE Settle_Page SHALL display a section header "Quick Cash" in small uppercase font above the buttons
6. THE Settle_Page SHALL style quick cash buttons with white/surface-dark background and border
7. THE Settle_Page SHALL position the quick cash section between action buttons and payment method buttons

### Requirement 5: Payment Method Selection

**User Story:** As a cashier, I want to select different payment methods, so that I can process various types of payments.

#### Acceptance Criteria

1. THE Settle_Page SHALL provide a Cash payment button with green background (#107C10)
2. THE Settle_Page SHALL provide a Credit Card payment button with primary blue background (#0078D4)
3. THE Settle_Page SHALL provide a Gift Card payment button with purple background (#8E44AD)
4. WHEN a payment method button is pressed, THE Settle_Page SHALL process the payment using the current tender amount
5. THE Settle_Page SHALL display payment method buttons with Material Icons and uppercase text labels
6. THE Settle_Page SHALL arrange payment method buttons vertically in the right panel with spacing
7. THE Settle_Page SHALL display a chevron right icon on each payment button
8. THE Settle_Page SHALL display payment icons on the left and chevron on the right within each button
9. THE Settle_Page SHALL apply shadow effects to payment method buttons
10. THE Settle_Page SHALL make payment buttons full width with rounded corners

### Requirement 6: Additional Payment Actions

**User Story:** As a cashier, I want access to additional payment-related actions, so that I can handle special payment scenarios.

#### Acceptance Criteria

1. THE Settle_Page SHALL provide an "ADD TIP" button with primary blue background and prominent styling at the top of the right panel
2. THE Settle_Page SHALL display a smiley face icon on the "ADD TIP" button
3. THE Settle_Page SHALL provide a "HOLD TICKET" button with pause icon in a 2x2 grid layout
4. THE Settle_Page SHALL provide a "SPLIT PMT" button with split icon in the grid layout
5. THE Settle_Page SHALL provide a "DISCOUNT" button with tag icon in the grid layout
6. THE Settle_Page SHALL provide a "RECEIPT" button with print icon in the grid layout
7. WHEN the "ADD TIP" button is pressed, THE Settle_Page SHALL display a tip entry interface
8. THE Settle_Page SHALL display all grid action buttons with white/surface-dark background and border
9. THE Settle_Page SHALL display action button labels in small uppercase font
10. THE Settle_Page SHALL separate the action button grid from quick cash section with a horizontal divider

### Requirement 7: Tax Exempt Functionality

**User Story:** As a cashier, I want to mark tickets as tax exempt, so that I can process tax-free transactions when applicable.

#### Acceptance Criteria

1. THE Settle_Page SHALL provide a "Tax Exempt" button in the header
2. WHEN the "Tax Exempt" button is pressed, THE Settle_Page SHALL toggle the tax exempt status
3. WHEN a ticket is marked tax exempt, THE Settle_Page SHALL recalculate the total without tax
4. WHEN a ticket is marked tax exempt, THE Settle_Page SHALL update the balance due accordingly

### Requirement 8: Settlement Cancellation

**User Story:** As a cashier, I want to cancel the settlement process, so that I can return to order entry if needed.

#### Acceptance Criteria

1. THE Settle_Page SHALL provide a "Cancel Settlement" button at the bottom of the ticket summary panel
2. WHEN the "Cancel Settlement" button is pressed, THE Settle_Page SHALL return to the order page without processing payment
3. WHEN the "Cancel Settlement" button is pressed, THE Settle_Page SHALL preserve the ticket state

### Requirement 9: Order Page Layout and Navigation

**User Story:** As a server, I want a clear and organized order page layout, so that I can efficiently enter and manage orders.

#### Acceptance Criteria

1. THE Order_Page SHALL display a header with height of 48px and dark background
2. THE Order_Page SHALL display system information including terminal name labeled "TERMINAL:", user name labeled "USER:", and system status labeled "SYSTEM:"
3. THE Order_Page SHALL display system status as "ONLINE" in green color when connected
4. THE Order_Page SHALL display the current time with a clock icon in a rounded background
5. THE Order_Page SHALL display the page title "POS - High Volume Mode" in small uppercase font
6. THE Order_Page SHALL organize content into two main sections: order sidebar (400px width) and product area (flexible width)
7. THE Order_Page SHALL provide a menu button with hamburger icon in the header
8. THE Order_Page SHALL display a footer with height of 32px showing station information and copyright
9. THE Order_Page SHALL display a green status indicator dot in the footer
10. THE Order_Page SHALL use dark theme colors: background (#1C1C1C), surface (#2C2C2C), sidebar (#202020)

### Requirement 10: Table and Guest Management

**User Story:** As a server, I want to select tables and specify guest counts, so that I can associate orders with the correct table.

#### Acceptance Criteria

1. THE Order_Page SHALL display the current table number and guest count in a prominent button at the top of the sidebar
2. THE Order_Page SHALL display table information with format "TABLE XX (GUESTS: X)" in large bold font
3. THE Order_Page SHALL display "Current Table" label above the table number in small uppercase font
4. THE Order_Page SHALL display a table_restaurant icon on the left side of the table button
5. THE Order_Page SHALL display an expand_more icon on the right side of the table button
6. WHEN the table selector is clicked, THE Order_Page SHALL display a table selection interface
7. THE Order_Page SHALL style the table button with primary blue background and shadow effect
8. THE Order_Page SHALL make the table button full width with rounded corners

### Requirement 11: Product Search

**User Story:** As a server, I want to search for products by name or SKU, so that I can quickly find items without browsing categories.

#### Acceptance Criteria

1. THE Order_Page SHALL provide a search input field with a search icon
2. WHEN text is entered in the search field, THE Order_Page SHALL filter products matching the search term
3. THE Order_Page SHALL search both product names and SKU codes
4. THE Order_Page SHALL display search results in real-time as the user types
5. THE Order_Page SHALL display a placeholder text "Search product or SKU..." in the search field

### Requirement 12: Order Item Display

**User Story:** As a server, I want to see all items in the current order, so that I can verify the order is correct before submitting.

#### Acceptance Criteria

1. THE Order_Page SHALL display all order items in a scrollable list with custom scrollbar styling
2. WHEN an order item has modifiers, THE Order_Page SHALL display the modifiers in italic text below the item name with reduced opacity
3. THE Order_Page SHALL display the quantity with "x" suffix in primary blue color before the item name
4. THE Order_Page SHALL display the item price aligned to the right
5. WHEN an order item is hovered, THE Order_Page SHALL highlight it with border and background color change
6. THE Order_Page SHALL provide "Edit" and "Remove" action links below items with modifiers
7. THE Order_Page SHALL display the ticket number with format "Ticket #XXXX" above the order item list
8. THE Order_Page SHALL provide person add and history icons next to the ticket number
9. THE Order_Page SHALL style order items with rounded corners and subtle background
10. THE Order_Page SHALL display special pricing notes (e.g., "Happy Hour Pricing Applied") in small italic text with reduced opacity

### Requirement 13: Order Item Modification

**User Story:** As a server, I want to edit or remove order items, so that I can correct mistakes or accommodate customer changes.

#### Acceptance Criteria

1. WHEN the "Edit" button is pressed for an order item, THE Order_Page SHALL display a modifier interface for that item
2. WHEN the "Remove" button is pressed for an order item, THE Order_Page SHALL remove the item from the order
3. WHEN an order item is removed, THE Order_Page SHALL update the order total immediately
4. THE Order_Page SHALL display edit and remove buttons on hover or selection of an order item

### Requirement 14: Order Total Calculation

**User Story:** As a server, I want to see the order total with tax breakdown, so that I can inform customers of the final amount.

#### Acceptance Criteria

1. THE Order_Page SHALL display the subtotal labeled as "Subtotal" in small uppercase font
2. THE Order_Page SHALL display the tax amount labeled as "Tax (8%)" showing the tax rate
3. THE Order_Page SHALL display the total labeled as "TOTAL" in large uppercase font
4. THE Order_Page SHALL display the total amount in 2xl font size with black font weight
5. THE Order_Page SHALL display the total amount in primary blue color
6. THE Order_Page SHALL display subtotal and tax labels with reduced opacity
7. THE Order_Page SHALL separate the total from subtotal/tax with top padding
8. WHEN order items are added or removed, THE Order_Page SHALL recalculate all amounts immediately
9. THE Order_Page SHALL display the totals section in a dark background panel at the bottom of the order sidebar

### Requirement 15: Quick Order Actions

**User Story:** As a server, I want quick access to common order actions, so that I can efficiently manage orders.

#### Acceptance Criteria

1. THE Order_Page SHALL provide a "Split" button with call_split icon in a 4-column grid
2. THE Order_Page SHALL provide a "Merge" button with merge_type icon in the grid
3. THE Order_Page SHALL provide a "Note" button with sticky_note_2 icon in the grid
4. THE Order_Page SHALL provide a "Print" button with print icon in the grid
5. THE Order_Page SHALL display quick action buttons with icons above text labels
6. THE Order_Page SHALL display action labels in 9px uppercase bold font
7. THE Order_Page SHALL style action buttons with white/surface background and subtle hover effects
8. THE Order_Page SHALL position quick action buttons above the settle and pay buttons in the order sidebar

### Requirement 16: Payment Initiation

**User Story:** As a server, I want to initiate payment processing, so that I can settle the order when the customer is ready to pay.

#### Acceptance Criteria

1. THE Order_Page SHALL provide a "SETTLE" button with account_balance_wallet icon
2. THE Order_Page SHALL provide a "PAY NOW" button with payments icon
3. WHEN the "SETTLE" button is pressed, THE Order_Page SHALL navigate to the settle page with the current ticket
4. WHEN the "PAY NOW" button is pressed, THE Order_Page SHALL initiate immediate payment processing
5. THE Order_Page SHALL display the "PAY NOW" button with green background (#107C10) to indicate primary action
6. THE Order_Page SHALL display the "SETTLE" button with white/transparent background
7. THE Order_Page SHALL arrange payment buttons in a 2-column grid layout
8. THE Order_Page SHALL display payment buttons with icons and uppercase text labels
9. THE Order_Page SHALL apply shadow effects to the "PAY NOW" button

### Requirement 17: Product Category Navigation

**User Story:** As a server, I want to browse products by category, so that I can find items organized by type.

#### Acceptance Criteria

1. THE Order_Page SHALL provide category tabs for Food, Drinks, Desserts, Sides, Popular, and Retail
2. WHEN a category tab is clicked, THE Order_Page SHALL display products in that category
3. THE Order_Page SHALL highlight the active category tab with a bottom border and background
4. THE Order_Page SHALL display category tabs with icons and text labels
5. THE Order_Page SHALL allow horizontal scrolling of category tabs if they exceed screen width

### Requirement 18: Product Subcategory Filtering

**User Story:** As a server, I want to filter products by subcategory, so that I can narrow down product selections within a category.

#### Acceptance Criteria

1. WHEN a category is selected, THE Order_Page SHALL display subcategory filter buttons
2. THE Order_Page SHALL provide subcategory filters relevant to the selected category
3. WHEN a subcategory filter is clicked, THE Order_Page SHALL display only products in that subcategory
4. THE Order_Page SHALL highlight the active subcategory filter with primary color styling
5. THE Order_Page SHALL allow horizontal scrolling of subcategory filters if they exceed screen width

### Requirement 19: Product Grid Display

**User Story:** As a server, I want to see products in a grid layout, so that I can quickly scan and select items.

#### Acceptance Criteria

1. THE Order_Page SHALL display products in a responsive grid with 2-6 columns based on screen width
2. THE Order_Page SHALL display each product in a card with height of 128px (h-32)
3. THE Order_Page SHALL display product name in small bold font at the top of the card
4. THE Order_Page SHALL display product price in small bold font with primary blue color at the bottom left
5. WHEN a product is hovered, THE Order_Page SHALL display an add_circle icon in primary blue color at the bottom right
6. WHEN a product is hovered, THE Order_Page SHALL change the border color to primary blue
7. THE Order_Page SHALL style product cards with surface-dark background and subtle border
8. THE Order_Page SHALL make the product grid scrollable when products exceed visible area
9. THE Order_Page SHALL use flexbox layout with space-between for product name and price positioning
10. THE Order_Page SHALL apply smooth transitions for hover effects

### Requirement 20: Product Selection

**User Story:** As a server, I want to add products to the order by clicking them, so that I can quickly build orders.

#### Acceptance Criteria

1. WHEN a product is clicked, THE Order_Page SHALL add the product to the order item list
2. WHEN a product is added, THE Order_Page SHALL update the order total immediately
3. WHEN a product with modifiers is clicked, THE Order_Page SHALL display a modifier selection interface
4. WHEN a product is added, THE Order_Page SHALL provide visual feedback (animation or highlight)

### Requirement 21: Session Management

**User Story:** As a manager, I want to start, pause, resume, and end POS sessions, so that I can track cash drawer activity and shifts with accurate timing.

#### Acceptance Criteria

1. THE Order_Page SHALL provide a "Start Session" button in the footer
2. THE Order_Page SHALL provide an "End Session" button in the footer
3. WHEN no session is active, THE Order_Page SHALL display the "Start Session" button
4. WHEN no session is active, THE Order_Page SHALL disable the "End Session" button
5. WHEN the "Start Session" button is pressed, THE Order_Page SHALL initiate a new session and change the button to "Pause Session"
6. WHEN a session is active, THE Order_Page SHALL enable the "End Session" button
7. WHEN the "Pause Session" button is pressed, THE Order_Page SHALL pause the current session and change the button to "Resume Session"
8. WHEN the "Resume Session" button is pressed, THE Order_Page SHALL resume the paused session and change the button to "Pause Session"
9. WHEN the "End Session" button is pressed, THE Order_Page SHALL close the current session and reset the button to "Start Session"
10. THE Order_Page SHALL display session control buttons with icons and labels
11. THE Order_Page SHALL maintain session state (not started, active, paused) throughout the application lifecycle
12. WHEN a session is active or paused, THE Order_Page SHALL display the session duration in HH:MM:SS timer format
13. WHEN a session is paused, THE Order_Page SHALL stop incrementing the session duration timer
14. WHEN a paused session is resumed, THE Order_Page SHALL continue incrementing the session duration from where it was paused
15. WHEN the "End Session" button is pressed, THE Order_Page SHALL calculate the total session duration
16. WHEN a session is ended, THE Order_Page SHALL add an expense line item to the current order based on the session duration
17. THE Order_Page SHALL update the session duration display in real-time every second while the session is active

### Requirement 22: Advanced Order Operations

**User Story:** As a server or manager, I want access to advanced order operations, so that I can handle special situations.

#### Acceptance Criteria

1. THE Order_Page SHALL provide a "REPRINT" button with receipt_long icon in the footer
2. THE Order_Page SHALL provide a "VOID" button with cancel icon in the footer
3. THE Order_Page SHALL provide a "DISCOUNT" button with loyalty icon in the footer
4. THE Order_Page SHALL provide a "FIRE TICKET" button with room_service icon in the footer
5. THE Order_Page SHALL display advanced operation buttons in a horizontal scrollable row
6. THE Order_Page SHALL display operation buttons with icons and uppercase text labels
7. THE Order_Page SHALL apply hover color effects: blue for reprint, red for void, orange for discount, indigo for fire ticket
8. THE Order_Page SHALL separate session controls from advanced operations with a vertical divider
9. THE Order_Page SHALL style operation buttons with white/surface background and border

### Requirement 23: Order Statistics Display

**User Story:** As a server, I want to see order statistics, so that I can monitor order status and timing.

#### Acceptance Criteria

1. THE Order_Page SHALL display the total number of items in the current order
2. THE Order_Page SHALL display the wait time for the current order
3. THE Order_Page SHALL update statistics in real-time as the order changes
4. THE Order_Page SHALL display statistics in the footer area

### Requirement 24: Dark Mode Support

**User Story:** As a user, I want the interface to support dark mode, so that I can work comfortably in low-light environments.

#### Acceptance Criteria

1. THE Settle_Page SHALL render correctly in dark mode with appropriate color contrast
2. THE Order_Page SHALL render correctly in dark mode with appropriate color contrast
3. WHEN dark mode is active, THE system SHALL use dark backgrounds and light text
4. WHEN dark mode is active, THE system SHALL maintain visual hierarchy and readability
5. THE system SHALL use consistent color schemes across both pages in dark mode

### Requirement 25: Responsive Layout

**User Story:** As a user, I want the interface to adapt to different screen sizes, so that I can use the system on various devices.

#### Acceptance Criteria

1. THE Settle_Page SHALL maintain usability on screens with minimum width of 1024px
2. THE Order_Page SHALL adjust the product grid columns based on available screen width
3. WHEN screen width is reduced, THE system SHALL allow horizontal scrolling for overflow content
4. THE system SHALL maintain touch-friendly button sizes on all screen sizes
5. THE system SHALL use responsive units for spacing and sizing

### Requirement 26: Visual Feedback and Interactions

**User Story:** As a user, I want visual feedback for my interactions, so that I know the system is responding to my actions.

#### Acceptance Criteria

1. WHEN a button is pressed, THE system SHALL provide visual feedback (scale, opacity, or color change)
2. WHEN a button is hovered, THE system SHALL change the button appearance
3. THE system SHALL use smooth transitions for state changes
4. THE system SHALL provide loading indicators for asynchronous operations
5. THE system SHALL use consistent interaction patterns across both pages

### Requirement 27: Accessibility and Usability

**User Story:** As a user, I want the interface to be accessible and easy to use, so that I can work efficiently without errors.

#### Acceptance Criteria

1. THE system SHALL use clear, readable fonts with appropriate sizing
2. THE system SHALL provide sufficient color contrast for text and interactive elements
3. THE system SHALL use icons alongside text labels for clarity
4. THE system SHALL provide clear visual hierarchy with spacing and typography
5. THE system SHALL use consistent styling and patterns across both pages
