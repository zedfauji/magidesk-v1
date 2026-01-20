# Implementation Plan: Settle and Order Page Redesign

## Overview

This implementation plan breaks down the redesign of the Settle Page and Order Page into discrete, incremental tasks. The approach follows the existing Magidesk MVVM architecture and integrates with the established domain models, commands, and queries.

The implementation will proceed in phases:
1. Create shared resources and styles
2. Implement Settle Page (ViewModel → View → Integration)
3. Implement Order Page (ViewModel → View → Integration)
4. Wire navigation and test end-to-end flows

## Tasks

- [x] 1. Set up shared resources and styling infrastructure
  - Create color palette resource dictionary with WinUI-inspired colors
  - Create typography styles for Inter font family
  - Create button styles (primary, secondary, payment method, quick action)
  - Create icon resources for Material Design Icons
  - Set up animation resources (scale, fade, slide transitions)
  - _Requirements: 24.1, 24.2, 24.5, 27.1, 27.5_

- [x] 2. Implement SettlePageViewModel
  - [x] 2.1 Create SettlePageViewModel class with properties and commands
    - Implement ticket information properties (TicketNumber, TableNumber)
    - Implement financial summary properties (TotalAmount, TaxAmount, PaidAmount, BalanceDue)
    - Implement tender entry properties (TenderAmountDisplay, _tenderAmount)
    - Implement payment method collection
    - Implement quick cash amounts collection
    - Implement state properties (IsTaxExempt, IsProcessingPayment)
    - Implement all ICommand properties
    - _Requirements: 1.1, 2.1, 2.2, 2.3, 2.4, 3.7, 3.8, 4.1, 5.1, 5.2, 5.3, 7.1_

  - [x] 2.2 Write property test for tender amount building
    - **Property 1: Tender Amount Building**
    - **Validates: Requirements 3.5**

  - [x] 2.3 Implement keypad digit command (KeypadDigitCommand)
    - Handle digit button presses (0-9)
    - Handle decimal point button press
    - Append digit to tender amount string
    - Update TenderAmountDisplay property
    - _Requirements: 3.5_

  - [x] 2.4 Write property test for tender amount clearing
    - **Property 2: Tender Amount Clearing**
    - **Validates: Requirements 3.6**

  - [x] 2.5 Implement clear tender command (ClearTenderCommand)
    - Reset tender amount to $0.00
    - Update TenderAmountDisplay property
    - _Requirements: 3.6, 3.10_

  - [x] 2.6 Write property test for quick cash selection
    - **Property 3: Quick Cash Selection**
    - **Validates: Requirements 4.2**

  - [x] 2.7 Implement quick cash command (QuickCashCommand)
    - Set tender amount to selected denomination
    - Update TenderAmountDisplay property
    - _Requirements: 4.2_

  - [x] 2.8 Write property test for payment processing
    - **Property 4: Payment Processing with Tender Amount**
    - **Validates: Requirements 5.4**

  - [x] 2.9 Write property test for payment balance calculation
    - **Property 5: Payment Balance Calculation**
    - **Validates: Requirements 2.7**

  - [x] 2.10 Implement process payment command (ProcessPaymentCommand)
    - Validate tender amount > 0
    - Execute ProcessPaymentCommand with amount and method
    - Update PaidAmount and BalanceDue
    - Handle overpayment (calculate change)
    - Handle partial payment
    - Show loading indicator during processing
    - Handle payment errors with user feedback
    - _Requirements: 5.4, 2.7_

  - [x] 2.11 Write property test for currency formatting
    - **Property 6: Currency Formatting**
    - **Validates: Requirements 2.8**

  - [x] 2.12 Write property test for tax exempt recalculation
    - **Property 7: Tax Exempt Recalculation**
    - **Validates: Requirements 7.3, 7.4**

  - [x] 2.13 Implement toggle tax exempt command (ToggleTaxExemptCommand)
    - Toggle IsTaxExempt property
    - Recalculate TotalAmount (with or without tax)
    - Update BalanceDue
    - _Requirements: 7.2, 7.3, 7.4_

  - [x] 2.14 Write property test for cancel settlement preserves state
    - **Property 8: Cancel Settlement Preserves State**
    - **Validates: Requirements 8.3**

  - [x] 2.15 Implement additional action commands
    - Implement AddTipCommand (show tip entry dialog)
    - Implement HoldTicketCommand (save ticket and return to order page)
    - Implement SplitPaymentCommand (show split payment dialog)
    - Implement ApplyDiscountCommand (show discount dialog)
    - Implement PrintReceiptCommand (print receipt)
    - _Requirements: 6.1, 6.3, 6.4, 6.5, 6.6, 6.7_

  - [x] 2.16 Implement navigation commands
    - Implement CancelSettlementCommand (navigate back without saving)
    - Implement NavigateBackCommand (navigate to order page)
    - _Requirements: 8.2, 8.3, 1.3_

  - [x] 2.17 Write unit tests for SettlePageViewModel
    - Test command execution and state changes
    - Test property change notifications
    - Test validation logic
    - Test error handling scenarios

