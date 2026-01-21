# Design Document: Settle and Order Page Redesign

## Overview

This design document outlines the technical approach for redesigning the Settle Page and Order Page in the Magidesk POS system. The redesign transforms the existing WPF/XAML-based interfaces into modern, touch-optimized screens that follow Windows UI (WinUI) design principles with a dark theme aesthetic.

The redesign focuses on three key objectives:
1. **Improved Usability**: Streamlined workflows with larger touch targets, clearer visual hierarchy, and intuitive layouts
2. **Modern Aesthetics**: Dark theme with accent colors, smooth animations, and contemporary typography using Inter font
3. **Enhanced Efficiency**: Quick access buttons, smart defaults, and reduced clicks for common operations

The implementation will leverage the existing MVVM architecture, domain models, and command/query patterns already established in the Magidesk codebase, while introducing new ViewModels and Views for the redesigned interfaces.

## Architecture

### High-Level Architecture

The redesigned pages will follow the existing Magidesk architecture:

```
┌─────────────────────────────────────────────────────────────┐
│                     Presentation Layer                       │
│  ┌──────────────────────┐      ┌──────────────────────┐    │
│  │  SettlePageView      │      │  OrderPageView       │    │
│  │  (XAML)              │      │  (XAML)              │    │
│  └──────────┬───────────┘      └──────────┬───────────┘    │
│             │                               │                │
│  ┌──────────▼───────────┐      ┌──────────▼───────────┐    │
│  │ SettlePageViewModel  │      │ OrderPageViewModel   │    │
│  │ (Commands, State)    │      │ (Commands, State)    │    │
│  └──────────┬───────────┘      └──────────┬───────────┘    │
└─────────────┼──────────────────────────────┼────────────────┘
              │                               │
┌─────────────▼───────────────────────────────▼────────────────┐
│                    Application Layer                          │
│  ┌────────────────────────────────────────────────────────┐  │
│  │  Command Handlers (ProcessPayment, AddOrderItem, etc.) │  │
│  └────────────────────────────────────────────────────────┘  │
└───────────────────────────────┬───────────────────────────────┘
                                │
┌───────────────────────────────▼───────────────────────────────┐
│                      Domain Layer                              │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐        │
│  │   Ticket     │  │   Payment    │  │  OrderItem   │        │
│  │  (Aggregate) │  │ (Value Obj)  │  │ (Entity)     │        │
│  └──────────────┘  └──────────────┘  └──────────────┘        │
└────────────────────────────────────────────────────────────────┘
```

### Component Responsibilities

**SettlePageView/ViewModel**:
- Display ticket financial summary
- Handle tender amount input via numeric keypad
- Process payment method selection
- Manage quick cash shortcuts
- Handle additional actions (tip, hold, split, discount)

**OrderPageView/ViewModel**:
- Display and manage order items
- Handle product selection and categorization
- Manage table/guest information
- Calculate order totals with tax
- Provide quick actions (split, merge, note, print)
- Initiate payment flow

### Navigation Flow

```
OrderPage ──[SETTLE button]──> SettlePageView
    │                               │
    │                               │
    └──[Back button]────────────────┘
    │
    └──[PAY NOW button]──> Direct Payment Processing
```

## Components and Interfaces

### Existing Infrastructure

The implementation will leverage existing session management infrastructure from the codebase:

**Domain Layer**:
- `TableSession` entity with full pause/resume/end support
- `TableSessionStatus` enum (Active, Paused, Ended)
- `GetBillableTime()` method that calculates duration excluding paused time

**Application Layer Commands**:
- `StartTableSessionCommand` - Starts a new table session
- `PauseTableSessionCommand` - Pauses an active session
- `ResumeTableSessionCommand` - Resumes a paused session
- `EndTableSessionCommand` - Ends a session and adds charges to ticket

**Existing ViewModels**:
- `OrderEntryViewModel` already has `PauseSessionCommand` and `ResumeSessionCommand`
- `TableMapViewModel` has full session management with dialogs
- `StartSessionDialogViewModel` and `EndSessionDialogViewModel` for session workflows

### 1. SettlePageViewModel

**Purpose**: Manages the state and behavior of the Settle Page

