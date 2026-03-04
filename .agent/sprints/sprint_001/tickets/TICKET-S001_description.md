# Ticket: TICKET-S001

## Title
Reduce all oversized files to under 300 lines

## Description
Six production files exceed the 300 line limit. Split each into partial 
classes. No logic changes. No behavior changes. No architectural changes.
Partial files named [ClassName].[FeatureArea].cs in the same folder.

## Acceptance Criteria
- [ ] Every file in the codebase is under 300 lines
- [ ] dotnet build passes with 0 errors
- [ ] Test results identical to baseline: 144/156 passing
- [ ] No XAML bindings broken

## Out of Scope
- Logic changes
- Sub-ViewModel extraction
- New DI registrations
- Any architectural change