- [x] 3. Create SettlePageView (XAML)
  - [x] 3.1 Create SettlePageView.xaml with main layout structure
    - Create header with back button, title, ticket info, tax exempt button, settings button
    - Create three-column main layout (ticket summary, tender entry, payment actions)
    - Create footer with terminal, user, cash balance, and time
    - Apply dark theme styling
    - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 1.10_

  - [x] 3.2 Implement ticket summary sidebar (left panel)
    - Display Total Amount with label
    - Display Tax Amount with label
    - Display Paid Amount with label (green styling)
    - Display Balance Due with label (uppercase, primary color)
    - Display Balance Due amount (4xl font)
    - Add top border separator for balance due section
    - Add Cancel Settlement button at bottom
    - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 2.6, 8.1_

  - [x] 3.3 Implement tender entry panel (center)
    - Create tender amount display panel (rounded, 6xl font)
    - Create numeric keypad grid (3x4 layout)
    - Style digit buttons (7-8-9, 4-5-6, 1-2-3, C-0-.)
    - Style clear button (red background)
    - Add button press animations (scale effect)
    - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.7, 3.8, 3.9, 3.11_

  - [x] 3.4 Implement payment actions sidebar (right panel)
    - Create ADD TIP button (primary blue, smiley icon)
    - Create action button grid (2x2: Hold Ticket, Split PMT, Discount, Receipt)
    - Add horizontal divider
    - Create Quick Cash section with header
    - Create quick cash button grid (3x2: $1, $5, $10, $20, $50, $100)
    - Create payment method buttons (Cash-green, Credit Card-blue, Gift Card-purple)
    - Style all buttons with icons and labels
    - _Requirements: 4.1, 4.3, 4.4, 4.5, 4.6, 4.7, 5.1, 5.2, 5.3, 5.5, 5.6, 5.7, 5.8, 5.9, 5.10, 6.1, 6.2, 6.3, 6.4, 6.5, 6.6, 6.8, 6.9, 6.10_

  - [x] 3.5 Write UI integration tests for SettlePageView
    - Test data binding correctness
    - Test button command bindings
    - Test UI element visibility based on state
    - Test accessibility features

- [ ] 4. Checkpoint - Settle Page Complete
  - Ensure all Settle Page tests pass
  - Manually test Settle Page UI in isolation
  - Ask the user if questions arise

