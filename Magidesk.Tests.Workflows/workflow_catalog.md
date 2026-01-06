# Workflow Catalog

Comprehensive inventory of POS workflows derived from Forensic Audit (F-XXXX) and Behavioral Analysis.

## 1. Foundation & Session Management
- [ ] **App Bootstrap**: Verify service registration and database connectivity.
- [ ] **Login Success**: [F-0003] Authenticate with valid PIN/Card.
- [ ] **Login Failure**: [F-0003] Reject invalid PINs.
- [ ] **Manager Override**: [F-0003] Manager login required for restricted actions.
- [ ] **Shift Start**: [F-0060] Open a new terminal shift.
- [ ] **Shift End**: [F-0061] Close terminal shift with drawer pull.
- [ ] **Drawer Assignment**: [F-0065] Assign user to specific drawer.
- [ ] **Drawer Logout**: [F-0008] Auto-logout after timeout (simulated).

## 2. Order Entry (Ticket Lifecycle)
- [x] **Create Dine-In Ticket**: [F-0019] Create ticket with Table Number.
- [ ] **Create Take-Out Ticket**: [F-0020] Create ticket with Customer Name.
- [ ] **Create Delivery Ticket**: [F-0083] Create ticket with Address/Customer.
- [ ] **Guest Count Entry**: [F-0023] Prompt for guest count on tables.
- [x] **Add Simple Item**: [F-0031] Add standard menu item.
- [ ] **Add Weighted Item**: [F-0024] Add item with fractional quantity.
- [ ] **Add Manual Price Item**: [F-0035] Item requiring price entry.
- [x] **Change Quantity**: [F-0026] Increment/Decrement item quantity.
- [x] **Remove Item**: [F-0028] Delete line item (Pre-Send).
- [ ] **Void Item**: [F-0028] Void line item (Post-Send) with Reason.

## 3. Modifiers & Complex Items
- [ ] **Standard Modifier**: [F-0038] Select single modifier (e.g., Meat Temp).
- [ ] **Forced Modifier Group**: [F-0038] Verify selection required (Min > 0).
- [ ] **Pizza Modifier (Whole)**: [F-0037] Topping on whole pizza.
- [ ] **Pizza Modifier (Half)**: [F-0037] Topping on Left/Right half.
- [ ] **Modifier Price Calculation**: [F-0037] Verify price addition.
- [ ] **Text Modifier**: [F-0036] Add cooking instruction/note.

## 4. Ticket Manipulation
- [ ] **Split Ticket (Drag & Drop)**: [F-0014] Move items to new ticket.
- [ ] **Split by Seat**: [F-0047] Auto-split based on seat assignment.
- [ ] **Split Evenly**: [F-0048] Divide total into N tickets.
- [ ] **Change Table**: [F-0080] Move ticket to empty table.
- [ ] **Merge Tickets**: [F-0075] Combine two tickets into one.
- [ ] **Transfer Server**: [F-0074] Reassign ticket owner.

## 5. Discounts & adjustments
- [ ] **Item Discount (Fixed)**: [F-0123] Apply fixed amount off item.
- [ ] **Item Discount (%)**: [F-0123] Apply percentage off item.
- [ ] **Ticket Discount**: [F-0122] Apply discount to entire order.
- [ ] **Tax Exemption**: [F-0109] Mark ticket as Tax Exempt.
- [ ] **Service Charge**: [F-0030] Add manual service fee.
- [ ] **Gratuity (Auto)**: [F-0055] Auto-add gratuity for large parties.

## 6. Payment & Settlement
- [ ] **Exact Cash**: [F-0042] Pay exact amount.
- [ ] **Cash with Change**: [F-0044] Pay over amount, verify change.
- [ ] **Credit Card (Manual)**: [F-0045] Key in card details.
- [ ] **Credit Card (Auth/Capture)**: [F-0017] Pre-auth then capture.
- [ ] **Split Tender**: [F-0046] Pay partial Cash, partial Card.
- [ ] **Gift Certificate**: [F-0054] Redeem GC, verify balance/change.
- [ ] **Quick Pay**: [F-0041] One-touch exact cash pay.
- [ ] **Tips (Adjust)**: [F-0056] Add tip to card transaction.
- [ ] **Refund Ticket**: [F-0073] Full refund of closed ticket.

## 7. Kitchen & Printing
- [ ] **Send to Kitchen**: [F-0027] fire items to printers.
- [ ] **Kitchen Display**: [F-0089] Verify items appear on KDS.
- [ ] **Mark Ready**: [F-0090] Bump items from KDS.
- [ ] **Reprint Ticket**: [F-0025] Send to receipt printer again.

## 8. Cash Management
- [ ] **Pay In**: [F-0064] Add cash to drawer.
- [ ] **Pay Out**: [F-0062] Remove cash for vendor payment.
- [ ] **Drawer Count**: [F-0067] Blind count of drawer.
- [ ] **Drawer Pull**: [F-0012] End of shift report generation.

## 9. Admin & Configuration
- [ ] **Update Menu Item**: [F-0114] Change price/name.
- [ ] **Disable Item**: [F-0114] Mark item out of stock.
- [ ] **Add User**: [F-0120] Create new employee.
- [ ] **Change Tax Rate**: [F-0109] Update global tax rate.

## 10. Edge Cases & Gap Verification
- [ ] **Partial Payment Guidance**: [Gap-01] Verify ticket state remains "Open/Partial" until fully paid (Prevent Zombie State).
- [ ] **Shift Close Block**: [Gap-02] Attempt to close shift with Open Tickets (Must Fail).
- [ ] **Refund Authentication**: [Gap-04] Verify Manager Password requirement for refunds.
- [ ] **Pizza Fraction Pricing**: [Gap-Logic-01] Verify 1/2 topping pricing rules (Avg price vs Sum).
- [ ] **Drawer Strictness**: [Gap-Logic-04] Attempt payment without assigned drawer (Must Fail).
- [ ] **Tax Exempt Reason**: [Gap-Logic-05] Apply tax exemption with specific reason code.