**Properties**:
```csharp
public class SettlePageViewModel : ViewModelBase
{
    // Ticket Information
    public string TicketNumber { get; }
    public string TableNumber { get; }
    
    // Financial Summary
    public decimal TotalAmount { get; }
    public decimal TaxAmount { get; }
    public decimal PaidAmount { get; private set; }
    public decimal BalanceDue { get; private set; }
    
    // Tender Entry
    public string TenderAmountDisplay { get; private set; }
    private decimal _tenderAmount;
    
    // Payment Methods
    public ObservableCollection<PaymentMethodViewModel> PaymentMethods { get; }
    
    // Quick Cash Amounts
    public ObservableCollection<decimal> QuickCashAmounts { get; }
    
    // State
    public bool IsTaxExempt { get; set; }
    public bool IsProcessingPayment { get; private set; }
    
    // Commands
    public ICommand KeypadDigitCommand { get; }
    public ICommand ClearTenderCommand { get; }
    public ICommand QuickCashCommand { get; }
    public ICommand ProcessPaymentCommand { get; }
    public ICommand AddTipCommand { get; }
    public ICommand HoldTicketCommand { get; }
    public ICommand SplitPaymentCommand { get; }
    public ICommand ApplyDiscountCommand { get; }
    public ICommand PrintReceiptCommand { get; }
    public ICommand ToggleTaxExemptCommand { get; }
    public ICommand CancelSettlementCommand { get; }
    public ICommand NavigateBackCommand { get; }
}
```

**Key Methods**:
- `AppendDigit(string digit)`: Appends digit to tender amount
- `ClearTender()`: Resets tender amount to $0.00
- `SetQuickCash(decimal amount)`: Sets tender to predefined amount
- `ProcessPayment(PaymentMethod method)`: Processes payment with selected method
- `RecalculateBalanceDue()`: Updates balance after payment applied
- `ToggleTaxExempt()`: Recalculates totals without tax

### 2. OrderPageViewModel

**Purpose**: Manages the state and behavior of the Order Page

**Properties**:
```csharp
public class OrderPageViewModel : ViewModelBase
{
    // Table Information
    public string TableNumber { get; set; }
    public int GuestCount { get; set; }
    
    // Ticket Information
    public string TicketNumber { get; }
    public DateTime TicketStartTime { get; }
    public TimeSpan WaitTime { get; }
    
    // Order Items
    public ObservableCollection<OrderItemViewModel> OrderItems { get; }
    
    // Financial Calculations
    public decimal Subtotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TaxRate { get; }
    public decimal Total { get; private set; }
    
    // Product Catalog
    public ObservableCollection<ProductCategoryViewModel> Categories { get; }
    public ProductCategoryViewModel SelectedCategory { get; set; }
    public ObservableCollection<string> Subcategories { get; }
    public string SelectedSubcategory { get; set; }
    public ObservableCollection<ProductViewModel> FilteredProducts { get; }
    
    // Search
    public string SearchQuery { get; set; }
    
    // Session Information
    public string TerminalName { get; }
    public string UserName { get; }
    public string SystemStatus { get; }
    public DateTime CurrentTime { get; }
    
    // Session State
    public SessionState CurrentSessionState { get; private set; }
    public bool IsSessionActive { get; }
    public bool IsSessionPaused { get; }
    public string SessionButtonText { get; }
    public bool IsEndSessionEnabled { get; }
    public TimeSpan SessionDuration { get; private set; }
    public string SessionDurationDisplay { get; }
    public DateTime? SessionStartTime { get; private set; }
    public TimeSpan AccumulatedPausedTime { get; private set; }
    
    // Statistics
    public int TotalItemCount { get; }
    
    // Commands
    public ICommand SelectTableCommand { get; }
    public ICommand SearchProductCommand { get; }
    public ICommand AddProductCommand { get; }
    public ICommand EditOrderItemCommand { get; }
    public ICommand RemoveOrderItemCommand { get; }
    public ICommand SelectCategoryCommand { get; }
    public ICommand SelectSubcategoryCommand { get; }
    public ICommand SplitOrderCommand { get; }
    public ICommand MergeOrderCommand { get; }
    public ICommand AddNoteCommand { get; }
    public ICommand PrintOrderCommand { get; }
    public ICommand NavigateToSettleCommand { get; }
    public ICommand PayNowCommand { get; }
    public ICommand ToggleSessionCommand { get; }
    public ICommand EndSessionCommand { get; }
    public ICommand ReprintCommand { get; }
    public ICommand VoidTicketCommand { get; }
    public ICommand ApplyDiscountCommand { get; }
    public ICommand FireTicketCommand { get; }
}

public enum SessionState
{
    NotStarted,
    Active,
    Paused
}
```