- [-] 5. Implement OrderPageViewModel
  - [x] 5.1 Create OrderPageViewModel class with properties and commands
    - Implement table information properties (TableNumber, GuestCount)
    - Implement ticket information properties (TicketNumber, TicketStartTime, WaitTime)
    - Implement order items collection
    - Implement financial calculation properties (Subtotal, TaxAmount, TaxRate, Total)
    - Implement product catalog properties (Categories, SelectedCategory, Subcategories, SelectedSubcategory, FilteredProducts)
    - Implement search property (SearchQuery)
    - Implement session information properties (TerminalName, UserName, SystemStatus, CurrentTime)
    - Implement statistics properties (TotalItemCount)
    - Implement all ICommand properties
    - _Requirements: 9.2, 9.3, 9.4, 9.5, 10.1, 11.1, 12.7, 14.1, 14.2, 14.3, 23.1, 23.2_

  - [x] 5.2 Write property test for table display format
    - **Property 9: Table Display Format**
    - **Validates: Requirements 10.2**
    - **Note**: Test created but cannot run due to unrelated test project compilation errors

  - [x] 5.3 Write property test for product search filtering
    - **Property 10: Product Search Filtering**
    - **Validates: Requirements 11.2, 11.3**

  - [x] 5.4 Implement search product command (SearchProductCommand)
    - Filter products by name (case-insensitive)
    - Filter products by SKU (case-insensitive)
    - Update FilteredProducts collection
    - _Requirements: 11.2, 11.3_

  - [x] 5.5 Write property test for order item display completeness
    - **Property 11: Order Item Display Completeness**
    - **Validates: Requirements 12.1, 12.2, 12.3**

  - [ ] 5.6 Write property test for order item removal and recalculation
    - **Property 12: Order Item Removal and Recalculation**
    - **Validates: Requirements 13.2, 13.3, 14.8**

  - [x] 5.7 Implement remove order item command (RemoveOrderItemCommand)
    - Remove item from OrderItems collection
    - Execute RemoveOrderItemCommand
    - Recalculate Subtotal, TaxAmount, and Total
    - Update TotalItemCount
    - _Requirements: 13.2, 13.3_

  - [ ] 5.8 Write property test for product addition and recalculation
    - **Property 13: Product Addition and Recalculation**
    - **Validates: Requirements 20.1, 20.2**

  - [ ] 5.9 Write property test for modifier dialog for products
    - **Property 16: Modifier Dialog for Products**
    - **Validates: Requirements 20.3**

  - [x] 5.10 Implement add product command (AddProductCommand)
    - Check if product has modifiers
    - If has modifiers, show modifier selection dialog
    - Execute AddOrderItemCommand with product and modifiers
    - Add item to OrderItems collection (or increment quantity)
    - Recalculate Subtotal, TaxAmount, and Total
    - Update TotalItemCount
    - _Requirements: 20.1, 20.2, 20.3_

  - [x] 5.11 Implement edit order item command (EditOrderItemCommand)
    - Show modifier selection dialog for item
    - Execute UpdateOrderItemCommand with new modifiers
    - Recalculate totals
    - _Requirements: 13.1_

  - [x] 5.12 Write property test for category filtering
    - **Property 14: Category Filtering**
    - **Validates: Requirements 17.2**

  - [x] 5.13 Write property test for subcategory filtering
    - **Property 15: Subcategory Filtering**
    - **Validates: Requirements 18.1, 18.2, 18.3**

  - [x] 5.14 Implement category and subcategory selection commands
    - Implement SelectCategoryCommand (filter products by category)
    - Implement SelectSubcategoryCommand (filter products by subcategory)
    - Update FilteredProducts collection
    - Update Subcategories collection when category changes
    - _Requirements: 17.2, 18.1, 18.2, 18.3_

  - [x] 5.15 Write property test for order item count
    - **Property 17: Order Item Count**
    - **Validates: Requirements 23.1**

  - [x] 5.16 Write property test for wait time calculation
    - **Property 18: Wait Time Calculation**
    - **Validates: Requirements 23.2**

  - [ ] 5.17 Implement quick action commands
    - Implement SplitOrderCommand (show split order dialog)
    - Implement MergeOrderCommand (show merge order dialog)
    - Implement AddNoteCommand (show note entry dialog)
    - Implement PrintOrderCommand (print order ticket)
    - _Requirements: 15.1, 15.2, 15.3, 15.4_

  - [ ] 5.18 Implement payment initiation commands
    - Implement NavigateToSettleCommand (navigate to settle page with ticket)
    - Implement PayNowCommand (initiate immediate payment flow)
    - _Requirements: 16.3, 16.4_

  - [ ] 5.19 Implement session management commands
    - Implement StartSessionCommand (start new POS session)
    - Implement EndSessionCommand (end current POS session)
    - _Requirements: 21.3, 21.4_

  - [ ] 5.20 Implement advanced operation commands
    - Implement ReprintCommand (reprint ticket)
    - Implement VoidTicketCommand (void current ticket)
    - Implement ApplyDiscountCommand (show discount dialog)
    - Implement FireTicketCommand (send order to kitchen)
    - _Requirements: 22.1, 22.2, 22.3, 22.4_

  - [ ] 5.21 Implement table selection command (SelectTableCommand)
    - Show table selection dialog
    - Update TableNumber and GuestCount
    - _Requirements: 10.6_

  - [ ] 5.22 Write unit tests for OrderPageViewModel
    - Test command execution and state changes
    - Test property change notifications
    - Test filtering logic
    - Test calculation logic
    - Test error handling scenarios

