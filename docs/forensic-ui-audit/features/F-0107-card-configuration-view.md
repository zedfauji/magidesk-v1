# Feature: Card Configuration View (F-0107)

## Classification
- **Parity classification**: PARITY REQUIRED
- **MagiDesk status**: MISSING

## Problem / Why this exists (grounded)
- **Operational need**: Configure credit card processing - payment gateway, merchant ID, terminal ID, supported card types, swipe vs chip preferences.
- **Evidence**: `CardConfigurationView.java` + `CardConfig.java` - card processor settings.

## User-facing surfaces
- **Surface type**: Configuration view
- **UI entry points**: BackOfficeWindow → Configuration → Cards
- **Exit paths**: Save / Cancel

## Preconditions & protections
- **User/role/permission checks**: Admin/configuration permission
- **State checks**: None
- **Manager override**: Not required

## Step-by-step behavior (forensic)
1. Open Card Configuration
2. View shows card settings:
   - Payment gateway selection (Authorize.Net, Mercury, etc.)
   - Merchant ID
   - Gateway credentials
   - Terminal ID (for integrated)
   - Card types accepted (Visa, MC, Amex, Discover)
   - Require signature threshold
   - Tip adjustment allowed
   - External terminal mode
   - Manual entry allowed
   - Pre-auth mode settings
3. Test connection button
4. Save persists settings
5. Affects all card transactions

## Edge cases & failure paths
- **Invalid credentials**: Test fails
- **Gateway unavailable**: Error on test
- **Missing required fields**: Validation error

## Data / audit / financial impact
- **Writes/updates**: CardConfig settings (encrypted)
- **Audit events**: Config changes logged
- **Financial risk**: Payment processing failure; Gateway fee implications

## Code traceability (REQUIRED)
- **Primary UI class(es)**: 
  - `CardConfigurationView` → `config/ui/CardConfigurationView.java`
  - `CardConfig` → `config/CardConfig.java`
- **Entry action(s)**: Part of ConfigurationDialog
- **Workflow/service enforcement**: CardConfig persistence; gateway integration
- **Messages/labels**: Setting labels

## MagiDesk parity notes
- **What exists today**: No card configuration
- **What differs / missing**: Entire card configuration system

## Porting strategy (PLAN ONLY)
- **Backend requirements**: CardConfig with encrypted credentials
- **API/DTO requirements**: Card config stored locally or encrypted in DB
- **UI requirements**: CardConfigurationView
- **Constraints for implementers**: Credentials must be encrypted; PCI considerations