**Implementation Notes**:
- The ViewModel will use existing `StartTableSessionCommand`, `PauseTableSessionCommand`, `ResumeTableSessionCommand`, and `EndTableSessionCommand` from the Application layer
- Session duration will be calculated using `TableSession.GetBillableTime()` which already handles pause/resume logic
- The ViewModel will maintain a timer (DispatcherTimer) to update the duration display every second
- Session state will be derived from `Ticket.SessionStatus` (from TicketDto)
```

**Key Methods**:
- `AddProduct(Product product)`: Adds product to order, shows modifier dialog if needed
- `RemoveOrderItem(OrderItem item)`: Removes item and recalculates totals
- `RecalculateTotals()`: Updates subtotal, tax, and total
- `FilterProducts()`: Filters products by category, subcategory, and search query
- `NavigateToSettle()`: Navigates to settle page with current ticket
- `ProcessImmediatePayment()`: Initiates quick payment flow
- `ToggleSession()`: Starts, pauses, or resumes session based on current state (uses existing commands)
- `EndSession()`: Ends the current session using `EndTableSessionCommand`, which automatically calculates duration and adds expense to order
- `UpdateSessionButtonText()`: Updates button text based on session state ("Start Session", "Pause Session", "Resume Session")
- `UpdateSessionDuration()`: Updates session duration display in HH:MM:SS format (called every second via DispatcherTimer)
- `GetSessionDuration()`: Retrieves current billable time from the session entity via ticket

**Session Management Implementation**:
The session management will reuse existing infrastructure:
1. Start: Execute `StartTableSessionCommand` with table info
2. Pause: Execute `PauseTableSessionCommand` with session ID
3. Resume: Execute `ResumeTableSessionCommand` with session ID
4. End: Execute `EndTableSessionCommand` which automatically:
   - Calculates billable time using `TableSession.GetBillableTime()`
   - Adds expense line item to the ticket
   - Updates session status to Ended

### 3. PaymentMethodViewModel

**Purpose**: Represents a payment method option

**Properties**:
```csharp
public class PaymentMethodViewModel
{
    public PaymentMethodType Type { get; }
    public string DisplayName { get; }
    public string IconName { get; }
    public string BackgroundColor { get; }
    public bool IsEnabled { get; }
}