- [ ] 6. Create OrderPageView (XAML)
  - [ ] 6.1 Create OrderPageView.xaml with main layout structure
    - Create header with menu button, title, system info (terminal, user, status, time)
    - Create two-column main layout (order sidebar, product area)
    - Create footer with station info, status indicator, copyright
    - Apply dark theme styling
    - _Requirements: 9.1, 9.2, 9.3, 9.4, 9.5, 9.6, 9.7, 9.8, 9.9, 9.10_

  - [ ] 6.2 Implement order sidebar (left panel)
    - Create table selector button (blue background, table icon, expand icon)
    - Create search input field with search icon
    - Create ticket number header with person add and history icons
    - Create order items scrollable list with custom scrollbar
    - Style order items (quantity in blue, modifiers in italic, edit/remove buttons)
    - Create totals section (subtotal, tax, total in blue)
    - Create quick action button grid (4 columns: Split, Merge, Note, Print)
    - Create payment button grid (2 columns: SETTLE, PAY NOW in green)
    - _Requirements: 10.1, 10.2, 10.3, 10.4, 10.5, 11.1, 11.5, 12.1, 12.2, 12.3, 12.4, 12.5, 12.6, 12.7, 12.8, 12.9, 12.10, 14.1, 14.2, 14.3, 14.4, 14.5, 14.6, 14.7, 14.9, 15.1, 15.2, 15.3, 15.4, 15.5, 15.6, 15.7, 15.8, 16.1, 16.2, 16.5, 16.6, 16.7, 16.8, 16.9_

  - [ ] 6.3 Implement product area (right panel)
    - Create category tabs (Food, Drinks, Desserts, Sides, Popular, Retail with icons)
    - Create subcategory filter pills (horizontal scrollable)
    - Create responsive product grid (2-6 columns)
    - Style product cards (name, price in blue, add icon on hover)
    - Add hover effects (border color change, icon appearance)
    - Make grid scrollable
    - _Requirements: 17.1, 17.2, 17.3, 17.4, 17.5, 18.1, 18.3, 18.4, 18.5, 19.1, 19.2, 19.3, 19.4, 19.5, 19.6, 19.7, 19.8, 19.9, 19.10_

  - [ ] 6.4 Implement footer with advanced operations
    - Create session control buttons (START SESSION, END SESSION)
    - Add vertical divider
    - Create advanced operation buttons (REPRINT, VOID, DISCOUNT, FIRE TICKET)
    - Make buttons horizontally scrollable
    - Add hover color effects
    - Display order statistics (Items: X, Wait: XX:XX)
    - _Requirements: 21.1, 21.2, 21.5, 22.1, 22.2, 22.3, 22.4, 22.5, 22.6, 22.7, 22.8, 22.9, 23.1, 23.2, 23.3, 23.4_

  - [ ] 6.5 Write UI integration tests for OrderPageView
    - Test data binding correctness
    - Test button command bindings
    - Test filtering and search UI updates
    - Test responsive grid behavior
    - Test accessibility features

- [ ] 7. Checkpoint - Order Page Complete
  - Ensure all Order Page tests pass
  - Manually test Order Page UI in isolation
  - Ask the user if questions arise

