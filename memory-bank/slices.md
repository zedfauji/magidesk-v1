# CANONICAL SLICE DEFINITIONS
**Status:** ACTIVE
**Authority:** Vertical Slice Execution Agent

This document defines the vertical slices for the Magidesk POS rebuild. Execution MUST follow these slices strictly to minimize drift.

## SLICE 1: FOUNDATION
**Goal**: A stable, bootable application with authentication, navigation, and basic state management.
**Entry Conditions**: None.
**Exit Conditions**: App boots, User can Login, Shift can Start/End, Navigation works.

### Features
| ID | Name | Status |
| :--- | :--- | :--- |
| **F-0001** | Application Bootstrap & System Initialization | |
| **F-0002** | POS Main Window Shell | |
| **F-0003** | Login Screen | |
| **F-0004** | Switchboard View | |
| **F-0008** | Logout Action | |
| **F-0009** | Clock In/Out Action | |
| **F-0060** | Shift Start Dialog | |
| **F-0061** | Shift End Dialog | |
| **F-0111** | Back Office Window | |
| **F-0129** | Message Banner Display | |
| **F-0130** | About Dialog | |
| **F-0131** | Confirmation Dialog Pattern | |
| **F-0132** | Progress/Loading Dialog | |

---

## SLICE 2: ORDER ENTRY
**Goal**: Fully functional order creation and manipulation.
**Entry Conditions**: Slice 1 Complete.
**Exit Conditions**: Can browse menu, add items, modify items, and manage tickets.

### Features
| ID | Name | Status |
| :--- | :--- | :--- |
| **F-0005** | Order Entry View Container | |
| **F-0006** | Ticket Panel | |
| **F-0011** | Open Tickets List Dialog | |
| **F-0013** | Void Ticket Dialog | |
| **F-0014** | Split Ticket Dialog | |
| **F-0019** | New Ticket Action | |
| **F-0020** | Order Type Selection Dialog | |
| **F-0021** | Ticket View Panel | |
| **F-0022** | Order View Container | |
| **F-0023** | Guest Count Entry Dialog | |
| **F-0024** | Quantity Entry Dialog | |
| **F-0026** | Increase/Decrease Quantity Action | |
| **F-0027** | Send to Kitchen Action | |
| **F-0028** | Delete Ticket Item Action | |
| **F-0029** | Misc Ticket Item Dialog | |
| **F-0030** | Ticket Fee Dialog | |
| **F-0031** | Menu Item Button View | |
| **F-0032** | Size Selection Dialog | |
| **F-0033** | Beverage Quick Add | |
| **F-0034** | Item Search Dialog | |
| **F-0035** | Price Entry Dialog | |
| **F-0036** | Cooking Instruction Dialog | |
| **F-0037** | Pizza Modifiers View | |
| **F-0038** | Modifier Selection Dialog | |
| **F-0039** | Addon Selection View | |
| **F-0040** | Combo Item Selection Dialog | |
| **F-0047** | Split by Seat Dialog | |
| **F-0048** | Split Even Dialog | |
| **F-0049** | Split by Amount Dialog | |
| **F-0068** | Ticket Explorer | |
| **F-0069** | Edit Ticket Action | |
| **F-0071** | Hold Ticket Action | |
| **F-0072** | Reopen Ticket Action | |
| **F-0075** | Merge Tickets Action | |
| **F-0077** | Customer Selector Dialog | |
| **F-0080** | Change Table Action | |
| **F-0081** | Floor Explorer | |
| **F-0082** | Table Map View | |
| **F-0083** | Home Delivery View | |
| **F-0084** | Pickup Order View | |
| **F-0085** | Bar Tab Selection View | |
| **F-0086** | Seat Selection Dialog | |
| **F-0087** | Table Browser | |

---

## SLICE 3: PAYMENT
**Goal**: Complete financial transaction handling.
**Entry Conditions**: Slice 2 Complete.
**Exit Conditions**: Can process all payment types, handle split payments, and settle tickets properly.

