# Category F: Customer & Member Management

## F.1 Customer catalog

**Feature ID:** F.1  
**Feature Name:** Customer catalog  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND (no Customers table)
- Domain entities: `Ticket.CustomerId` exists but no `Customer` entity
- Services: `SetCustomerCommandHandler.cs` exists but handles external ID only
- APIs / handlers: `SetCustomerCommand` - minimal implementation
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): `CustomerSelectionDialog.xaml` exists but is a stub
- ViewModels: `CustomerSelectionViewModel.cs` - minimal/stub implementation
- Navigation path: Order Entry → Assign Customer (stub)
- User-visible workflow: Button exists, no real functionality

**Notes:**
- `Ticket.CustomerId` is a dangling reference
- CustomerSelectionDialog is empty stub (hardcoded mock data)
- No actual customer database

**Risks / Gaps:**
- Cannot store customer information
- Cannot look up returning customers
- No customer relationship management

**Recommendation:** IMPLEMENT - Create Customer entity (Name, Phone, Email, Address)

---

## F.2 Customer history

**Feature ID:** F.2  
**Feature Name:** Customer history  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Requires Customer entity first
- Would show past tickets, spending, visits
- Essential for customer service

**Risks / Gaps:**
- Cannot recognize repeat customers
- Cannot see spending patterns
- No loyalty tracking

**Recommendation:** IMPLEMENT - Add when Customer entity exists

---

## F.3 Member profiles

**Feature ID:** F.3  
**Feature Name:** Member profiles  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND (no Member or Membership table)
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Member = Customer with club membership
- Additional fields: MembershipPlan, StartDate, ExpiryDate, Status
- Distinction from regular customer needed

**Risks / Gaps:**
- Cannot track club members
- No membership benefits possible
- No member-specific features

**Recommendation:** IMPLEMENT - Extend Customer with membership properties

---

## F.4 Member photos (upload / webcam)

**Feature ID:** F.4  
**Feature Name:** Member photos  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Photo for member identification
- Webcam capture or file upload
- Storage as blob or file path

**Risks / Gaps:**
- Cannot verify member identity
- Card sharing/fraud possible

**Recommendation:** IMPLEMENT - Add photo field and capture UI

---

## F.5 Barcode-based member identification

**Feature ID:** F.5  
**Feature Name:** Barcode-based member identification  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND (barcode scanning exists for products only)
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Member card with barcode
- Scan to identify and attach to session
- Fast lookup at check-in

**Risks / Gaps:**
- Cannot use member cards
- Manual lookup required
- Slow customer flow

**Recommendation:** IMPLEMENT - Add Member.BarcodeId and scanner integration

---

## F.6 Member discounts

**Feature ID:** F.6  
**Feature Name:** Member discounts  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: Discount entity exists but no member link
- Domain entities: `Discount.cs` - no MemberId or MembershipPlanId FK
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Discounts exist but not linked to members
- Cannot auto-apply based on membership level
- Manual discount required

**Risks / Gaps:**
- Members don't get automatic benefits
- Staff must manually apply discounts
- Inconsistent member experience

**Recommendation:** IMPLEMENT - Link Discount to MembershipPlan, auto-apply

---

## F.7 Membership plans

**Feature ID:** F.7  
**Feature Name:** Membership plans  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND (no MembershipPlans table)
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Plan tiers: Bronze, Silver, Gold
- Different benefits per tier
- Pricing, duration, renewal rules

**Risks / Gaps:**
- Cannot offer tiered memberships
- No benefit differentiation
- Upsell opportunity lost

**Recommendation:** IMPLEMENT - Create MembershipPlan entity

---

## F.8 Subscription billing

**Feature ID:** F.8  
**Feature Name:** Subscription billing  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Recurring membership fees
- Monthly/Annual billing
- Auto-renewal

**Risks / Gaps:**
- Must manually collect membership fees
- No recurring revenue automation
- Member follow-up burden

**Recommendation:** IMPLEMENT - Add subscription billing with reminders

---

## F.9 Hour banks / prepaid hours

**Feature ID:** F.9  
**Feature Name:** Hour banks / prepaid hours  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Buy 10 hours, use as credit
- Balance tracking per member
- Deduct when table session closed

**Risks / Gaps:**
- No prepaid packages
- Revenue left on table
- Common competitor feature

**Recommendation:** IMPLEMENT - Add HourBalance to Member, deduction logic

---

## F.10 Historical play data per member

**Feature ID:** F.10  
**Feature Name:** Historical play data per member  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Sessions played, total hours, favorite tables
- For analytics and personalization
- Requires Member and TableSession entities

**Risks / Gaps:**
- Cannot analyze member behavior
- No personalization possible

**Recommendation:** IMPLEMENT - When Member and TableSession exist

---

## F.11 Member usage reports

**Feature ID:** F.11  
**Feature Name:** Member usage reports  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Aggregate reports on member activity
- Top members, churn risk, etc.
- Requires Member data first

**Risks / Gaps:**
- Cannot identify valuable customers
- No retention analytics

**Recommendation:** IMPLEMENT - Add with Member module

---

## F.12 Promotional mailings

**Feature ID:** F.12  
**Feature Name:** Promotional mailings  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Email/SMS marketing
- Requires customer contact info
- Usually external integration

**Risks / Gaps:**
- Cannot communicate with customers
- Marketing campaigns impossible

**Recommendation:** DEFER - Usually handled by external CRM/marketing tools

---

## F.13 Member status (active / suspended)

**Feature ID:** F.13  
**Feature Name:** Member status  
**Status:** NOT IMPLEMENTED

**Backend Evidence:**
- Database tables / columns: NO EVIDENCE FOUND
- Domain entities: NO EVIDENCE FOUND
- Services: NO EVIDENCE FOUND
- APIs / handlers: NO EVIDENCE FOUND
- Background jobs: NO EVIDENCE FOUND

**Frontend Evidence:**
- Views (XAML): NO EVIDENCE FOUND
- ViewModels: NO EVIDENCE FOUND
- Navigation path: NO EVIDENCE FOUND
- User-visible workflow: NO EVIDENCE FOUND

**Notes:**
- Active = can use membership benefits
- Suspended = blocked (payment issue, etc.)
- Expired = renewal required

**Risks / Gaps:**
- Cannot deactivate problem members
- No status management

**Recommendation:** IMPLEMENT - Add status field to Member

---

## Category F COMPLETE

- Features audited: 13
- Fully implemented: 0
- Partially implemented: 0
- Not implemented: 13