- [ ] 8. Implement navigation and integration
  - [ ] 8.1 Register views and view models in dependency injection container
    - Register SettlePageViewModel and SettlePageView
    - Register OrderPageViewModel and OrderPageView
    - Configure navigation service mappings
    - _Requirements: 1.3, 8.2, 16.3_

  - [ ] 8.2 Implement navigation between Order Page and Settle Page
    - Navigate from Order Page to Settle Page (pass ticket ID)
    - Navigate from Settle Page back to Order Page
    - Preserve ticket state during navigation
    - _Requirements: 1.3, 8.2, 8.3, 16.3_

  - [ ] 8.3 Implement dialogs and modal interactions
    - Create tip entry dialog
    - Create modifier selection dialog
    - Create table selection dialog
    - Create discount dialog
    - Create split payment dialog
    - Wire up dialog commands in ViewModels
    - _Requirements: 6.7, 13.1, 10.6, 6.5, 6.4_

  - [ ] 8.4 Write integration tests for navigation flows
    - Test Order Page → Settle Page navigation
    - Test Settle Page → Order Page navigation
    - Test ticket state preservation
    - Test dialog interactions

- [ ] 9. Implement error handling and user feedback
  - [ ] 9.1 Add error handling to SettlePageViewModel
    - Handle payment processing failures
    - Handle network connectivity loss
    - Display error dialogs with specific messages
    - Provide retry and recovery options
    - _Requirements: 5.4_

  - [ ] 9.2 Add error handling to OrderPageViewModel
    - Handle product not available errors
    - Handle invalid quantity errors
    - Handle order item modification conflicts
    - Handle table already occupied errors
    - Handle session not started errors
    - Display error dialogs and toast notifications
    - _Requirements: 20.1, 13.1, 10.6, 21.3_

  - [ ] 9.3 Implement loading indicators
    - Add loading indicator for payment processing
    - Add loading indicator for async operations > 300ms
    - Disable UI during processing
    - _Requirements: 26.4_

  - [ ] 9.4 Write unit tests for error handling
    - Test error scenarios and recovery flows
    - Test user feedback mechanisms
    - Test loading indicator behavior

- [ ] 10. Implement accessibility features
  - [ ] 10.1 Add keyboard navigation support
    - Configure tab order for all interactive elements
    - Add Enter key activation for primary actions
    - Add Escape key for cancel/back actions
    - _Requirements: 27.1, 27.2, 27.3, 27.4, 27.5_

  - [ ] 10.2 Add screen reader support
    - Add AutomationProperties.Name to all buttons
    - Configure LiveRegion for status messages
    - Associate form field labels
    - _Requirements: 27.2, 27.3_

  - [ ] 10.3 Ensure touch-friendly design
    - Verify minimum touch target size (44x44 pixels)
    - Verify touch-friendly spacing
    - Test on touch-enabled devices
    - _Requirements: 25.4_

  - [ ] 10.4 Write accessibility tests
    - Test keyboard navigation
    - Test screen reader compatibility
    - Test touch target sizes

- [ ] 11. Performance optimization
  - [ ] 11.1 Implement virtualization for large lists
    - Use VirtualizingStackPanel for order items list
    - Use VirtualizingWrapPanel for product grid
    - Test with large datasets (100+ items, 500+ products)
    - _Requirements: 12.1, 19.1_

  - [ ] 11.2 Optimize async operations
    - Ensure all command handlers are async
    - Add cancellation token support
    - Implement proper async/await patterns
    - _Requirements: 5.4, 20.1_

  - [ ] 11.3 Implement memory management
    - Implement IDisposable on ViewModels
    - Unsubscribe event handlers on disposal
    - Clean up large collections properly
    - _Requirements: All_

  - [ ] 11.4 Write performance tests
    - Test with large orders (50+ items)
    - Test with large product catalogs (500+ products)
    - Measure memory usage
    - Measure UI responsiveness

- [ ] 12. Final checkpoint - End-to-end testing
  - Test complete workflow: Order Page → Add Products → Navigate to Settle → Process Payment → Return to Order Page
  - Test all error scenarios and recovery flows
  - Test accessibility features
  - Test performance with realistic data volumes
  - Ensure all property tests pass (100 iterations each)
  - Ensure all unit tests pass
  - Ask the user if questions arise

## Notes

- All tasks are required for comprehensive implementation
- Each task references specific requirements for traceability
- Checkpoints ensure incremental validation
- Property tests validate universal correctness properties (19 properties total)
- Unit tests validate specific examples and edge cases
- Integration tests validate end-to-end workflows
- All property tests configured to run 100 iterations minimum