### Features
| ID | Name | Status |
| :--- | :--- | :--- |
| **F-0007** | Password Entry Dialog & Payment Keypad | |
| **F-0010** | Cash Drops/Drawer Bleed | |
| **F-0012** | Drawer Pull Report Dialog | |
| **F-0015** | Payment Process Wait Dialog | |
| **F-0016** | Swipe Card Dialog | |
| **F-0017** | Authorization Code Dialog | |
| **F-0018** | Authorization Capture Batch Dialog | |
| **F-0025** | Print Ticket Action | |
| **F-0041** | Quick Pay Action | |
| **F-0042** | Exact Due Button | |
| **F-0043** | Quick Cash Buttons | |
| **F-0044** | Cash Payment Button | |
| **F-0045** | Credit Card Payment Button | |
| **F-0046** | Group Settle Ticket Dialog | |
| **F-0050** | Swipe Card Dialog (Conflict?) | |
| **F-0052** | Check Payment Action | |
| **F-0053** | House Account Payment Action | |
| **F-0054** | Gift Certificate Entry | |
| **F-0055** | Gratuity Input | |
| **F-0056** | Tip Adjustment Action | |
| **F-0058** | Multi Currency Tender Dialog | |
| **F-0059** | Card Signature Capture | |
| **F-0062** | Payout Dialog | |
| **F-0063** | No Sale Action | |
| **F-0064** | Cash Drop Action | |
| **F-0065** | Drawer Assignment Action | |
| **F-0066** | Tip Declare Action | |
| **F-0067** | Drawer Count Dialog | |
| **F-0070** | View Receipt Action | |
| **F-0073** | Refund Action | |
| **F-0076** | Multi Ticket Payment | |

---

## SLICE 4: KITCHEN
**Goal**: Kitchen display and order routing.
**Entry Conditions**: Slice 3 Complete.
**Exit Conditions**: Orders routed to kitchen, status updates visible.

### Features
| ID | Name | Status |
| :--- | :--- | :--- |
| **F-0088** | Kitchen Display Window | |
| **F-0089** | Kitchen Ticket View | |
| **F-0090** | Kitchen Status Selector | |
| **F-0091** | Kitchen Ticket List Panel | |

---

## SLICE 5: ADMIN / REPORTING
**Goal**: System configuration and reporting.
**Entry Conditions**: Slice 4 Complete (Preferred) or Independent.
**Exit Conditions**: Reports generate correctly, configuration persists.

### Features
| ID | Name | Status |
| :--- | :--- | :--- |
| **F-0092** | Sales Summary Report | |
| **F-0093** | Sales Detail Report | |
| **F-0094** | Sales Balance Report | |
| **F-0095** | Sales Exception Report | |
| **F-0096** | Credit Card Report | |
| **F-0097** | Payment Report | |
| **F-0098** | Menu Usage Report | |
| **F-0099** | Server Productivity Report | |
| **F-0100** | Hourly Labor Report | |
| **F-0101** | Tip Report | |
| **F-0102** | Attendance Report View | |
| **F-0103** | Journal Report View | |
| **F-0104** | Cash Out Report | |
| **F-0105** | Restaurant Configuration View | |
| **F-0106** | Terminal Configuration View | |
| **F-0107** | Card Configuration View | |
| **F-0108** | Print Configuration View | |
| **F-0109** | Tax Configuration View | |
| **F-0110** | Language Selection Dialog | |
| **F-0112** | Menu Category Explorer | |
| **F-0113** | Menu Group Explorer | |
| **F-0114** | Menu Item Explorer | |
| **F-0115** | Modifier Explorer | |
| **F-0116** | Modifier Group Explorer | |
| **F-0117** | Coupon Explorer | |
| **F-0118** | Tax Explorer | |
| **F-0119** | Shift Explorer | |
| **F-0120** | User Explorer | |
| **F-0121** | Order Type Explorer | |
| **F-0122** | Coupon and Discount Dialog | |
| **F-0123** | Discount Application Dialog | |
| **F-0124** | Table Section Configuration | |
| **F-0125** | Notes Dialog | |
| **F-0126** | Delivery Zone Configuration | |
| **F-0127** | Printer Group Configuration | |
| **F-0128** | Database Backup Dialog | |
| **F-0133** | Role Management View | |