public enum PaymentMethodType
{
    Cash,
    CreditCard,
    GiftCard
}
```

### 4. OrderItemViewModel

**Purpose**: Represents an order item in the list

**Properties**:
```csharp
public class OrderItemViewModel : ViewModelBase
{
    public Guid OrderItemId { get; }
    public string ProductName { get; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; }
    public decimal LineTotal { get; }
    public ObservableCollection<string> Modifiers { get; }
    public string SpecialNote { get; }
    public bool HasModifiers { get; }
    public bool IsSelected { get; set; }
}
```

### 5. ProductViewModel

**Purpose**: Represents a product in the catalog grid

**Properties**:
```csharp
public class ProductViewModel
{
    public Guid ProductId { get; }
    public string Name { get; }
    public string SKU { get; }
    public decimal Price { get; }
    public string CategoryName { get; }
    public string SubcategoryName { get; }
    public bool HasModifiers { get; }
    public bool IsAvailable { get; }
}
```

### 6. ProductCategoryViewModel

**Purpose**: Represents a product category tab

**Properties**:
```csharp
public class ProductCategoryViewModel
{
    public string Name { get; }
    public string IconName { get; }
    public ObservableCollection<string> Subcategories { get; }
}
```

## Data Models

### Domain Models (Existing)

The redesign will use existing domain models from the Magidesk.Domain layer:

**Ticket** (Aggregate Root):
- Properties: Id, TableId, TicketNumber, Status, CreatedAt, Items, Payments
- Methods: AddItem(), RemoveItem(), ApplyPayment(), CalculateTotal(), MarkTaxExempt()

**OrderItem** (Entity):
- Properties: Id, ProductId, ProductName, Quantity, UnitPrice, Modifiers, LineTotal
- Methods: AddModifier(), UpdateQuantity(), CalculateLineTotal()

**Payment** (Value Object):
- Properties: Amount, Method, ProcessedAt, TransactionId
- Immutable value object representing a payment transaction

**Product** (Entity):
- Properties: Id, Name, SKU, Price, CategoryId, SubcategoryId, HasModifiers
- Read from product catalog

### View Models (New)

The ViewModels listed in the Components section will be created to support the redesigned UI. These ViewModels will:
- Expose domain data in UI-friendly formats
- Handle UI-specific state (selection, hover, focus)
- Implement INotifyPropertyChanged for data binding
- Provide ICommand implementations for user actions

### Data Flow

1. **Loading Order Page**:
   - ViewModel queries current ticket via `GetTicketByIdQuery`
   - Loads product catalog via `GetProductCatalogQuery`
   - Populates categories and products
   - Calculates initial totals

2. **Adding Product**:
   - User clicks product → `AddProductCommand` executes
   - If product has modifiers → Show modifier dialog
   - Execute `AddOrderItemCommand` with product and modifiers
   - Domain: Ticket.AddItem() creates OrderItem
   - ViewModel refreshes order items and recalculates totals

3. **Navigating to Settle**:
   - User clicks SETTLE → `NavigateToSettleCommand` executes
   - Navigation service passes ticket ID to SettlePageViewModel
   - SettlePageViewModel loads ticket financial data
   - Displays summary and initializes tender entry

4. **Processing Payment**:
   - User enters tender amount via keypad
   - User selects payment method → `ProcessPaymentCommand` executes
   - Execute `ProcessPaymentCommand` with amount and method
   - Domain: Ticket.ApplyPayment() creates Payment
   - If balance > 0 → Allow additional payments
   - If balance = 0 → Complete settlement and return to order page

### State Management

**SettlePageViewModel State**:
- Tender amount (string for display, decimal for calculation)
- Payment history (list of applied payments)
- Tax exempt flag
- Processing state (for async operations)

**OrderPageViewModel State**:
- Selected category and subcategory
- Search query
- Selected order items
- Filter state
- Session information

Both ViewModels will use `INotifyPropertyChanged` to update the UI reactively when state changes.

## Correctness Properties


*A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: Tender Amount Building

*For any* sequence of digit button presses (0-9 and decimal point), the tender amount display should correctly concatenate the digits to form a valid currency string.

**Validates: Requirements 3.5**

### Property 2: Tender Amount Clearing

*For any* current tender amount value, pressing the clear button should reset the tender amount display to "$0.00".

**Validates: Requirements 3.6**

### Property 3: Quick Cash Selection

*For any* quick cash denomination ($1, $5, $10, $20, $50, $100), clicking the quick cash button should set the tender amount to exactly that denomination.

**Validates: Requirements 4.2**

### Property 4: Payment Processing with Tender Amount

*For any* valid tender amount and payment method, processing a payment should create a payment record with the exact tender amount and update the ticket's paid amount.

**Validates: Requirements 5.4**

### Property 5: Payment Balance Calculation

*For any* sequence of payments applied to a ticket, the paid amount should equal the sum of all payment amounts, and the balance due should equal the total minus the paid amount.

**Validates: Requirements 2.7**

### Property 6: Currency Formatting

*For any* monetary amount (total, tax, paid, balance, tender), the display should format the amount as currency with exactly two decimal places (e.g., "$42.50").

**Validates: Requirements 2.8**

### Property 7: Tax Exempt Recalculation

*For any* ticket, when marked as tax exempt, the total should be recalculated to equal the subtotal (without tax), and the balance due should be updated accordingly.

**Validates: Requirements 7.3, 7.4**

### Property 8: Cancel Settlement Preserves State

*For any* ticket in the settlement process, canceling the settlement should return to the order page with the ticket in its original state (no payments applied, no modifications made).

**Validates: Requirements 8.3**

### Property 9: Table Display Format

*For any* table number and guest count, the table selector should display the information in the format "TABLE XX (GUESTS: X)" where XX is the table number and X is the guest count.

**Validates: Requirements 10.2**

### Property 10: Product Search Filtering

*For any* search query, the product list should include only products where either the product name or SKU contains the search query (case-insensitive).

**Validates: Requirements 11.2, 11.3**

### Property 11: Order Item Display Completeness

*For any* order with items, all order items should be displayed in the list with quantity (formatted as "Xx"), product name, line total, and modifiers (if present) shown below the item name.

**Validates: Requirements 12.1, 12.2, 12.3**

### Property 12: Order Item Removal and Recalculation

*For any* order item in an order, removing the item should delete it from the order items list and immediately recalculate the subtotal, tax, and total to reflect the removal.

**Validates: Requirements 13.2, 13.3, 14.8**

### Property 13: Product Addition and Recalculation

*For any* product, adding it to the order should create a new order item (or increment quantity if already present) and immediately recalculate the subtotal, tax, and total.

**Validates: Requirements 20.1, 20.2**

### Property 14: Category Filtering

*For any* product category, selecting the category should display only products that belong to that category.

**Validates: Requirements 17.2**

### Property 15: Subcategory Filtering

*For any* selected category and subcategory, the product list should display only products that belong to both the selected category and subcategory.

**Validates: Requirements 18.1, 18.2, 18.3**

### Property 16: Modifier Dialog for Products

*For any* product with modifiers, clicking the product should display a modifier selection dialog before adding the product to the order.

**Validates: Requirements 20.3**

### Property 17: Order Item Count

*For any* order, the displayed item count should equal the sum of quantities of all order items in the order.

**Validates: Requirements 23.1**

### Property 18: Wait Time Calculation

*For any* order, the displayed wait time should equal the time elapsed since the ticket was created.

**Validates: Requirements 23.2**

### Property 19: Ticket Number Display Format

*For any* ticket, the ticket number should be displayed in the format "Ticket #XXXX" where XXXX is the ticket number.

**Validates: Requirements 12.7**

### Property 20: Session State Transitions

*For any* sequence of session commands (toggle, end), the session state should transition correctly: NotStarted → (toggle) → Active → (toggle) → Paused → (toggle) → Active → (end) → NotStarted, and the End Session button should only be enabled when the session is Active or Paused.

**Validates: Requirements 21.3, 21.4, 21.5, 21.6, 21.7, 21.8, 21.9**

### Property 21: Session Button Text

*For any* session state, the session toggle button text should correctly reflect the next action: "Start Session" when NotStarted, "Pause Session" when Active, and "Resume Session" when Paused.

**Validates: Requirements 21.3, 21.5, 21.7, 21.8**

### Property 22: Session Duration Calculation

*For any* session with start time, pause periods, and resume periods, the calculated session duration should equal the total elapsed time minus the accumulated paused time.

**Validates: Requirements 21.12, 21.13, 21.14, 21.15**

### Property 23: Session Duration Display Format

*For any* session duration, the display should format the duration as HH:MM:SS where HH is hours (zero-padded), MM is minutes (zero-padded), and SS is seconds (zero-padded).

**Validates: Requirements 21.12**

### Property 24: Session Expense Generation

*For any* ended session with a duration, ending the session should add exactly one expense line item to the current order with an amount calculated based on the session duration.

**Validates: Requirements 21.16**

## Error Handling

### Settle Page Error Scenarios

**Invalid Tender Amount**:
- **Scenario**: User enters invalid characters or malformed decimal (e.g., "12..34")
- **Handling**: Keypad input validation prevents invalid sequences; only digits 0-9 and single decimal point allowed
- **User Feedback**: Invalid button presses are ignored (no visual feedback)

**Payment Processing Failure**:
- **Scenario**: Payment gateway returns error or times out
- **Handling**: Display error dialog with specific error message; payment not applied to ticket
- **User Feedback**: Toast notification with error details; allow retry or select different payment method
- **Recovery**: Ticket remains in settlement state; user can retry or cancel

**Insufficient Tender Amount**:
- **Scenario**: User attempts to process payment with tender amount less than balance due
- **Handling**: Allow partial payment; update paid amount and balance due
- **User Feedback**: Display updated balance due; prompt for additional payment method

**Overpayment**:
- **Scenario**: Tender amount exceeds balance due
- **Handling**: Calculate and display change due; complete payment
- **User Feedback**: Display change amount prominently; print receipt with change

**Network Connectivity Loss**:
- **Scenario**: Network connection lost during settlement
- **Handling**: Queue payment for processing when connection restored; display offline indicator
- **User Feedback**: Toast notification indicating offline mode; prevent navigation until payment queued

### Order Page Error Scenarios

**Product Not Available**:
- **Scenario**: User attempts to add out-of-stock product
- **Handling**: Display error dialog; product not added to order
- **User Feedback**: Toast notification indicating product unavailable

**Invalid Quantity**:
- **Scenario**: User attempts to set quantity to zero or negative
- **Handling**: Validation prevents invalid quantities; minimum quantity is 1
- **User Feedback**: Invalid input ignored; quantity remains at previous valid value

**Order Item Modification Conflict**:
- **Scenario**: Order item already sent to kitchen; user attempts to modify
- **Handling**: Display confirmation dialog warning about kitchen impact
- **User Feedback**: Require explicit confirmation; log modification for kitchen notification

**Table Already Occupied**:
- **Scenario**: User attempts to select table that's already in use
- **Handling**: Display error dialog; table selection not changed
- **User Feedback**: Toast notification indicating table occupied; suggest available tables

**Session Not Started**:
- **Scenario**: User attempts to create order without active session
- **Handling**: Display error dialog prompting to start session
- **User Feedback**: Provide "Start Session" button in error dialog for quick recovery

**Search Returns No Results**:
- **Scenario**: Search query matches no products
- **Handling**: Display empty state with helpful message
- **User Feedback**: Show "No products found" message; suggest clearing search or browsing categories

### General Error Handling Principles

1. **Graceful Degradation**: System remains functional even when individual features fail
2. **Clear Messaging**: Error messages are specific, actionable, and user-friendly
3. **Recovery Options**: Always provide clear path to recover from error state
4. **Logging**: All errors logged with context for debugging and support
5. **Validation**: Input validation prevents errors before they occur
6. **Async Error Handling**: All async operations wrapped in try-catch with proper error propagation

## Testing Strategy

### Dual Testing Approach

The redesigned Settle and Order pages will be validated using both unit tests and property-based tests:

**Unit Tests**: Verify specific examples, edge cases, and error conditions
- Specific UI interactions (button clicks, navigation)
- Edge cases (empty orders, zero amounts, boundary values)
- Error scenarios (network failures, invalid input)
- Integration points between components

**Property Tests**: Verify universal properties across all inputs
- Financial calculations (totals, tax, balance)
- Data transformations (formatting, filtering, sorting)
- State management (order modifications, payment application)
- Comprehensive input coverage through randomization

Both testing approaches are complementary and necessary for comprehensive coverage. Unit tests catch concrete bugs in specific scenarios, while property tests verify general correctness across the input space.

### Property-Based Testing Configuration

**Framework**: We will use FsCheck for C# property-based testing, which integrates well with xUnit.

**Test Configuration**:
- Minimum 100 iterations per property test (due to randomization)
- Each property test must reference its design document property
- Tag format: **Feature: settle-order-page-redesign, Property {number}: {property_text}**

**Example Property Test Structure**:
```csharp
[Property]
[Trait("Feature", "settle-order-page-redesign")]
[Trait("Property", "5")]
public Property PaymentBalanceCalculation()
{
    return Prop.ForAll(
        Arb.From<List<decimal>>(), // Generate random payment amounts
        payments =>
        {
            // Arrange
            var ticket = CreateTicketWithTotal(100m);
            var viewModel = new SettlePageViewModel(ticket);
            
            // Act
            foreach (var payment in payments.Where(p => p > 0))
            {
                viewModel.ProcessPayment(PaymentMethod.Cash, payment);
            }
            
            // Assert
            var expectedPaid = payments.Where(p => p > 0).Sum();
            var expectedBalance = Math.Max(0, 100m - expectedPaid);
            
            return viewModel.PaidAmount == expectedPaid &&
                   viewModel.BalanceDue == expectedBalance;
        });
}
```

### Unit Testing Strategy

**ViewModel Tests**:
- Test command execution and state changes
- Test property change notifications
- Test navigation logic
- Test validation logic

**View Tests**:
- Test data binding correctness
- Test UI element visibility based on state
- Test user interaction flows
- Test accessibility features

**Integration Tests**:
- Test complete workflows (add product → modify → settle → pay)
- Test navigation between pages
- Test data persistence
- Test command/query handler integration

### Test Coverage Goals

- **ViewModels**: 90%+ code coverage
- **Commands/Queries**: 95%+ code coverage
- **Domain Logic**: 100% code coverage
- **Property Tests**: All 19 correctness properties implemented
- **Unit Tests**: All edge cases and error scenarios covered

### Testing Tools

- **xUnit**: Primary test framework
- **FsCheck**: Property-based testing
- **Moq**: Mocking framework for dependencies
- **FluentAssertions**: Assertion library for readable tests
- **WPF Test Framework**: For UI testing

### Continuous Integration

- All tests run on every commit
- Property tests run with 100 iterations in CI
- Failed property tests report counterexamples for debugging
- Test results published to build dashboard
- Code coverage tracked and reported

## Implementation Notes

### Technology Stack

- **UI Framework**: WPF with XAML
- **MVVM Framework**: CommunityToolkit.Mvvm (formerly Microsoft.Toolkit.Mvvm)
- **Styling**: Custom WPF styles mimicking WinUI design language
- **Icons**: Material Design Icons for WPF
- **Fonts**: Inter font family (imported via NuGet or embedded)
- **Animations**: WPF Storyboards and Triggers

### Styling Approach

**Color Palette**:
```csharp
public static class ColorPalette
{
    public const string Primary = "#0078D4";
    public const string BackgroundDark = "#1C1C1C";
    public const string SurfaceDark = "#2B2B2B";
    public const string SidebarDark = "#202020";
    public const string AccentGreen = "#107C10";
    public const string AccentRed = "#C42B1C";
    public const string AccentPurple = "#8E44AD";
    public const string AccentGold = "#FFB900";
}
```

**Typography**:
- Primary Font: Inter (weights: 300, 400, 500, 600, 700)
- Fallback: Segoe UI
- Font smoothing: ClearType enabled

**Spacing System**:
- Base unit: 4px
- Common spacings: 4px, 8px, 12px, 16px, 24px, 32px, 48px

### Performance Considerations

**Virtualization**:
- Order items list uses VirtualizingStackPanel for large orders
- Product grid uses VirtualizingWrapPanel for large catalogs

**Async Operations**:
- All command handlers are async
- UI remains responsive during payment processing
- Loading indicators shown for operations > 300ms

**Memory Management**:
- ViewModels implement IDisposable
- Event handlers unsubscribed on disposal
- Large collections use ObservableCollection with proper cleanup

### Accessibility

**Keyboard Navigation**:
- All interactive elements accessible via Tab key
- Logical tab order follows visual flow
- Enter key activates primary actions
- Escape key cancels dialogs and returns to previous screen

**Screen Reader Support**:
- All buttons have AutomationProperties.Name
- Status messages announced via LiveRegion
- Form fields have associated labels

**Touch Support**:
- Minimum touch target size: 44x44 pixels
- Touch-friendly spacing between interactive elements
- Swipe gestures for navigation (optional)

### Migration Strategy

**Phased Rollout**:
1. **Phase 1**: Deploy new Settle Page alongside existing page; feature flag controls which version loads
2. **Phase 2**: Deploy new Order Page alongside existing page; feature flag controls which version loads
3. **Phase 3**: Monitor usage and gather feedback; fix issues
4. **Phase 4**: Enable new pages by default; keep old pages as fallback
5. **Phase 5**: Remove old pages after successful validation period

**Data Compatibility**:
- New pages use existing domain models and database schema
- No data migration required
- Backward compatible with existing tickets and orders

**Training**:
- Create video tutorials for new UI
- Provide in-app tooltips for first-time users
- Offer side-by-side comparison guide

### Future Enhancements

**Potential Improvements**:
- Split payment across multiple methods in single flow
- Tip percentage quick buttons (15%, 18%, 20%)
- Recent products quick access
- Favorite products customization
- Custom quick cash denominations
- Multi-language support
- Customizable color themes
- Tablet-optimized layout
- Offline mode with sync
